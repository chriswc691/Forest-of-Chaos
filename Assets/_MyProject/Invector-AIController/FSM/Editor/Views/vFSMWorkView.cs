#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [Serializable]
    public class vFSMWorkView : FSMViewBase
    {
        
        #region Public Variables

        #endregion

        #region Protected Variables

        protected Vector2 mousePosition;
        protected bool indrag = false;
        protected int selectedNodeID = 0;
        protected bool inDrawRect = false;
        protected Rect selectorRect = new Rect();
        protected bool inDragNode;
        protected int selectedNodesCount;
        protected List<vFSMState> selectedStates = new List<vFSMState>();
        protected Texture backgroundTexture;
        
        #endregion

        #region Constructor
        public vFSMWorkView() : base("Work View")
        {

        }
        #endregion

        

        #region Main Methods
        public override void UpdateView(Event e, vFSMBehaviour curGraph)
        {
            base.UpdateView(e, curGraph);

            if (!backgroundTexture) this.backgroundTexture = Resources.Load("grid") as Texture;
            var color = GUI.color;
            GUI.color = vFSMBehaviourPreferences.gridBackgroundColor;
            if (!indrag)
            {     
                if(curGraph)
                {
                    curGraph.panOffset.x = curGraph.panOffset.x.NearestRound(vFSMHelper.dragSnap);
                    curGraph.panOffset.y = curGraph.panOffset.y.NearestRound(vFSMHelper.dragSnap);
                }
                
            }
            GUI.Box(viewRect, GUIContent.none);
            if (Event.current.type == EventType.Repaint)
            { // Draw Background when Repainting
              // Offset from origin in tile units

                Vector2 tileOffset = new Vector2(-(1 + (curGraph ? curGraph.panOffset.x : 0)) / backgroundTexture.width,
                    ((1 + (curGraph ? curGraph.panOffset.y : 0))) / backgroundTexture.height);
                // Amount of tiles
                Vector2 tileAmount = new Vector2(Mathf.Round(viewRect.width * 1) / backgroundTexture.width,
                    Mathf.Round(viewRect.height * 1) / backgroundTexture.height);
                // Draw tiled background
                GUI.color = vFSMBehaviourPreferences.gridLinesColor;

                GUI.DrawTextureWithTexCoords(viewRect, backgroundTexture, new Rect(tileOffset, tileAmount));
                if (curGraph == null)
                {
                    GUI.Label(viewRect, "NO BEHAVIOUR", viewSkin.GetStyle("ViewMessage"));
                }
            }
          
            GUI.color = color;
         
            if (inDrawRect)
            {
                GUI.Box(selectorRect, "", viewSkin.GetStyle("SelectorArea"));
            }
        
            if (curGraph)
            {
                if (curGraph.onSelectState == null) curGraph.onSelectState = OnSelectState;
                curGraph.UpdateGraphGUI(e, viewRect, viewSkin);

            }
            #region Draw Work View Icons
            GUI.BeginGroup(viewRect);
          
            GUI.color = new Color(1, 1, 1, 0.2f);            
            GUI.DrawTexture(new Rect(viewRect.width - 105, viewRect.height - 105, 100, 100), Resources.Load("Textures/Editor/logo") as Texture2D);
            if (currentFSM != null && currentFSM.icon != null)
                GUI.DrawTexture(new Rect(viewRect.width - 105, viewRect.y + 5, 100, 100), currentFSM ? currentFSM.icon : null);
            GUI.color = color;
            #endregion
            GUI.EndGroup();
            GUI.Box(viewRect, "", viewSkin.GetStyle("BoxShadown"));
            ProcessEvents(e);
        }      

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
            mousePosition = e.mousePosition;
            if (currentFSM && viewRect.Contains(mousePosition) || inDrawRect)
            {
                // GUILayout.BeginArea(viewRect);
              
                if (e.button == 0 && !e.alt)
                {
                    if (e.type == EventType.MouseDrag && !currentFSM.wantConnection)
                    {
                        if (currentFSM.states.Count > 0)
                        {
                            for (int i = 0; i < currentFSM.states.Count; i++)
                            {
                                if (currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                {
                                    inDragNode = true;
                                }
                            }
                        }
                        if (inDragNode && !inDrawRect)
                        {
                            if (selectedStates.Count > 0)
                            {
                                for (int i = 0; i < selectedStates.Count; i++)
                                {
                                    if (selectedStates[i].isSelected)
                                    {
                                        selectedStates[i].OnDrag(e.delta);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!inDrawRect)
                            {
                                inDrawRect = true;
                                selectorRect = new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0);


                            }
                            if (inDrawRect)
                            {
                                selectorRect.width += e.delta.x;
                                selectorRect.height += e.delta.y;
                                if (currentFSM.states.Count > 0)
                                {

                                    for (int i = 0; i < currentFSM.states.Count; i++)
                                    {

                                        if (selectorRect.Overlaps(currentFSM.states[i].nodeRect, true))
                                        {
                                            if (!currentFSM.states[i].isSelected || !selectedStates.Contains(currentFSM.states[i]))
                                            {
                                                currentFSM.states[i].isSelected = true;
                                                if (!selectedStates.Contains(currentFSM.states[i]))
                                                    selectedStates.Add(currentFSM.states[i]);
                                            }

                                        }
                                        else
                                        {
                                            currentFSM.states[i].isSelected = false;
                                            if (selectedStates.Contains(currentFSM.states[i]))
                                                selectedStates.Remove(currentFSM.states[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (e.type == EventType.MouseDown)
                    {
                        if (currentFSM != null)
                        {
                            currentFSM.overNode = false;
                            if (currentFSM.states.Count > 0 && !inDragNode && !inDrawRect)
                            {
                                for (int i = 0; i < currentFSM.states.Count; i++)
                                {

                                    if (currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                    {
                                        currentFSM.overNode = true;
                                        {
                                            selectedNodeID = i;
                                            if (!e.shift && currentFSM.states[i].isSelected == false)
                                            {
                                                if (currentFSM.selectedNode) currentFSM.selectedNode.isSelected = false;
                                                Selection.activeObject = currentFSM.states[i];
                                                currentFSM.states[i].isSelected = true;
                                                currentFSM.selectedNode = currentFSM.states[i];
                                                selectedStates.Clear();
                                                selectedStates.Add(currentFSM.states[i]);
                                                currentFSM.DeselectAllExcludinCurrent();
                                                selectedNodesCount = 1;
                                            }

                                            else if (e.shift && !selectedStates.Contains(currentFSM.states[i]))
                                            {
                                                currentFSM.states[i].isSelected = true;
                                                selectedStates.Add(currentFSM.states[i]);
                                                selectedNodesCount++;
                                            }
                                            else
                                            {
                                                Selection.activeObject = currentFSM.states[i];
                                            }
                                            e.Use();
                                        }
                                        break;
                                    }

                                }
                                if (!currentFSM.overNode && !inDragNode && !inDrawRect)
                                {
                                    currentFSM.DeselectAll();
                                }
                            }
                            if (!currentFSM.overNode)
                            {
                                Selection.activeGameObject = null;
                            }
                        }
                    }
                    else if (e.type == EventType.MouseUp)
                    {

                        currentFSM.overNode = false;
                        if (currentFSM != null)
                        {

                            if (currentFSM.states.Count > 0 && !inDragNode && !inDrawRect)
                            {
                                for (int i = 0; i < currentFSM.states.Count; i++)
                                {
                                    if (currentFSM.states[i]!= currentFSM.transitionPreview.state && currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                    {
                                        currentFSM.overNode = true;
                                        if (currentFSM.wantConnection)
                                        {
                                            currentFSM.ConnectToState(currentFSM.states[i]);
                                        }

                                        break;
                                    }
                                }
                            }

                            if (!currentFSM.overNode && !inDragNode && !inDrawRect)
                            {
                                currentFSM.wantConnection = false;
                            }
                        }
                        if(inDragNode)
                        {
                            inDragNode = false;
                            if (selectedStates.Count > 0)
                            {
                                for (int i = 0; i < selectedStates.Count; i++)
                                {
                                    if (selectedStates[i].isSelected)
                                    {
                                        selectedStates[i].OnEndDrag();
                                    }
                                }
                            }
                        }
                       
                        inDrawRect = false;

                    }
                }
                if (e.button == 1 && !e.alt)
                {
                    if (e.type == EventType.MouseUp)
                    {
                        currentFSM.overNode = false;
                        if (currentFSM != null)
                        {
                            if (currentFSM.states.Count > 0)
                            {
                                for (int i = 0; i < currentFSM.states.Count; i++)
                                {
                                    if (currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                    {
                                        currentFSM.overNode = true; break;
                                    }
                                }
                            }
                        }

                    }

                    if (e.type == EventType.MouseDown)
                    {
                        currentFSM.overNode = false;

                        var validNode = false;
                        if (currentFSM != null)
                        {
                            if (currentFSM.states.Count > 0)
                            {
                                for (int i = 0; i < currentFSM.states.Count; i++)
                                {
                                    if (currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                    {
                                        if (currentFSM.states[i] is vFSMState) validNode = true;
                                        selectedNodeID = i;
                                        currentFSM.overNode = true; break;
                                    }
                                }
                            }
                        }
                        if (validNode || !currentFSM.overNode)
                            ProcessContexMenu(e, currentFSM.overNode ? 1 : 0);
                    }


                }
                if (e.button == 2 || e.alt && e.button==0)
                {
                    if (e.type == EventType.MouseDrag)
                    {
                        if (currentFSM != null)
                        {
                            if (currentFSM.states.Count > 0)
                            {
                                currentFSM.overNode = false;
                                for (int i = 0; i < currentFSM.states.Count; i++)
                                {
                                    if (currentFSM.states[i].nodeRect.Contains(e.mousePosition))
                                    {
                                        selectedNodeID = i;
                                        currentFSM.overNode = true; break;
                                    }
                                }
                            }
                        }
                        if (!currentFSM.overNode)
                        {
                            indrag = true;
                            var delta = e.delta;// RoundedDelta(e.delta);
                            currentFSM.panOffset.x += delta.x;
                            currentFSM.panOffset.y += delta.y;
                            for (int i = 0; i < currentFSM.states.Count; i++)
                            {

                                currentFSM.states[i].OnDrag(delta,false);
                            }
                        }
                    }
                    if (e.type == EventType.MouseUp && indrag)
                    {
                        indrag = false;
                        for (int i = 0; i < currentFSM.states.Count; i++)
                        {

                            currentFSM.states[i].OnEndDrag();
                        }
                    }
                   

                       
                }
                if (e.isKey && e.keyCode == KeyCode.F)
                {
                    CenterView(currentFSM);
                }
                if (indrag)
                    EditorGUIUtility.AddCursorRect(new Rect(e.mousePosition.x - 50, e.mousePosition.y - 50, 100, 100), MouseCursor.Pan);
            }
            else if (currentFSM)
            {
                GUILayout.BeginArea(viewRect);
                currentFSM.overNode = false;

                GUILayout.EndArea();
                if (e.type == EventType.MouseUp && viewRect.Contains(e.mousePosition))
                {
                    inDrawRect = false;
                    inDragNode = false;
                }
            }
        }

        public void CenterView(vFSMBehaviour behaviour)
        {
            behaviour.panOffset.x = behaviour.states[0].nodeRect.x;
            behaviour.panOffset.y = behaviour.states[0].nodeRect.y;
            var x = behaviour.states[0].nodeRect.x;
            var y = behaviour.states[0].nodeRect.y;
            Vector2 position = new Vector2(x, y);
            behaviour.states[0].nodeRect.x = this.viewRect.position.x + this.viewRect.width / 2;
            behaviour.states[0].nodeRect.y = 100;
            if (behaviour.states.Count > 1)
            {
                for (int i = 1; i < behaviour.states.Count; i++)
                {
                    var diferencePosition = behaviour.states[i].nodeRect.position - position;
                    var newPosition = behaviour.states[0].nodeRect.position + diferencePosition;
                    behaviour.states[i].nodeRect.position = newPosition;
                }
            }
        }

        #endregion

        #region Utility Methods
        void ProcessContexMenu(Event e, int contexID)
        {
            GenericMenu menu = new GenericMenu();
            if (contexID == 0)
            {
                if (currentFSM != null)
                {
                    menu.AddItem(new GUIContent("New Node"), false, () => { vNodeUtility.CreateNode(currentFSM, GUIUtility.ScreenToGUIPoint(mousePosition) ); });
                }
            }
            if (contexID == 1)
            {
                if (currentFSM != null)
                {
                    Undo.RegisterCompleteObjectUndo(currentFSM, "Delete Node");
                    Undo.RecordObjects(currentFSM.states.ToArray(), "Delete Nodes");
                    if (currentFSM.states[selectedNodeID].useDecisions)
                        menu.AddItem(new GUIContent("New Transition"), false, () => { currentFSM.states[selectedNodeID].AddNewTransition();});
                    if (currentFSM.states[selectedNodeID].canSetAsDefault)
                        menu.AddItem(new GUIContent("Set as Default Node"), false, () => { currentFSM.states[0].defaultTransition = currentFSM.states[selectedNodeID] as vFSMState; });
                    if (currentFSM.states[selectedNodeID].canRemove)
                        menu.AddItem(new GUIContent("Delete Node"), false, () => { vNodeUtility.DeleteNode(selectedNodeID, currentFSM); foreach (var state in selectedStates) vNodeUtility.DeleteNode(currentFSM.states.IndexOf(state), currentFSM); });

                    menu.ShowAsContext();
                }
            }
            menu.ShowAsContext();
            e.Use();

        }
        void OnSelectState(vFSMState state)
        {
            Event e = Event.current;
            if (!e.shift && !inDrawRect)
                state.parentGraph.DeselectAll();
            if (!state.isSelected)
                state.isSelected = true;
            if (Selection.activeObject != state) Selection.activeObject = state;
            if (!selectedStates.Contains(state))
            {
                selectedStates.Add(state);
            }
        }
        public Vector2 offset;
        public IEnumerable<Type> FindSubClassesOfNode<T>()
        {
            IEnumerable<Type> exporters = typeof(T)
             .Assembly.GetTypes()
             .Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            return exporters;
        }
        #endregion
    }
}