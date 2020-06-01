using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [System.Serializable]
    public class FSMViewBase
    {
        // public event System.Action<Event> actions;
        #region Public Variables
        public string viewTitle;
        public Rect viewRect;
        #endregion

        #region Protected Variables
        protected GUISkin viewSkin;
        protected vFSMBehaviour currentFSM;
        #endregion

        #region Constructors
        public FSMViewBase(string title)
        {
            this.viewTitle = title;
        }

        public virtual void InitView()
        {
            GetEditorSkin();
        }
        #endregion

        #region Main Methods
        //public virtual void InitActionEvents()
        //{
        //    actions += a=> { if (a.type == EventType.mouseDown) { } };
        //}

        public virtual void UpdateView(Event e, vFSMBehaviour curGraph)
        {
            if (viewSkin == null)
            {
                GetEditorSkin();
                return;
            }
            // Set the current View Graph
            this.currentFSM = curGraph;
        }

        public virtual void ProcessEvents(Event e) { }
        #endregion

        #region Utility Methods
        protected void GetEditorSkin()
        {
            viewSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
        }
        #endregion
    }
}