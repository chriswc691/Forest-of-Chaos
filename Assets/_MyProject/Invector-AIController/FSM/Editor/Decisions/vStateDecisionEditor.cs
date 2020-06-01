using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Invector.vCharacterController.AI.FSMBehaviour
{    
    [CustomEditor(typeof(vStateDecision), true)]
    public class vStateDecisionEditor : Editor
    {
        public string valueName;
        public readonly string[] ignoreProperties = new string[] { "m_Script", "parentFSM","editingName", "trueState", "falseState", "trueRect", "falseRect", "nodeParent",  "selectedTrue", "selectedFalse", "stateInUseCount" };
        public GUISkin skin;
        public vStateDecision decision;
      
        public bool isOpen;
        public override void OnInspectorGUI()
        {
            if (target)
            {
                var contentColor = GUI.contentColor;
                GUI.contentColor = Color.white;
                serializedObject.Update();
                if (!decision) decision = target as vStateDecision;
                if (decision)
                {
                    if (skin == null) skin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");

                    GUILayout.BeginVertical(skin.box);
                    if (target)
                    {
                        Draw();
                    }
                    GUILayout.EndVertical();

                }
                serializedObject.ApplyModifiedProperties();
                GUI.contentColor = contentColor;
            }
        }
        protected virtual void Draw()
        {
            isOpen = GUILayout.Toggle(isOpen, new GUIContent(target.name, target.name), skin.GetStyle("FoldoutClean"), GUILayout.MaxWidth(250), GUILayout.Height(18));

            if (isOpen)
            {
                DrawProperties();
            }
        }
        protected virtual void DrawProperties()
        {
            var attribute = target.GetType().GetCustomAttributes(typeof(vFSMHelpboxAttribute), true).FirstOrDefault() as vFSMHelpboxAttribute;
            if (attribute != null)
            {
                EditorGUILayout.HelpBox(attribute.text, attribute.messageType);
            }
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), GUIContent.none, GUILayout.MinWidth(50));
            GUI.enabled = true;
            GUI.SetNextControlName("Name");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Name"), GUIContent.none);
            GUI.SetNextControlName("Default");
            DrawPropertiesExcluding(serializedObject, ignoreProperties);

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
                    var countSameName = target.GetSameComponentNameCount<vStateDecision>();
                    if (countSameName > 0) serializedObject.FindProperty("m_Name").stringValue += " " + (countSameName - 1).ToString();
                    valueName = serializedObject.FindProperty("m_Name").stringValue;
                    AssetDatabase.SaveAssets();

                }
                serializedObject.FindProperty("editingName").boolValue = false;
            }
        }
    }
}