using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector
{
    using IK;
    using vShooter;
    namespace PlayerController
    {
        public interface vILockCamera
        {
            bool LockCamera { get; set; }
        }

    }

    public interface vIShooterIKController
    {       
        GameObject gameObject { get; }
        vWeaponIKAdjustList WeaponIKAdjustList { get; set; }
        vWeaponIKAdjust CurrentWeaponIK { get; }  
        void SetIKAdjust(vWeaponIKAdjust iKAdjust);
        void LoadIKAdjust(string weaponCategory);
        bool LockAiming { get; set; }
        vIKSolver LeftIK { get; }
        vIKSolver RightIK { get; }
        vShooterWeapon CurrentActiveWeapon { get; }
        bool IsAiming { get; }
        bool IsCrouching { get; set; }
        bool IsLeftWeapon { get; }
    }
}