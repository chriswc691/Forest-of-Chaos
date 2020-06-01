using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [SelectionBase]
    [vClassHeader("AI BASIC CONTROLLER", iconName = "AI-icon")]
    public class vControlAI : vAIMotor, vIControlAI
    {
        #region Inspector Variables
        [vEditorToolbar("Start")]
        public bool disableAgentOnStart = true;

        [vEditorToolbar("Agent", order = 5)]
        [SerializeField] protected bool useNavMeshAgent = true;
        [SerializeField] protected vAIUpdateQuality updatePathQuality = vAIUpdateQuality.Medium;
        [SerializeField] [Range(1f, 10f)] protected float aceleration = 8f;
        [SerializeField] [Range(0.05f, 10f)] protected float _stopingDistance = 0.2f;
        [Header("Increase StoppingDistance by speed")]
        [SerializeField] [Range(0.05f, 10f)] protected float _walkingStopingDistance = 0.0f;
        [SerializeField] [Range(0.05f, 10f)] protected float _runningStopingDistance = 0.1f;
        [SerializeField] [Range(0.05f, 10f)] protected float _sprintingStopingDistance = 0.15f;

        [vEditorToolbar("Waypoint", order = 6)]
        [vHelpBox("You can create a new WaypointArea at the Invector/AIController/Components/Create new WaypointArea", vHelpBoxAttribute.MessageType.Info)]
        [SerializeField] protected vWaypointArea _waypointArea;
        [SerializeField] protected float _changeWaypointDistance;
        [SerializeField] protected bool _invertWaypointsOrder;
        [SerializeField] protected bool _randomStartingPoint = true;
        [SerializeField] protected bool _randomWaypoint = true;
        [SerializeField] protected bool startUsingSpecificWaypoint = false;
        [SerializeField] protected int startWaypointIndex;
        [SerializeField] protected bool startUsingNearWayPoint = false;
        [SerializeField] protected bool _selfStartingPoint;
        [SerializeField] protected Transform _customStartingPoint;

        [vEditorToolbar("Detection", order = 7)]
        [vHelpBox("Use a empty trasform inside the headBone transform as reference to the character Eyes", vHelpBoxAttribute.MessageType.None)]
        public Transform detectionPointReference;
        [SerializeField, vEnumFlag] public vAISightMethod sightMethod = vAISightMethod.Center | vAISightMethod.Top;
        [SerializeField] protected vAIUpdateQuality findTargetUpdateQuality = vAIUpdateQuality.High;
        [SerializeField] protected vAIUpdateQuality canseeTargetUpdateQuality = vAIUpdateQuality.Medium;
        [SerializeField, Tooltip("find target with current target found")] protected bool findOtherTarget = false;

        [SerializeField][Range(1,100)] protected int maxTargetsDetection = 10;
        [SerializeField] protected float _changeTargetDelay = 2f;
        [SerializeField] protected bool findTargetByDistance = true;
        [SerializeField] protected float _fieldOfView = 90f;
        [SerializeField] protected float _minDistanceToDetect = 3f;
        [SerializeField] protected float _maxDistanceToDetect = 6f;
        [SerializeField] [vReadOnly] protected bool _hasPositionOfTheTarget;
        [SerializeField] [vReadOnly] protected bool _targetInLineOfSight;
        [vHelpBox("Considerer maxDistanceToDetect value + lostTargetDistance", vHelpBoxAttribute.MessageType.None)]
        [SerializeField] protected float _lostTargetDistance = 4f;
        [SerializeField] protected float _timeToLostWithoutSight = 5f;

        [Header("--- Layers to Detect ----")]
        [SerializeField] protected LayerMask _detectLayer;
        [SerializeField] protected vTagMask _detectTags;
        [SerializeField] protected LayerMask _obstacles = 1 << 0;
        [vEditorToolbar("Debug")]
        [vHelpBox("Debug Options")]
        [SerializeField] protected bool _debugVisualDetection;
        [SerializeField] protected bool _debugRaySight;
        [SerializeField] protected bool _debugLastTargetPosition;
        [SerializeField] protected vAITarget _currentTarget;
        [SerializeField] protected vAIReceivedDamegeInfo _receivedDamage = new vAIReceivedDamegeInfo();
        internal vAIHeadtrack _headtrack;
        internal Collider[] targetsInRange;
       
        protected Vector3 _lastTargetPosition;
        protected int _currentWaypoint;

        private float lostTargetTime;
        private Vector3 lastValidDestination;
        private UnityEngine.AI.NavMeshHit navHit;
        private float changeTargetTime;

        public virtual void CreatePrimaryComponents()
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.mass = 50f;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            if (GetComponent<CapsuleCollider>() == null)
            {
                var capsuler = gameObject.AddComponent<CapsuleCollider>();
                animator = GetComponent<Animator>();
                if (animator)
                {
                    var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                    var height = (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
                    capsuler.height = height;
                    capsuler.center = new Vector3(0, (float)System.Math.Round(capsuler.height * 0.5f, 2), 0);
                    capsuler.radius = (float)System.Math.Round(capsuler.height * 0.15f, 2);
                }
            }
            if (GetComponent<UnityEngine.AI.NavMeshAgent>() == null) gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public virtual void CreateSecondaryComponents()
        {

        }
        protected bool isWaypointStarted;
        #endregion

        #region NavMeshAgent Variables

        protected Vector3 _destination;
        protected Vector3 lasDestination;
        protected Vector3 temporaryDirection;
        [HideInInspector] public UnityEngine.AI.NavMeshAgent navMeshAgent;
        protected UnityEngine.AI.NavMeshHit navMeshHit;
        protected float updatePathTime;
        protected float updateFindTargetTime;
        protected float canseeTargetUpdateTime;
        protected float temporaryDirectionTime;
        protected float timeToResetOutDistance;
        protected float forceUpdatePathTime;
        protected bool isOutOfDistance;
        private int findAgentDestinationRadius;
        #endregion

        #region OVERRIDE METHODS. AI

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (_debugLastTargetPosition)
            {
                if (currentTarget.transform && _hasPositionOfTheTarget)
                {
                    var color = _targetInLineOfSight ? Color.green : Color.red;
                    color.a = 0.2f;
                    Gizmos.color = color;
                    Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, lastTargetPosition + Vector3.up * 1.5f);
                    color.a = 1;
                    Gizmos.color = color;
                    Gizmos.DrawLine(lastTargetPosition, lastTargetPosition + Vector3.up * 1.5f);
                    var forward = (lastTargetPosition - transform.position).normalized;
                    forward.y = 0;
                    var right = Quaternion.AngleAxis(90, Vector3.up) * forward;
                    var p1 = lastTargetPosition + Vector3.up * 1.5f - forward;
                    var p2 = lastTargetPosition + Vector3.up * 1.5f + forward * 0.5f + right * 0.25f;
                    var p3 = lastTargetPosition + Vector3.up * 1.5f + forward * 0.5f - right * 0.25f;
                    Gizmos.DrawLine(p1, p2);
                    Gizmos.DrawLine(p1, p3);
                    Gizmos.DrawLine(p3, p2);
                    Gizmos.DrawSphere(lastTargetPosition + Vector3.up * 1.5f, 0.1f);
                }
            }
        }

        protected override void Start()
        {
            _receivedDamage = new vAIReceivedDamegeInfo();
            changeWaypointDistance = _changeWaypointDistance;
            selfStartPosition = (!_selfStartingPoint && _customStartingPoint) ? _customStartingPoint.position : transform.position;
            _destination = transform.position;
            lasDestination = _destination;
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (!navMeshAgent) return;
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            if (isOnNavMesh) navMeshAgent.enabled = true;
            RotateTo(transform.forward);

            if (currentTarget != null) currentTarget.InitTarget(currentTarget.transform);
            _headtrack = GetComponent<vAIHeadtrack>();
            targetsInRange = new Collider[maxTargetsDetection];
            base.Start();
            aiComponents = new Dictionary<System.Type, vIAIComponent>();
            var _aiComponents = GetComponents<vIAIComponent>();
            for (int i = 0; i < _aiComponents.Length; i++)
            {
                if (!aiComponents.ContainsKey(_aiComponents[i].ComponentType))
                {
                    aiComponents.Add(_aiComponents[i].ComponentType, _aiComponents[i]);
                }
            }
            StartCoroutine(AlignDetectionPoint());
        }

        protected virtual IEnumerator AlignDetectionPoint()
        {
            yield return new WaitForSeconds(.1f);
            if (detectionPointReference) detectionPointReference.rotation = transform.rotation;
        }

        protected override void UpdateAI()
        {
            base.UpdateAI();
            CalcMovementDirection();

            HandleTarget();
            if (receivedDamage != null) receivedDamage.Update();

        }

        public override void ResetRagdoll()
        {
            base.ResetRagdoll();
            if (_headtrack) _headtrack.canLook = true;
        }

        public override void EnableRagdoll()
        {
            base.EnableRagdoll();
            if (_headtrack) _headtrack.canLook = false;
        }

        public override void RemoveComponents()
        {
            base.RemoveComponents();
            if (removeComponentsAfterDie)
                Destroy(navMeshAgent);
        }

        protected override void OnAnimatorMove()
        {
            if (Time.deltaTime == 0) return;
            if (!customAction && useNavMeshAgent && navMeshAgent && navMeshAgent.enabled)
            {
                navMeshAgent.velocity = ((animator.deltaPosition) / Time.deltaTime) * Mathf.Clamp(remainingDistanceWithoutAgent - stopingDistance, 0, 1f);
                //navMeshAgent.speed = Mathf.Clamp((float)System.Math.Round((double)(animator.deltaPosition / Time.deltaTime).magnitude , 2), 0.5f, maxSpeed);              
                navMeshAgent.speed = Mathf.Lerp(navMeshAgent.speed, maxSpeed, aceleration * Time.deltaTime);
                navMeshAgent.nextPosition = animator.rootPosition;
            }

            base.OnAnimatorMove();
        }

        public override void Stop()
        {
            base.Stop();
            if (useNavMeshAgent && navMeshAgent && navMeshAgent.isOnNavMesh && !navMeshAgent.isStopped)
            {
                //_turnOnSpotDirection = transform.forward;
                //temporaryDirection = transform.forward;
                navMeshAgent.isStopped = true;
                this.destination = transform.position;
                ForceUpdatePath();
                navMeshAgent.ResetPath();
            }
        }

        public override void DisableAIController()
        {
            if (disableAgentOnStart && navMeshAgent)
                navMeshAgent.enabled = false;
            base.DisableAIController();
        }

        #endregion

        #region METHODS. AIAgent/Interfaces

        #region Protected methods

        protected virtual Dictionary<System.Type, vIAIComponent> aiComponents { get; set; }

        protected virtual vWaypoint GetWaypoint()
        {
            if (waypointArea == null) return null;
            var waypoints = waypointArea.GetValidPoints(_invertWaypointsOrder);

            if (!isWaypointStarted)
            {
                if (startUsingSpecificWaypoint)
                    _currentWaypoint = startWaypointIndex % waypoints.Count;

                else if (startUsingNearWayPoint)
                    _currentWaypoint = GetNearPointIndex();

                else if (_randomWaypoint)
                    _currentWaypoint = Random.Range(0, waypoints.Count);
                else _currentWaypoint = 0;

            }

            if (isWaypointStarted)
            {
                if (_randomWaypoint)
                    _currentWaypoint = Random.Range(0, waypoints.Count);
                else
                    _currentWaypoint++;

            }

            if (!isWaypointStarted)
            {
                isWaypointStarted = true;
                visitedWaypoints = new List<vWaypoint>();
            }

            if (_currentWaypoint >= waypoints.Count)
                _currentWaypoint = 0;

            if (waypoints.Count == 0)
                return null;

            if (visitedWaypoints.Count == waypoints.Count)
                visitedWaypoints.Clear();

            if (visitedWaypoints.Contains(waypoints[_currentWaypoint]))
                return null;

            return waypoints[_currentWaypoint];
        }

        public int GetNearPointIndex()
        {
            var waypoint = waypointArea.GetValidPoints(_invertWaypointsOrder);
            int targetWay = 0;
            var dist = Mathf.Infinity;
            for (int i = 0; i < waypoint.Count; i++)
            {
                var d = Vector3.Distance(transform.position, waypoint[i].position);
                if (d < dist)
                {
                    targetWay = i;
                    dist = d;
                }
            }
            return targetWay;
        }

        protected float GetUpdateTimeFromQuality(vAIUpdateQuality quality)
        {
            return quality == vAIUpdateQuality.VeryLow ? 2 : quality == vAIUpdateQuality.Low ? 1f : quality == vAIUpdateQuality.Medium ? 0.75f : quality == vAIUpdateQuality.High ? .25f : 0.1f;
        }

        protected virtual Vector3 destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
            }
        }

        protected virtual void UpdateAgentPath()
        {
            updatePathTime -= Time.deltaTime;
            if (updatePathTime > 0 && forceUpdatePathTime <= 0f && navMeshAgent.hasPath) return;
            forceUpdatePathTime -= Time.deltaTime;
            updatePathTime = GetUpdateTimeFromQuality(updatePathQuality);

            if (!isDead && !isJumping && isGrounded)
            {
                var destin = _destination;

                if ((movementSpeed != vAIMovementSpeed.Idle && destin != lasDestination) || !navMeshAgent.hasPath)
                {
                    if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                    {
                        if (UnityEngine.AI.NavMesh.SamplePosition(destin, out navHit, _capsuleCollider.radius + findAgentDestinationRadius, navMeshAgent.areaMask) && (navHit.position - navMeshAgent.destination).magnitude > stopingDistance)
                        {
                            navMeshAgent.destination = (navHit.position);
                            lasDestination = destin;
                        }
                        else if ((navHit.position - navMeshAgent.destination).magnitude > stopingDistance)
                        {
                            findAgentDestinationRadius++;
                            if (findAgentDestinationRadius >= 10)
                            {
                                findAgentDestinationRadius = 0;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void CalcMovementDirection()
        {
            if (isDead || isJumping) return;

            if (useNavMeshAgent && navMeshAgent)
            {
                ControlNavMeshAgent();
                UpdateAgentPath();
            }

            bool forceMovement = !navMeshAgent.hasPath && remainingDistanceWithoutAgent > navMeshAgent.stoppingDistance + _capsuleCollider.radius;
            var dir = !forceMovement && navMeshAgent != null && navMeshAgent.enabled && useNavMeshAgent ? navMeshAgent.desiredVelocity * (!isInDestination ? 1 : 0) :
                ((new Vector3(destination.x, transform.position.y, destination.z) - transform.position).normalized * Mathf.Clamp(remainingDistanceWithoutAgent - stopingDistance, 0, 1f));
            //Convert Direction to Input
            var movementInput = transform.InverseTransformDirection(dir);

            if (useNavMeshAgent && navMeshAgent.enabled)
            {
                var data = navMeshAgent.currentOffMeshLinkData;
                if (navMeshAgent.isOnOffMeshLink)
                {
                    dir = (data.endPos - transform.position);
                    movementInput = transform.InverseTransformDirection(dir);
                }
            }
            if (movementInput.magnitude > 0.1f)
            {
                if (temporaryDirectionTime <= 0 && isStrafing == false)
                    SetMovementInput(movementInput, aceleration);
                else
                    SetMovementInput(movementInput, temporaryDirectionTime <= 0 ? transform.forward : temporaryDirection, aceleration);
            }
            else if (temporaryDirectionTime > 0 && temporaryDirection.magnitude >= 0.1f && movementInput.magnitude < 0.1f)
            {
                TurnOnSpot(temporaryDirection);
            }
            else input = Vector3.zero;
            if (!isGrounded || isJumping || isRolling) navMeshAgent.enabled = false;
            temporaryDirectionTime -= Time.deltaTime;
        }

        protected virtual void CheckAgentDistanceFromAI()
        {
            if (!useNavMeshAgent || !navMeshAgent || !navMeshAgent.enabled) return;
            if (Vector3.Distance(transform.position, navMeshAgent.nextPosition) > stopingDistance * 1.5f && !isOutOfDistance)
            {
                timeToResetOutDistance = 3f;
                isOutOfDistance = true;
            }
            if (isOutOfDistance)
            {
                timeToResetOutDistance -= Time.deltaTime;
                if (timeToResetOutDistance <= 0)
                {
                    isOutOfDistance = false;
                    if (Vector3.Distance(transform.position, navMeshAgent.nextPosition) > stopingDistance)
                    {
                        navMeshAgent.enabled = false;
                    }

                }
            }
        }

        protected virtual void ControlNavMeshAgent()
        {
            if (isDead) return;
            if (useNavMeshAgent && navMeshAgent)
                navMeshAgent.stoppingDistance = stopingDistance;
            if (Time.deltaTime == 0 || navMeshAgent.enabled == false)
            {
                if (!ragdolled && !isJumping && isGrounded && !navMeshAgent.enabled && isOnNavMesh)
                {
                    navMeshAgent.enabled = true;
                }
            }

            if (navMeshAgent.enabled && isOnJumpLink && !isJumping && isGrounded)
            {
                var jumpDir = navMeshAgent.currentOffMeshLinkData.endPos - transform.position;
                var jumpTarget = transform.position+jumpDir.normalized*(jumpDir.magnitude+stopingDistance);
                JumpTo(jumpTarget);
            }

            if (isJumping || !isGrounded || ragdolled)
                navMeshAgent.enabled = false;
            CheckAgentDistanceFromAI();
        }

        protected virtual bool CheckCanSeeTarget()
        {
            if (currentTarget != null && currentTarget.transform != null && currentTarget.collider == null && InFOVAngle(currentTarget.transform.position, _fieldOfView))
            {
                if (sightMethod == 0) return true;
                var eyesPoint = detectionPointReference ? detectionPointReference.position : transform.position + Vector3.up * (selfCollider.bounds.size.y * 0.8f);
                if (!Physics.Linecast(eyesPoint, currentTarget.transform.position, _obstacles))
                {
                    if (_debugRaySight)
                        Debug.DrawLine(eyesPoint, currentTarget.transform.position, Color.green, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                    return true;
                }
                else
                {
                    if (_debugRaySight)
                        Debug.DrawLine(eyesPoint, currentTarget.transform.position, Color.red, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                }
            }
            else if (currentTarget.collider) return CheckCanSeeTarget(currentTarget.collider);

            return false;
        }

        protected virtual bool CheckCanSeeTarget(Collider target)
        {
            if (target != null && InFOVAngle(target.bounds.center, _fieldOfView))
            {
                if (sightMethod == 0) return true;
                var detectionPoint = detectionPointReference ? detectionPointReference.position : transform.position + Vector3.up * (selfCollider.bounds.size.y * 0.8f);
                if (sightMethod.Contains<vAISightMethod>(vAISightMethod.Center))
                    if (!Physics.Linecast(detectionPoint, target.bounds.center, _obstacles))
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.bounds.center, Color.green, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                        return true;
                    }
                    else
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.bounds.center, Color.red, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                    }
                if (sightMethod.Contains<vAISightMethod>(vAISightMethod.Top))
                    if (!Physics.Linecast(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.9f, _obstacles))
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.9f, Color.green, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                        return true;
                    }
                    else
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.9f, Color.red, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                    }
                if (sightMethod.Contains<vAISightMethod>(vAISightMethod.Bottom))
                    if (!Physics.Linecast(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.1f, _obstacles))
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.1f, Color.green, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                        return true;
                    }
                    else
                    {
                        if (_debugRaySight) Debug.DrawLine(detectionPoint, target.transform.position + Vector3.up * target.bounds.size.y * 0.1f, Color.red, GetUpdateTimeFromQuality(canseeTargetUpdateQuality));
                    }
            }
            return false;
        }

        protected virtual bool InFOVAngle(Vector3 viewPoint, float fieldOfView)
        {
            var eyesPoint = (detectionPointReference ? detectionPointReference.position : _capsuleCollider.bounds.center);
            if (Vector3.Distance(eyesPoint, viewPoint) < _minDistanceToDetect) return true;
            if (Vector3.Distance(eyesPoint, viewPoint) > _maxDistanceToDetect) return false;

            var lookDirection = viewPoint - eyesPoint;
            var rot = Quaternion.LookRotation(lookDirection, Vector3.up);
            var detectionAngle = detectionPointReference ? detectionPointReference.eulerAngles : transform.eulerAngles;
            var newAngle = rot.eulerAngles - detectionAngle;
            var fovAngleY = newAngle.NormalizeAngle().y;
            var fovAngleX = newAngle.NormalizeAngle().x;
            if (fovAngleY <= (fieldOfView * 0.5f) && fovAngleY >= -(fieldOfView * 0.5f) && fovAngleX <= (fieldOfView * 0.5f) && fovAngleX >= -(fieldOfView * 0.5f))
                return true;

            return false;
        }

        protected virtual void HandleTarget()
        {
            if (_hasPositionOfTheTarget && currentTarget.transform) lastTargetPosition = currentTarget.transform.position;
            canseeTargetUpdateTime -= Time.deltaTime;
            if (canseeTargetUpdateTime > 0) return;
            if (currentTarget != null && currentTarget.transform)
            {
                _targetInLineOfSight = CheckCanSeeTarget();
                if (!_targetInLineOfSight || targetDistance >= (_maxDistanceToDetect + _lostTargetDistance))
                {
                    if (lostTargetTime < Time.time)
                    {
                        _hasPositionOfTheTarget = false;
                        lostTargetTime = Time.time + _timeToLostWithoutSight;
                    }
                }
                else
                {
                    lostTargetTime = Time.time + _timeToLostWithoutSight;
                    _hasPositionOfTheTarget = true;
                    currentTarget.isLost = false;
                }
            }
            else
            {
                _targetInLineOfSight = false;
                _hasPositionOfTheTarget = false;
            }
            HandleLostTarget();
            canseeTargetUpdateTime = GetUpdateTimeFromQuality(canseeTargetUpdateQuality);
        }

        protected virtual void HandleLostTarget()
        {
            if (currentTarget != null && currentTarget.transform != null)
            {
                if (currentTarget.hasHealthController && (currentTarget.isDead || targetDistance > (_maxDistanceToDetect + _lostTargetDistance) || (!targetInLineOfSight && !_hasPositionOfTheTarget)))
                {
                    if (currentTarget.isFixedTarget)
                        currentTarget.isLost = true;
                    else
                        currentTarget.ClearTarget();
                }
                else if (!currentTarget.hasHealthController && (currentTarget.transform == null || !currentTarget.transform.gameObject.activeSelf || targetDistance > (_maxDistanceToDetect + _lostTargetDistance) || (!targetInLineOfSight && !_hasPositionOfTheTarget)))
                {
                    if (currentTarget.isFixedTarget)
                        currentTarget.isLost = true;
                    else
                        currentTarget.ClearTarget();
                }
            }
        }

        protected static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        #endregion

        #region Public methods 

        public virtual void SetDetectionLayer(LayerMask mask)
        {
            _detectLayer = mask;
        }

        public virtual void SetDetectionTags(List<string> tags)
        {
            _detectTags = tags;
        }

        public virtual void SetObstaclesLayer(LayerMask mask)
        {
            _obstacles = mask;
        }

        public virtual void SetLineOfSight(float fov = -1, float minDistToDetect = -1, float maxDistToDetect = -1, float lostTargetDistance = -1)
        {
            if (fov != -1) _fieldOfView = fov;
            if (minDistToDetect != -1) _minDistanceToDetect = minDistToDetect;
            if (maxDistToDetect != -1) _maxDistanceToDetect = maxDistToDetect;
            if (lostTargetDistance != -1) _lostTargetDistance = lostTargetDistance;
        }

        public virtual vAIReceivedDamegeInfo receivedDamage { get { return _receivedDamage; } protected set { _receivedDamage = value; } }

        public virtual bool targetInLineOfSight { get { return _targetInLineOfSight; } }

        public virtual vAITarget currentTarget { get { return _currentTarget; } protected set { _currentTarget = value; } }

        public virtual Vector3 lastTargetPosition
        {
            get
            {
                return _lastTargetPosition;
            }
            protected set
            {
                _lastTargetPosition = value;
            }
        }

        public virtual float targetDistance
        {
            get
            {
                if (currentTarget == null || currentTarget.isDead) return Mathf.Infinity;
                return Vector3.Distance(currentTarget.transform.position, transform.position);
            }
        }

        public virtual void FindTarget()
        {
            FindSpecificTarget(_detectTags, _detectLayer, true);
        }

        public virtual void FindTarget(bool checkForObstacles)
        {
            FindSpecificTarget(_detectTags, _detectLayer, checkForObstacles);
        }
        
        public virtual void FindSpecificTarget(List<string> m_detectTags, LayerMask m_detectLayer, bool checkForObstables = true)
        {
            if (updateFindTargetTime > Time.time) return;
            updateFindTargetTime = Time.time + GetUpdateTimeFromQuality(findTargetUpdateQuality);
            if (!findOtherTarget && currentTarget.transform) return;
            if (currentTarget.transform && currentTarget.isFixedTarget && !findOtherTarget) return;
            int targetsCount = Physics.OverlapSphereNonAlloc(transform.position + transform.up, _maxDistanceToDetect, targetsInRange, m_detectLayer);
            if (targetsCount > 0)
            {
                Transform target = currentTarget != null && _hasPositionOfTheTarget ? currentTarget.transform : null;
                var _targetDistance = target && targetInLineOfSight ? targetDistance : Mathf.Infinity;

                for (int i = 0; i < targetsCount; i++)
                {
                    if (targetsInRange[i] != null && targetsInRange[i].transform != transform && m_detectTags.Contains(targetsInRange[i].gameObject.tag) && (!checkForObstables || CheckCanSeeTarget(targetsInRange[i])))
                    {
                        if (findTargetByDistance)
                        {
                            var newTargetDistance = Vector3.Distance(targetsInRange[i].transform.position, transform.position);

                            if (newTargetDistance < _targetDistance)
                            {
                                target = targetsInRange[i].transform;
                                _targetDistance = newTargetDistance;
                            }
                        }
                        else
                        {
                            target = targetsInRange[i].transform;
                            break;
                        }
                    }
                }

                if (currentTarget == null || target != null && target != currentTarget.transform)
                {
                    if (target != null)
                        SetCurrentTarget(target);
                }
            }
            
        }        
      
        public bool TryGetTarget(out vAITarget target)
        {
            return TryGetTarget(_detectTags, out target);
        }

        public bool TryGetTarget(string tag,out vAITarget target)
        {
            Collider[] ts = System.Array.FindAll(targetsInRange, c => c != null && c.gameObject.CompareTag(tag));
            if(ts!=null && ts.Length>1)
            {
                System.Array.Sort(ts, delegate (Collider a, Collider b)
                {
                    return Vector2.Distance(this.transform.position, a.transform.position)
                    .CompareTo(
                      Vector2.Distance(this.transform.position, b.transform.position));
                });
            }
           
            if (ts != null && ts.Length > 0)
            {
                target = new vAITarget();
                target.InitTarget(ts[0].transform);
                return true;
            }
            target = null;
            return false;
        }

        public bool TryGetTarget(List<string> detectTags, out vAITarget target)
        {
            Collider[] ts = System.Array.FindAll(targetsInRange, c => c!=null && detectTags.Contains(c.gameObject.tag));
            if (ts != null && ts.Length > 1)
            {
                System.Array.Sort(ts, delegate (Collider a, Collider b)
                {
                    return Vector2.Distance(this.transform.position, a.transform.position)
                    .CompareTo(
                      Vector2.Distance(this.transform.position, b.transform.position));
                });
            }

            if (ts != null && ts.Length > 0)
            {
                target = new vAITarget();
                target.InitTarget(ts[0].transform);
                return true;
            }
            target = null;
            return false;
        }

        public virtual void SetCurrentTarget(Transform target)
        {
            SetCurrentTarget(target, false);
        }

        public virtual void SetCurrentTarget(Transform target, bool overrideCanseTarget)
        {
            if (changeTargetTime < Time.time)
            {
                changeTargetTime = _changeTargetDelay + Time.time;
                currentTarget.InitTarget(target);
                if (overrideCanseTarget)
                {
                    currentTarget.isLost = false;
                    _targetInLineOfSight = true;
                    _hasPositionOfTheTarget = false;
                }
                updateFindTargetTime = 0f;
                updatePathTime = 0f;
                lastTargetPosition = target.position;                
                LookToTarget(target, 2);
            }
        }

        public virtual void RemoveCurrentTarget()
        {
            currentTarget.ClearTarget();
        }

        public virtual void LookAround()
        {
            if (_headtrack) _headtrack.LookAround();
        }

        public virtual void LookTo(Vector3 point, float stayLookTime = 1, float offsetLookHeight = -1)
        {
            if (_headtrack) _headtrack.LookAtPoint(point, stayLookTime, offsetLookHeight);
        }

        public virtual void LookToTarget(Transform target, float stayLookTime = 1, float offsetLookHeight = -1)
        {
            if (_headtrack) _headtrack.LookAtTarget(target, stayLookTime, offsetLookHeight);
        }

        public virtual void SetSpeed(vAIMovementSpeed movementSpeed)
        {
            if (this.movementSpeed != movementSpeed)
            {
                if (movementSpeed == vAIMovementSpeed.Idle)
                {
                    Stop();

                }
                base.movementSpeed = movementSpeed;
            }
        }

        public virtual bool isInDestination
        {
            get
            {
                if (useNavMeshAgent && (remainingDistance <= stopingDistance || navMeshAgent.hasPath && remainingDistance > stopingDistance && navMeshAgent.desiredVelocity.magnitude < 0.1f)) return true;
                return remainingDistance <= stopingDistance;
            }
        }

        public virtual bool isMoving
        {
            get
            {
                return input.sqrMagnitude > 0.1f;
            }
        }

        public virtual float remainingDistance
        {
            get
            {
                return navMeshAgent && navMeshAgent.enabled && useNavMeshAgent && isOnNavMesh ? navMeshAgent.remainingDistance : remainingDistanceWithoutAgent;
            }
        }

        protected virtual float remainingDistanceWithoutAgent
        {
            get
            {
                return Vector3.Distance(transform.position, new Vector3(destination.x, transform.position.y, destination.z));
            }
        }

        public virtual Collider selfCollider
        {
            get { return _capsuleCollider; }
        }

        public virtual bool isOnJumpLink
        {
            get
            {
                if (!useNavMeshAgent) return false;
                if (navMeshAgent.isOnOffMeshLink && navMeshAgent.currentOffMeshLinkData.linkType == UnityEngine.AI.OffMeshLinkType.LinkTypeJumpAcross) return true;
                var linkData = navMeshAgent.currentOffMeshLinkData.offMeshLink;
                if (linkData != null)
                {
                    if (linkData.area == UnityEngine.AI.NavMesh.GetAreaFromName("Jump"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual bool isOnNavMesh
        {
            get
            {
                if (!useNavMeshAgent) return false;
                if (navMeshAgent.enabled) return navMeshAgent.isOnNavMesh;

                if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out navMeshHit, _capsuleCollider.radius, navMeshAgent.areaMask))
                {
                    return true;
                }
                return false;
            }
        }

        public virtual void MoveTo(Vector3 newDestination)
        {
            // if (useNavMeshAgent && navMeshAgent && navMeshAgent.isOnNavMesh && navMeshAgent.isStopped) navMeshAgent.isStopped = false;
            if (isStrafing) updatePathTime = 0;
            SetFreeLocomotion();
            var dir = newDestination - transform.position;
            dir.y = 0;
            this.destination = newDestination;
            temporaryDirection = transform.forward;
            temporaryDirectionTime = 0;
        }

        public virtual void StrafeMoveTo(Vector3 newDestination, Vector3 targetDirection)
        {
            if (useNavMeshAgent && navMeshAgent && navMeshAgent.isOnNavMesh && navMeshAgent.isStopped) navMeshAgent.isStopped = false;
            SetStrafeLocomotion();
            destination = newDestination;
            temporaryDirection = targetDirection;
            temporaryDirectionTime = 2f;
        }
      
        public virtual void RotateTo(Vector3 targetDirection)
        {
            targetDirection.y = 0;
            if (Vector3.Angle(transform.forward, targetDirection) > 20)
            {
                temporaryDirection = targetDirection;
                temporaryDirectionTime = 2f;
            }
        }

        public virtual Vector3 targetDestination
        {
            get { return _destination; }
        }

        public virtual float stopingDistance { get { return stopingDistanceRelativeToSpeed + _stopingDistance; } set { _stopingDistance = value; } }

        protected virtual float stopingDistanceRelativeToSpeed
        {
            get
            {
                return movementSpeed == vAIMovementSpeed.Idle ? 1 : movementSpeed == vAIMovementSpeed.Running ? _runningStopingDistance : movementSpeed == vAIMovementSpeed.Sprinting ? _sprintingStopingDistance : _walkingStopingDistance;
            }
        }

        public virtual Vector3 selfStartPosition { get; set; }

        public virtual vWaypointArea waypointArea
        {
            get
            {
                return _waypointArea;
            }
            set
            {
                if (value != null && value != _waypointArea)
                {
                    var waypoints = value.GetValidPoints();
                    if (_randomStartingPoint)
                        _currentWaypoint = Random.Range(0, waypoints.Count);
                }
                _waypointArea = value;
            }
        }

        public virtual vWaypoint targetWaypoint { get; protected set; }

        public virtual List<vWaypoint> visitedWaypoints { get; set; }

        public virtual bool selfStartingPoint { get { return _selfStartingPoint; } protected set { _selfStartingPoint = value; } }

        public virtual float changeWaypointDistance { get; protected set; }

        public bool customStartPoint
        {
            get
            {
                return !selfStartingPoint && _customStartingPoint != null;
            }
        }

        public Vector3 customStartPosition
        {
            get
            {
                return customStartPoint ? _customStartingPoint.position : transform.position;
            }
        }

        public virtual void NextWayPoint()
        {
            targetWaypoint = GetWaypoint();
        }

        public override void TakeDamage(vDamage damage)
        {
            base.TakeDamage(damage);

            if (damage.damageValue > 0)
            {
                //Check condition to add a new target
                if (!currentTarget.transform || (currentTarget.transform && !currentTarget.isFixedTarget || (currentTarget.isFixedTarget && findOtherTarget)))
                {
                    //Check if new target is in detections settings
                    if (damage.sender && IsInLayerMask(damage.sender.gameObject.layer, _detectLayer) && _detectTags.Contains(damage.sender.gameObject.tag))
                    {
                        SetCurrentTarget(damage.sender, false);
                    }
                }
                receivedDamage.UpdateDamage(damage);

                updatePathTime = 0f;
            }
        }

        public void ForceUpdatePath(float timeInUpdate = 1f)
        {
            forceUpdatePathTime = timeInUpdate;
        }

        public bool HasComponent<T>() where T : vIAIComponent
        {
            return aiComponents.ContainsKey(typeof(T));
        }

        public T GetAIComponent<T>() where T : vIAIComponent
        {
            return aiComponents.ContainsKey(typeof(T)) ? (T)aiComponents[typeof(T)] : default(T);
        }

        #endregion

        #endregion

    }
}