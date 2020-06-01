using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI COMPANION CONTROL")]
    public class vAICompanionControl : vMonoBehaviour
    {
        public List<vAICompanion> aICompanions;        
        public KeyCode followInput = KeyCode.F;

        void Update()
        {
            if (Input.GetKeyDown(followInput))
            {
                for (int i = 0; i < aICompanions.Count; i++)
                {
                    aICompanions[i].forceFollow = !aICompanions[i].forceFollow;
                }
            }
        }

        public void ReceiveDamage(vDamage damage)
        {
            if (damage.sender)
            {
                for (int i = 0; i < aICompanions.Count; i++)
                {
                    if (aICompanions[i].controlAI && aICompanions[i].controlAI.currentTarget.transform == null)
                    {
                        aICompanions[i].controlAI.SetCurrentTarget(damage.sender, true);
                    }
                }
            }
        }
    }
}