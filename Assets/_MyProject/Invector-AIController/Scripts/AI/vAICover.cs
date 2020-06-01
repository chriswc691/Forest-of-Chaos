using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI Cover")]
    public class vAICover : vMonoBehaviour, vIAIComponent
    {
        public Type ComponentType
        {
            get
            {
                return typeof(vAICover);
            }
        }

        [vHelpBox("This component requires CoverPoint in your scene, please check the documentation for more information on how to set up CoverPoints")]

        [vEditorToolbar("Settings")]
        public float getCoverRange = 10;
        public float minDistanceOfThreat = 5f;
        public float maxDistanceOfThreat = 50f;
        public float minAngleOfThreat = 100f;
        [vMinMax(minLimit = 2f,maxLimit = 100f)]
        public Vector2 timeToChangeCover = new Vector2(2, 10);
        public string coverTag = "CoverPoint";
        public LayerMask coverLayer;
        [vHelpBox("<i>\n Check AI Controller receivedDamage in Debug Toolbar to debug massive damage Values</i>",vHelpBoxAttribute.MessageType.Info)]
        public bool changeCoverByDamage = true;
        [vToggleOption("Compare Damage", "Less", "GreaterEqual")]
        [SerializeField] protected bool greaterValue = true;
        [vToggleOption("Compare Type", "Massive Count", "Massive Value")]
        [SerializeField] protected bool massiveValue = true;
        [SerializeField] protected int valueToCompare = 100;
        [vMinMax(minLimit = 0f, maxLimit = 100f)]
        [Tooltip("Use this time to control when the valid damage checks are true after cover is changed to prevent that change cover every time")]
        [SerializeField] protected Vector2 timeToChangeByDamge= new Vector2(0, 2);
        [vEditorToolbar("Debug")]
        public bool debugMode;
        [vReadOnly] public bool isGointToCoverPoint;
        [vButton("Get Cover Test", "GetCoverTest", typeof(vAICover))]
        [SerializeField] private Transform targetTest = null;
        public vAICoverPoint coverPoint;

        internal vIControlAICombat controller;   
        internal Vector3 threatdir;
        internal Vector3 threatPos;

        private float _timeInCover;
        private float _timeToChangeByDamage;
        private bool inGetCover;
        public List<vAICoverPoint> _coverPoints = new List<vAICoverPoint>();
        
        protected virtual void Start()
        {
            controller = GetComponent<vIControlAICombat>();
            controller.onDead.AddListener(RemoveCoverOnDead);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (debugMode)
            {
                Gizmos.DrawWireSphere(transform.position, getCoverRange);
                if (_coverPoints.Count == 0) return;

                for (int i = 0; i < _coverPoints.Count; i++)
                {
                    var a = Vector3.Angle(_coverPoints[i].transform.forward, threatdir);

                    vAICoverPoint c = _coverPoints[i];
                    Gizmos.color = a > minAngleOfThreat && !c.isOccuped ? Color.blue : Color.red;
                    Gizmos.DrawSphere(c.posePosition, 0.25f);
                    Gizmos.DrawRay(c.posePosition, Vector3.up);
                }

                if (coverPoint)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(coverPoint.posePosition, 0.25f);
                    Gizmos.DrawRay(coverPoint.posePosition, Vector3.up);
                    Gizmos.DrawSphere(threatPos, 0.2f);
                    Gizmos.DrawRay(threatPos, -threatdir.normalized);
                    Gizmos.DrawRay(threatPos, Vector3.up);
                    Gizmos.color = Color.red * 0.8f;
                    Gizmos.DrawLine(transform.position, threatPos);
                }
            }

        }

        internal void OnExitCover()
        {
            if (coverPoint) coverPoint.isOccuped = false;
            coverPoint = null;
            controller.isCrouching = false;
        }

        public void GetCoverTest()
        {
            if (targetTest)
            {
                threatPos = targetTest.position;
                threatdir = threatPos - transform.position;

            }
            else
            {
                threatPos = transform.position + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(minDistanceOfThreat, maxDistanceOfThreat);
                threatPos.y = transform.position.y;
                threatdir = threatPos - transform.position;
            }
            GetCover(true);
        }

        public virtual void GetCoverFromRandomThreat()
        {           
            if (controller == null) return;
            CheckController();
            if (!coverPoint)
            {
                threatPos = transform.position+transform.forward * UnityEngine.Random.Range(minDistanceOfThreat, maxDistanceOfThreat) + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(0,minDistanceOfThreat);
                threatPos.y = transform.position.y;
                
            }
            threatdir = threatPos - transform.position;
            GetCover();
        }

        public virtual void GetCoverFromTargetThreat()
        {

            if (controller == null) return;
            if (controller.currentTarget.transform && controller.currentTarget.isLost == false)
            {
                CheckController();
                threatPos = controller.currentTarget.transform.position;
                threatdir = threatPos - transform.position;
                GetCoverOfTarget();               
            }
            else GetCoverFromRandomThreat();
        }

        protected virtual void CheckController()
        {
            if (!isGointToCoverPoint && ChangeCoverByDamage() && _timeToChangeByDamage < Time.time)
            {
                _timeInCover = 0; _timeToChangeByDamage = Time.time+ UnityEngine.Random.Range(timeToChangeByDamge.x, timeToChangeByDamge.y);
            }
            else if (isGointToCoverPoint && ChangeCoverByDamage())
                _timeToChangeByDamage = Time.time+ UnityEngine.Random.Range(timeToChangeByDamge.x, timeToChangeByDamge.y);

            if (controller.ragdolled)
            {
                controller.isCrouching = false;
                if (coverPoint) coverPoint.isOccuped = false;
                coverPoint = null;
            }
        }

        public virtual void UpdateCoverPoints(Collider[] coverColliders)
        {
            _coverPoints.Clear();
            for (int i = 0; i < coverColliders.Length; i++)
            {
                vAICoverPoint c = coverColliders[i].GetComponent<vAICoverPoint>();
                if (c && c.gameObject.CompareTag(coverTag)) _coverPoints.Add(c);
            }
        }

        public virtual bool HasValidCoversFromPosition(Vector3 threatdir, Vector3 threatPosition)
        {
            var has = _coverPoints.Exists(c => Vector3.Angle(c.transform.forward, threatdir) > minAngleOfThreat && (c.transform.position - threatPosition).magnitude > minDistanceOfThreat && c!=coverPoint);

            return has;
        }

        public virtual void RemoveCoverPoint()
        {
            if (coverPoint)
            {
                coverPoint.isOccuped = false;
                coverPoint = null;
            }
        }

        protected virtual void GetCover(bool forceGet = false)
        {
            if (inGetCover) return;
            if (coverPoint && !forceGet && _timeInCover>Time.time)
            {
                return;
            }
            inGetCover = true;
            if (_coverPoints.Count == 0 || !HasValidCoversFromPosition(threatdir, threatPos))
            {
                var coverColliders = Physics.OverlapSphere(transform.position, getCoverRange, coverLayer);
                UpdateCoverPoints(coverColliders);
               
            }
            var dist = maxDistanceOfThreat;
            var angle = minAngleOfThreat;
            vAICoverPoint cover = null;
            for (int i = 0; i < _coverPoints.Count; i++)
            {
                var coverP = _coverPoints[i];
                if (coverP.isOccuped || !coverP) continue;

                var d = (threatPos - coverP.posePosition).magnitude;
                var a = Vector3.Angle(coverP.transform.forward, threatdir);
                if (d < dist && a > angle && (coverP != coverPoint))
                {
                    dist = d;
                    cover = coverP;
                }
            }
            if (cover != coverPoint && !isGointToCoverPoint && cover)
            {
                if (coverPoint) coverPoint.isOccuped = false;
                coverPoint = cover;
                coverPoint.isOccuped = true;
                _coverPoints.Remove(coverPoint);
                StartCoroutine(GoToCoverPoint());
            }
            inGetCover = false;
        }

        protected virtual void GetCoverOfTarget()
        {
            if (inGetCover) return;
            if (_timeInCover > Time.time)
            {
                if (coverPoint)
                {
                    var a = Vector3.Angle(coverPoint.transform.forward, threatdir);
                    if (a > minAngleOfThreat && controller.targetDistance > minDistanceOfThreat && !controller.currentTarget.isLost) return;
                }
                if (controller.currentTarget.transform && controller.currentTarget.isLost)
                {
                    if (coverPoint)
                        coverPoint.isOccuped = false;
                    coverPoint = null;
                    return;
                }
                if (isGointToCoverPoint) return;
            }
            else if (controller.currentTarget.isLost)
            {
                _timeInCover = Time.time + UnityEngine.Random.Range(timeToChangeCover.x, timeToChangeCover.y);
                if (_coverPoints.Count > 0) _coverPoints.Clear();
                return;
            }
            inGetCover = true;
            var angle = minAngleOfThreat;
            if (_coverPoints.Count == 0 || !HasValidCoversFromPosition(threatdir, threatPos))
            {
                var coverColliders = Physics.OverlapSphere(transform.position, getCoverRange, coverLayer);
                UpdateCoverPoints(coverColliders);
            }

            var dist = maxDistanceOfThreat;

            vAICoverPoint newcoverP = null;

            for (int i = 0; i < _coverPoints.Count; i++)
            {
                var coverp = _coverPoints[i];
                var d = (threatPos - coverp.posePosition).magnitude;// Vector3.Distance(controlAI.currentTarget.transform.position, _coverPoints[i].transform.position);                   
                var a = Vector3.Angle(coverp.transform.forward, threatdir);

                if (coverp.isOccuped) continue;

                if ((d < dist && d > minDistanceOfThreat && a > angle) && coverp != coverPoint)
                {

                    dist = d;
                    newcoverP = coverp;
                }

            }
            if (newcoverP != null && newcoverP != coverPoint && !isGointToCoverPoint)
            {
                if (coverPoint) coverPoint.isOccuped = false;
                coverPoint = newcoverP;
                coverPoint.isOccuped = true;
                _coverPoints.Remove(coverPoint);
                StartCoroutine(GoToCoverPointFromTarget());
            }

            inGetCover = false;
        }

        protected virtual bool ChangeCoverByDamage()
        {
            
            if (!changeCoverByDamage || controller.receivedDamage == null) return false;
            var value = massiveValue ? controller.receivedDamage.massiveValue : controller.receivedDamage.massiveCount;
            return (greaterValue ? (value >= valueToCompare) : (value < valueToCompare));
        }

        protected virtual IEnumerator GoToCoverPointFromTarget()
        {
            if (Vector3.Distance(transform.position, coverPoint.posePosition) > controller.stopingDistance)
            {
                controller.isCrouching = Vector3.Distance(transform.position, coverPoint.posePosition) < 2;
                isGointToCoverPoint = true;
                if(controller.isAiming || controller.isStrafing)
                    controller.StrafeMoveTo(coverPoint.posePosition, controller.currentTarget.transform.position - transform.position);
                else controller.MoveTo(coverPoint.posePosition);
                yield return new WaitForSeconds(1f);
                while (!controller.isInDestination)
                {
                    if (controller.remainingDistance < 1f + controller.stopingDistance && !controller.isCrouching)
                    {
                        controller.isCrouching = true;
                    }

                    if (coverPoint != null)
                    {
                        if (controller.isAiming || controller.isStrafing)
                            controller.StrafeMoveTo(coverPoint.posePosition, controller.currentTarget.transform.position - transform.position);
                        else controller.MoveTo(coverPoint.posePosition);
                    }                       

                    if (controller.targetDistance < minDistanceOfThreat || coverPoint == null)
                    {
                        
                        _timeInCover = 0;
                        
                        break;
                    }
                    yield return null;
                }
            }
            if (coverPoint && controller.targetDistance > minDistanceOfThreat)
                controller.isCrouching = true;
            _timeInCover = Time.time + UnityEngine.Random.Range(timeToChangeCover.x, timeToChangeCover.y);
            isGointToCoverPoint = false;
        }

        protected virtual IEnumerator GoToCoverPoint()
        {
            if (Vector3.Distance(transform.position, coverPoint.posePosition) > controller.stopingDistance)
            {
                controller.isCrouching = Vector3.Distance(transform.position, coverPoint.posePosition) < 2;
                isGointToCoverPoint = true;
                controller.ForceUpdatePath(2);
                if (controller.isAiming || controller.isStrafing)
                    controller.StrafeMoveTo(coverPoint.posePosition, transform.forward);
                else controller.MoveTo(coverPoint.posePosition);
                controller.MoveTo(coverPoint.posePosition);
                yield return new WaitForSeconds(1f);
                while (!controller.isInDestination)
                {
                    if (controller.remainingDistance < 1f + controller.stopingDistance && !controller.isCrouching)
                    {
                        controller.isCrouching = true;
                    }
                    if (!coverPoint) break;
                    yield return null;
                }

            }
            if (coverPoint)
            {
               // controlAI.RotateTo(-coverPoint.transform.forward);
                controller.isCrouching = true;
                _timeInCover = Time.time + UnityEngine.Random.Range(timeToChangeCover.x, timeToChangeCover.y);
            }
            isGointToCoverPoint = false;
        }
        
        protected virtual void RemoveCoverOnDead(GameObject g)
        {
            RemoveCoverPoint();
        }
    }
}