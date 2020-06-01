using System.Collections.Generic;
using UnityEngine;

namespace Invector.vShooter
{
    using IK;
    [CreateAssetMenu(menuName = "Invector/Shooter/New Weapon IK Adjust")]
    public class vWeaponIKAdjust : ScriptableObject
    {
        public List<string> weaponCategories = new List<string> { "HandGun", "Pistol" };
        public IKAdjust standing;
        public IKAdjust crouching;
        public IKAdjust standingAiming;
        public IKAdjust crouchingAiming;

        public IKAdjust GetIKAdjust(bool isAming, bool isCrouching)
        {
            if (isAming && isCrouching) return crouchingAiming;
            if (isAming && !isCrouching) return standingAiming;
            if (!isAming && isCrouching) return crouching;
            return standing;
        }

        [ContextMenu("Reset Standing")]
        public void ResetStanding()
        {
            standing = new IKAdjust();
        }

        [ContextMenu("Reset Standing Aiming")]
        public void ResetStandingAiming()
        {
            standingAiming = new IKAdjust();
        }

        [ContextMenu("Reset Crouching")]
        public void ResetCrouching()
        {
            crouching = new IKAdjust();
        }

        [ContextMenu("Reset Crouching Aiming")]
        public void ResetCrouchingAiming()
        {
            crouchingAiming = new IKAdjust();
        }

        void Reset()
        {
            ResetStanding();
            ResetStandingAiming();
            ResetCrouching();
            ResetCrouchingAiming();
        }
    }
}