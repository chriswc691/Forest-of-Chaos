using UnityEngine;
using System.Collections.Generic;

namespace Invector.vShooter
{
    [vClassHeader("Decal Manager", openClose = false)]
    public class vDecalManager : vMonoBehaviour
    {
        public LayerMask layermask;
        public List<DecalObject> decalObjects;

        public virtual void CreateDecal(RaycastHit hitInfo)
        {
            CreateDecal(hitInfo.collider.gameObject, hitInfo.point, hitInfo.normal);
        }

        public virtual void CreateDecal(GameObject target, Vector3 position, Vector3 normal)
        {
            if (layermask == (layermask | (1 << target.layer)))
            {
                var decalObj = decalObjects.Find(d => d.tag.Equals(target.tag));
                if (decalObj != null)
                {
                    RaycastHit hit;
                    if (Physics.SphereCast(new Ray(position + (normal * 0.1f), -normal), 0.0001f, out hit, 1f, layermask))
                    {
                        if (hit.collider.gameObject == target)
                        {
                            var rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                            // instantiate hit effect particle
                            if (decalObj.hitEffect != null)
                            {
                                var effect = Instantiate(decalObj.hitEffect, hit.point, rotation)as GameObject;
                                effect.transform.SetParent(vObjectContainer.root,true);
                            }

                            // instantiate decal based on the gameobject tag
                            var decal = decalObj.GetDecal();
                            if (decal != null)
                            {
                                var _decal = Instantiate(decal, hit.point, rotation) as GameObject;
                                _decal.transform.SetParent(target.gameObject.transform,true);
                            }
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class DecalObject
        {
            public string tag;
            public GameObject hitEffect;
            public List<GameObject> decals;

            public GameObject GetDecal()
            {
                if (decals.Count > 1)
                {
                    var index = Random.Range(0, decals.Count - 1);
                    return decals[index];
                }
                else if (decals.Count == 1)
                    return decals[0];
                else
                    return null;
            }
        }
    }
}