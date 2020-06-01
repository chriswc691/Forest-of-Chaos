using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [CustomEditor(typeof(vStateAction), true)]
    public class vStateActionEditor : Editor
    {
        public readonly string[] ignoreProperties = new string[] { "m_Script", "parentFSM", "editingName", "stateInUseCount" };
        public bool isOpen;
        GUISkin skin;
        string valueName;

        public override void OnInspectorGUI()
        {
            if (target)
            {
                serializedObject.Update();
                if (skin == null) skin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
                GUILayout.BeginVertical(skin.box);
               
                isOpen = GUILayout.Toggle(isOpen, target.name, skin.GetStyle("FoldoutClean"), GUILayout.MaxWidth(250), GUILayout.Height(16));

                if (isOpen)
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), GUIContent.none, GUILayout.MinWidth(50));
                    GUI.enabled = true;
                    GUI.SetNextControlName("Name");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Name"), GUIContent.none);
                    var attribute = target.GetType().GetCustomAttributes(typeof(vFSMHelpboxAttribute), true).FirstOrDefault() as vFSMHelpboxAttribute;
                    if (attribute != null)
                    {
                        EditorGUILayout.HelpBox(attribute.text, attribute.messageType);
                    }
                    if (GUI.GetNameOfFocusedControl().Equals("Name"))
                    {
                        if (!serializedObject.FindProperty("editingName").boolValue)
                        {
                            serializedObject.FindProperty("editingName").boolValue = true;
                            valueName = serializedObject.FindProperty("m_Name").stringValue;
                        }

                        if (Event.current.keyCode == KeyCode.Return) GUI.FocusControl("NONE");
                    }
                    else if (serializedObject.FindProperty("editingName").boolValue)
                    {
                        if (valueName != serializedObject.FindProperty("m_Name").stringValue)
                        {
                            var countSameName = target.GetSameComponentNameCount<vStateAction>();
                            if (countSameName > 0) serializedObject.FindProperty("m_Name").stringValue += " " + (countSameName - 1).ToString();
                            valueName = serializedObject.FindProperty("m_Name").stringValue;
                            AssetDatabase.SaveAssets();
                        }
                        serializedObject.FindProperty("editingName").boolValue = false;
                    }

                    GUI.SetNextControlName("Default");
                    DrawPropertiesExcluding(serializedObject, ignoreProperties);
                }

                GUILayout.EndVertical();
               
                serializedObject.ApplyModifiedProperties();
            }
        }        
    }
}