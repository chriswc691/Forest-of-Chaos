using UnityEngine;

namespace Invector.IK
{
    [vClassHeader("FPS Arm IK", "Simple IK Solution for the FPS Controller", openClose = false)]
    public class vFPSArmIK : vMonoBehaviour
    {
        public Transform handTarget;
        public Transform hintTarget;
        [SerializeField] protected float _ikWeight;
        public float smothIKWeight;
        public vIKSolver arm;
        private float currentIKWeight;
        private bool waitUpdate;

        public float ikWeight
        {
            get { return _ikWeight; }
            set { _ikWeight = value; }
        }       

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) HandleIK();
        }

        private void Update()
        {
            waitUpdate = true;
        }

        public void LateUpdate()
        {
            HandleIK();
        }

        private void HandleIK()
        {
            if (!waitUpdate && Application.isPlaying) return;
            waitUpdate = false;
            if (handTarget)
            {
                currentIKWeight = Mathf.Lerp(currentIKWeight, ikWeight, smothIKWeight * Time.deltaTime);
                if (currentIKWeight > 0.01f)
                {
                    arm.SetIKWeight(currentIKWeight);
                    arm.SetIKPosition(handTarget.position);
                    arm.SetIKRotation(handTarget.rotation);
                    if (hintTarget) arm.SetIKHintPosition(hintTarget.position);
                }
            }
        }
    }
}
