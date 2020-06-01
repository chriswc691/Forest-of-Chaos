using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [CustomEditor(typeof(AICheckState), true)]
    public class AICheckStateEditor : vStateDecisionEditor
    {
        protected string[] stateList = new string[0];
        protected override void DrawProperties()
        {
            base.DrawProperties();
            if(serializedObject!=null)
            {
                var stateIndex = serializedObject.FindProperty("stateIndex");
                if(stateIndex!=null)
                {
                    var stateList = GetStateList();
                    if (stateList.Length>0)
                    {                        
                        stateIndex.intValue = EditorGUILayout.Popup("FSM State Equals",stateIndex.intValue, stateList);
                    }
                    else
                    {
                        if (decision.parentFSM)
                            GUILayout.Box("No States In FSM", skin.box);
                        else
                            EditorGUILayout.HelpBox("State selector will appear when it is in a State", MessageType.Info);
                    }
                }
            }
        }
        protected virtual string[] GetStateList()
        {
            if(decision && decision.parentFSM)
            {
                if(stateList==null || stateList.Length != decision.parentFSM.states.Count-2)
                stateList = new string[decision.parentFSM.states.Count];
                for(int i=0;i<decision.parentFSM.states.Count-2;i++)
                {
                    stateList[i] = decision.parentFSM.states[i+2].Name;
                }
            }
            return stateList;
        }
    }
}