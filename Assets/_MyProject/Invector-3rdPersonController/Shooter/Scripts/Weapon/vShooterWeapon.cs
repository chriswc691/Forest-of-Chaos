using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Invector.vShooter
{
    [vClassHeader("Shooter Weapon", openClose = false)]
    public class vShooterWeapon : vShooterWeaponBase
    {
        #region variables

        [vEditorToolbar("Weapon Settings")]
      
        public bool isLeftWeapon = false;
        [Tooltip("Hold Charge Input to charge")]
        public bool chargeWeapon = false;
        [vHideInInspector("chargeWeapon")]
        public bool autoShotOnFinishCharge = false;
        [vHideInInspector("chargeWeapon")]
        public float chargeSpeed = 0.1f;
        [vHideInInspector("chargeWeapon")]
        public float chargeDamageMultiplier = 2;
        [vHideInInspector("chargeWeapon")]
        public bool changeVelocityByCharge = true;
        [vHideInInspector("chargeWeapon")]
        public float chargeVelocityMultiplier = 2;
        [Tooltip("Change between automatic weapon or shot once")]
        [vHideInInspector("chargeWeapon", true)]
        public bool automaticWeapon;


        [vEditorToolbar("Ammo")]        
        public float reloadTime = 1f;
        public bool reloadOneByOne;
        [Tooltip("Max clip size of your weapon")]
        public int clipSize;       
        [Tooltip("Check this to combine extra ammo with the current ammo, the Reload will not be used")]
        public bool dontUseReload;
        [vHideInInspector("dontUseReload", true)]
        [Tooltip("Automatically reload the weapon when it's empty")]
        public bool autoReload;
        [Tooltip("Ammo ID - make sure your AmmoManager and ItemListData use the same ID"), vHideInInspector("isInfinityAmmo", true)]
        public int ammoID;

        [vEditorToolbar("Weapon ID")]
        [Tooltip("What moveset the underbody will play")]
        public float moveSetID;
        [Tooltip("What moveset the uperbody will play")]
        public float upperBodyID;
        [Tooltip("What shot animation will trigger")]
        public float shotID;
        [vHideInInspector("autoReload", true)]
        [Tooltip("What reload animation will play")]
        public int reloadID;       
        [Tooltip("What equip animation will play")]
        public int equipID;

        [vEditorToolbar("IK Options")]       
        [Tooltip("IK will help the right hand to align where you actually is aiming")]
        public bool alignRightHandToAim = true;
        [Tooltip("IK will help the right hand to align where you actually is aiming")]
        public bool alignRightUpperArmToAim = true;
        public bool raycastAimTarget = true;
        [Tooltip("Left IK on Idle")]
        public bool useIkOnIdle = true;
        [Tooltip("Left IK on free locomotion")]
        public bool useIkOnFree = true;
        [Tooltip("Left IK on strafe locomotion")]
        public bool useIkOnStrafe = true;
        [Tooltip("Left IK while attacking")]
        public bool useIkAttacking = false;
        public bool disableIkOnShot = false;
        public bool useIKOnAiming = true;
        public bool canAimWithoutAmmo = true;      
        public Transform handIKTarget;
        [vEditorToolbar("Projectile")]
        public vShooterWeapon secundaryWeapon;       
        [Tooltip("Assign the aimReference of your weapon")]
        public Transform aimReference;
        [Tooltip("how much precision the weapon have, 1 means no cameraSway and 0 means maxCameraSway from the ShooterManager")]
        [Range(0, 1)]
        public float precision = 0.5f;
        [Tooltip("Creates a right recoil on the camera")]
        public float recoilRight = 1;
        [Tooltip("Creates a left recoil on the camera")]
        public float recoilLeft = -1;
        [Tooltip("Creates a up recoil on the camera")]
        public float recoilUp = 1;

        [vEditorToolbar("Audio & VFX")]
        public AudioSource reloadSource;
        public AudioClip reloadClip;
        public AudioClip finishReloadClip;
        [vEditorToolbar("Scope UI")]
        [vHelpBox("Third Person Controller Only", vHelpBoxAttribute.MessageType.Info)]
        public bool onlyUseScopeUIView;

        [Tooltip("Check this bool to use an UI image for the scope, ex: snipers")]
        public bool useUI;
        [Tooltip("You can create different Aim sprites and use for different weapons")]
        public int scopeID;
        [Tooltip("change the FOV of the scope view\n **The calc is default value (60)-scopeZoom**"), Range(-118, 60)]
        public float scopeZoom = 60;
        [Tooltip("Used with the TPCamera to use a custom CameraState, if it's empty it will use the 'Aiming' CameraState.")]
        public string customAimCameraState;

        [Tooltip("assign an empty transform with the pos/rot of your scope view")]
        public Transform scopeTarget;

        public Camera zoomScopeCamera;
        [vHelpBox("Keep Scope Camera Z is used to align z rotation of the zoomScopeCamera to z rotation of the weapon muzzle<color=red> (Projectile toolbar)</color>. if you want to align camera with Vector3.up in z rotation enable this.")]
        public bool keepScopeCameraRotationZ = true;
        
        [System.Serializable]
        public class OnChangePowerCharger : UnityEvent<float> { }
        [HideInInspector]
        public bool isAiming, usingScope;

        [vEditorToolbar("Events")]
        public UnityEvent  onReload, onFinishReload,onFinishAmmo,  onEnableAim, onDisableAim, onEnableScope, onDisableScope, onFullPower;
        public OnChangePowerCharger onChangerPowerCharger;

        [HideInInspector]
        public Transform root;
        [HideInInspector]
        public bool isSecundaryWeapon;
        private float _charge;
        public delegate bool CheckAmmoHandle(ref bool isValid, ref int totalAmmo);
        public delegate void ChangeAmmoHandle(int value);
        public CheckAmmoHandle checkAmmoHandle;
        public ChangeAmmoHandle changeAmmoHandle;
        #endregion

        [System.NonSerialized] private float testTime;

        void OnDrawGizmos()
        {
            if (!Application.isPlaying && testShootEffect)
            {
                if (testTime <= 0)
                    Shootest();
                else testTime -= Time.deltaTime;
            }
        }

        private void Start()
        {
            if (!reloadSource) reloadSource = source;
            SetScopeZoom(scopeZoom);
            
        }

        public void Shootest()
        {
            testTime = shootFrequency;

            StartEmitters();
            lightOnShot.enabled = true;
            source.PlayOneShot(fireClip);
            Invoke("StopShootTest", .037f);
        }

        void StopShootTest()
        {
            StopEmitters();
            lightOnShot.enabled = false;
        }

        public float powerCharge
        {
            get
            {
                return _charge;
            }
            set
            {
                if (value != _charge)
                {
                    _charge = value;
                    onChangerPowerCharger.Invoke(_charge);
                    if (_charge >= 1) onFullPower.Invoke();
                }
            }
        }
       
        public void SetPrecision(float value)
        {
            precision = Mathf.Clamp(value, 0, 1);
        }

        public override bool HasAmmo()
        {
           
            if (checkAmmoHandle != null)
            {
                bool isValidAmmo = false;
                int totalAmmo = 0;
                var hasAmmo = checkAmmoHandle.Invoke(ref isValidAmmo, ref totalAmmo);
                if (isValidAmmo) return hasAmmo;
                else return ammo > 0;
            }
            else
                return ammo > 0;
           
        }

        public int ammoCount
        {
            get
            {
                if (checkAmmoHandle != null)
                {                  
                    bool isValidAmmo = false;
                    int totalAmmo = 0;
                    checkAmmoHandle.Invoke(ref isValidAmmo, ref totalAmmo);
                    if (isValidAmmo) return totalAmmo;
                    else return ammo;
                }
                return ammo;
            }
        }

        public void AddAmmo(int value)
        {
            if (checkAmmoHandle != null && changeAmmoHandle != null)
            {

                bool isValidAmmo = false;
                int totalAmmo = 0;
                checkAmmoHandle.Invoke(ref isValidAmmo, ref totalAmmo);
                if (isValidAmmo)
                    changeAmmoHandle(value);
                else
                    ammo += value;
            }
            else
                ammo += value;
        }

        public override void UseAmmo(int count = 1)
        {
            if (checkAmmoHandle != null && changeAmmoHandle != null)
            {

                bool isValidAmmo = false;
                int totalAmmo = 0;
                checkAmmoHandle.Invoke(ref isValidAmmo, ref totalAmmo);
                if (isValidAmmo)
                    changeAmmoHandle(-count);
                else
                    ammo -= count;
            }
            else
                ammo -= count;
             
        }

       
        public virtual void ReloadEffect()
        {           

            if (reloadSource && reloadClip)
            {
                reloadSource.Stop();
                reloadSource.PlayOneShot(reloadClip);
            }
            onReload.Invoke();
        }

        public virtual void FinishReloadEffect()
        {
            if (reloadSource && finishReloadClip)
            {
                reloadSource.Stop();
                reloadSource.PlayOneShot(finishReloadClip);
            }
            onFinishReload.Invoke();
        } 

        protected override float damageMultiplier
        {
            get
            {
                if (!chargeWeapon) return base.damageMultiplier;
                return (int)(1 + Mathf.Lerp(0, chargeDamageMultiplier, _charge))+ damageMultiplierMod;
            }
        }

        protected override float velocityMultiplier
        {
            get
            {
                if (!chargeWeapon || !changeVelocityByCharge) return base.velocityMultiplier;
                return (1 + Mathf.Lerp(0, chargeVelocityMultiplier, _charge))+velocityMultiplierMod;
            }
        }

        public void SetScopeZoom(float value)
        {
            if (zoomScopeCamera)
            {
                var zoom = Mathf.Clamp(61 - value, 1, 179);
                zoomScopeCamera.fieldOfView = zoom;
            }
        }

        public void SetActiveAim(bool value)
        {
            if (isAiming != value)
            {
                isAiming = value;
                if (isAiming)
                    onEnableAim.Invoke();
                else
                    onDisableAim.Invoke();
            }
        }

        /// <summary>
        /// Set if Weapon is using scope
        /// </summary>
        /// <param name="value"></param>
        public void SetActiveScope(bool value)
        {
            if (usingScope != value)
            {
                usingScope = value;
                if (usingScope)
                    onEnableScope.Invoke();
                else
                    onDisableScope.Invoke();
            }
        }

        /// <summary>
        /// Set look target point to Zoom scope camera
        /// </summary>
        /// <param name="point"></param>
        public void SetScopeLookTarget(Vector3 point)
        {
            if (zoomScopeCamera)
            {
                var euler = Quaternion.LookRotation(point - zoomScopeCamera.transform.position, Vector3.up).eulerAngles;
                if (keepScopeCameraRotationZ) euler.z = muzzle.transform.eulerAngles.z;

                zoomScopeCamera.transform.eulerAngles = euler;
            }
        }
    }
}
