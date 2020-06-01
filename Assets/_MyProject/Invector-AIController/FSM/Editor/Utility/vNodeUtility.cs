using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Invector.vCharacterController.AI.FSMBehaviour
{

    public static class vNodeUtility
    {
        public const string newfsmpath = "/Invector-AIController/My FSM Behaviours";

        public static void CreateGraph()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New FSM Behaviour.asset");

            if (!string.IsNullOrEmpty(assetPathAndName))
            {
                CreateNewGraph(assetPathAndName);
            }
        }
        public static float NearestRound(float x, float multiple)
        {
            if (multiple < 1)
            {
                float i = (float)Math.Floor(x);
                float x2 = i;
                while ((x2 += multiple) < x) ;
                float x1 = x2 - multiple;
                return (Math.Abs(x - x1) < Math.Abs(x - x2)) ? x1 : x2;
            }
            else
            {
                return (float)Math.Round(x / multiple, MidpointRounding.AwayFromZero) * multiple;
            }
        }

        public static void NearestRound(this Vector2 vector,float multiple)
        {
            vector.x.NearestRound(multiple);
            vector.y.NearestRound(multiple);
        }

        public static void CreateNewGraph(string path)
        {
            vFSMBehaviour curGraph = ScriptableObject.CreateInstance<vFSMBehaviour>();
            if (curGraph != null)
            {
                AssetDatabase.CreateAsset(curGraph, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = curGraph;
                var entryNode = CreateNode<vFSMState>("Entry", curGraph);
                entryNode.resetCurrentDestination = false;
                entryNode.parentGraph = curGraph;
                entryNode.useDecisions = false;
                entryNode.useActions = false;
                entryNode.canEditName = false;
                entryNode.canRemove = false;
                entryNode.canTranstTo = false;
                entryNode.canEditColor = false;
                entryNode.canSetAsDefault = false;
                entryNode.description = "This State Run Just in Start\n to init first state";
                entryNode.nodeColor = Color.green;
                var anyState = CreateNode<vFSMState>("AnyState", curGraph);
                anyState.resetCurrentDestination = false;            
                anyState.useDecisions = true;
                anyState.useActions = false;
                anyState.canEditName = false;
                anyState.canRemove = false;
                anyState.canTranstTo = false;
                anyState.canEditColor = false;
                anyState.canSetAsDefault = false;
                anyState.description = "This State Run after current state";
                anyState.nodeColor = Color.cyan;
                anyState.nodeRect.y += 100;
                anyState.parentGraph = curGraph;
                curGraph.states.Add(entryNode);
                curGraph.states.Add(anyState);
                curGraph.InitGraph();
                vFSMNodeEditorWindow.InitEditorWindow(curGraph);
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message", "Unable to create new graph, please see your friendly programmer!", "OK");
            }
        }

        public static int GetSameComponentNameCount<T>(this UnityEngine.Object obj)
        {
            var objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj.GetInstanceID()));
            int count = 0;
            foreach (var o in objs)
            {
                if(o!=null)
                {
                    if ((o.GetType().Equals(typeof(T)) || o.GetType().IsSubclassOf(typeof(T))) && o != obj && o.name.Equals(obj.name)) count++;
                }
               
            }
            return count;
        }

        public static void LoadGraph()
        {
            vFSMBehaviour curGraph = null;
            string graphPath = EditorUtility.OpenFilePanel("Load FSM Behaviour", Application.dataPath + "/Invector-AIController/", "asset");
            if (!string.IsNullOrEmpty(graphPath))
            {
                int dataPathLength = Application.dataPath.Length - 6;
                string finalPah = graphPath.Substring(dataPathLength);
                curGraph = (vFSMBehaviour)AssetDatabase.LoadAssetAtPath(finalPah, typeof(vFSMBehaviour));

                vFSMNodeEditorWindow curwindow = EditorWindow.GetWindow<vFSMNodeEditorWindow>();
                if (curwindow != null)
                {
                    curwindow.curGraph = curGraph;
                    Selection.activeObject = curGraph;
                }
            }
        }

        public static void UnloadGraph()
        {
            vFSMNodeEditorWindow curwindow = EditorWindow.GetWindow<vFSMNodeEditorWindow>();
            if (curwindow != null)
            {
                if (curwindow.curGraph != null && Selection.activeObject == curwindow.curGraph)
                    Selection.activeObject = null;
                curwindow.curGraph = null;
            }
        }

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static void CreateNode(vFSMBehaviour curGraph, Vector2 mousePos)
        {
            if (curGraph != null)
            {
                vFSMState curState = null;
                Undo.RecordObject(curGraph, "New Node");
                curState = ScriptableObject.CreateInstance<vFSMState>();

                curState.Name = "State " + (curGraph.states.Count > 1 ? (curGraph.states.Count - 2).ToString() : "");

                if (curGraph.states.Count > 0)
                {
                    if (curGraph.states[0].defaultTransition == null)
                    {
                        curGraph.states[0].defaultTransition = curState;
                    }
                }
                curState.nodeColor = Color.white;
                if (curState != null)
                {
                    curState.InitNode();
                    curState.nodeRect.x = mousePos.x;
                    curState.nodeRect.y = mousePos.y;
                    curState.parentGraph = curGraph;
                    curState.hideFlags = HideFlags.HideInHierarchy;

                    curGraph.states.Add(curState);

                    AssetDatabase.AddObjectToAsset(curState, curGraph);
                    var count = curState.GetSameComponentNameCount<vFSMState>();
                    if (count > 0)
                        curState.Name += " " + (count - 1).ToString();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public static void CreateNode(Type type, vFSMBehaviour curGraph, Vector2 mousePos)
        {
            if (curGraph != null && type.IsSubclassOf(typeof(vFSMState)))
            {
                var curState = ScriptableObject.CreateInstance(type.FullName);
                Undo.RegisterCreatedObjectUndo(curState, "Create object");
                curState.name = "State " + (curGraph.states.Count > 1 ? (curGraph.states.Count - 2).ToString() : "");

                if (curGraph.states.Count > 0)
                {
                    if (curGraph.states[0].defaultTransition == null)
                    {
                        curGraph.states[0].defaultTransition = curState as vFSMState;
                    }
                }
                (curState as vFSMState).nodeColor = Color.white;
                if (curState != null)
                {
                    (curState as vFSMState).InitNode();
                    (curState as vFSMState).nodeRect.x = mousePos.x;
                    (curState as vFSMState).nodeRect.y = mousePos.y;
                    (curState as vFSMState).parentGraph = curGraph;
                    curState.hideFlags = HideFlags.HideInHierarchy;
                    curGraph.states.Add((curState as vFSMState));

                    AssetDatabase.AddObjectToAsset(curState, curGraph);
                    var count = curState.GetSameComponentNameCount<vFSMState>();
                    if (count > 0)
                        (curState as vFSMState).Name += " " + (count - 1).ToString();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public static T CreateNode<T>(string name, vFSMBehaviour parentGraph) where T : ScriptableObject
        {

            T curNode = null;
            curNode = (T)ScriptableObject.CreateInstance<T>();
            curNode.name = name;

            if (curNode != null)
            {
                AssetDatabase.AddObjectToAsset(curNode, parentGraph);
                curNode.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return curNode;

        }

        public static T Get<T>(Type type)
        {
            return (T)Convert.ChangeType(type, typeof(T));
        }

        public static void DeleteNode(int id, vFSMBehaviour curGraph)
        {
            if (curGraph)
            {
                if (id >= 0 && id < curGraph.states.Count)
                {
                    vFSMState deleteNode = curGraph.states[id];
                    if (deleteNode)
                    {
                        curGraph.selectedNode = null;
                        curGraph.overNode = false;
                        curGraph.states.RemoveAt(id);

                        for (int i = 0; i < deleteNode.actions.Count; i++)
                        {
                            if (deleteNode.actions[i])
                            {
                                var o = new SerializedObject(deleteNode.actions[i]);
                                o.ApplyModifiedProperties();
                            }
                        }
                        for (int i = 0; i < deleteNode.transitions.Count; i++)
                        {
                            if (deleteNode.transitions[i].decisions != null)
                            {
                                for (int a = 0; a < deleteNode.transitions[i].decisions.Count; a++)
                                {
                                    if (deleteNode.transitions[i].decisions[i].decision)
                                    {                                        
                                        var o = new SerializedObject(deleteNode.transitions[i].decisions[i].decision);
                                        o.ApplyModifiedProperties();
                                    }
                                }
                            }
                        }
                        if (curGraph.states.Count > 2 && curGraph.states[0].defaultTransition == deleteNode)
                        {
                            curGraph.states[0].defaultTransition = curGraph.states[2] as vFSMState;
                        }

                        Undo.DestroyObjectImmediate(deleteNode);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}