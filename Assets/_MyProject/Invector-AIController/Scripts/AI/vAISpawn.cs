using Invector.vCharacterController.AI.FSMBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI Spawn System", openClose = false)]
    public class vAISpawn : vMonoBehaviour
    {
        public List<vAISpawnProperties> spawnPropertiesList;
        readonly WaitForSeconds waitBetweenSpawnProps = new WaitForSeconds(0.1f);

        IEnumerator Start()
        {
            for (int i = 0; i < spawnPropertiesList.Count; i++)
            {
                yield return waitBetweenSpawnProps;
                StartCoroutine(spawnPropertiesList[i].Spawn(this, OnAISpawned));
            }
        }

        /// <summary>
        /// Spawn a single AI of specific <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="spawnName">Spawn Propertie Name</param>
        public void Spawn(string spawnName)
        {
            var spawnProp = spawnPropertiesList.Find(sp => sp.spawnName.Equals(spawnName));
            if (spawnProp != null)
            {
                StartCoroutine(spawnProp.Spawn(this, OnAISpawned, true));
            }
        }

        /// <summary>
        /// Spawn a single AI of specific  <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="index">Spawn Propertie Index</param>
        public void Spawn(int index)
        {
            if (spawnPropertiesList.Count > 0 && index < spawnPropertiesList.Count)
            {
                StartCoroutine(spawnPropertiesList[index].Spawn(this, null, true));
            }
        }

        /// <summary>
        /// Spawn a single AI of all <seealso cref="vAISpawnProperties"/>
        /// </summary>
        public void SpawnOneOfAll()
        {
            StartCoroutine(SpawnOneOfAllRoutine());
        }

        /// <summary>
        /// Start a specific <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="spawnName">Spawn Propertie Name</param>
        public void StartSpawn(string spawnName)
        {
            var spawnProp = spawnPropertiesList.Find(sp => sp.spawnName.Equals(spawnName));
            if (spawnProp != null)
                spawnProp.pauseSpawning = false;
        }

        /// <summary>
        /// Start a specific <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="spawnName">Spawn Propertie Index</param>
        public void StartSpawn(int index)
        {
            if (spawnPropertiesList.Count > 0 && index < spawnPropertiesList.Count)
            {
                spawnPropertiesList[index].pauseSpawning = false;
            }
        }

        /// <summary>
        /// Start all  <seealso cref="vAISpawnProperties"/>
        /// </summary>
        public void StartSpawnAll()
        {
            StartCoroutine(StartAllRoutine());
        }

        /// <summary>
        /// Pause a specific <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="spawnName">Spawn Propertie Name</param>
        public void PauseSpawn(string spawnName)
        {
            var spawnProp = spawnPropertiesList.Find(sp => sp.spawnName.Equals(spawnName));
            if (spawnProp != null)
                spawnProp.pauseSpawning = true;
        }

        /// <summary>
        /// Pause a specific <seealso cref="vAISpawnProperties"/>
        /// </summary>
        /// <param name="spawnName">Spawn Propertie Index</param>
        public void PauseSpawn(int index)
        {
            if (spawnPropertiesList.Count > 0 && index < spawnPropertiesList.Count)
            {
                spawnPropertiesList[index].pauseSpawning = true;
            }
        }

        /// <summary>
        /// Pause all  <seealso cref="vAISpawnProperties"/>
        /// </summary>
        public void PauseSpawnAll()
        {
            StartCoroutine(PauseAllRoutine());
        }

        IEnumerator SpawnOneOfAllRoutine()
        {
            for (int i = 0; i < spawnPropertiesList.Count; i++)
            {
                yield return waitBetweenSpawnProps;
                StartCoroutine(spawnPropertiesList[i].Spawn(this, null, true));
            }
        }

        IEnumerator StartAllRoutine()
        {
            for (int i = 0; i < spawnPropertiesList.Count; i++)
            {
                yield return waitBetweenSpawnProps;
                spawnPropertiesList[i].pauseSpawning = false;
            }
        }

        IEnumerator PauseAllRoutine()
        {
            for (int i = 0; i < spawnPropertiesList.Count; i++)
            {
                yield return waitBetweenSpawnProps;
                spawnPropertiesList[i].pauseSpawning = false;
            }
        }

        void OnAISpawned(vAIMotor ai, vAISpawnProperties spawnProperties)
        {
            StartCoroutine(spawnProperties.Spawn(this, OnAISpawned));
        }

        [System.Serializable]
        public class vAISpawnProperties : System.Object
        {
            [Header("Spawn Properties")]            
            public string spawnName;
            public vAIMotor prefab;
            public vAIMotor[] randomPrefab;
            public List<vSpawnPoint> spawnPoints;
            [Tooltip("Check this option to Pause the Spawn routine")]
            public bool pauseSpawning;
            public float timeToFirstSpawn = 1f;           
            [Tooltip("Enable or Disable the FSM Controller when Spawn")]
            public bool enableFSMOnSpawn = true;
            [Tooltip("Delay to Enable the FSM Controller")]
            public float delayToEnableFSM = 2;            
            public bool randomTimeToSpawn = true;
            [vHideInInspector("randomTimeToSpawn")]
            public float minTimeBetweenSpawn = 1, maxTimeBetweenSpawn = 10;
            public int maxQuantity = 10;
            public bool keepMaxQuantity = true;
            [Header("AI Detection Settings")]
            public bool overrideDetectionSettings;            
            public vTagMask detectionTags;            
            public LayerMask detectionLayer;                        
            public vTagMask damageTags;
            public LayerMask damageLayer;

            [Header("Spawn Destination")]
            public List<Transform> spawnDestinations;
            public vAIMovementSpeed destinationSpeed = vAIMovementSpeed.Running;
            public float setWaypointAreaDelay;
            public vWaypointArea waypointArea;
            public bool randomDestination;

            [Header("AIs Spawned")]
            public List<vAIMotor> aiSpawnedList;

            [Header("Spawn Events")]
            public UnityEngine.Events.UnityEvent onStartSpawn;
            public UnityEngine.Events.UnityEvent onSpawn;
            public UnityEngine.Events.UnityEvent onDead;
            private bool firstSpawnDone;
            public delegate void OnSpawnAI(vAIMotor ai, vAISpawnProperties spawnProperties);

            private int spawned;
            private int indexOfDestination;
            private bool inSpawn;

            public IEnumerator Spawn(MonoBehaviour mono, OnSpawnAI callBack = null, bool forceSpawn = false)
            {
                aiSpawnedList.RemoveAll(ai => ai == null || ai.isDead);
                spawnPoints.RemoveAll(sp => sp == null);
                spawnDestinations.RemoveAll(sd => sd == null);
                vAIMotor _ai = null;

                if (forceSpawn) spawned = aiSpawnedList.Count;
                if (canSpawn && (!pauseSpawning || forceSpawn) && !inSpawn)
                {
                    inSpawn = true;
                    yield return new WaitForEndOfFrame();
                    var _spawnPoints = spawnPoints.FindAll(sp => sp.isValid);

                    if (_spawnPoints.Count > 0)
                    {
                        onStartSpawn.Invoke();
                        yield return new WaitForSeconds(!firstSpawnDone ? timeToFirstSpawn : randomTimeToSpawn ? Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn) : maxTimeBetweenSpawn);
                        var randomPoint = Mathf.Clamp(Random.Range(-1, _spawnPoints.Count), 0, _spawnPoints.Count - 1);
                        var point = _spawnPoints[randomPoint];

                        if (randomPrefab.Length > 0)
                        {
                            var _prefab = randomPrefab[Random.Range(0, randomPrefab.Length - 1)];
                            if (_prefab)
                            {
                                _ai = Instantiate(_prefab, point.transform.position, point.transform.rotation) as vAIMotor;
                            }
                        }
                        else _ai = Instantiate(prefab, point.transform.position, point.transform.rotation) as vAIMotor;
                        firstSpawnDone = true;

                        if (_ai)
                        {
                            _ai.onDead.AddListener(OnDead);
                            onSpawn.Invoke();

                            aiSpawnedList.Add(_ai);

                            var aiController = _ai.GetComponent<vIControlAI>();
                            yield return new WaitForSeconds(.1f);

                            if (enableFSMOnSpawn)
                            {
                                var fsm = _ai.GetComponent<vIFSMBehaviourController>();
                                if (fsm != null && enableFSMOnSpawn)
                                {
                                    fsm.isStopped = true;
                                    if (delayToEnableFSM <= 0) fsm.isStopped = false;
                                    else mono.StartCoroutine(EnableFSM(fsm));
                                }
                            }

                            if (aiController != null)
                            {
                                aiController.SetSpeed(destinationSpeed);
                                var destination = spawnDestinations.Count == 0 ? aiController.transform.position : GetSpawnDestination();
                                aiController.selfStartPosition = destination;
                                if (spawnDestinations.Count > 0)
                                {
                                    aiController.MoveTo(destination);
                                    //aiController.RotateTo();
                                }

                                if (waypointArea)
                                {
                                    if (setWaypointAreaDelay <= 0)
                                        aiController.waypointArea = waypointArea;
                                    else
                                        mono.StartCoroutine(SetWaypointAreaToAI(aiController));
                                }

                                if (overrideDetectionSettings)
                                {
                                    aiController.SetDetectionLayer(detectionLayer);
                                    aiController.SetDetectionTags(detectionTags);
                                    if (aiController is vIControlAIMelee)
                                    {
                                        var melee = aiController as vIControlAIMelee;
                                        melee.SetMeleeHitTags(damageTags);
                                    }
                                    if (aiController is vIControlAIShooter)
                                    {
                                        var shooter = aiController as vIControlAIShooter;
                                        shooter.SetShooterHitLayer(damageLayer);
                                    }
                                }
                            }
                            spawned++;
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(!firstSpawnDone ? timeToFirstSpawn : randomTimeToSpawn ? Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn) : maxTimeBetweenSpawn);
                }
                inSpawn = false;
                if (!forceSpawn && callBack != null)
                    callBack.Invoke(_ai, this);

            }

            Vector3 GetRandomPoint()
            {
                var pointX = Random.Range(-20, 20);
                var pointZ = Random.Range(-20, 20);
                return new Vector3(pointX, 0, pointZ);
            }

            protected Vector3 GetSpawnDestination()
            {
                var destination = Vector3.zero;
                if (randomDestination)
                {
                    indexOfDestination = Mathf.Clamp(Random.Range(-1, spawnDestinations.Count), 0, spawnDestinations.Count - 1);
                    destination = spawnDestinations[indexOfDestination].transform.position;
                }
                else
                {
                    if (!(indexOfDestination < spawnDestinations.Count))
                    {
                        indexOfDestination = 0;
                    }

                    destination = spawnDestinations[indexOfDestination].transform.position;
                    indexOfDestination++;
                }

                return destination;
            }

            IEnumerator EnableFSM(vIFSMBehaviourController vIFSM)
            {
                if (vIFSM != null)
                {
                    yield return new WaitForSeconds(delayToEnableFSM);
                    vIFSM.isStopped = false;
                }
            }

            IEnumerator SetWaypointAreaToAI(vIControlAI controller)
            {
                yield return new WaitForSeconds(setWaypointAreaDelay);
                controller.waypointArea = waypointArea;
            }

            protected void OnDead(GameObject obj)
            {
                onDead.Invoke();
            }

            public bool canSpawn
            {
                get { return (keepMaxQuantity ? aiSpawnedList.Count : spawned) < maxQuantity && spawnPoints.Count > 0 && (prefab || randomPrefab.Length > 0); }
            }

        }
    }
}