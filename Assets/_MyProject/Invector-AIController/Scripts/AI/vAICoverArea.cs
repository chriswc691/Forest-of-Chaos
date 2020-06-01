using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI.Cover
{
    [SelectionBase]
    [vClassHeader("AI Cover Area",openClose = false)]
    public class vAICoverArea : vMonoBehaviour
    {
        public string coverLayer = "Triggers";
        public string coverTag = "CoverPoint";
        [vHelpBox("Collider Settings")]
        public float colliderWidth = 1;
        public float colliderHeight = 1;
        public float colliderThickness = 0.5f;
        [SerializeField] protected float _colliderCenterY = 0;
        [SerializeField] protected float _colliderCenterZ = 0;
        [vHelpBox("Character Destination Settings")]
        [Tooltip("Target position to the character stay")]
        public float posePositionZ = 0.5f;
        public float centerY
        {
            get
            {
                return _colliderCenterY + (colliderHeight * 0.5f);
            }
        }
        public float centerZ
        {
            get
            {
                return _colliderCenterZ + (colliderThickness * 0.5f);
            }
        }
        void Reset()
        {
            int childCount = transform.childCount;
           for(int i = childCount-1;i>0;i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
        [HideInInspector] public bool closeLine;
        [HideInInspector] public List<CoverLine> coverLines = new List<CoverLine>();
        [System.Serializable]       
        public class CoverLine
        {
            public Transform p1, p2;
            public bool inverse;
            public List<vAICoverPoint> coverPoints = new List<vAICoverPoint>();
            public Vector3 forward;
        }

        private void Start()
        {
            var childCount = transform.childCount;
            var _layer =LayerMask.NameToLayer(coverLayer);
            
            for (int i = 0; i < childCount; i++)
            {
                var c = transform.GetChild(i);
                c.gameObject.layer = _layer;
                c.gameObject.tag = coverTag;
            }
        }
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var isSelected = UnityEditor.Selection.activeGameObject == gameObject;
            Gizmos.color = !isSelected ? Color.white * 0.8f : Color.green * 0.5f;
           
            //if (UnityEditor.Selection.activeGameObject != gameObject)
            {
                for (int i = 0; i < coverLines.Count; i++)
                {
                    if (!isSelected)
                    {
                        Gizmos.DrawSphere(coverLines[i].p1.position, 0.1f);
                        Gizmos.DrawSphere(coverLines[i].p2.position, 0.1f);
                    }
                    var right = (coverLines[i].p2.position -coverLines[i].p1.position);
                    if (right.magnitude>colliderWidth)
                    {
                        var p1Up = coverLines[i].p1.position + Vector3.up * (centerY + (colliderHeight * 0.5f));
                        var p1Down = coverLines[i].p1.position;
                        var p2Up = coverLines[i].p2.position + Vector3.up * (centerY + (colliderHeight * 0.5f));
                        var p2Down = coverLines[i].p2.position;
                       
                        
                        Gizmos.DrawLine(p1Up, p2Up);
                        Gizmos.DrawLine(p1Down, p2Down);
                        Gizmos.DrawLine(p1Up, p1Down);
                        Gizmos.DrawLine(p2Up, p2Down);
                        var pLength = right.magnitude + 0.02f;
                        
                        var cpCount = (int)(pLength / colliderWidth);
                        var realWidth = ((pLength / (float)cpCount));
                        var startP = coverLines[i].p1.position + (right.normalized * (colliderWidth * 0.5f))+ Vector3.up * (centerY + (colliderHeight * 0.5f)); 
                        for (int b =0;b<cpCount;b++)
                        {               
                            var _p1RUp = (startP + (right.normalized * (realWidth * b)))+right.normalized*(colliderWidth*0.5f);
                            var _p1LUp = (startP + (right.normalized * (realWidth * b))) - right.normalized * (colliderWidth * 0.5f);

                            var p1FRUp = _p1RUp + coverLines[i].forward * (centerZ + colliderThickness * 0.5f);
                            var p1FLUp = _p1LUp + coverLines[i].forward * (centerZ + colliderThickness * 0.5f);

                            Gizmos.DrawLine(_p1LUp, p1FLUp);
                            Gizmos.DrawLine(_p1RUp, p1FRUp);
                            Gizmos.DrawLine(p1FRUp, p1FLUp);
                           

                        }  
                    }
                   
                }
            }
#endif

        }

    }
}