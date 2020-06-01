using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Invector.vCharacterController.AI.FSMBehaviour;
using System;
using System.Reflection;
using System.Linq;
using Invector.vCharacterController.AI;

public class vCreateAIWindow : EditorWindow
{
    [MenuItem("Invector/FSM AI/Create New AI", false, 0)]
    public static void ShowWindow()
    {
        var window = CreateInstance<vCreateAIWindow>();
        window.titleContent = new GUIContent("Create New AI");
        window.ShowUtility();
    }

    public GUISkin skin;
    public UnityEngine.Object selectedPrefab;
    public UnityEngine.Object selectedFSM;
    public Texture2D m_Logo;
    public int selectedType;
    public Type targetType;
    public Editor humanoidpreview;
    RuntimeAnimatorController controller;

    void OnEnable()
    {
        m_Logo = Resources.Load("icon_v2") as Texture2D;
    }

    private void OnGUI()
    {
        if (!skin) skin = Resources.Load("vSkin") as GUISkin;
        GUI.skin = skin;
        GUILayout.BeginVertical("AI CREATOR WINDOW", "window");
        GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
        GUILayout.Space(5);
        selectedPrefab = EditorGUILayout.ObjectField("Character Model", selectedPrefab, typeof(GameObject), true);
        controller = EditorGUILayout.ObjectField("Animator Controller: ", controller, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
        selectedFSM = EditorGUILayout.ObjectField("FSM Behaviour Controller", selectedFSM, typeof(vFSMBehaviour), false);
        if (selectedFSM)
        {
            var requiredTypes = (selectedFSM as vFSMBehaviour).GetRequiredTypes();
            var types = FindDerivedTypes(typeof(vIControlAI).Assembly, typeof(vIControlAI), requiredTypes);

            if (types != null)
            {
                var _types = types.Cast<Type>().ToList();

                if (selectedType >= _types.Count) selectedType = 0;
                if (_types.Count > 0)
                {
                    var names = TypesToStringArray(_types);
                    selectedType = EditorGUILayout.Popup("Type of Controller", selectedType, names);
                    targetType = _types[selectedType];
                    if (selectedPrefab)
                    {
                        bool hasController = (selectedPrefab as GameObject).GetComponent<vIControlAI>() != null;
                        var fsmBehaviour = (selectedPrefab as GameObject).GetComponent<vIFSMBehaviourController>();
                        Animator charAnimator = null;
                        if (selectedPrefab)
                            charAnimator = (selectedPrefab as GameObject).GetComponent<Animator>();
                        var charExist = charAnimator != null;                        
                        var isHuman = charExist ? charAnimator.isHuman : false;
                        var isValidAvatar = charExist ? isHuman && charAnimator.avatar.isValid : false;                        

                        if (hasController || fsmBehaviour != null)
                        {
                            this.minSize = new Vector2(400, 160);
                            this.maxSize = new Vector3(400, 160);
                            EditorGUILayout.HelpBox("Please select a Model without any AI Component", MessageType.Error);
                        }
                        else if (!isValidAvatar)
                        {
                            this.minSize = new Vector2(400, 160);
                            this.maxSize = new Vector3(400, 160);
                            string message = "";
                            if (!charExist)
                                message += "*Missing a Animator Component";
                            else if (!isHuman)
                                message += "\n*This is not a Humanoid";
                            else if (!isValidAvatar)
                                message += "\n*" + selectedPrefab.name + " is a invalid Humanoid";
                            EditorGUILayout.HelpBox(message, MessageType.Error);
                        }
                        else
                        {
                            if (humanoidpreview == null || humanoidpreview.target != selectedPrefab)
                            {
                                humanoidpreview = Editor.CreateEditor(selectedPrefab);
                            }
                            else
                            {
                                this.minSize = new Vector2(400, 570);
                                this.maxSize = new Vector3(400, 570);
                                DrawHumanoidPreview();
                            }

                            if (isValidAvatar && controller != null && GUILayout.Button("CREATE"))
                            {
                                Create();
                            }
                        }
                    }
                    else
                    {
                        this.minSize = new Vector2(400, 100);
                        this.maxSize = new Vector3(400, 120);
                    }
                }
                else
                {
                    this.minSize = new Vector2(400, 100);
                    this.maxSize = new Vector3(400, 120);
                }
            }
        }
        else
        {
            this.minSize = new Vector2(400, 100);
            this.maxSize = new Vector3(400, 120);
        }
        GUILayout.EndVertical();
    }

    void Create()
    {
        var instance = Instantiate(selectedPrefab) as GameObject;
        if (instance)
        {
            var t = instance.AddComponent(targetType);
            (t as vIControlAI).CreatePrimaryComponents();
            DestroyImmediate(t);
            var fms = instance.AddComponent<vFSMBehaviourController>();
            fms.fsmBehaviour = selectedFSM as vFSMBehaviour;
            t = instance.AddComponent(targetType);
            if (controller) instance.GetComponent<Animator>().runtimeAnimatorController = controller;
            instance.tag = "Enemy";
            var aiLayer = LayerMask.NameToLayer("Enemy");
            instance.layer = aiLayer;
            (t as vIControlAI).CreateSecondaryComponents();
        }
        this.Close();
    }

    void DrawHumanoidPreview()
    {
        GUILayout.FlexibleSpace();

        if (humanoidpreview != null)
        {
            humanoidpreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 400), "window");
        }
    }

    public List<Type> GetValidTypes(List<Type> types, List<Type> requiredTypes)
    {
        List<Type> validTypes = new List<Type>();
        for (int i = 0; i < types.Count; i++)
        {
            if (IsValidType(types[i], requiredTypes) && !validTypes.Contains(types[i])) validTypes.Add(types[i]);
        }
        return validTypes;
    }

    public string[] TypesToStringArray(List<Type> types)
    {
        string[] names = new string[types.Count];
        for (int i = 0; i < types.Count; i++)
        {
            names[i] = types[i].Name;
        }
        return names;
    }

    public bool IsValidType(Type type, List<Type> types)
    {
        var interfaces = type.GetInterfaces();

        var typeCount = 0;
        for (int i = 0; i < interfaces.Length; i++)
        {
            if (types.Contains(interfaces[i]))
            {
                typeCount++;
            }
        }
        return typeCount == types.Count || types.Contains(type);
    }

    public IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType, List<Type> requiredTypes)
    {
        return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && (t.IsSubclassOf(typeof(MonoBehaviour)) || t.Equals(typeof(MonoBehaviour))) && IsValidType(t, requiredTypes));
    }
}