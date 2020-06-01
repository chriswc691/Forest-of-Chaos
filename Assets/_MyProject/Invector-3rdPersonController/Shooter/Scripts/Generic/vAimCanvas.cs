using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Invector.vShooter
{
    public class vAimCanvas : MonoBehaviour
    {
        public RectTransform aimTarget, aimCenter;

        public Camera scopeCamera;
        public bool useScopeTransition = true;
        public bool scaleAimWithMovement = true;
        public float scaleWithMovement = 2;
        public float smothChangeScale = 2;
        [Range(0, 1)]
        public float movementSensibility = 0.1f;
        public UnityEvent onEnableAim;
        public UnityEvent onDisableAim;
        public UnityEvent onCheckvalidAim;
        public UnityEvent onCheckInvalidAim;

        public UnityEvent onEnableScopeCamera;
        public UnityEvent onDisableScopeCamera;
        public UnityEvent onEnableScopeUI;
        public UnityEvent onDisableScopeUI;

        [HideInInspector]
        public bool isValid;
        [HideInInspector]
        public bool isAimActive;
        [HideInInspector]
        public bool isScopeCameraActive;
        [HideInInspector]
        public bool isScopeUIActive;
        [HideInInspector]
        public Vector2 sizeDeltaTarget;
        [HideInInspector]
        public Vector2 sizeDeltaCenter;

        protected virtual void Start()
        {
            onDisableScopeCamera.Invoke();
            onDisableScopeUI.Invoke();
            onDisableAim.Invoke();
            if (aimCenter)
                sizeDeltaCenter = aimCenter.sizeDelta;
            if (aimTarget)
                sizeDeltaTarget = aimTarget.sizeDelta;
        }
        public void DisableAll()
        {
            onDisableScopeCamera.Invoke();
            onDisableScopeUI.Invoke();
            onDisableAim.Invoke();
            isValid = false;
            isAimActive = false;
            isScopeCameraActive = false;
            isScopeUIActive = false;
        }
    }
}