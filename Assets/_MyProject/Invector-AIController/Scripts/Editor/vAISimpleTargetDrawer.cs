using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

namespace Invector.vCharacterController.AI
{
    [CustomPropertyDrawer(typeof(vAISimpleTarget),true)]
    public class vAISimpleTargetDrawer : PropertyDrawer
    {
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

           
            label = EditorGUI.BeginProperty(position, label, property);
           
            Rect rect = position;
            rect.width = EditorGUIUtility.labelWidth;
            rect.height = EditorGUIUtility.singleLineHeight;


            if (!property.propertyPath.Contains("Array"))
            {
                GUI.Label(rect, label);
                rect.x += rect.width;
                rect.width = position.width - rect.width;
            }
            else
            {
                rect.width = position.width;               
            }

            if (property.hasVisibleChildren)
            {
                var oldWidth = rect.width;
                rect.width = EditorGUIUtility.singleLineHeight;
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, "");
                rect.width = oldWidth;
            } 
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("_transform"), !property.propertyPath.Contains("Array")?GUIContent.none:label);          
           
            rect.y += EditorGUIUtility.singleLineHeight;
           
            if (property.hasVisibleChildren && property.isExpanded)
            {
                var childEnum = property.GetEnumerator();
               
                while (childEnum.MoveNext())
                {
                    var current = childEnum.Current as SerializedProperty;
                    if (property.name!=("_transform"))
                    {
                        rect.height = EditorGUI.GetPropertyHeight(current);
                        EditorGUI.PropertyField(rect, current);
                        rect.y += EditorGUI.GetPropertyHeight(current);
                    }
                   
                }
            }
           
            if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
          
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (property.hasVisibleChildren && property.isExpanded)
            {
                var childEnum = property.GetEnumerator();
                while (childEnum.MoveNext())
                {
                    var current = childEnum.Current as SerializedProperty;
                    if (property.name != ("_transform"))
                    {
                        height += EditorGUI.GetPropertyHeight(current);
                    }

                }
            }
               
            return height;
        }
    }  

}