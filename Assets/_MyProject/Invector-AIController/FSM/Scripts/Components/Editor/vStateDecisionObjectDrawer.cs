
using UnityEngine;
using UnityEditor;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [CustomPropertyDrawer(typeof(vStateDecisionObject))]
    public class vStateDecisionObjectDrawer : PropertyDrawer
    {
        public Editor decisionEditor;
        public GUIContent[] valueSelector = new GUIContent[] { new GUIContent("True", "Validate if decision return true"), new GUIContent("False", "Validate if decision return False") };     
        public SerializedProperty isOpen;
        public SerializedProperty valueProp;
        public SerializedProperty decisionProp;     
        public int selected;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginProperty(position, label, property);
            isOpen = property.FindPropertyRelative("isOpen");
            valueProp = property.FindPropertyRelative("trueValue");
            decisionProp = property.FindPropertyRelative("decision");
            rect.width = position.width * 0.6f;           
            GUI.Label(rect,decisionProp.objectReferenceValue?decisionProp.objectReferenceValue.name:decisionProp.displayName);
            //if(decisionProp.objectReferenceValue)
            {
               isOpen.boolValue = EditorGUI.Foldout(rect, isOpen.boolValue, "", true);
            }
            if (valueProp.boolValue) selected = 0;
            else selected = 1;
            rect.width = position.width * 0.4f;
            
            rect.x += position.width * 0.6f;
            selected = EditorGUI.Popup(rect, selected, valueSelector);
            valueProp.boolValue = selected == 0 ? true : false;
            EditorGUI.EndProperty();
        }

        void DrawDecision(Rect position,SerializedProperty decision)
        {

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

