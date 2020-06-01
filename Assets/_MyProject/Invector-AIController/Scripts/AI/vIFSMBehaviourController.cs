using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public partial interface vIFSMBehaviourController
    {
        vMessageReceiver messageReceiver
        {
            get;
        }
    }
}