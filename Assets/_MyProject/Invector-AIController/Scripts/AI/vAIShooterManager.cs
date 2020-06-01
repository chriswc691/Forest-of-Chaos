
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vCharacterController.AI
{
    using vShooter;
    [vClassHeader("AI SHOOTER MANAGER", "Make sure to set the Damage Layers to 'Default' and 'BodyPart', or any other layer you need to inflict damage.")]
    public class vAIShooterManager : vMonoBehaviour
    {
        #region variables

        [System.Serializable]
        public class OnReloadWeapon : UnityEngine.Events.UnityEvent<vShooterWeapon> { }

        [vEditorToolbar("Aim")]     
        [Tooltip("min distance to aim")]
        public float minDistanceToAim = 1;
        public float checkAimRadius = 0.1f;
        [Tooltip("smooth of the right hand when correcting the aim")]
        public float smoothHandRotation = 30f;
        [Tooltip("Limit the maxAngle for the right hand to correct the aim")]
        public float maxHandAngle = 60f;  
        [Tooltip("Check this to syinc the weapon aim to the camera aim")]
        public bool raycastAimTarget = true;
        [Tooltip("Layer to aim")]
        public LayerMask damageLayer = 1 << 0;
        [Tooltip("Tags to the Aim ignore - tag this gameObject to avoid shot on yourself")]
        public List<string> ignoreTags;

        [vEditorToolbar("IK Adjust")]
        [Tooltip("Check this to use IK on the left hand")]
        public bool useLeftIK = true, useRightIK = true;  
        public vWeaponIKAdjustList weaponIKAdjustList;

        [vEditorToolbar("Weapons")]
        public vShooterWeapon rWeapon, lWeapon;
        [HideInInspector]
        public OnReloadWeapon onReloadWeapon;

      
        private Animator animator;
        private int totalAmmo;
        private int secundaryTotalAmmo;
        private float currentShotTime;
        protected vWeaponIKAdjust currentWeaponIKAdjust;
        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();

            if (animator)
            {
                var _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var _lefttHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                var weaponR = _rightHand.GetComponentInChildren<vShooterWeapon>(true);
                var weaponL = _lefttHand.GetComponentInChildren<vShooterWeapon>(true);
                if (weaponR != null)
                    SetRightWeapon(weaponR.gameObject);
                if (weaponL != null)
                    SetLeftWeapon(weaponL.gameObject);
            }

            if (!ignoreTags.Contains(gameObject.tag))
                ignoreTags.Add(gameObject.tag);
        }
        public void SetDamageLayer(LayerMask mask)
        {
            damageLayer = mask;
            if (currentWeapon) currentWeapon.hitLayer = mask;
        }
        public void SetLeftWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponentInChildren<vShooterWeapon>(true);
                lWeapon = w;
                if (lWeapon)
                {
                    lWeapon.ignoreTags = ignoreTags;
                    lWeapon.hitLayer = damageLayer;
                    lWeapon.root = transform;
                    lWeapon.isSecundaryWeapon = false;
                    lWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (lWeapon.dontUseReload) ReloadWeaponAuto(lWeapon, false);
                    if (lWeapon.secundaryWeapon)
                    {
                        lWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        lWeapon.secundaryWeapon.hitLayer = damageLayer;
                        lWeapon.secundaryWeapon.root = transform;
                        lWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (lWeapon.secundaryWeapon.dontUseReload) ReloadWeaponAuto(lWeapon.secundaryWeapon, true);
                    }

                    currentShotTime = 0;
                    LoadIKAdjust(lWeapon.weaponCategory);
                }
            }
        }

        public void SetRightWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponentInChildren<vShooterWeapon>(true);
                rWeapon = w;
                if (rWeapon)
                {
                    rWeapon.ignoreTags = ignoreTags;
                    rWeapon.hitLayer = damageLayer;
                    rWeapon.root = transform;
                    rWeapon.isSecundaryWeapon = false;
                    rWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (rWeapon.dontUseReload) ReloadWeaponAuto(rWeapon, false);
                    if (rWeapon.secundaryWeapon)
                    {
                        rWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        rWeapon.secundaryWeapon.hitLayer = damageLayer;
                        rWeapon.secundaryWeapon.root = transform;
                        rWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (rWeapon.secundaryWeapon.dontUseReload) ReloadWeaponAuto(rWeapon.secundaryWeapon, true);
                    }

                    currentShotTime = 0;
                    LoadIKAdjust(rWeapon.weaponCategory);
                }
            }
        }
        public virtual vWeaponIKAdjust CurrentWeaponIK
        {
            get
            {
                return currentWeaponIKAdjust;
            }
        }
        public virtual void LoadIKAdjust(string category)
        {
            if (weaponIKAdjustList)
                currentWeaponIKAdjust = weaponIKAdjustList.GetWeaponIK(category);
        }

        public virtual void SetIKAdjust(vWeaponIKAdjust iKAdjust)
        {
            currentWeaponIKAdjust = iKAdjust;
        }
        public void OnDestroyWeapon(GameObject otherGameObject)
        {
            currentShotTime = 0;
        }

        public int GetMoveSetID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.moveSetID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.moveSetID;
            return id;
        }

        public int GetUpperBodyID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.upperBodyID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.upperBodyID;
            return id;
        }

        public int GetShotID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.shotID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.shotID;
            return id;
        }

        public int GetAttackID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.shotID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.shotID;
            return id;
        }

        public int GetEquipID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.equipID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.equipID;
            return id;
        }

        public int GetReloadID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.reloadID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.reloadID;
            return id;
        }

        public bool isShooting
        {
            get { return currentShotTime > 0; }
        }

        public void ReloadWeapon()
        {
            var weapon = currentWeapon;

            if (!weapon || !weapon.gameObject.activeSelf) return;

            bool primaryWeaponAnim = false;
            if (!((weapon.ammoCount >= weapon.clipSize)) && !weapon.dontUseReload)
            {
               
                onReloadWeapon.Invoke(weapon);
                var needAmmo = weapon.clipSize - weapon.ammoCount;

                weapon.AddAmmo(needAmmo);

                if (animator)
                {
                    animator.SetInteger("ReloadID", GetReloadID());
                    animator.SetTrigger("Reload");
                }              
                weapon.ReloadEffect();
                primaryWeaponAnim = true;
            }
            if (weapon.secundaryWeapon && !((weapon.secundaryWeapon.ammoCount >= weapon.secundaryWeapon.clipSize)) && !weapon.secundaryWeapon.dontUseReload)
            {
                var needAmmo = weapon.secundaryWeapon.clipSize - weapon.secundaryWeapon.ammoCount;

                weapon.secundaryWeapon.AddAmmo(needAmmo);

                if (!primaryWeaponAnim)
                {
                    if (animator)
                    {
                        primaryWeaponAnim = true;
                        animator.SetInteger("ReloadID", weapon.secundaryWeapon.reloadID);
                        animator.SetTrigger("Reload");
                    }                   
                    weapon.secundaryWeapon.ReloadEffect();
                }
            }
        }

        protected void ReloadWeaponAuto(vShooterWeapon weapon, bool secundaryWeapon = false)
        {
            if (!weapon || !weapon.gameObject.activeSelf) return;

            if (!((weapon.ammoCount >= weapon.clipSize)))
            {
                var needAmmo = weapon.clipSize - weapon.ammoCount;
                weapon.AddAmmo(needAmmo);
            }
        }

        public virtual bool weaponHasAmmo
        {
            get
            {
                if (!currentWeapon) return false;
                return currentWeapon.ammoCount > 0;
            }
        }

        public virtual vShooterWeapon currentWeapon
        {
            get { return rWeapon && rWeapon.gameObject.activeSelf ? rWeapon : lWeapon && lWeapon.gameObject.activeSelf ? lWeapon : null; }
        }

        public bool IsLeftWeapon
        {
            get
            {
                var isLeftWp = (rWeapon == null) ?
                    (lWeapon) : false;
                return isLeftWp;
            }
        }

        public virtual void Shoot(Vector3 aimPosition, bool useSecundaryWeapon = false)
        { 
            var weapon = currentWeapon;
            if (!weapon || !weapon.gameObject.activeSelf) return;
            var secundaryWeapon = weapon.secundaryWeapon;
            if (useSecundaryWeapon && !secundaryWeapon)
            {
                return;
            }

            var targetWeapon = useSecundaryWeapon ? secundaryWeapon : weapon;
            if (targetWeapon.dontUseReload) ReloadWeaponAuto(targetWeapon, useSecundaryWeapon);
            var applyRecoil = false;
            targetWeapon.Shoot(aimPosition, transform,(bool sucessful)=> { applyRecoil = sucessful; });
            if(applyRecoil)
                StartCoroutine(Recoil());
            if (targetWeapon.dontUseReload) ReloadWeaponAuto(targetWeapon, useSecundaryWeapon);
            currentShotTime = weapon.shootFrequency;
        }

        IEnumerator Recoil()
        {
            yield return new WaitForSeconds(0.02f);
            if (animator) animator.SetTrigger("Shoot");
        }

        public void UpdateShotTime()
        {
            if (currentShotTime > 0) currentShotTime -= Time.deltaTime;
        }
    }
}
