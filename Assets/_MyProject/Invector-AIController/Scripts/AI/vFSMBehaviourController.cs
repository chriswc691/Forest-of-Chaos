using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public partial class vFSMBehaviourController : vMonoBehaviour, vIFSMBehaviourController
    {
        public vMessageReceiver messageReceiver
        {
            get
            {
                if (_messageReceiver == null && !tryGetMessageReceiver) _messageReceiver = GetComponent<vMessageReceiver>();
                if (_messageReceiver == null && !tryGetMessageReceiver) tryGetMessageReceiver = true;

                return _messageReceiver;
            }
        }
        private vMessageReceiver _messageReceiver;
        private bool tryGetMessageReceiver;
    }
}