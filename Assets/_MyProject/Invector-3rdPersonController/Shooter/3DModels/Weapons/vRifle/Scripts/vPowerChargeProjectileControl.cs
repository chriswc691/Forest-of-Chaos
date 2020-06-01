﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vShooter
{
    public class vPowerChargeProjectileControl : MonoBehaviour
    {
        public List<vProjectilePerPower> projectiles;
        private vShooterWeapon weapon;
        private vProjectilePerPower lastProjectilePerPower;

        void Start()
        {
            weapon = GetComponent<vShooterWeapon>();
            if (weapon)
            {
                weapon.onChangerPowerCharger.AddListener(OnChangerPower);
            }
        }

        public void OnChangerPower(float value)
        {
            if (value <= 0) return;

            if (weapon)
            {
                var projectilePerPower = projectiles.Find(projectile => value >= projectile.min && value <= projectile.max);
                if (projectilePerPower != null && projectilePerPower.projectile && lastProjectilePerPower == null || lastProjectilePerPower != projectilePerPower)
                {
                    lastProjectilePerPower = projectilePerPower;
                    weapon.projectile = projectilePerPower.projectile;
                    projectilePerPower.OnValidatePower.Invoke();
                }
            }
        }

        [System.Serializable]
        public class vProjectilePerPower
        {
            public float min;
            public float max;
            public GameObject projectile;
            [Tooltip("Called when power is between min and max value")]
            public UnityEngine.Events.UnityEvent OnValidatePower;
        }
    }
}

