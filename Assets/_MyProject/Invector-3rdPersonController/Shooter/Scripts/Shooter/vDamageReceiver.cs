using UnityEngine;

namespace Invector.vCharacterController
{
    using vEventSystems;
    [vClassHeader("DAMAGE RECEIVER", "You can add damage multiplier for example causing twice damage on Headshots", openClose = false)]
    public partial class vDamageReceiver : vMonoBehaviour, vIAttackReceiver
    {
        public GameObject targetReceiver;
        public vIHealthController healthController;

        public void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {
            float multiplier = (useRandomValues && !fixedValues) ? Random.Range(minDamageMultiplier, maxDamageMultiplier) :
                                (useRandomValues && fixedValues) ? randomChange ? maxDamageMultiplier : minDamageMultiplier : damageMultiplier;

            if (overrideReactionID)
            {
                if (useRandomValues && !fixedValues) damage.reaction_id = Random.Range(minReactionID, maxReactionID);
                else if (useRandomValues && fixedValues) damage.reaction_id = randomChange ? maxReactionID : minReactionID;
                else
                    damage.reaction_id = reactionID;
            }

            if (ragdoll && !ragdoll.iChar.isDead)
            {
                var _damage = new vDamage(damage);
                var value = (float)_damage.damageValue;
                _damage.damageValue = (int)(value * multiplier);
                if (multiplier == maxDamageMultiplier) OnGetMaxValue.Invoke();
                ragdoll.gameObject.ApplyDamage(_damage, attacker);
                onReceiveDamage.Invoke(_damage);
            }
            else
            {
                if (healthController == null && targetReceiver)
                    healthController = targetReceiver.GetComponent<vIHealthController>();
                else if (healthController == null)
                    healthController = GetComponentInParent<vIHealthController>();

                if (healthController != null)
                {
                    var _damage = new vDamage(damage);
                    var value = (float)_damage.damageValue;
                    _damage.damageValue = (int)(value * multiplier);
                    if (multiplier == maxDamageMultiplier) OnGetMaxValue.Invoke();
                    try
                    {
                        healthController.gameObject.ApplyDamage(_damage, attacker);
                        onReceiveDamage.Invoke(_damage);
                    }
                    catch
                    {
                        this.enabled = false;
                    }
                }
            }
        }
    }
}