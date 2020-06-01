using System.Collections.Generic;
using UnityEngine;

namespace Invector.vShooter
{
    using Invector.vEventSystems;
    [vClassHeader("Projectile Control", "The damage value is changed from minDamage, maxDamage, DropOffStart, DropOffEnd of the ShooterWeapon", openClose = false)]
    public class vProjectileControl : vMonoBehaviour
    {
        public vBulletLifeSettings bulletLifeSettings;
        public int bulletLife = 100;
        public bool debugTrajetory;
        public bool debugHittedObject;
        public vDamage damage;
        public float forceMultiplier = 1;
        public bool destroyOnCast = true;
        public ProjectilePassDamage onPassDamage;
        public ProjectileCastColliderEvent onCastCollider;
        public ProjectileCastColliderEvent onDestroyProjectile;
        [HideInInspector]
        public bool damageByDistance;
        [HideInInspector]
        public int minDamage;
        [HideInInspector]
        public int maxDamage;
        [HideInInspector]
        public float DropOffStart = 8f;
        [HideInInspector]
        public float velocity = 580;
        [HideInInspector]
        public float DropOffEnd = 50f;
        [HideInInspector]
        public Vector3 startPosition;
        [HideInInspector]
        public LayerMask hitLayer = -1;
        [HideInInspector]
        public List<string> ignoreTags;
        [HideInInspector]
        public Transform shooterTransform;

        protected Vector3 previousPosition;
        protected Rigidbody _rigidBody;
        protected Color debugColor = Color.green;
        private int debugLife;
        private float castDist;

        protected virtual void Start()
        {
            transform.SetParent(vObjectContainer.root, true);
            debugLife = bulletLife;
            _rigidBody = GetComponent<Rigidbody>();
            startPosition = transform.position;
            previousPosition = transform.position - transform.forward * 0.1f;
        }

