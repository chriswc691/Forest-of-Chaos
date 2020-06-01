using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vFSMNodeEditorWindow : EditorWindow
    {
        [OnOpenAssetAttribute(1)]
        public static bool step1(int instanceID, int line)
        {
            var _object = EditorUtility.InstanceIDToObject(instanceID);
            if (_object is vFSMBehaviour)
            {
                InitEditorWindow(_object as vFSMBehaviour);
            }

            return false; // we did not handle the open
        }
        #region Variables
        public static vFSMNodeEditorWindow curWindow;
        public vFSMPropertyView propertyView;
        public vFSMWorkView workView;
        //public Rect workViewRect;
        //public Rect FSMPropertyViewRect;
        //public Rect nodePropertyViewRect;
        public Rect dragFSMSize = new Rect();
        public Rect dragNodeProperySize = new Rect();
        public Rect oldRect = new Rect();
        public Rect showPropertyToggleRect = new Rect();
        public vFSMBehaviour curGraph = null;
        public float fsmPropertyViewSize = 0.15f;
        public float toobarSize = 20;

        public float fsmViewOffset;
        public float propertyViewOffset;
        public bool inResizePropertyView;
        public GUISkin skin;
        public bool showProperties = true;
        #endregion

        #region Main Methods
        public static void InitEditorWindow()
        {
            curWindow = (vFSMNodeEditorWindow)EditorWindow.GetWindow<vFSMNodeEditorWindow>("AI FSM", false, typeof(SceneView));
            curWindow.titleContent.image = Resources.Load("Textures/Editor/FSMIconWindow") as Texture2D;
            CreateViews();
            if (curWindow.workView != null && curWindow.curGraph && curWindow.curGraph.states.Count > 0)
            {
                var states = curWindow.curGraph.states;
                for (int i = 0; i < states.Count; i++) states[i].inDrag = false;
                curWindow.workView.CenterView(curWindow.curGraph);
            }

        }
        public static void InitEditorWindow(vFSMBehaviour graph)
        {
            curWindow = (vFSMNodeEditorWindow)EditorWindow.GetWindow<vFSMNodeEditorWindow>("AI FSM", false, typeof(SceneView));
            curWindow.titleContent.image = Resources.Load("Textures/Editor/FSMIconWindow") as Texture2D;

            curWindow.curGraph = graph;
            CreateViews();
            if (curWindow.workView != null && graph.states.Count > 0)
            {
                var states = graph.states;
                for (int i = 0; i < states.Count; i++) states[i].inDrag = false;
                curWindow.workView.CenterView(graph);
            }

        }

        void OnEnable()
        {
            if (workView == null || propertyView == null)
            {
                CreateViews();
                return;
            }
        }

        void OnDestroy()
        {

        }

        void Update()
        {
            if (Selection.activeObject != null && curWindow)
            {
                if ((curWindow.curGraph == null || curWindow.curGraph != Selection.activeObject) && Selection.activeObject is vFSMBehaviour)
                {
                    curWindow.curGraph = Selection.activeObject as vFSMBehaviour;
                }
                else if (Selection.activeObject)
                {
                    vIFSMBehaviourController fsmBehaviour = (Selection.activeGameObject ? Selection.activeGameObject.GetComponent<vIFSMBehaviourController>() : null);

                    if (fsmBehaviour != null && fsmBehaviour.fsmBehaviour)
                    {
                        curWindow.curGraph = fsmBehaviour.fsmBehaviour;
                    }
                }
            }
        }

        void OnGUI()
        {
            if (workView == null || propertyView == null)
            {
                CreateViews();
            }
            fsmViewOffset = Mathf.Clamp(fsmViewOffset, 0, position.width * 0.75f);

            showPropertyToggleRect = new Rect(0, toobarSize, (this.position.width * fsmPropertyViewSize) + fsmViewOffset, toobarSize);
            propertyView.viewRect = new Rect(0, toobarSize * 2f, (this.position.width * fsmPropertyViewSize) + fsmViewOffset, this.position.height - toobarSize * 2f);

            showPropertyToggleRect.width = Mathf.Clamp(showPropertyToggleRect.width, 200, 300);
            propertyView.viewRect.width = Mathf.Clamp(propertyView.viewRect.width, 200, 300);
            if (!showProperties)
            {
                showPropertyToggleRect.width = toobarSize;
                propertyView.viewRect.width = 0;
            }

            workView.viewRect = new Rect(propertyView.viewRect.width, toobarSize, (position.width - propertyView.viewRect.width), position.height - toobarSize);
            dragFSMSize = new Rect(propertyView.viewRect.width - 2.5f, toobarSize, 5f, position.height - toobarSize);

            //Get and Process the current Event
            Event e = Event.current;
            ProcessEvents(e);
            EditorGUI.BeginChangeCheck();

            workView.UpdateView(e, curGraph);

            GUILayout.BeginArea(showPropertyToggleRect);
            {
                GUILayout.BeginHorizontal(skin.GetStyle("ToolBar"));
                GUILayout.FlexibleSpace();
                showProperties = GUILayout.Toggle(showProperties, "", skin.GetStyle("ShowHideToggle"), GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            if (showProperties)
                propertyView.UpdateView(e, curGraph);
            if (EditorGUI.EndChangeCheck() && curGraph)
            {

                Undo.RecordObjects(curGraph.states.ToArray(), "CurGraph");
                Undo.RegisterFullObjectHierarchyUndo(curGraph, "CurGraph");
                Undo.RegisterCompleteObjectUndo(curGraph, "CurGraph");
            }

            DrawToolStrip();

            Repaint();
        }
        #endregion

        #region Utility Methods

        void DrawToolStrip()
        {
            GUILayout.BeginHorizontal(skin.box, GUILayout.Height(toobarSize));

            if (GUILayout.Button("File", skin.GetStyle("DropDown"), GUILayout.Width(50), GUILayout.Height(toobarSize)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("New Behaviour"), false, () => { vNodeUtility.CreateGraph(); });
                menu.AddItem(new GUIContent("Load Behaviour"), false, () => { vNodeUtility.LoadGraph(); });
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.y += 20;
                menu.DropDown(rect);
                EditorGUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Tools", skin.GetStyle("DropDown"), GUILayout.Width(50), GUILayout.Height(toobarSize)))
            {
                GenericMenu toolsMenu = new GenericMenu();

                toolsMenu.AddItem(new GUIContent("FSM Debugger"), false, OnDebuggerWindow);

                // Offset menu from right of editor window
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.y += 20;
                rect.x += 50;
                toolsMenu.DropDown(rect);
                EditorGUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Help", skin.GetStyle("DropDown"), GUILayout.Width(50), GUILayout.Height(toobarSize)))
            {
                GenericMenu toolsMenu = new GenericMenu();

                toolsMenu.AddItem(new GUIContent("Documentation"), false, OnTools_Help);

                // Offset menu from right of editor window
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.y += 20;
                rect.x += 50;
                toolsMenu.DropDown(rect);
                EditorGUIUtility.ExitGUI();
            }
            var style = new GUIStyle(skin.GetStyle("ToolBar"));
            style.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button(curGraph ? "--- " + curGraph.name.ToUpper() + " ---" : " No Behaviour", style, GUILayout.ExpandWidth(true), GUILayout.Height(toobarSize)))
            {
                if (curGraph) Selection.activeObject = curGraph;
            }
            var color = GUI.color;
            var _style = skin.GetStyle("CenterViewButton");

            if (GUILayout.Button(new GUIContent("", "Center View (F)"), _style, GUILayout.Width(40), GUILayout.Height(toobarSize - 2)))
            {
                workView.CenterView(curGraph);
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }

        void OnDebuggerWindow()
        {
            vFSMBehaviourControllerDebugWindow.InitEditorWindow();
        }

        void OnTools_Help()
        {
            Help.BrowseURL("http://www.invector.xyz/aidocumentation");
        }

        static void CreateViews()
        {
            if (!curWindow)
            {
                curWindow = (vFSMNodeEditorWindow)EditorWindow.GetWindow<vFSMNodeEditorWindow>("AI FSM", false, typeof(SceneView));
                curWindow.titleContent.image = Resources.Load("Textures/Editor/FSMIconWindow") as Texture2D;
            }
            if (curWindow != null)
            {
                curWindow.GetEditorSkin();
                curWindow.workView = new vFSMWorkView();
                curWindow.workView.InitView();
                curWindow.propertyView = new vFSMPropertyView();
                curWindow.propertyView.InitView();
                curWindow.propertyView.viewRect = new Rect(0, curWindow.toobarSize, curWindow.position.width * curWindow.fsmPropertyViewSize + curWindow.fsmViewOffset, curWindow.position.height - curWindow.toobarSize);
                curWindow.workView.viewRect = new Rect(curWindow.propertyView.viewRect.width, curWindow.toobarSize, (curWindow.position.width - curWindow.propertyView.viewRect.width), curWindow.position.height - curWindow.toobarSize);
                curWindow.dragFSMSize = new Rect(curWindow.propertyView.viewRect.width - 2.5f, curWindow.toobarSize, 5f, curWindow.position.height - curWindow.toobarSize);
            }
        }

        void ProcessEvents(Event e)
        {
            if (!inResizePropertyView)
                EditorGUIUtility.AddCursorRect(dragFSMSize, MouseCursor.ResizeHorizontal);
            else
                EditorGUIUtility.AddCursorRect(new Rect(e.mousePosition.x - 25, e.mousePosition.y - 25, 50, 50), MouseCursor.ResizeHorizontal);

            if (dragFSMSize.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    inResizePropertyView = true;
                }
            }

            if (e.type == EventType.MouseUp)
            {
                inResizePropertyView = false;

                // resizingPropView = false;
            }
            if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0)
                {
                    if (inResizePropertyView)
                    {
                        fsmViewOffset += e.delta.x;
                        e.Use();
                    }
                }
            }

        }

        protected void GetEditorSkin()
        {
            skin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
        }
        #endregion
    }
}