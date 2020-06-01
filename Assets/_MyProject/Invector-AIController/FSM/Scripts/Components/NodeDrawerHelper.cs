#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public static class NodeDrawerHelper
    {
        public static void InitNode(this vFSMState state)
        {
            state.nodeRect.width = 150;
            state.nodeRect.height = 30;
            state.positionRect = state.nodeRect.position;
            state.transitions = new List<vStateTransition>();
        }

        /// <summary>
        /// Update Node GUI for Node
        /// </summary>
        /// <param name="state"></param>
        /// <param name="e"></param>
        /// <param name="viewRect"></param>
        /// <param name="viewSkin"></param>
        public static void UpdateNodeGUI(this vFSMState state, Event e, Rect viewRect, GUISkin viewSkin)
        {
            var color = GUI.color;
            var stateIndex = state.parentGraph.states.IndexOf(state);
            GUI.color = (stateIndex > 1 ? state.nodeColor : Color.white);
            if (!state.inDrag && !state.resizeLeft && !state.resizeRight)
            {
                state.positionRect.x = state.nodeRect.position.x.NearestRound(vFSMHelper.dragSnap);
                state.positionRect.y = state.nodeRect.position.y.NearestRound(vFSMHelper.dragSnap);
                state.nodeRect.position = state.positionRect;
            }
            if (!state.resizeLeft && !state.resizeRight)
            {
                state.rectWidth = state.nodeRect.width.NearestRound(vFSMHelper.dragSnap);
                state.nodeRect.width = state.rectWidth;
            }
            vIFSMBehaviourController fsmBehaviour = (Selection.activeGameObject ? Selection.activeGameObject.GetComponent<vIFSMBehaviourController>() : null);

            if (fsmBehaviour != null)
            {
                var controller = (Selection.activeGameObject ? Selection.activeGameObject.GetComponent<vIControlAI>() : null);
                if (controller != null)
                {
                    var interfaces = controller.GetType().GetInterfaces();
                    bool contains = false;
                    for (int i = 0; i < interfaces.Length; i++)
                    {
                        if (interfaces[i].Equals(state.requiredType))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains)
                        Debug.Log("REQUIRED TYPE OF CONTROLLER IS " + state.requiredType.Name);
                }
            }
            bool isRunningInPlayMode = fsmBehaviour != null && Application.isPlaying && fsmBehaviour.fsmBehaviour && fsmBehaviour.fsmBehaviour.states.Contains(state) && fsmBehaviour.fsmBehaviour.states.IndexOf(state) == fsmBehaviour.indexOffCurrentState;

            var nodeBackgroundStyle = viewSkin.GetStyle("NodeDefault");
            var nodeBorderStyle = viewSkin.GetStyle("NodeBorder");
            if ((!Application.isPlaying && state.parentGraph && state.parentGraph.states[0] && state.parentGraph.states[0].defaultTransition == state) || isRunningInPlayMode)
            {

                var shadowRect = new Rect(state.nodeRect.x - 5, state.nodeRect.y - 5, state.nodeRect.width + 10, state.nodeRect.height + 10);
                if (isRunningInPlayMode && fsmBehaviour != null && !fsmBehaviour.isStopped && fsmBehaviour.aiController != null && (fsmBehaviour.aiController as MonoBehaviour).enabled)
                {
                    var t = EditorPrefs.GetFloat("vStateBorderTimer", 1f);
                    t += 0.01f;
                    GUI.color *= Mathf.Clamp(Mathf.PingPong(t, 1f), 0.5f, 1f);
                    EditorPrefs.SetFloat("vStateBorderTimer", t);
                }
                else if (EditorPrefs.HasKey("vStateBorderTimer")) EditorPrefs.DeleteKey("vStateBorderTimer");
                GUI.Box(shadowRect, "", viewSkin.GetStyle("Glow"));
            }

            GUI.SetNextControlName(state.name);
            var borderWidth = state.isSelected ? 5 : 0;
            var borderRect = new Rect(state.nodeRect.x - borderWidth, state.nodeRect.y - (borderWidth), state.nodeRect.width + (borderWidth * 2), state.nodeRect.height + (borderWidth * 2));
            GUI.color = (stateIndex > 1 ? state.nodeColor : Color.white);
            GUI.color *= vFSMBehaviourPreferences.borderAlpha;
            GUI.Box(borderRect, "", nodeBorderStyle);
            if (stateIndex > 1) GUI.color = state.isSelected ? vFSMBehaviourPreferences.selectedStateColor : vFSMBehaviourPreferences.defaultStateColor;
            else GUI.color = state.isSelected ? (stateIndex == 1 ? vFSMBehaviourPreferences.anySelectedColor : vFSMBehaviourPreferences.entrySelectedColor) : (stateIndex == 1 ? vFSMBehaviourPreferences.anyNormalColor : vFSMBehaviourPreferences.entryNormalColor);

            GUI.Box(state.nodeRect, "", nodeBackgroundStyle);
            GUI.color = color;


            GUILayout.BeginArea(state.nodeRect);
            {
                try
                {
                    var style = new GUIStyle(nodeBackgroundStyle);
                    style.normal.background = null;
                    style.hover.background = null;
                    style.active.background = null;
                    style.alignment = TextAnchor.MiddleCenter;
                    if (stateIndex > 1)
                        style.normal.textColor = state.isSelected ? vFSMBehaviourPreferences.selectedStateFontColor : vFSMBehaviourPreferences.defaultStateFontColor;
                    else
                    {
                        if (stateIndex == 0) style.normal.textColor = state.isSelected ? vFSMBehaviourPreferences.entrySelectedFontColor : vFSMBehaviourPreferences.entryNormalFontColor;
                        else if (stateIndex == 1) style.normal.textColor = state.isSelected ? vFSMBehaviourPreferences.anySelectedFontColor : vFSMBehaviourPreferences.anyNormalFontColor;
                    }
                    GUILayout.Label(new GUIContent(state.Name, state.description), style, GUILayout.Height(30));
                }
                catch { }

            }
            GUILayout.EndArea();
            state.UpdateStateGUI(e, viewRect, viewSkin);
            EditorUtility.SetDirty(state);

        }

        public static float NearestRound(this float x, float delX)
        {
            float rem = x % delX;
            return rem >= 5 ? (x - rem + delX) : (x - rem);
        }

        /// <summary>
        /// Update Node GUI for vFSMState
        /// </summary>
        /// <param name="state"></param>
        /// <param name="e"></param>
        /// <param name="viewRect"></param>
        /// <param name="viewSkin"></param>
        public static void UpdateStateGUI(this vFSMState state, Event e, Rect viewRect, GUISkin viewSkin)
        {
            var color = GUI.color;
            GUI.color = Color.white;

            if ((state.useDecisions) && state.transitions.Count > 0)
            {
                var foldoutRect = new Rect(state.nodeRect.x + 10f, state.nodeRect.y + 5f, 20, 20);
                state.isOpen = EditorGUI.Toggle(foldoutRect, state.isOpen, viewSkin.GetStyle("FoldoutClean"));
            }
            else
                state.isOpen = false;
            GUI.color = color;
            if (state.useDecisions)
                state.DrawTransitionHandles(e, viewRect, viewSkin);

            var resizeLeft = state.nodeRect;
            var resizeRight = state.nodeRect;
            resizeLeft.width = 2;
            resizeRight.width = 2;
            resizeLeft.x -= 2;
            resizeRight.x += state.nodeRect.width;
            state.Resize(resizeLeft, e, ref state.resizeLeft, true);
            state.Resize(resizeRight, e, ref state.resizeRight, false);
        }

        public static void Resize(this vFSMState state, Rect rect, Event e, ref bool inResize, bool left = false)
        {
            if (!inResize)
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
            else EditorGUIUtility.AddCursorRect(new Rect(e.mousePosition.x - 25, e.mousePosition.y - 25, 50, 50), MouseCursor.ResizeHorizontal);
            if (rect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    inResize = true;
                }
            }

            if (e.type == EventType.MouseUp)
            {
                inResize = false;
                // resizingPropView = false;
            }
            if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0)
                {
                    if (inResize)
                    {
                        if (state.nodeRect.width <= 400 && state.nodeRect.width >= 100)
                        {
                            if (left)
                            {
                                if ((state.nodeRect.width - e.delta.x) <= 400 && (state.nodeRect.width - e.delta.x) >= 100)
                                {
                                    state.rectWidth += -e.delta.x;
                                    state.positionRect.x += e.delta.x;

                                }
                            }
                            else if ((state.nodeRect.width + e.delta.x) <= 400 && (state.nodeRect.width + e.delta.x) >= 100)
                            {
                                state.rectWidth += e.delta.x;
                            }
                        }
                        else if (state.nodeRect.width < 100)
                        {
                            state.rectWidth = 100;
                        }
                        else if (state.nodeRect.width > 400)
                        {
                            state.rectWidth = 400;
                        }

                        state.nodeRect.width = state.rectWidth.NearestRound(vFSMHelper.dragSnap);
                        state.nodeRect.x = state.positionRect.x.NearestRound(vFSMHelper.dragSnap);
                        e.Use();
                    }
                }
            }
        }

        static void DrawTransitionHandles(this vFSMState state, Event e, Rect viewRect, GUISkin viewSkin)
        {
            var color = GUI.color;

            if (state.transitions.Count > 0)
            {
                Vector2 transitionSize = new Vector2(10f, 10f);
                float space = state.isOpen ? 2f : 0;
                float height = state.isOpen ? ((((transitionSize.y * 2) * state.transitions.Count)) + (state.transitions.Count > 0 ? space * state.transitions.Count : 0) + 30) + 10 : 30;
                state.nodeRect.height = height;
                var labelStyle = new GUIStyle(EditorStyles.whiteMiniLabel);
                labelStyle.alignment = TextAnchor.MiddleCenter;

                for (int i = 0; i < state.transitions.Count; i++)
                {
                    var nullDecisions = state.transitions[i].decisions.FindAll(t => t.decision == null);

                    for (int iNull = 0; iNull < nullDecisions.Count; iNull++) state.transitions[i].decisions.Remove(nullDecisions[iNull]);
                    bool trueRightSide = state.transitions[i].trueState ? state.nodeRect.x > state.transitions[i].trueState.nodeRect.x ? false : true : true;
                    bool falseRightSide = state.transitions[i].falseState ? state.nodeRect.x > state.transitions[i].falseState.nodeRect.x ? false : true : true;
                    var decisionRect = new Rect(state.nodeRect.x + 5f, state.transitions[i].trueRect.y, state.nodeRect.width - 10, transitionSize.y * 2);
                    if (state.isOpen)
                    {
                        state.transitions[i].trueSideRight = trueRightSide;
                        state.transitions[i].falseSideRight = falseRightSide;
                        state.transitions[i].trueRect.width = transitionSize.x;
                        state.transitions[i].trueRect.height = transitionSize.y;
                        state.transitions[i].falseRect.width = transitionSize.x;
                        state.transitions[i].falseRect.height = transitionSize.y;

                        state.transitions[i].trueRect.x = trueRightSide ? ((state.nodeRect.x) + state.nodeRect.width) : (state.nodeRect.x) - transitionSize.x;
                        state.transitions[i].trueRect.y = state.isOpen ? ((state.nodeRect.y + ((transitionSize.y * 2) * i)) + (i > 0 ? space * i : 0) + 30) : (state.nodeRect.y + 15) - transitionSize.y;
                        state.transitions[i].falseRect.x = falseRightSide ? (state.nodeRect.x + state.nodeRect.width) : state.nodeRect.x - transitionSize.x;
                        state.transitions[i].falseRect.y = state.isOpen ? ((state.nodeRect.y + (transitionSize.y + ((transitionSize.y * 2) * i))) + (i > 0 ? space * i : 0) + 30) : transitionSize.y + (state.nodeRect.y + 15) - transitionSize.y;
                    }
                    else
                    {
                        state.transitions[i].trueRect.size = Vector2.zero;
                        state.transitions[i].trueRect.x = state.nodeRect.center.x;
                        state.transitions[i].trueRect.y = state.nodeRect.center.y;
                        state.transitions[i].falseRect.size = Vector2.zero;
                        state.transitions[i].falseRect.x = state.nodeRect.center.x;
                        state.transitions[i].falseRect.y = state.nodeRect.center.y;
                    }

                    GUI.color = color * 0.5f;

                    ///Transition selector
                    {
                        if (state.isOpen)
                        {
                            GUI.enabled = state.selectedDecisionIndex == i;
                            GUILayout.BeginArea(decisionRect, "", EditorStyles.helpBox);
                            {
                                GUI.color = color;
                                try
                                {
                                    var text = "";
                                    if (trueRightSide && falseRightSide || (!trueRightSide && !falseRightSide) || (!trueRightSide && falseRightSide))
                                    {
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse || state.transitions[i].transitionType == vTransitionOutputType.Default) text += (state.transitions[i].trueState ? state.transitions[i].trueState.name : "None");
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse) text += " || ";
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse) text += (state.transitions[i].falseState ? state.transitions[i].falseState.name : "None");
                                    }
                                    else if (trueRightSide && !falseRightSide)
                                    {
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse) text += (state.transitions[i].falseState ? state.transitions[i].falseState.name : "None");
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse) text += " || ";
                                        if (state.transitions[i].transitionType == vTransitionOutputType.TrueFalse || state.transitions[i].transitionType == vTransitionOutputType.Default) text += (state.transitions[i].trueState ? state.transitions[i].trueState.name : "None");
                                    }
                                    GUILayout.Space(-transitionSize.y / 2);

                                    GUILayout.Label(text, labelStyle);
                                }
                                catch { }
                            }
                            GUILayout.EndArea();
                            GUI.enabled = true;

                            if ( GUI.Button(decisionRect, "", GUIStyle.none) && viewRect.Contains(e.mousePosition))
                            {
                                state.parentGraph.onSelectState(state);
                                state.selectedDecisionIndex = i;
                                state.transitions[i].Select();
                            }

                            if ( decisionRect.Contains(e.mousePosition) && viewRect.Contains(e.mousePosition))
                            {
                                if (e.button == 1)
                                {
                                    GenericMenu menu = new GenericMenu();
                                    var transition = new vStateTransition(null);

                                    for (int a = 0; a < state.transitions[i].decisions.Count; a++)
                                    {
                                        transition.decisions.Add(state.transitions[i].decisions[a].Copy());
                                    }
                                    int index = i;
                                    var transitionToChange = state.transitions[i];
                                    menu.AddItem(new GUIContent("Remove"), false, () =>
                                    {
                                        transitionToChange.parentState.transitions.RemoveAt(index); e.Use();
                                    });

                                    menu.AddItem(new GUIContent("Duplicate"), false, () =>
                                    {
                                        state.transitions.Add(transition);
                                        SerializedObject serializedObject = new SerializedObject(state);
                                        serializedObject.ApplyModifiedProperties();
                                        e.Use();
                                    });

                                    menu.ShowAsContext();
                                }
                            }
                        }
                    }
                    ///Output Button
                    {
                        GUI.enabled = true;
                        GUI.color = color;
                        decisionRect.x = decisionRect.x + decisionRect.width;
                        decisionRect.width = 15;
                        decisionRect.height = 15;
                        GUI.color = vFSMBehaviourPreferences.transitionTrueColor;
                        if (state.transitions[i].decisions.Count == 0) GUI.color = vFSMBehaviourPreferences.transitionDefaultColor;
                        if (state.transitions[i].useTruState && state.isOpen)
                        {
                            var matrix = GUI.matrix;
                            if (!trueRightSide)
                            {
                                var pivotPoint = new Vector2(state.transitions[i].trueRect.x + state.transitions[i].trueRect.width / 2, state.transitions[i].trueRect.y + state.transitions[i].trueRect.height / 2);
                                GUIUtility.RotateAroundPivot(180, pivotPoint);
                            }
                            GUI.Box(state.transitions[i].trueRect, "", viewSkin.GetStyle("InputButton"));
                            if (state.isOpen && state.transitions[i].trueRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                            {
                                if (e.button == 0)
                                {
                                    state.parentGraph.wantConnection = true;
                                    state.parentGraph.transitionPreview.sideRight = state.transitions[i].trueSideRight;
                                    state.parentGraph.transitionPreview.transitionRect = state.transitions[i].trueRect;
                                    state.parentGraph.transitionPreview.state = state;
                                    state.parentGraph.transitionPreview.onValidate = state.transitions[i].SetTrueState;
                                }
                            }
                            GUI.matrix = matrix;
                        }

                        GUI.color = vFSMBehaviourPreferences.transitionFalseColor;
                        if (state.transitions[i].decisions.Count == 0) GUI.color = vFSMBehaviourPreferences.transitionDefaultColor;
                        if (state.transitions[i].useFalseState && state.isOpen)
                        {
                            var matrix = GUI.matrix;
                            if (!falseRightSide)
                            {
                                var pivotPoint = new Vector2(state.transitions[i].falseRect.x + state.transitions[i].falseRect.width / 2, state.transitions[i].falseRect.y + state.transitions[i].falseRect.height / 2);
                                GUIUtility.RotateAroundPivot(180, pivotPoint);
                            }
                            GUI.Box(state.transitions[i].falseRect, "", viewSkin.GetStyle("InputButton"));
                            if (state.transitions[i].falseRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                            {
                                if (e.button == 0)
                                {
                                    state.parentGraph.wantConnection = true;                                  
                                    state.parentGraph.transitionPreview.sideRight = state.transitions[i].falseSideRight;
                                    state.parentGraph.transitionPreview.transitionRect = state.transitions[i].falseRect;
                                    state.parentGraph.transitionPreview.state = state;
                                    state.parentGraph.transitionPreview.onValidate = state.transitions[i].SetFalseState;
                                }
                            }
                            GUI.matrix = matrix;
                        }
                    }
                }
            }
            else state.nodeRect.height = 30;
            GUI.color = color;
        } 
        public static void AddNewTransition(this vFSMState state)
        {
           state.transitions.Add(new vStateTransition(null));
          
           state.parentGraph.wantConnection = true;
            state.parentGraph.transitionPreview.sideRight = null;
           state.parentGraph.transitionPreview.transitionRect = state.nodeRect;
            state.parentGraph.transitionPreview.state = state;
            state.parentGraph.transitionPreview.onValidate = state.transitions[state.transitions.Count - 1].SetTrueState;
        }
        static void AddActionsMenu(this vFSMState state, ref GenericMenu menu)
        {
            List<GenericMenuItem> menuItems = new List<GenericMenuItem>();

            for (int i = 0; i < state.parentGraph.actions.Count; i++)
            {
                if (state.parentGraph.actions[i] && state.parentGraph.actions[i].target)
                {
                    if (state.parentGraph.actions[i].target.GetType().IsSubclassOf(typeof(vStateAction)) || state.parentGraph.actions[i].target.GetType().Equals(typeof(vStateAction)))
                    {
                        var action = state.parentGraph.actions[i].target as vStateAction;
                        menuItems.Add(new GenericMenuItem(new GUIContent("Action/" + state.parentGraph.actions[i].target.name), () =>
                        {

                            state.actions.Add(action);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }));
                    }
                }
            }
            menuItems.Sort((x, y) => string.Compare(x.content.text, y.content.text));
            foreach (var item in menuItems)
            {
                menu.AddItem(item.content, false, item.func);
            }
        }
       
        static void AddDecisionsMenu(this vStateTransition transition, ref GenericMenu menu)
        {
            List<GenericMenuItem> menuItems = new List<GenericMenuItem>();

            for (int i = 0; i < transition.parentState.parentGraph.decisions.Count; i++)
            {
                if (transition.parentState.parentGraph.decisions[i] && transition.parentState.parentGraph.decisions[i].target)
                {
                    if (transition.parentState.parentGraph.decisions[i].target.GetType().IsSubclassOf(typeof(vStateDecision)) || transition.parentState.parentGraph.decisions[i].target.GetType().Equals(typeof(vStateDecision)))
                    {
                        var decision = transition.parentState.parentGraph.decisions[i].target as vStateDecision;
                        menuItems.Add(new GenericMenuItem(new GUIContent("Decision/" + transition.parentState.parentGraph.decisions[i].target.name), () =>
                        {
                            transition.decisions.Add(new vStateDecisionObject(decision));
                            EditorUtility.SetDirty(transition.parentState);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }));
                    }
                }
            }
            menuItems.Sort((x, y) => string.Compare(x.content.text, y.content.text));
            foreach (var item in menuItems)
            {
                menu.AddItem(item.content, false, item.func);
            }
        }

        public static void DrawPrimaryProperties(this vFSMState state, SerializedObject serializedObject, GUISkin viewSkin)
        {
            SerializedProperty description = serializedObject.FindProperty("description");
            if (description != null) EditorGUILayout.PropertyField(description);
            if (state.canEditColor)
            {
                SerializedProperty color = serializedObject.FindProperty("nodeColor");
                if (color != null) EditorGUILayout.PropertyField(color);
            }
            if (state.canEditName)
            {
                SerializedProperty changeCurrentSpeed = serializedObject.FindProperty("changeCurrentSpeed");
                SerializedProperty customSpeed = serializedObject.FindProperty("customSpeed");
                SerializedProperty resetCurrentDestination = serializedObject.FindProperty("resetCurrentDestination");
                if (changeCurrentSpeed != null) EditorGUILayout.PropertyField(changeCurrentSpeed);
                if (customSpeed != null) EditorGUILayout.PropertyField(customSpeed);
                if (resetCurrentDestination != null) EditorGUILayout.PropertyField(resetCurrentDestination);
            }
        }

        public static void DrawProperties(this vFSMState state, SerializedObject serializedObject, GUISkin viewSkin)
        {
            try
            {
                Event e = Event.current;
                state.actions = state.actions.FindAll(a => a != null);
                state.transitions = state.transitions.FindAll(t => t != null);

                if (state.useActions)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginVertical(viewSkin.box);
                    //Draw Actions
                    {
                        GUILayout.Label("State Actions", viewSkin.GetStyle("LabelHeader"), GUILayout.ExpandWidth(true)); GUILayout.Space(5);
                        if (state.actions.Count > 0 && state.parentGraph.actions.Count > 0)
                        {
                            var actionsToDraw = state.parentGraph.actions.FindAll(a => state.actions.Contains(a.target as vStateAction));

                            var rect = new Rect();
                            bool click = false;
                            for (int i = 0; i < state.actions.Count; i++)
                            {
                                state.actions[i].parentFSM = state.parentGraph;
                            }
                            for (int i = 0; i < actionsToDraw.Count; i++)
                            {
                                if (!(actionsToDraw[i] == null || actionsToDraw[i].target == null))
                                {
                                    actionsToDraw[i].OnInspectorGUI();
                                    rect = GUILayoutUtility.GetLastRect();
                                    rect.x = rect.width - EditorGUIUtility.singleLineHeight * 0.4f;
                                    rect.height = EditorGUIUtility.singleLineHeight;
                                    rect.width = EditorGUIUtility.singleLineHeight;
                                    click = GUI.Button(rect, "-", viewSkin.box);
                                    if (rect.Contains(e.mousePosition) && click)
                                    {
                                        if (e.button == 0)
                                        {
                                            GenericMenu menu = new GenericMenu();
                                            int index = state.actions.IndexOf(actionsToDraw[i].target as vStateAction);
                                            menu.AddItem(new GUIContent("Remove"), false, () => { state.actions.RemoveAt(index); e.Use(); });
                                            menu.ShowAsContext();
                                        }
                                    }
                                    click = false;
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    /*Add Actions To State*/
                    {
                        var plusButtonRect = GUILayoutUtility.GetLastRect();
                        plusButtonRect.y += plusButtonRect.height;
                        plusButtonRect.x += plusButtonRect.width - EditorGUIUtility.singleLineHeight;
                        plusButtonRect.width = EditorGUIUtility.singleLineHeight;
                        plusButtonRect.height = EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(plusButtonRect, new GUIContent("+", "Add Decision"), viewSkin.box))
                        {
                            GenericMenu menu = new GenericMenu();
                            AddActionsMenu(state, ref menu);
                            menu.ShowAsContext();
                        }
                    }
                }


                if (state.useDecisions && state.transitions.Count > 0)
                {
                    GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
                    GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(2), GUILayout.ExpandWidth(true));
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    GUILayout.BeginVertical(viewSkin.box);
                    GUILayout.BeginVertical(viewSkin.box);
                    //Draw Transition Selector
                    {
                        GUILayout.Label("State Transitions ", viewSkin.GetStyle("LabelHeader"), GUILayout.ExpandWidth(true)); GUILayout.Space(5);

                        for (int i = 0; i < state.transitions.Count; i++)
                        {
                            GUILayout.BeginVertical("", viewSkin.box);
                            {
                                GUI.enabled = state.selectedDecisionIndex == i;
                                if (!state.transitions[i].parentState) state.transitions[i].parentState = state;

                                state.transitions[i].DrawTransitionSelector(e, viewSkin, (state.selectedDecisionIndex == i));

                                GUI.enabled = true;
                            }
                            GUILayout.EndVertical();
                            var decisionRect = GUILayoutUtility.GetLastRect();
                            if (GUI.Button(decisionRect, "", GUIStyle.none))
                            {
                                if (state.selectedDecisionIndex != i)
                                {
                                    state.selectedDecisionIndex = i;
                                    state.transitions[i].Select();

                                    if (Selection.activeObject != state)
                                        Selection.activeObject = state;
                                }
                                else
                                {
                                    state.transitions[i].Deselect();
                                }
                            }
                        }
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(10);
                        if (state.transitions.Count > 0 && state.selectedDecisionIndex >= 0 && state.selectedDecisionIndex < state.transitions.Count)
                        {
                            state.transitions[state.selectedDecisionIndex].DrawTransitionsProperties(e, viewSkin, true);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
            }
            catch { }
        }

        public static void UpdateNodeConnections(this vFSMState state, Rect viewRect, Event e)
        {
            if (state.useDecisions) state.UpdateStateConnections(viewRect, e);
            else if (state.defaultTransition)
                DrawNodeCurve(e, state.nodeRect, state.defaultTransition.nodeRect, vFSMBehaviourPreferences.transitionDefaultColor);
        }

        static void UpdateStateConnections(this vFSMState state, Rect viewRect, Event e)
        {
            for (int i = state.transitions.Count - 1; i >= 0; i--)
            {
                if (state.transitions[i].trueState && state.transitions[i].useTruState)
                {
                    state.DrawNodeCurve(e, state.transitions[i].trueRect, state.transitions[i].trueState.nodeRect, vFSMBehaviourPreferences.transitionTrueColor, state.transitions[i], true);
                }
                if (state.transitions[i].falseState && state.transitions[i].useFalseState)
                {
                    state.DrawNodeCurve(e, state.transitions[i].falseRect, state.transitions[i].falseState.nodeRect, vFSMBehaviourPreferences.transitionFalseColor, state.transitions[i], false);
                }
            }
            for (int i = 0; i < state.transitions.Count; i++)
            {
                if (state.transitions[i].trueState && state.transitions[i].useTruState)
                {
                    state.DrawNodeCurveSelectable(viewRect, e, state.transitions[i].trueRect, state.transitions[i].trueState.nodeRect, vFSMBehaviourPreferences.transitionTrueColor, state.transitions[i], true);
                }
                if (state.transitions[i].falseState && state.transitions[i].useFalseState)
                {
                    state.DrawNodeCurveSelectable(viewRect, e, state.transitions[i].falseRect, state.transitions[i].falseState.nodeRect, vFSMBehaviourPreferences.transitionFalseColor, state.transitions[i], false);
                }
            }
        }

        static void DrawNodeCurveSelectable(this vFSMState state, Rect viewRect, Event e, Rect start, Rect end, Color color, vStateTransition transition, bool value)
        {
            Handles.BeginGUI();
            Vector3 startPos = Vector3.zero;
            Vector3 endPos = Vector3.zero;
            Vector3 startTan = Vector3.zero;
            Vector3 endTan = Vector3.zero;
            CalculateBezier(start, end, transition, value, ref startPos, ref startTan, ref endPos, ref endTan);
            var dist = (endPos - startPos).magnitude;
            var points = Handles.MakeBezierPoints(startPos, endPos, state.isOpen ? startTan : startPos, state.isOpen ? endTan : endPos, (int)(Mathf.Clamp(dist, 2, 100)));
            var length = (uint)points.Length;
            var transitionCount = state.transitions.FindAll(t => state.transitions.IndexOf(t) > state.transitions.IndexOf(transition) && ((value && t.trueState && t.trueState == transition.trueState) || (!value && t.falseState && t.falseState == transition.falseState)));

            if (!state.isOpen && transitionCount.Count > 0)
            {
                length = (uint)Mathf.Clamp((points.Length - (points.Length * .15f) * (1 + transitionCount.Count)), 2, points.Length);
            }
            #region Debug Selector
            //for (int i = 0; i < length; i++)
            //{
            //    var rect = new Rect(points[i].x - ((i == length - 1) ? 5 : 2.5f), points[i].y - ((i == length - 1) ? 5 : 2.5f), ((i == length - 1) ? 10 : 5f), ((i == length - 1) ? 10 : 5f));

            //    GUI.Box(rect, "-");
            //}
            #endregion
            if (e.type == EventType.MouseDown && !state.parentGraph.overNode && viewRect.Contains(e.mousePosition) && !state.nodeRect.Contains(e.mousePosition))
            {
                var buttom = e.button;
                var selected = false;

                for (int i = 0; i < length; i++)
                {
                    var rect = new Rect(points[i].x - ((i == length - 1) ? 10 : 2.5f), points[i].y - ((i == length - 1) ? 10 : 2.5f), ((i == length - 1) ? 20 : 5f), ((i == length - 1) ? 20 : 5f));
                    if (rect.Contains(e.mousePosition))
                    {
                        selected = true;
                        state.isSelected = true;
                        Selection.activeObject = state;
                        state.parentGraph.selectedNode = state;
                        state.selectedDecisionIndex = state.transitions.IndexOf(transition);
                        state.parentGraph.DeselectAllExcludinCurrent();
                    }
                }

                if (selected)
                {
                    if (buttom == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete"), false, () => { if (value) transition.trueState = null; else transition.falseState = null; });
                        menu.ShowAsContext();
                    }
                    else
                    {
                        if (value)
                        {
                            transition.Select(false, true);
                        }
                        else
                        {
                            transition.Select(true, false);
                        }
                        EditorUtility.SetDirty(state);
                    }
                    e.Use();
                }
            }
            Handles.EndGUI();
        }

        static void DrawNodeCurve(this vFSMState state, Event e, Rect start, Rect end, Color color, vStateTransition transition, bool value)
        {
            //if(decision!=null)
            {
                Handles.BeginGUI();
                Vector3 startPos = Vector3.zero;
                Vector3 endPos = Vector3.zero;
                Vector3 startTan = Vector3.zero;
                Vector3 endTan = Vector3.zero;
                CalculateBezier(start, end, transition, value, ref startPos, ref startTan, ref endPos, ref endTan);
                var dist = (endPos - startPos).magnitude;

                var points = Handles.MakeBezierPoints(startPos, endPos, state.isOpen ? startTan : startPos, state.isOpen ? endTan : endPos, (int)(Mathf.Clamp(dist, 2, 100)));
                var length = (uint)0;
                var transitionCount = state.transitions.FindAll(t => state.transitions.IndexOf(t) > state.transitions.IndexOf(transition) && ((value && t.trueState && t.trueState == transition.trueState) || (!value && t.falseState && t.falseState == transition.falseState)));

                if (!state.isOpen && transitionCount.Count > 0)
                {
                    length = (uint)Mathf.Clamp((points.Length - (points.Length * .15f) * (1 + transitionCount.Count)), 2, points.Length);
                    var pRef = new Vector3[length];
                    for (int i = 0; i < length; i++)
                    {
                        pRef[i] = points[i];
                    }
                    points = pRef;
                }

                if (value && transition.muteTrue || !value && transition.muteFalse) color = vFSMBehaviourPreferences.transitionMuteColor;
                else if (transition.decisions.Count == 0) color = vFSMBehaviourPreferences.transitionDefaultColor;
                var lineWidth = 3;
                var isSelectedLine = (value) ? transition.selectedTrue : transition.selectedFalse;
                if (isSelectedLine)
                {
                    if (value && transition.muteTrue || !value && transition.muteFalse) color = color + Color.cyan * 0.2f;
                    else color = color + Color.cyan;
                    lineWidth = 5;
                }

                var _color = Handles.color;
                Handles.color = color;
                Handles.DrawAAPolyLine(Resources.Load("line") as Texture2D, lineWidth, points);
                DrawArrow(points[Mathf.Clamp(points.Length - 10, 0, points.Length)], points[points.Length - 1], lineWidth, color);
                Handles.color = _color;
                Handles.EndGUI();
            }
        }

        static void DrawNodeCurve(Event e, Rect start, Rect end, Color color)
        {
            //if(decision!=null)
            {
                Handles.BeginGUI();

                Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
                Vector3 endPos = new Vector3(end.x + end.width * 0.5f, end.y + end.height * 0.5f, 0);
                var dist = (endPos - startPos).magnitude;
                Bounds bound = new Bounds(end.center, end.size);
                endPos = bound.ClosestPoint(endPos + (startPos - endPos));
                endPos += (startPos - endPos).normalized * 20f;
                var magniture = Mathf.Clamp(((endPos - startPos).magnitude / 200f) - 0.5f, 0f, 1f);
                Vector3 startTan = startPos;
                var endTanDir = -(endPos - startPos).normalized;
                Vector3 endTan = endPos + endTanDir * (100 * magniture);
                var lineWidth = 4;
                Handles.DrawBezier(startPos, endPos, startTan, endTan, color, Resources.Load("line") as Texture2D, lineWidth);
                DrawArrow(startPos, endPos, lineWidth, color);
                Handles.EndGUI();
            }
        }

        static void CalculateBezier(Rect start, Rect end, vStateTransition transition, bool value, ref Vector3 refStart, ref Vector3 refStartTan, ref Vector3 refEnd, ref Vector3 refEndTan)
        {
            Handles.BeginGUI();
            var sideRight = value ? transition.trueSideRight : transition.falseSideRight;
            Vector3 startPos = new Vector3(start.x + (sideRight ? start.width : 0), start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width * 0.5f, end.y + end.height * 0.5f, 0);
            Bounds bound = new Bounds(value ? transition.trueState.nodeRect.center : transition.falseState.nodeRect.center, value ? transition.trueState.nodeRect.size : transition.falseState.nodeRect.size);
            endPos = bound.ClosestPoint(endPos + (startPos - endPos));
            endPos += (startPos - endPos).normalized * 20f;
            var magniture = Mathf.Clamp(((endPos - startPos).magnitude / 200) - 0.5f, 0f, 1f);
            Vector3 startTan = startPos + (sideRight ? Vector3.right : Vector3.left) * (200 * magniture);
            var endTanDir = -(endPos - startPos).normalized;
            Vector3 endTan = endPos + endTanDir * (100 * magniture);

            refStart = startPos;
            refStartTan = startTan;
            refEnd = endPos;
            refEndTan = endTan;
            Handles.EndGUI();
        }

        static void DrawArrow(Vector3 start, Vector3 end, float lineWidth, Color color)
        {
            var forward = Quaternion.identity * (new Vector3(end.x, 0, end.y) - new Vector3(start.x, 0, start.y));
            Matrix4x4 m = GUI.matrix;
            var _color = GUI.color;

            GUI.color = color;

            var texture = Resources.Load("line_arrow") as Texture2D;
            var width = 8 + lineWidth * 2;
            var pos = new Vector3(end.x - width * 0.5f, end.y - width * 0.5f, 0);

            var angle = forward != Vector3.zero ? new Vector3(0, -(Quaternion.LookRotation(forward).eulerAngles.y - 90), 0).NormalizeAngle().y : 0;

            GUIUtility.RotateAroundPivot(angle, end);
            GUI.DrawTexture(new Rect(pos.x, pos.y, width, width), texture);
            GUI.matrix = m;
            GUI.color = _color;
        }

        public static void OnDrag(this vFSMState state, Vector2 delta, bool snap = true)
        {
            state.inDrag = true;
            if (state.positionRect.magnitude < state.nodeRect.position.magnitude) state.positionRect = state.nodeRect.position;
            if (snap)
            {
                state.positionRect.x += delta.x;
                state.positionRect.y += delta.y;
                state.nodeRect.x = state.positionRect.x.NearestRound(vFSMHelper.dragSnap);
                state.nodeRect.y = state.positionRect.y.NearestRound(vFSMHelper.dragSnap);
            }
            else
            {
                state.nodeRect.x += delta.x;
                state.nodeRect.y += delta.y;
                state.positionRect = state.nodeRect.position;
            }
        }

        public static void OnEndDrag(this vFSMState state)
        {
            state.inDrag = false;
        }

        public static void DrawTransitionSelector(this vStateTransition transition, Event e, GUISkin viewSkin, bool isSelected = false)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    var color = GUI.color;
                    transition.DrawConnectionText(true, true);
                    GUI.color = color;

                    if (GUILayout.Button(new GUIContent("X", "Remove Transition"), viewSkin.box, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        GenericMenu menu = new GenericMenu();
                        int index = transition.parentState.transitions.IndexOf(transition);
                        menu.AddItem(new GUIContent("Remove"), false, () =>
                        {
                            transition.parentState.transitions.RemoveAt(index); e.Use();
                        });
                        menu.ShowAsContext();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                if (transition.decisions.Count > 0)
                {
                    GUILayout.Label("Output", EditorStyles.whiteMiniLabel);
                    transition.transitionType = (vTransitionOutputType)EditorGUILayout.EnumPopup("", transition.transitionType, viewSkin.GetStyle("DropDown"), GUILayout.Width(80), GUILayout.Height(EditorGUIUtility.singleLineHeight * 0.9f));
                }
                else
                {
                    transition.transitionType = vTransitionOutputType.Default;
                    GUILayout.Label(new GUIContent("Output Direct", "If transition dont have decisions, this value is ever true"), EditorStyles.whiteMiniLabel);
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("Transition Delay", EditorStyles.whiteMiniLabel);
                transition.transitionDelay = EditorGUILayout.FloatField("", transition.transitionDelay, GUILayout.Width(50));
                if (GUI.changed) EditorUtility.SetDirty(transition.parentState);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public static void DrawTransitionsProperties(this vStateTransition transition, Event e, GUISkin viewSkin, bool isSelected = false)
        {
            if (isSelected)
            {
                GUILayout.Space(10);
                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(2), GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
                GUILayout.BeginVertical(viewSkin.box);
                {
                    var stl = new GUIStyle(EditorStyles.helpBox);
                    stl.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("Decisions", stl, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    if (transition.decisions.Count > 0)
                    {
                        EditorGUILayout.HelpBox("Decisions will return a True or False result", MessageType.Info);
                        var rect = new Rect();
                        vIFSMBehaviourController fsmBehaviour = (Selection.activeGameObject && Application.isPlaying ? Selection.activeGameObject.GetComponent<vIFSMBehaviourController>() : null);
                        bool isRunningInPlayMode = fsmBehaviour != null && fsmBehaviour.fsmBehaviour;
                        bool click = false;
                        for (int i = 0; i < transition.decisions.Count; i++)
                        {
                            if (transition.decisions[i].decision)
                            {
                                transition.decisions[i].decision.parentFSM = transition.parentState.parentGraph;
                            }

                            var color = GUI.color;
                            if (isRunningInPlayMode)
                            {
                                GUI.color = transition.decisions[i].isValid ? Color.green : Color.red;
                            }
                            transition.decisions[i].DrawDecisionEditor();
                            GUI.color = color;
                            rect = GUILayoutUtility.GetLastRect();
                            rect.x = rect.width - 50;
                            rect.width = 44;
                            rect.y += 2;
                            rect.height = EditorGUIUtility.singleLineHeight;
                            rect.height -= 4;
                            transition.decisions[i].trueValue = (EditorGUI.Popup(rect, transition.decisions[i].trueValue ? 0 : 1, new string[] { "true", "false" }, viewSkin.GetStyle("DropDown")) == 0 ? true : false);

                            rect.y -= 2;
                            rect.height += 4;
                            rect.x += 44;// rect.width - EditorGUIUtility.singleLineHeight * 0.4f;
                            rect.width = EditorGUIUtility.singleLineHeight;
                            click = GUI.Button(rect, "-", viewSkin.box);
                            if (rect.Contains(e.mousePosition) && click)
                            {
                                if (e.button == 0)
                                {
                                    transition.decisions.RemoveAt(i);
                                }
                            }
                            click = false;
                        }
                    }
                }
                GUILayout.EndVertical();

                /*Add Decisions To Transition*/
                {
                    var plusButtonRect = GUILayoutUtility.GetLastRect();
                    plusButtonRect.y += plusButtonRect.height;
                    plusButtonRect.x += plusButtonRect.width - EditorGUIUtility.singleLineHeight;
                    plusButtonRect.width = EditorGUIUtility.singleLineHeight;
                    plusButtonRect.height = EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(plusButtonRect, new GUIContent("+", "Add Decision"), viewSkin.box))
                    {
                        GenericMenu menu = new GenericMenu();
                        transition.AddDecisionsMenu(ref menu);
                        e.Use();
                        menu.ShowAsContext();
                    }
                }
            }
        }

        public static void DrawConnectionText(this vStateTransition transition, bool ignoreTrue = false, bool ignoreFalse = false)
        {
            GUILayout.BeginVertical();
            var fontTransitionStyle = new GUIStyle(UnityEditor.EditorStyles.whiteMiniLabel);
            if (transition.useTruState && (transition.selectedTrue || ignoreTrue))
            {

                GUILayout.BeginHorizontal();
                var color = GUI.color;
                GUI.color = transition.muteTrue ? Color.black : Color.grey;
                transition.muteTrue = GUILayout.Toggle(transition.muteTrue, new GUIContent("", "Mute Transition"), EditorStyles.radioButton, GUILayout.ExpandWidth(false));
                GUI.color = color;
                GUILayout.Label((transition.transitionType == vTransitionOutputType.TrueFalse ? "True: " : "") + transition.parentState.name + "   >>>  " + (transition.trueState && !transition.muteTrue ? transition.trueState.name : "None"), fontTransitionStyle);
                GUILayout.EndHorizontal();
            }

            if (transition.useFalseState && (transition.selectedFalse || ignoreFalse))
            {
                GUILayout.BeginHorizontal();
                var color = GUI.color;
                GUI.color = transition.muteFalse ? Color.black : Color.grey;
                transition.muteFalse = GUILayout.Toggle(transition.muteFalse, new GUIContent("", "Mute Transition"), EditorStyles.radioButton, GUILayout.ExpandWidth(false));
                GUI.color = color;
                GUILayout.Label((transition.transitionType == vTransitionOutputType.TrueFalse ? "False: " : "") + transition.parentState.name + "   >>>  " + (transition.falseState && !transition.muteFalse ? transition.falseState.name : "None"), fontTransitionStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        public static void Deselect(this vStateTransition transition)
        {
            if (!transition.parentState) return;
            for (int i = 0; i < transition.parentState.transitions.Count; i++)
            {
                transition.parentState.transitions[i].selectedFalse = false;
                transition.parentState.transitions[i].selectedTrue = false;
            }
            transition.selectedTrue = false;
            transition.selectedFalse = false;
        }

        public static void Select(this vStateTransition transition, bool selectFalse = true, bool selectTrue = true)
        {
            if (!transition.parentState) return;

            for (int i = 0; i < transition.parentState.transitions.Count; i++)
            {
                transition.parentState.transitions[i].selectedFalse = false;
                transition.parentState.transitions[i].selectedTrue = false;
            }
            transition.selectedTrue = selectTrue ? true : false;
            transition.selectedFalse = selectFalse ? true : false;
        }
    }
}
#endif