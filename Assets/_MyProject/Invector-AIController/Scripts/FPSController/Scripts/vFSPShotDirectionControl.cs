using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    [vClassHeader("FPS Shot Direction Control", openClose = false)]
    public class vFSPShotDirectionControl : vMonoBehaviour
    {
        public vShooter.vShooterWeaponBase shooterWeapon;
        public List<string> shooterWeaponIgnoreTags;
        public LayerMask shooterWeaponHitLayer;
        private vFPSController controller;

        private void Start()
        {
            controller = GetComponentInParent<vFPSController>();
            if (shooterWeapon)
            {
                shooterWeapon.ignoreTags = shooterWeaponIgnoreTags;
                shooterWeapon.hitLayer = shooterWeaponHitLayer;
            }
        }

        RaycastHit hitObject;
        public void Shot()
        {
            if (shooterWeapon)
            {
                if (Physics.Raycast(controller._camera.transform.position, controller._camera.transform.forward, out hitObject, controller._camera.farClipPlane, shooterWeaponHitLayer))
                {
                    shooterWeapon.Shoot(hitObject.point, controller.transform);
                }
                else
                {
                    shooterWeapon.Shoot(controller._camera.transform.position + controller._camera.transform.forward * controller._camera.farClipPlane, controller.transform);
                }
            }
        }
    }
}