using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [CustomEditor(typeof(vControlAI), true)]
    public class vControlAIEditor : vEditorBase
    {
        public SerializedProperty fov;
        public SerializedProperty minDist, maxDist;
        public SerializedProperty lostDist;
        public SerializedProperty eyes;
        public SerializedProperty debug;
        public Color minDistColor = new Color(0, 0, 0, 1f);
        public Color maxDistColor = new Color(1, 1, 0, 1f);
        public Color lostDistColor = new Color(0.5f, 0.5f, 0, 1f);
        public Color combatColor = new Color(0, 0, 1, 1f);
        public GUIStyle labelStyle;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (serializedObject != null)
            {
                debug = serializedObject.FindProperty("_debugVisualDetection");
                minDist = serializedObject.FindProperty("_minDistanceToDetect");
                maxDist = serializedObject.FindProperty("_maxDistanceToDetect");
                fov = serializedObject.FindProperty("_fieldOfView");
                lostDist = serializedObject.FindProperty("_lostTargetDistance");
                eyes = serializedObject.FindProperty("detectionPointReference");
            }
            labelStyle = new GUIStyle(skin.label);
            labelStyle.normal.textColor = Color.white;
        }

        protected virtual void OnSceneGUI()
        {
            if (debug == null || !debug.boolValue) return;
            vIControlAICombat combatControl = null;
            if (target is vIControlAICombat)
            {
                combatControl = target as vIControlAICombat;
            }
            
            DrawGizmos(combatControl);
            DrawDebugWindow(combatControl);
        }

        private void DrawGizmos(vIControlAICombat combatControl)
        {
            minDistColor.a = .2f;
            maxDistColor.a = .2f;
            lostDistColor.a = .2f;
            combatColor.a = .2f;

            var transform = (eyes != null && eyes.objectReferenceValue != null ? (eyes.objectReferenceValue as Transform) : (target as MonoBehaviour).transform);
            float _fov = fov != null ? fov.floatValue : 0;

            if (combatControl != null)
            {
                Handles.color = combatColor;
                Handles.DrawSolidDisc((target as MonoBehaviour).transform.position, Vector3.up, combatControl.combatRange);
            }

            if (maxDist != null)
            {
                Handles.color = maxDistColor;
                var forward = transform.forward;
                forward.y = 0;

                Handles.DrawSolidArc(transform.position, Vector3.up, forward, _fov * 0.5f, maxDist.floatValue);
                Handles.DrawSolidArc(transform.position, Vector3.up, forward, -(_fov * 0.5f), maxDist.floatValue);
                Handles.color = lostDistColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, maxDist.floatValue + lostDist.floatValue);
            }

            if (minDist != null)
            {
                Handles.color = minDistColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, minDist.floatValue);
            }
        }

        private void DrawDebugWindow(vIControlAICombat combatControl)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Screen.width - 170, Screen.height - 195, 170, 195));
            minDistColor.a = .8f;
            maxDistColor.a = .8f;
            lostDistColor.a = .8f;
            combatColor.a = .8f;
            var color = GUI.color;

            GUILayout.BeginVertical("VISUAL DEBUG", skin.window, GUILayout.Width(150));
            GUILayout.Label(m_Logo, skin.label, GUILayout.MaxHeight(25));
            GUILayout.Space(10);

            GUI.color = minDistColor;
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = color;
                GUILayout.Box("Min Distance To Detect", labelStyle);
            }
            GUILayout.EndHorizontal();

            GUI.color = maxDistColor;
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = color;
                GUILayout.Label("Max Distance To Detect", labelStyle);
            }
            GUILayout.EndHorizontal();

            GUI.color = lostDistColor;
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = color;
                GUILayout.Box("Lost Target Distance", labelStyle);
            }
            GUILayout.EndHorizontal();

            if (combatControl != null)
            {
                GUI.color = combatColor;
                GUILayout.BeginHorizontal("box");
                {
                    GUI.color = color;
                    GUILayout.Box("Combat Range", labelStyle);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUI.color = color;
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}