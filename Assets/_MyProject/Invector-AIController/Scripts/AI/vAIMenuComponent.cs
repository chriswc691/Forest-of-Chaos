using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Invector.vCharacterController.AI
{
    public partial class vAIMenuComponent
    {
        public const string AIForum = "http://invector.proboards.com/";
        public const string AIVersion = "1.1.0";
        public const string AIChangeLog = "http://invector.proboards.com/board/23/ai-changelog";
        public const string AIDocumentation = "http://www.invector.xyz/aidocumentation";

        [MenuItem("Invector/FSM AI/Help/Forum")]
        public static void Forum()
        {
            Application.OpenURL(AIForum);
        }

        [MenuItem("Invector/FSM AI/Help/Version: "+ AIVersion)]
        public static void Version()
        {
            Application.OpenURL(AIChangeLog);
        }
        [MenuItem("Invector/FSM AI/Help/Online Documentation")]
        public static void Documentation()
        {
            Application.OpenURL(AIDocumentation);
        }

        [MenuItem("Invector/FSM AI/Components/AI Companion")]
        static void AICompanionMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vAICompanion>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/Player Companion Control")]
        static void AIPlayerCompanionControlMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<vAICompanionControl>();
            else
                Debug.Log("Please select a object to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Throw")]
        static void AIThrowMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vAIThrowObject>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Cover")]
        static void AICoverMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vAICover>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI MeleeManager")]
        static void AIMeleeManagerMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vMelee.vMeleeManager>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI ShooterManager")]
        static void AIShooterManagerMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vAIShooterManager>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Headtrack")]
        static void AIHeadtrackMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vAIHeadtrack>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI MessageReceiver")]
        static void AIMessageReceiver()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<vControlAI>() != null)
                Selection.activeGameObject.AddComponent<vMessageReceiver>();
            else
                Debug.Log("Please select a FSM AI to add the component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Spawn System")]
        static void AISpawnerMenu()
        {
            var wp = new GameObject("AI Spawn", typeof(vAISpawn));

            SceneView view = SceneView.lastActiveSceneView;
            if (SceneView.lastActiveSceneView == null)
                throw new UnityException("The Scene View can't be access");

            Vector3 spawnPos = view.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            wp.transform.position = spawnPos;
        }

        [MenuItem("Invector/FSM AI/Components/AI Simple Holder")]
        static void AISimpleHolderMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<vSimpleHolder>();
            else
                Debug.Log("Please select a GameObject to add this component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Noise Listener")]
        static void AINoiseListenerMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<vAINoiseListener>();
            else
                Debug.Log("Please select a GameObject to add this component.");
        }

        [MenuItem("Invector/FSM AI/Components/AI Move to Position")]
        static void AIMoveToPositionMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<vAIMoveToPosition>();
            else
                Debug.Log("Please select a GameObject to add this component.");
        }        

    }
}

#endif