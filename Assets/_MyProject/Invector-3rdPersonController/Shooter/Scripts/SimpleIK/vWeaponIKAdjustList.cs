using System.Collections.Generic;
using UnityEngine;

namespace Invector.vShooter
{
    [CreateAssetMenu(menuName = "Invector/Shooter/New Weapon IK Adjust List")]
    public class vWeaponIKAdjustList : ScriptableObject
    {
        public List<vWeaponIKAdjust> weaponIKAdjusts = new List<vWeaponIKAdjust>();

        public Vector3 ikRotationOffsetR;
        public Vector3 ikPositionOffsetR;
        public Vector3 ikRotationOffsetL;
        public Vector3 ikPositionOffsetL;

        public vWeaponIKAdjust GetWeaponIK(string category)
        {
            return weaponIKAdjusts.Find(ik => ik.weaponCategories.Contains(category));
        }

        public void ReplaceWeaponIKAdjust(vWeaponIKAdjust currentIK, vWeaponIKAdjust newIK)
        {
            if (weaponIKAdjusts != null && weaponIKAdjusts.Contains(currentIK))
            {
                int index = IndexOfIK(currentIK);
                weaponIKAdjusts[index] = newIK;
            }
        }

        public int IndexOfIK(vWeaponIKAdjust currentIK)
        {
            if (weaponIKAdjusts != null && weaponIKAdjusts.Contains(currentIK))
            {
                int index = weaponIKAdjusts.IndexOf(currentIK);
                return index;
            }
            else return -1;
        }
    }
}