        protected virtual void Update()
        {
            RaycastHit hitInfo;
            if (_rigidBody.velocity.magnitude > 1)
                transform.rotation = Quaternion.LookRotation(_rigidBody.velocity.normalized, transform.up);
            if (Physics.Linecast(previousPosition, transform.position + transform.forward * 0.5f, out hitInfo, hitLayer))
            {
                if (!hitInfo.collider)
                    return;

                var dist = Vector3.Distance(startPosition, transform.position) + castDist;
                if (!(ignoreTags.Contains(hitInfo.collider.gameObject.tag) || (shooterTransform != null && hitInfo.collider.transform.IsChildOf(shooterTransform))))
                {
                    if (debugHittedObject) Debug.Log(hitInfo.collider.gameObject.name, hitInfo.collider);
                    onCastCollider.Invoke(hitInfo);
                    damage.damageValue = maxDamage;
                    if (damageByDistance)
                    {
                        var result = 0f;
                        var damageDifence = maxDamage - minDamage;

                        //Calc damage per distance
                        if (dist - DropOffStart >= 0)
                        {
                            int percentComplete = (int)System.Math.Round((double)(100 * (dist - DropOffStart)) / (DropOffEnd - DropOffStart));
                            result = Mathf.Clamp(percentComplete * 0.01f, 0, 1f);
                            damage.damageValue = maxDamage - (int)(damageDifence * result);
                        }
                        else
                            damage.damageValue = maxDamage;
                    }
                    damage.hitPosition = hitInfo.point;
                    damage.receiver = hitInfo.collider.transform;

                    if (damage.damageValue > 0)
                    {
                        onPassDamage.Invoke(damage);
                        hitInfo.collider.gameObject.ApplyDamage(damage, damage.sender.GetComponent<vIMeleeFighter>());
                    }

                    var rigb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                    if (rigb && !hitInfo.collider.gameObject.isStatic)
                    {
                        rigb.AddForce(transform.forward * damage.damageValue * forceMultiplier, ForceMode.Impulse);
                    }
                    transform.position = hitInfo.point + transform.forward * 0.02f;

                    startPosition = transform.position;
                    castDist = dist;
                    if (debugTrajetory) Debug.DrawLine(transform.position, previousPosition, debugColor, 10f);

                    if (destroyOnCast)
                    {
                        if (bulletLifeSettings)
                        {
                            var bulletLifeInfo = bulletLifeSettings.GetReduceLife(hitInfo.collider.gameObject.tag, hitInfo.collider.gameObject.layer);
                            bulletLife -= bulletLifeInfo.lostLife;
                            if (debugTrajetory) DrawHitPoint(hitInfo.point);

                            var crossed = false;
                            if (bulletLife > 0 && !bulletLifeInfo.ricochet)
                            {
                                for (float i = 0; i <= bulletLifeInfo.maxThicknessToCross; i += 0.01f)
                                {
                                    var point = transform.position + transform.forward * (i);
                                    if (!hitInfo.collider.bounds.Contains(point))
                                    {
                                        hitInfo.point = point;
                                        hitInfo.normal = transform.forward;
                                        onCastCollider.Invoke(hitInfo);
                                        crossed = true;
                                        break;
                                    }
                                }
                            }

                            if (!crossed && !bulletLifeInfo.ricochet)
                            {
                                bulletLife = 0;
                                transform.position = hitInfo.point;
                                onDestroyProjectile.Invoke(hitInfo);
                                Destroy(gameObject);
                            }

                            maxDamage -= (maxDamage) - ((maxDamage * bulletLifeInfo.lostDamage) / 100);
                            minDamage -= (minDamage) - ((minDamage * bulletLifeInfo.lostDamage) / 100);
                            if (maxDamage < 0) maxDamage = 0;
                            if (minDamage < 0) minDamage = 0;
                            var x = Random.Range(bulletLifeInfo.minChangeTrajectory, bulletLifeInfo.maxChangeTrajectory) * (Random.Range(-1, 1) >= 0 ? 1 : -1);
                            var y = Random.Range(bulletLifeInfo.minChangeTrajectory, bulletLifeInfo.maxChangeTrajectory) * (Random.Range(-1, 1) >= 0 ? 1 : -1);

                            if (y > 60 || y < -60) x = Mathf.Clamp(x, -15, 15);
                            if (x != 0 || y != 0)
                            {
                                var dir = Quaternion.Euler(x, y, 0) * _rigidBody.velocity;
                                if (dir != Vector3.zero)
                                {
                                    _rigidBody.velocity = dir * (bulletLifeInfo.ricochet ? -1 : 1);

                                    transform.forward = dir * (bulletLifeInfo.ricochet ? -1 : 1);
                                }
                            }
                            if (debugTrajetory)
                            {
                                var lostedLifePercent = ((float)bulletLife / (float)debugLife) * 100f;
                                debugColor = lostedLifePercent > 76 ? Color.green : lostedLifePercent > 51 ? Color.yellow : lostedLifePercent > 26 ? new Color(1, .5f, 0) : Color.red;
                                debugColor.a = 0.5f;
                            }
                        }

                        if (bulletLife <= 0 || bulletLifeSettings == null)
                        {
                            transform.position = hitInfo.point;
                            onDestroyProjectile.Invoke(hitInfo);
                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    transform.position = hitInfo.point + transform.forward * 0.02f;
                    if (debugTrajetory) Debug.DrawLine(transform.position, previousPosition, debugColor, 10f);
                }
            }
            else if (debugTrajetory)
            {
                Debug.DrawLine(transform.position, previousPosition, debugColor, 10f);
            }

            previousPosition = transform.position;

        }

        void DrawHitPoint(Vector3 point)
        {
            Debug.DrawRay(point, -transform.forward * 0.1f, Color.red, 10f);
            Debug.DrawRay(point, transform.right * 0.1f, Color.red, 10f);
            Debug.DrawRay(point, -transform.right * 0.1f, Color.red, 10f);
            Debug.DrawRay(point, transform.up * 0.1f, Color.red, 10f);
            Debug.DrawRay(point, -transform.up * 0.1f, Color.red, 10f);
        }

        public void RemoveParentOfOther(Transform other)
        {
            other.SetParent(vObjectContainer.root, true);
        }

        [System.Serializable]
        public class ProjectileCastColliderEvent : UnityEngine.Events.UnityEvent<RaycastHit> { }
        [System.Serializable]
        public class ProjectilePassDamage : UnityEngine.Events.UnityEvent<vDamage> { }

    }
}