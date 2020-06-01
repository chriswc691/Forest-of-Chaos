using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Invector.vCharacterController.AI.Cover
{
   
    [CustomEditor(typeof(vAICoverArea))]
    public class vAICoverAreaEditor : vEditorBase
    {
        [MenuItem("GameObject/Invector/FSM AI/New Cover Area", false, -100)]
        [MenuItem("Invector/FSM AI/Components/New Cover Area")]
        static void CreateNewCoverArea()
        {
            var cA = new GameObject("CoverArea", typeof(vAICoverArea));

            SceneView view = SceneView.lastActiveSceneView;
            if (SceneView.lastActiveSceneView == null)
                throw new UnityException("The Scene View can't be access");

            Vector3 spawnPos = view.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
            cA.transform.position = spawnPos;
            Selection.activeGameObject = cA.gameObject;
        }
        public vAICoverArea coverArea;
        public Transform currentSelectedP;
        public vAICoverArea.CoverLine currentSelecteLine;

        public int indexSelected;
        public bool isFistPointSelected;

        private void OnSceneGUI()
        {
            if (!coverArea) return;
            var color = Handles.color;
            
            if (coverArea.coverLines.Count > 0)
            {
                coverArea.coverLines[0].p1.localPosition = Vector3.zero;                
                if (Handles.Button(coverArea.coverLines[0].p2.position, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                {
                    currentSelectedP = coverArea.coverLines[0].p2;
                    currentSelecteLine = coverArea.coverLines[0];
                    indexSelected = 0;
                    isFistPointSelected = false;
                    Repaint();

                }


                for (int i = 0; i < coverArea.coverLines.Count; i++)
                {
                    Handles.color = Color.white;
                    if (i > 0 && coverArea.coverLines[0].p1 != coverArea.coverLines[i].p2)
                        if (Handles.Button(coverArea.coverLines[i].p2.position, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                        {


                            currentSelectedP = coverArea.coverLines[i].p2;
                            currentSelecteLine = coverArea.coverLines[i];
                            indexSelected = i; isFistPointSelected = false; Repaint();


                        }

                    HandleCoverPoints(coverArea.coverLines[i]);
                    Handles.color = Color.white;
                }
            }

            if (currentSelectedP && coverArea.coverLines.Count>0 && coverArea.coverLines[0].p1!= currentSelectedP)
            { 
                Handles.SphereHandleCap(0, currentSelectedP.position, Quaternion.identity, 0.2f, EventType.Repaint);
                currentSelectedP.position = Handles.DoPositionHandle(currentSelectedP.position, Quaternion.identity);
                if (currentSelectedP.position.y < coverArea.transform.position.y)
                {
                    var p = currentSelectedP.position;
                    p.y = coverArea.transform.position.y;
                    currentSelectedP.position = p;
                }
            }
            Handles.color = color;
            if (coverArea.coverLines.Count > 1)
            {
                if (coverArea.closeLine)
                {
                    var firstP = coverArea.coverLines[0].p1;
                    var lastP = coverArea.coverLines[coverArea.coverLines.Count - 1].p2;
                    if (lastP != firstP)
                    {
                        DestroyImmediate(lastP.gameObject);
                        coverArea.coverLines[coverArea.coverLines.Count - 1].p2 = firstP;
                        currentSelectedP = coverArea.coverLines[0].p1;
                        currentSelecteLine = coverArea.coverLines[0];
                        indexSelected = 0;
                       
                    }
                }
                else
                {
                    var firstP = coverArea.coverLines[0].p1;
                    var lastP = coverArea.coverLines[coverArea.coverLines.Count - 1].p2;
                    if (lastP == firstP)
                    {
                        coverArea.coverLines[coverArea.coverLines.Count - 1].p2 = new GameObject("P2_" + coverArea.coverLines.Count).transform;
                        coverArea.coverLines[coverArea.coverLines.Count - 1].p2.hideFlags = HideFlags.HideInHierarchy;
                        coverArea.coverLines[coverArea.coverLines.Count - 1].p2.parent = coverArea.transform;
                        coverArea.coverLines[coverArea.coverLines.Count - 1].p2.position = firstP.position + (coverArea.coverLines[coverArea.coverLines.Count - 1].p1.position - firstP.position) * 0.5f;
                        currentSelectedP = coverArea.coverLines[coverArea.coverLines.Count - 1].p2;
                        currentSelecteLine = coverArea.coverLines[coverArea.coverLines.Count - 1];
                        indexSelected = coverArea.coverLines.Count - 1;
                    }
                }
            }
            Handles.color = color;
        }

        protected override  void OnEnable()
        {
            base.OnEnable();
            coverArea = target as vAICoverArea;
            currentSelectedP = null;
            currentSelecteLine = null;
            indexSelected = 0;

        }

        private void OnDisable()
        {
            coverArea = null;
            currentSelectedP = null;
            indexSelected = 0;
        }

        public void HandleCoverPoints(vAICoverArea.CoverLine coverLine)
        {           
            var p1 = coverLine.p1.position;
            var p2 = coverLine.p2.position;
            var right = (p2 - p1);
            var linearRight = right;
            linearRight.y = 0;
            var pLength = right.magnitude+0.02f;
            
            var changeForwardButtonPos = (p1+ p2) * 0.5f;
            Handles.color = Color.blue;
            if (Handles.Button(changeForwardButtonPos + Vector3.up * ((coverArea.colliderHeight * 0.5f) + coverArea.centerY), Quaternion.LookRotation(coverLine.forward != Vector3.zero ? coverLine.forward:Vector3.down), pLength >= coverArea.colliderWidth ?0.5f:0, pLength >= coverArea.colliderWidth?0.5f:0, Handles.ArrowHandleCap))
                coverLine.inverse = !coverLine.inverse;
            Handles.color = Color.white;
           
            var cpCount = (int)(pLength / coverArea.colliderWidth);
            var realWidth = ((pLength / (float)cpCount));
            right.Normalize();
            linearRight.Normalize();
            var forward = Quaternion.AngleAxis(-90, coverArea.transform.up) * linearRight;
            var startP = p1 + (right * (coverArea.colliderWidth * 0.5f));
            if (cpCount == 1) startP = (p1 + p2) * 0.5f;

            coverLine.forward = coverLine.inverse ? -forward : forward;
            if (coverLine.coverPoints.Count < cpCount)
            {
                var countDif = cpCount - coverLine.coverPoints.Count;
                for (int i = 0; i < countDif; i++)
                {
                    coverLine.coverPoints.Add(CreateCoverPoint());
                }
            }
            else if (coverLine.coverPoints.Count > cpCount)
            {
                var countDif = coverLine.coverPoints.Count - cpCount;
                for (int i = 0; i < countDif; i++)
                {
                    var cp = coverLine.coverPoints[coverLine.coverPoints.Count - 1];
                    DestroyImmediate(cp.gameObject);
                    coverLine.coverPoints.RemoveAt(coverLine.coverPoints.Count - 1);                 
                }
            }

            DrawCoverPoint(coverLine, right, realWidth, startP);                  

        }

        private void DrawCoverPoint(vAICoverArea.CoverLine coverLine, Vector3 right, float realWidth, Vector3 startP)
        {
            var rec = new Rect();
            var matrix = Handles.matrix;
            for (int i = 0; i < coverLine.coverPoints.Count; i++)
            {
                if (coverLine.coverPoints[i].transform.parent == null || coverLine.coverPoints[i].transform.parent!= coverArea.transform) coverLine.coverPoints[i].transform.parent = coverArea.transform;
                if (coverLine.coverPoints[i].boxCollider == null)
                {
                    CreateBoxColider(coverLine.coverPoints[i]);
                    break;
                }
                coverLine.coverPoints[i].transform.position = startP + (right * (realWidth * i));
                coverLine.coverPoints[i].transform.rotation = Quaternion.LookRotation(coverLine.forward);
                coverLine.coverPoints[i].boxCollider.size = new Vector3(coverArea.colliderWidth, coverArea.colliderHeight, coverArea.colliderThickness);
                coverLine.coverPoints[i].boxCollider.center = Vector3.up * coverArea.centerY + Vector3.forward * coverArea.centerZ;
                coverLine.coverPoints[i].posePositionZ = coverArea.posePositionZ;

                Handles.color = Color.green * 0.8f;
                Handles.DrawSolidDisc(coverLine.coverPoints[i].posePosition, Vector3.up, 0.25f);
                Handles.color = Color.green * 0.4f;
                Handles.DrawSolidArc(coverLine.coverPoints[i].posePosition, coverLine.coverPoints[i].transform.forward, coverLine.coverPoints[i].transform.right, 180, 0.25f);
                Handles.DrawSolidArc(coverLine.coverPoints[i].posePosition, -coverLine.coverPoints[i].transform.right, coverLine.coverPoints[i].transform.forward, 180, 0.25f);
                Handles.color = Color.white;
                Handles.DrawWireDisc(coverLine.coverPoints[i].posePosition, Vector3.up, 0.25f);
                Handles.DrawWireArc(coverLine.coverPoints[i].posePosition, coverLine.coverPoints[i].transform.forward, coverLine.coverPoints[i].transform.right, 180, 0.25f);
                Handles.DrawWireArc(coverLine.coverPoints[i].posePosition, -coverLine.coverPoints[i].transform.right, coverLine.coverPoints[i].transform.forward, 180, 0.25f);

                Handles.color = Color.white *  0.8f;
                rec.position = new Vector2(coverLine.coverPoints[i].boxCollider.center.x - coverLine.coverPoints[i].boxCollider.size.x * 0.5f, coverLine.coverPoints[i].boxCollider.center.y - coverLine.coverPoints[i].boxCollider.size.y * 0.5f);
                rec.size = new Vector2(coverLine.coverPoints[i].boxCollider.size.x, coverLine.coverPoints[i].boxCollider.size.y);

                Matrix4x4 m = Matrix4x4.TRS(coverLine.coverPoints[i].transform.position, coverLine.coverPoints[i].transform.rotation, coverLine.coverPoints[i].transform.lossyScale);
                Handles.matrix = m;
                Handles.DrawWireCube(coverLine.coverPoints[i].boxCollider.center, coverLine.coverPoints[i].boxCollider.size);
                m = Matrix4x4.TRS(coverLine.coverPoints[i].transform.position + coverLine.coverPoints[i].transform.forward * (coverArea.centerZ + (coverArea.colliderThickness * 0.5f)), coverLine.coverPoints[i].transform.rotation, coverLine.coverPoints[i].transform.lossyScale);
                Handles.matrix = m;
                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(rec, Color.green * .25f , Color.green *  1 );
                Handles.matrix = matrix;
                Handles.color = Color.green *  0.8f;
                var normalStartP = coverLine.coverPoints[i].boxCollider.bounds.center + coverLine.coverPoints[i].transform.forward * (coverArea.colliderThickness * 0.5f);               
                Handles.ArrowHandleCap(0, normalStartP, coverLine.coverPoints[i].transform.rotation, .25f, EventType.Repaint);                
            }
        }

        public vAICoverPoint CreateCoverPoint()
        {
            var obj = new GameObject("CoverPoint", typeof(BoxCollider));
            obj.transform.parent = coverArea.transform;
            return obj.AddComponent<vAICoverPoint>();
        }

        public void CreateCoverline()
        {
            var coverline = new vAICoverArea.CoverLine();
            if (coverArea.coverLines.Count == 0)
            {
                coverline.p1 = new GameObject("P1_0").transform;
                coverline.p1.hideFlags = HideFlags.HideInHierarchy;
                coverline.p1.parent = coverArea.transform;
                coverline.p1.position = coverArea.transform.position;
            }
            else
            {
                coverline.p1 = coverArea.coverLines[coverArea.coverLines.Count - 1].p2;

            }
            coverline.p2 = new GameObject("P2_" + coverArea.coverLines.Count).transform;
            coverline.p2.hideFlags = HideFlags.HideInHierarchy;
            coverline.p2.parent = coverArea.transform;
            var p2Position = coverline.p1.position;
            if (coverArea.coverLines.Count > 0)
                p2Position += (coverline.p1.position - coverArea.coverLines[coverArea.coverLines.Count - 1].p1.position).normalized * coverArea.colliderWidth * 2.1f ;
            else
            {
                p2Position += coverArea.transform.right * coverArea.colliderWidth * 2.1f;
            }
            coverline.p2.position = p2Position;
            coverArea.coverLines.Add(coverline);
            currentSelecteLine = coverline;
            indexSelected = coverArea.coverLines.Count - 1;
            currentSelectedP = coverline.p2;
            HandleCoverPoints(coverline);
        }

        public void RemoveLine()
        {
            var lines = coverArea.coverLines.Count;
            if (indexSelected > 0 && indexSelected < lines - 1)
            {
                var a = coverArea.coverLines[indexSelected];
                var b = coverArea.coverLines[indexSelected + 1];
                b.p1 = a.p1;
                DestroyPointOfLine(a);
                indexSelected = indexSelected % coverArea.coverLines.Count;
                currentSelectedP = coverArea.coverLines[indexSelected].p2;
                currentSelecteLine = coverArea.coverLines[indexSelected];
            }
            else if (lines - 1 >= 0 && indexSelected == lines - 1)
            {
                var a = coverArea.coverLines[indexSelected];
                DestroyPointOfLine(a);
                indexSelected = lines - 2;
                currentSelectedP = coverArea.coverLines[indexSelected].p2;
                currentSelecteLine = coverArea.coverLines[indexSelected];
            }
            else if (indexSelected == 0 && lines > 1)
            {
                var a = coverArea.coverLines[indexSelected];
                var b = coverArea.coverLines[indexSelected + 1];
                a.p2 = b.p2;

                DestroyPointOfLine(b, false);
            }
        }

        public void DestroyPointOfLine(vAICoverArea.CoverLine line, bool p2 = true)
        {
            var needToDestroy = line.coverPoints;
            var destroyP = p2 ? line.p2 : line.p1;
            coverArea.coverLines.Remove(line);
            DestroyImmediate(destroyP.gameObject);
            for (int i = 0; i < needToDestroy.Count; i++)
            {
                if (needToDestroy[i] != null && needToDestroy[i].gameObject)
                    DestroyImmediate(needToDestroy[i].gameObject);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical(skin.box);
            base.OnInspectorGUI();
            if (coverArea)
            { 
                GUI.enabled = !coverArea.closeLine;
                if (GUILayout.Button("Add New Point") || coverArea.coverLines.Count == 0)
                {
                    CreateCoverline();
                    coverArea.closeLine = false;
                    serializedObject.ApplyModifiedProperties();
                    SceneView.RepaintAll();
                }
                GUILayout.BeginHorizontal(skin.box);
                GUI.enabled = coverArea.coverLines.Count > 2;
                if (GUILayout.Button(coverArea.closeLine ? "Desconnect End" : "Connect End"))
                {
                    coverArea.closeLine = !coverArea.closeLine; serializedObject.ApplyModifiedProperties(); SceneView.RepaintAll();
                }
                GUI.enabled = ((!coverArea.closeLine || coverArea.coverLines.Count > 3) && !isFistPointSelected && coverArea.coverLines.Count > 1 && (indexSelected < coverArea.coverLines.Count && coverArea.coverLines[indexSelected] != null));
                if (GUILayout.Button("Dell Selected"))
                {
                    RemoveLine();
                    serializedObject.ApplyModifiedProperties();
                    SceneView.RepaintAll();

                }
                GUILayout.EndHorizontal();
                
                GUI.enabled = true;
            }
           
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        public void CreateBoxColider(vAICoverPoint target)
        {
            BoxCollider c = null;
            c = target.gameObject.GetComponent<BoxCollider>();
            if (!c) c = target.gameObject.AddComponent<BoxCollider>();
            target.boxCollider = c;
           
        }
    }
}