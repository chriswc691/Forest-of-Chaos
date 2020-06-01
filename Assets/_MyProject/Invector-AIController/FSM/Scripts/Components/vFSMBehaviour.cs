#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.vCharacterController.AI.FSMBehaviour
{
  

    public sealed class vFSMBehaviour : ScriptableObject
    {
       
        #region Public Variables      

        public string graphName
        {
            get { return this.name; }
            set { this.name = value; }
        }
       
        [HideInInspector, SerializeField] public vFSMState selectedNode;
        [HideInInspector, SerializeField] public bool wantConnection = false;
        [HideInInspector, SerializeField] public TransitionPreview transitionPreview = new TransitionPreview();
      
        [HideInInspector, SerializeField] public vFSMState connectionNode;

        [HideInInspector, SerializeField] public bool showProperties;
        [HideInInspector, SerializeField] public List<vFSMState> states;
        [HideInInspector, SerializeField] public Vector2 panOffset;
        [HideInInspector, SerializeField] public bool overNode = false;
        public struct TransitionPreview
        {
            public Rect transitionRect;
            public bool? sideRight;
            public Action<vFSMState> onValidate;
            public vFSMState state;
        }
        #endregion       

        void OnEnable()
        {
            if (states == null) states = new List<vFSMState>();
#if UNITY_EDITOR
            ReloadChilds();
#endif
        }

#if UNITY_EDITOR       
        [HideInInspector, SerializeField]
        public List<Editor> actions;
        [HideInInspector, SerializeField]
        public List<Editor> decisions;
        public Texture icon;
        public UnityEngine.Events.UnityAction<vFSMState> onSelectState;
        #region Main Methods

        public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            //if (nodes.Count > 0)
            {
               
                //Line For Clear AnnyState Actions and Entry Decisions and actions
                //if (states.Count > 1)
                //{
                //    states[0].transitions.Clear();
                //    states[0].actions.Clear();
                //    states[0].resetCurrentDestination = false;
                //    states[0].resetCurrentSpeed = false;
                //    states[1].resetCurrentDestination = false;
                //    states[1].resetCurrentSpeed = false;
                //    states[1].actions.Clear();
                //}
                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i] == null)
                    {
                        states.RemoveAt(i); break;
                    }
                  
                    states[i].UpdateNodeConnections(viewRect,e);
                }

                for (int i = 0; i < states.Count; i++)
                {
                    if (selectedNode == null || selectedNode != states[i])
                    {
                        states[i].UpdateNodeGUI(e, viewRect, viewSkin);
                    }
                }

                if (selectedNode)
                {
                    selectedNode.UpdateNodeGUI(e, viewRect, viewSkin);
                }
                if (wantConnection)
                {
                    //if(connectionNode!=null)
                    {
                        DrawConnectionToMouse(e.mousePosition);
                    }
                }
              //  GUILayout.EndArea();

                if (e.type == EventType.Layout)
                {
                    if (selectedNode != null)
                    {
                        showProperties = true;
                    }
                }
            }
            //Lets Look for Connection Node
            EditorUtility.SetDirty(this);
        }

        void ReloadChilds()
        {
            var data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this.GetInstanceID()));
            if (actions == null) actions = new List<Editor>();
            if (decisions == null) decisions = new List<Editor>();
            actions.Clear();
            decisions.Clear();
            foreach (var d in data)
            {
                if (d == null)
                {

                }
                else
                {
                    if (d.GetType().Equals(typeof(vStateAction)) || d.GetType().IsSubclassOf(typeof(vStateAction)))
                        actions.Add(Editor.CreateEditor(d));

                    if (d.GetType().Equals(typeof(vStateDecision)) || d.GetType().IsSubclassOf(typeof(vStateDecision)))
                        decisions.Add(Editor.CreateEditor(d));
                }
              
            }
        }

        [ContextMenu("Show Components")]
        void ShowComponents()
        {
            HideShowComponents();
        }

        [ContextMenu("Hide Components")]
        void HideComponents()
        {
            HideShowComponents(false);
        }
        [ContextMenu("Delete Unused States")]
        void DeleteUnusedComponents()
        {
            var data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this.GetInstanceID()));

            for (int i = 0; i < data.Length; i++)
            {
                var d = data[i];
                if (d != null)
                {
                    if (d.GetType().Equals(typeof(vFSMState)) || d.GetType().IsSubclassOf(typeof(vFSMState)) || d is vFSMState)
                        if (!states.Contains(d as vFSMState))
                        {
                            Debug.Log("Delet " + d.name + " of " + this.name);
                            DestroyImmediate(d as vFSMState,true);
                        }

                }

            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void HideShowComponents(bool show = true)
        {
            var data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this.GetInstanceID()));

            foreach (var d in data)
            {        
                if(d!=null)
                {
                    if (d.GetType().Equals(typeof(vFSMState)) || d.GetType().IsSubclassOf(typeof(vFSMState)) || d is vFSMState)
                        if (show) d.hideFlags = HideFlags.None; else d.hideFlags = HideFlags.HideInHierarchy;
                    if (d.GetType().Equals(typeof(vStateAction)) || d.GetType().IsSubclassOf(typeof(vStateAction)))
                        if (show) d.hideFlags = HideFlags.None; else d.hideFlags = HideFlags.HideInHierarchy;
                    if (d.GetType().Equals(typeof(vStateDecision)) || d.GetType().IsSubclassOf(typeof(vStateDecision)))
                        if (show) d.hideFlags = HideFlags.None; else d.hideFlags = HideFlags.HideInHierarchy;
                }
              
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void OnChangeChilds()
        {
            ReloadChilds();
        }

        public void InitGraph()
        {
            if (states.Count > 0)
            {               
                for (int i = 0; i < states.Count; i++)
                {
                    states[i].InitNode();
                }
            }
        }

        #endregion

        #region Utility Methods
        
        void ProcessEvents(Event e, Rect viewRect)
        {
            //if (viewRect.Contains(e.mousePosition))
            //{
            //    GUILayout.BeginArea(viewRect);
            //    if (e.button == 0)
            //    {
            //        if (e.type == EventType.mouseDown && !wantConnection || e.type == EventType.mouseUp && wantConnection)
            //        {
            //            showProperties = false;
            //            SelectNodes(e);
            //        }
            //    }
            //    GUILayout.EndArea();
            //}
        }

        void DrawConnectionToMouse(Vector2 mousePosition)
        {
            Handles.BeginGUI();
            {
                Rect a = transitionPreview.transitionRect;
                Rect b = new Rect(mousePosition.x, mousePosition.y, 0, 0);
                DrawNodeCurve(a, b,transitionPreview.sideRight);
            }
            Handles.EndGUI();
        }

        void DrawNodeCurve(Rect start, Rect end, bool? sideRight)
        {
            Vector3 startPos = sideRight!=null? new Vector3(start.x + start.width, start.y + start.height / 2, 0): new Vector3(start.x + (start.width/2),start.y+(start.height/2),0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            var magniture = Mathf.Clamp(((endPos - startPos).magnitude / 200f) - 0.5f, 0f, 1f);

            Vector3 startTan = startPos + (sideRight!=null?((bool)sideRight ? Vector3.right : Vector3.left) * (200 * magniture):Vector3.zero);
            Vector3 endTan = endPos + (sideRight != null ? ((bool)sideRight ? Vector3.left : Vector3.right) * (200 * magniture) : Vector3.zero);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 2);
        }

        public void SelectState(vFSMState state)
        {
           
            if(onSelectState!=null)
                onSelectState.Invoke(state);
        }

        public void DeselectAll()
        {
            if (states.Count > 0)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    states[i].isSelected = false;
                    if (states[i] is vFSMState)
                    {

                        var node = states[i] as vFSMState;
                        node.selectedDecisionIndex = 0;
                        for (int a = 0; a < node.transitions.Count; a++)
                        {
                            node.transitions[a].selectedFalse = false;
                            node.transitions[a].selectedTrue = false;
                            node.selectedDecisionIndex = 0;
                        }
                    }
                }
            }
        }

        public void DeselectAllExcludinCurrent()
        {
            if (states.Count > 0)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    if (selectedNode == null || selectedNode != states[i])
                    {
                        states[i].isSelected = false;
                        if (states[i] is vFSMState)
                        {
                            var node = states[i] as vFSMState;
                            node.selectedDecisionIndex = 0;
                            for (int a = 0; a < node.transitions.Count; a++)
                            {
                                node.transitions[a].selectedFalse = false;
                                node.transitions[a].selectedTrue = false;
                            }
                        }
                    }
                }
            }
        }

        public void ConnectToState(vFSMState state)
        {
            if (wantConnection)
            {
                transitionPreview.onValidate.Invoke(state);
               
            }
            wantConnection = false;
        }

        public List<Type> GetRequiredTypes()
        {
            List < Type > types = new List<Type>();
            for (int i = 0; i < states.Count; i++)
            {
                if(states[i] != null)
                {
                    if (!types.Contains(states[i].requiredType))
                        types.Add(states[i].requiredType);
                    for (int a = 0; a < states[i].actions.Count; a++)
                    {                       
                        if (states[i].actions[a] != null && !types.Contains(states[i].actions[a].requiredType))
                            types.Add(states[i].actions[a].requiredType);                                                   
                    }
                    for (int a = 0; a < states[i].transitions.Count; a++)
                    {
                        for (int b = 0; b < states[i].transitions[a].decisions.Count; b++)
                        {
                            if (states[i].transitions[a] != null && states[i].transitions[a].decisions[b] != null && states[i].transitions[a].decisions[b].decision != null)
                            {
                                if (!types.Contains(states[i].transitions[a].decisions[b].decision.requiredType))
                                    types.Add(states[i].transitions[a].decisions[b].decision.requiredType);
                            }
                        }
                    }
                }                
            }          
            return types;
        }
        #endregion

#endif
    }
}