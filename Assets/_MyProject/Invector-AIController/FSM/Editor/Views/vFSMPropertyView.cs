using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vFSMPropertyView : FSMViewBase
    {
        public vFSMPropertyView() : base("FSM Property View") { }
        public string[] toolBar = new string[] { "Parameters", "Components" };
        public int selected;
        public Vector2 componentsScrollView, parametersScrollView;
        public SerializedObject curGraphSerialized;
        
        public override void UpdateView(Event e, vFSMBehaviour curGraph)
        {
            if ((!Application.isPlaying || Selection.activeGameObject == null) && curGraph)
            {
                if (curGraphSerialized == null || curGraphSerialized.targetObject != curGraph) curGraphSerialized = new SerializedObject(curGraph);
            }
            else if (Application.isPlaying && Selection.activeGameObject != null)
            {
                var fsmBehaviour = Selection.activeGameObject.GetComponent<vIFSMBehaviourController>();
                if (fsmBehaviour != null)
                {
                    List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
                    Selection.activeGameObject.GetComponents<MonoBehaviour>(monoBehaviours);
                    if (monoBehaviours.Count > 0)
                    {
                        var monoFSM = monoBehaviours.Find(m => m is vIFSMBehaviourController);
                        if (monoFSM) curGraphSerialized = new SerializedObject(monoFSM);
                        else if (curGraphSerialized == null || curGraphSerialized.targetObject != curGraph) curGraphSerialized = new SerializedObject(curGraph);
                    }

                }
                else if (curGraph)
                {
                    if (curGraphSerialized == null || curGraphSerialized.targetObject != curGraph) curGraphSerialized = new SerializedObject(curGraph);
                }
            }
            else if (curGraph)
            {
                if (curGraphSerialized == null || curGraphSerialized.targetObject != curGraph) curGraphSerialized = new SerializedObject(curGraph);
            }
            base.UpdateView(e, curGraph);
            GUI.Box(viewRect, "", viewSkin.GetStyle("ToolBar"));
            var toolbarRect = viewRect;
            toolbarRect.height = 20;
            // selected = GUI.Toolbar(toolbarRect,selected, toolBar,viewSkin.GetStyle("ToolBar"));
            var rectoffset = viewRect;
            // rectoffset.y += 20;
            rectoffset.x += 5;
            rectoffset.width -= 10;
            rectoffset.height -= 20;
            GUILayout.BeginArea(rectoffset);
            {
                if (curGraph)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal(viewSkin.box, GUILayout.Height(20));
                    GUILayout.Label("FSM COMPONENTS", viewSkin.GetStyle("LabelHeader"));
                    if (GUILayout.Button("+", viewSkin.box, GUILayout.Width(20), GUILayout.ExpandHeight(true)))
                    {
                        var rect = GUILayoutUtility.GetLastRect();
                        AddComponentContext(curGraph, rect);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    componentsScrollView = GUILayout.BeginScrollView(componentsScrollView);
                    GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(2), GUILayout.ExpandWidth(true));
                    GUILayout.Space(10);
                    GUILayout.BeginVertical(viewSkin.box);
                    GUILayout.Label("Actions", viewSkin.GetStyle("LabelHeader"), GUILayout.ExpandWidth(true)); GUILayout.Space(5);
                    if (curGraph.actions.Count > 0)
                    {
                        for (int a = 0; a < curGraph.actions.Count; a++)
                        {
                            if (curGraph.actions[a])
                            {
                                curGraph.actions[a].OnInspectorGUI();
                                var rect = GUILayoutUtility.GetLastRect();
                                rect.height = 15;
                                if (rect.Contains(e.mousePosition) && e.type != EventType.ContextClick)
                                {
                                    if (e.button == 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        int index = a;
                                        var refList = curGraph.actions;
                                        var action = curGraph.actions[a].target as vStateAction;
                                        menu.AddItem(new GUIContent("Delete"), false, () => { DeleteObjet(ref refList, index, GetStatesUsingAction(action)); e.Use(); });
                                        menu.ShowAsContext();
                                    }
                                }

                                var tooltip = GetStatesUsingAction(curGraph.actions[a].target as vStateAction);
                                bool isInUse = !String.IsNullOrEmpty(tooltip);
                                var _color = GUI.color;
                                GUI.color = isInUse ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 0.1f);
                                if (!isInUse)
                                {
                                    tooltip = "Not being used";
                                }
                                var _checkerRect = rect;
                                _checkerRect.x = viewRect.width - 25 + componentsScrollView.x;
                                _checkerRect.width = 10;
                                _checkerRect.height = 10;
                                GUI.Toggle(_checkerRect, false, new GUIContent("", tooltip), EditorStyles.radioButton);
                                GUI.color = _color;
                            }
                            else
                            {
                                curGraph.actions.RemoveAt(a);
                            }
                        }
                    }
                    else GUILayout.Box("NONE", viewSkin.box, GUILayout.ExpandWidth(true));
                    GUILayout.EndVertical();
                    GUILayout.Space(10);
                    GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(2), GUILayout.ExpandWidth(true));
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(viewSkin.box);
                    GUILayout.Box("Decisions", viewSkin.GetStyle("LabelHeader"), GUILayout.ExpandWidth(true)); GUILayout.Space(5);
                    if (curGraph.decisions.Count > 0)
                    {
                        for (int a = 0; a < curGraph.decisions.Count; a++)
                        {
                            if (curGraph.decisions[a])
                            {
                                curGraph.decisions[a].OnInspectorGUI();
                                var rect = GUILayoutUtility.GetLastRect();
                                rect.height = 15;
                                if (rect.Contains(e.mousePosition) && e.type != EventType.ContextClick)
                                {
                                    if (e.button == 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        int index = a;
                                        var refList = curGraph.decisions;
                                        var decision = curGraph.decisions[a].target as vStateDecision;
                                        menu.AddItem(new GUIContent("Delete"), false, () => { DeleteObjet(ref refList, index, GetStatesUsingDecision(decision)); e.Use(); });
                                        menu.ShowAsContext();
                                    }
                                }
                                var tooltip = GetStatesUsingDecision(curGraph.decisions[a].target as vStateDecision);
                                bool isInUse = !String.IsNullOrEmpty(tooltip);
                                var _color = GUI.color;
                                GUI.color = isInUse ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 0.1f);
                                if (!isInUse)
                                {
                                    tooltip = "Not being used";
                                }
                                var _checkerRect = rect;
                                _checkerRect.x = viewRect.width - 25 + componentsScrollView.x;
                                _checkerRect.width = 10;
                                _checkerRect.height = 10;
                                GUI.Toggle(_checkerRect, false, new GUIContent("", tooltip), EditorStyles.radioButton);
                                GUI.color = _color;
                            }
                            else
                            {
                                curGraph.decisions.RemoveAt(a);
                                break;
                            }
                        }
                    }
                    else GUILayout.Box("NONE", viewSkin.box, GUILayout.ExpandWidth(true));
                    GUILayout.EndVertical();

                    GUILayout.EndScrollView();
                }
            }
            GUILayout.EndArea();
        }

        string GetStatesUsingAction(vStateAction action)
        {
            if (!action) return string.Empty;
            var states = currentFSM.states.FindAll(state => state != null && state.actions != null && state.actions.Exists(a => a != null && a.Equals(action)));
            if (states == null || states.Count == 0) return string.Empty;
            StringBuilder text = new StringBuilder();


            text.Append(action.name + " is being used in " + states.Count + " state(s)\n");
            for (int i = 0; i < states.Count; i++)
            {
                text.Append(states[i].Name);
                if (i < states.Count - 1) text.Append(" , ");
            }

            return text.ToString();
        }

        string GetStatesUsingDecision(vStateDecision decision)
        {
            if (!decision) return string.Empty;
            var states = currentFSM.states.FindAll(state => state != null && state.transitions != null && state.transitions.Exists(t => t != null && t.decisions != null && t.decisions.Exists(d => d != null && d.decision.Equals(decision))));
            if (states == null || states.Count == 0) return string.Empty;
            StringBuilder text = new StringBuilder();

            text.Append(decision.Name + " is being used in " + states.Count + " state(s)\n");
            for (int i = 0; i < states.Count; i++)
            {
                text.Append(states[i].Name);
                if (i < states.Count - 1) text.Append(" , ");
            }

            return text.ToString();
        }

        void AddComponentContext(vFSMBehaviour graph, Rect rect)
        {
            try
            {
                GenericMenu menu = new GenericMenu();
                List<GenericMenuItem> menuItems = new List<GenericMenuItem>();
                var possibleActions = typeof(vStateAction).FindSubClasses();
                menu.AddItem(new GUIContent("Action/New Action Script"), false, () => { vNodeMenus.CreateNewAction(); });
                menu.AddSeparator("Action/");
               
                foreach (var type in possibleActions)
                {
                    var instance = (vStateAction)ScriptableObject.CreateInstance(type.FullName);
                    if (instance)
                        menuItems.Add(new GenericMenuItem(new GUIContent("Action/" + (instance.categoryName) + (instance.defaultName)), () => { AddAction(graph, type); if (instance) GameObject.DestroyImmediate(instance); }));                    
                }
                menuItems.Sort((x, y) => string.Compare(x.content.text, y.content.text));
                foreach (var item in menuItems)
                {
                    menu.AddItem(item.content, false, item.func);
                }
                menuItems.Clear();


                menu.AddItem(new GUIContent("Decision/New Decision Script"), false, () => { vNodeMenus.CreateNewDecision(); });
                menu.AddSeparator("Decision/");
                var possibleDecisions = typeof(vStateDecision).FindSubClasses();
                foreach (var type in possibleDecisions)
                {
                    var instance = (vStateDecision)ScriptableObject.CreateInstance(type.Name);
                    if (instance)
                        menuItems.Add(new GenericMenuItem(new GUIContent("Decision/" + (instance.categoryName) + (instance.defaultName)), () => { AddTransition(graph, type); if (instance) GameObject.DestroyImmediate(instance); }));
                }
                menuItems.Sort((x, y) => string.Compare(x.content.text, y.content.text));
                foreach (var item in menuItems)
                {
                    menu.AddItem(item.content, false, item.func);
                }

                menu.ShowAsContext();
            }
            catch (UnityException e) { Debug.LogWarning("Add FSM Compornent Error :\n" + e.Message, currentFSM); }
        }

        public void AddTransition(vFSMBehaviour curGraph, Type type)
        {
            if (curGraph != null && type != null)
            {
                var decision = ScriptableObject.CreateInstance(type) as vStateDecision;
                decision.name = decision.defaultName;
                if (decision != null)
                {
                    decision.hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(decision, curGraph);
                    curGraph.OnChangeChilds();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public void AddAction(vFSMBehaviour curGraph, Type type)
        {
            if (curGraph != null && type != null)
            {
                var action = ScriptableObject.CreateInstance(type) as vStateAction;
                action.name = action.defaultName;
                if (action != null)
                {
                    action.hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(action, curGraph);
                    curGraph.OnChangeChilds();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public void DeleteObjet(ref List<Editor> editorList, int index, string aditionalText)
        {
            editorList[index].serializedObject.Update();

            if (EditorUtility.DisplayDialog("Delete " + editorList[index].target.name + " Component", "Are you sure you want to remove this component?" + "\n" + aditionalText, "OK", "Cancel"))
            {
                GameObject.DestroyImmediate(editorList[index].target, true); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
                editorList.RemoveAt(index);
            }
        }

    }
}