using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vFSMBehaviourControllerDebugWindow : EditorWindow
    {
        public vStateWindowDisplay StateWindowDisplay;
        public MonoBehaviour target;
        public Rect viewRect = new Rect();
        public Rect nodeDebugRect = new Rect();
        public Rect debugWindowRect = new Rect();
        public GUISkin fsmSkin;
        public Vector2 nodeDebugScroll, debugMessageScrollView;
        public Vector2 debugWindowScroll;
        public Rect dragFSMSize = new Rect();
        public bool resizingFSMView;
        public float fsmViewOffset;
        public GUIStyle debugLineStyle;
        [SerializeField]
        public GameObject lastValidSelection;
       

        public static void InitEditorWindow()
        {
            var win = GetWindow<vFSMBehaviourControllerDebugWindow>("FSM Debugger", false, typeof(SceneView));
            win.StateWindowDisplay = new vStateWindowDisplay();
            win.viewRect = new Rect(0, 0, win.position.width, win.position.height);
            win.fsmSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
        }

        void CheckComponents()
        {
            if (StateWindowDisplay == null)
                StateWindowDisplay = new vStateWindowDisplay();

            if (!fsmSkin) fsmSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
            try
            {
                var selectedObject = lastValidSelection ? lastValidSelection : Selection.activeGameObject;
                if(Selection.activeGameObject != null && selectedObject && selectedObject != Selection.activeGameObject)
                {
                    selectedObject = Selection.activeGameObject;
                }
                if (selectedObject)
                {
                    if ((lastValidSelection == null || lastValidSelection != selectedObject || target == null) && selectedObject.GetComponent<vIFSMBehaviourController>() != null)
                    {
                        target = selectedObject.GetComponent<vIFSMBehaviourController>() as MonoBehaviour;
                        lastValidSelection = selectedObject;
                    }
                }
            }
            catch { }
        }

        private void OnGUI()
        {
            CheckComponents();

            viewRect = new Rect(0, 0, position.width, position.height);
           debugLineStyle = new GUIStyle(fsmSkin.box);
            this.minSize = new Vector2(200, 100);
            debugWindowRect.y = viewRect.y + 20;
            debugWindowRect.x = position.width - Mathf.Clamp((500 - fsmViewOffset), 200f, position.width);
            debugWindowRect.width = Mathf.Clamp((500 - fsmViewOffset), 200f, position.width);
            debugWindowRect.height = viewRect.height - 20;

            nodeDebugRect.y = viewRect.y + 20;
            nodeDebugRect.height = viewRect.height - 20;
            nodeDebugRect.width = (target != null && target is vIFSMBehaviourController && (target as vIFSMBehaviourController).debugMode) ? position.width - debugWindowRect.width : position.width;

            dragFSMSize.x = debugWindowRect.x - 2.5f;
            dragFSMSize.y = viewRect.y + 20;
            dragFSMSize.width = 5;
            dragFSMSize.height = viewRect.height;

            DrawBackground(viewRect);
            ///State debug window
            GUILayout.BeginArea(nodeDebugRect);
            {
                nodeDebugScroll = GUILayout.BeginScrollView(nodeDebugScroll);
                {
                    if (StateWindowDisplay != null)
                    {

                        if (target != null && target is vIFSMBehaviourController)
                        {
                            StateWindowDisplay.Draw(target as vIFSMBehaviourController, fsmSkin);
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
            ///Debug message window
            if ((target != null && target is vIFSMBehaviourController && (target as vIFSMBehaviourController).debugMode))
            {
                GUILayout.BeginArea(debugWindowRect, fsmSkin.GetStyle("ToolBar"));
                {
                    try
                    {
                        if ((target as vIFSMBehaviourController).debugList != null)
                        {
                            debugWindowScroll = EditorGUILayout.BeginScrollView(debugWindowScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                            {
                                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                                debugLineStyle.richText = true; debugLineStyle.alignment = TextAnchor.MiddleLeft; debugLineStyle.fontSize = 10;
                                debugLineStyle.fontStyle = FontStyle.Normal;
                                for (int i = 0; i < (target as vIFSMBehaviourController).debugList.Count; i++)
                                {
                                    try
                                    {
                                        var height = debugLineStyle.CalcHeight(new GUIContent((target as vIFSMBehaviourController).debugList[i].message), debugWindowRect.width );

                                        if (GUILayout.Button(" " + (target as vIFSMBehaviourController).debugList[i].message, debugLineStyle, GUILayout.Height(height + 10), GUILayout.Width(debugWindowRect.width )))
                                        {
                                            if ((target as vIFSMBehaviourController).debugList[i].sender != null)
                                            {
                                                try
                                                {
                                                    Selection.activeObject = (target as vIFSMBehaviourController).debugList[i].sender;
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                GUILayout.EndVertical();
                            }
                            EditorGUILayout.EndScrollView();
                        }
                    }
                    catch
                    {

                    }
                }
                GUILayout.EndArea();
            }

            if (StateWindowDisplay == null) StateWindowDisplay = new vStateWindowDisplay();
            //Tool bar
            GUILayout.BeginHorizontal(fsmSkin.GetStyle("ToolBar"));

            if (lastValidSelection)
            {

                if (GUILayout.Button(!Application.isPlaying ? "Display of " + lastValidSelection.name + " only Work in Play mode" : lastValidSelection.name, fsmSkin.GetStyle("ToolBar"), GUILayout.Height(20), GUILayout.Width(position.width - debugWindowRect.width)))
                {
                    Selection.activeObject = lastValidSelection;
                }
                GUILayout.Box("Debug Message Window", fsmSkin.GetStyle("ToolBar"), GUILayout.Height(20), GUILayout.Width(debugWindowRect.width));
                GUILayout.Space(-20);
                (target as vIFSMBehaviourController).debugMode = GUILayout.Toggle((target as vIFSMBehaviourController).debugMode, "", fsmSkin.GetStyle("ShowHideToggle"), GUILayout.Width(18), GUILayout.Height(18));
            }
            else
            {
                if (!Application.isPlaying)
                    GUILayout.Box("Select a FSMBeviourController in Play mode to Display FSM Info", fsmSkin.GetStyle("ToolBar"), GUILayout.Height(20), GUILayout.Width(position.width- debugWindowRect.width));
                else GUILayout.Box("Select a FSMBeviourController to Display FSM Info", fsmSkin.GetStyle("ToolBar"), GUILayout.Height(20), GUILayout.Width(position.width - debugWindowRect.width));
            }

            GUILayout.EndHorizontal();
            ProcessEvents(Event.current);
            Repaint();
        }

        void ProcessEvents(Event e)
        {
            if (!resizingFSMView)
                EditorGUIUtility.AddCursorRect(dragFSMSize, MouseCursor.ResizeHorizontal);
            else
                EditorGUIUtility.AddCursorRect(new Rect(e.mousePosition.x - 25, e.mousePosition.y - 25, 50, 50), MouseCursor.ResizeHorizontal);

            if (dragFSMSize.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    resizingFSMView = true;
                }
            }

            if (e.type == EventType.MouseUp)
            {
                resizingFSMView = false;
                // resizingPropView = false;
            }

            if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0)
                {
                    if (resizingFSMView)
                    {
                        fsmViewOffset += e.delta.x;
                        fsmViewOffset = Mathf.Clamp(fsmViewOffset, -500, 250);
                        e.Use();
                    }
                }
            }
        }

        public Texture backgroundTexture;

        public void DrawBackground(Rect viewRect)
        {
            var color = GUI.color;
            GUI.color = vFSMBehaviourPreferences.gridBackgroundColor;
            GUI.Box(viewRect, GUIContent.none);
            if (!backgroundTexture) backgroundTexture = Resources.Load("grid") as Texture;

            if (Event.current.type == EventType.Repaint)
            { // Draw Background when Repainting
              // Offset from origin in tile units

                Vector2 tileOffset = new Vector2((-1) / backgroundTexture.width, 1 / backgroundTexture.height);

                // Amount of tiles
                Vector2 tileAmount = new Vector2(Mathf.Round(viewRect.width * 1) / backgroundTexture.width,
                    Mathf.Round(viewRect.height * 1) / backgroundTexture.height);
                // Draw tiled background
                GUI.color = vFSMBehaviourPreferences.gridLinesColor;
                GUI.DrawTextureWithTexCoords(viewRect, backgroundTexture, new Rect(tileOffset, tileAmount));
            }
            GUI.color = color;
        }
    }
    [System.Serializable]
    public class vStateWindowDisplay
    {
        public Vector2 scroolView;
        public Vector2 scroolView2;
        public Vector2 scroolView3;
        public GUIStyle validationStyle;
        public void Draw(vIFSMBehaviourController target, GUISkin skin)
        {
            if (target == null) return;           
            DrawFSMStateInfo(target, skin);
           
        }

        void DrawFSMStateInfo(vIFSMBehaviourController target, GUISkin skin)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                var color = GUI.color;
                GUILayout.BeginVertical();
                {
                    var anyState = (target).anyState;
                    if (anyState)
                    {
                        GUI.color = anyState.nodeColor;
                        DrawState(target, anyState, skin.box, color, skin);
                        GUI.color = color;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    var lastState = (target).lastState;
                    if (lastState)
                    {
                        GUI.color = lastState.nodeColor;
                        DrawState(target, lastState, skin.box, color, skin, "Last State");
                        GUI.color = color;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    var currentState = (target).currentState;
                    if (currentState)
                    {
                        GUI.color = currentState.nodeColor;
                        DrawState(target, currentState, skin.GetStyle("BoxLarge"), color, skin, "Current State");
                        GUI.color = color;
                    }
                }
                GUILayout.EndVertical();
                GUI.color = color;
            }

            GUILayout.EndHorizontal();
        }

        void DrawState(vIFSMBehaviourController target, vFSMState state, GUIStyle style, Color color, GUISkin skin, string label = "")
        {
            if (state)
            {
                GUILayout.BeginVertical(style, GUILayout.MaxWidth(200));
                {
                    if (!string.IsNullOrEmpty(label))
                        GUILayout.Box(label, style, GUILayout.Height(30));
                    GUILayout.Box(state.name, style, GUILayout.Height(30));
                    GUI.color = color;
                    GUILayout.BeginVertical(skin.box);
                    {
                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(200));
                        {
                            GUILayout.Space(5);
                            GUILayout.BeginVertical(GUILayout.Width(200));
                            {
                                var fontTransitionStyle = new GUIStyle(EditorStyles.whiteMiniLabel);
                                fontTransitionStyle.alignment = TextAnchor.MiddleCenter;
                                fontTransitionStyle.stretchWidth = true;
                                //  fontTransitionStyle.wordWrap = true;
                                if (state.transitions.Count>0)
                                {
                                    GUILayout.Label("Transitions", fontTransitionStyle);
                                    for (int i = 0; i < state.transitions.Count; i++)
                                    {
                                        var transition = state.transitions[i];

                                        GUILayout.BeginVertical(skin.GetStyle("BoxLarge"));
                                        {
                                            if (transition.useTruState)
                                                GUILayout.Label((transition.transitionType== vTransitionOutputType.TrueFalse? "Output True:":"Output Default")+" >>>  " + (transition.trueState && !transition.muteTrue ? transition.trueState.name : "None"), fontTransitionStyle);
                                            if (transition.useFalseState && transition.transitionType == vTransitionOutputType.TrueFalse)
                                                GUILayout.Label((transition.transitionType == vTransitionOutputType.TrueFalse ? "Output False:" : "Output Default") + " >>>  " + (transition.falseState && !transition.muteFalse ? transition.falseState.name : "None"), fontTransitionStyle);
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (transition.decisions.Count > 0)
                                                {
                                                    GUILayout.Space(5);
                                                    GUILayout.BeginVertical();
                                                    for (int a = 0; a < transition.decisions.Count; a++)
                                                    {
                                                        //  fontTransitionStyle.fontSize = 10;
                                                        GUILayout.BeginHorizontal(skin.box, GUILayout.Height(20));
                                                        {
                                                            var decision = transition.decisions[a];
                                                            if (decision.decision)
                                                            {
                                                                var valid = false;
                                                                if (decision.validationByController != null && decision.validationByController.ContainsKey(target))
                                                                {
                                                                    valid = decision.validationByController[target];
                                                                }

                                                                validationStyle = new GUIStyle(skin.box);
                                                                GUILayout.Label(decision.decision.Name + " = " + decision.trueValue, fontTransitionStyle, GUILayout.Height(15));
                                                                //vcolor = valid ? Color.green : Color.red;
                                                                validationStyle.fontSize = 12;
                                                                validationStyle.fontStyle = FontStyle.Bold;
                                                                validationStyle.normal.textColor = valid ? Color.green : Color.red;
                                                                GUILayout.FlexibleSpace();
                                                                GUILayout.Box(valid ? "YES" : "NO", validationStyle, GUILayout.Width(30), GUILayout.ExpandHeight(true));
                                                                GUI.color = color;
                                                            }
                                                        }
                                                        GUILayout.EndHorizontal();
                                                    }
                                                    GUILayout.EndVertical();
                                                    GUILayout.Space(5);
                                                }
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.Space(5);
                                        }
                                        GUILayout.EndVertical();
                                    }
                                   
                                }
                                else if (state.defaultTransition)
                                {
                                    GUILayout.Label("Output Direct >>> "+state.defaultTransition.name, fontTransitionStyle);
                                }
                                else
                                {
                                    GUILayout.Label("Without Transition", fontTransitionStyle);
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }
    }
}