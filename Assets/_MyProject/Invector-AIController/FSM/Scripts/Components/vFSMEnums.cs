
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [System.Flags]
    public enum vFSMComponentExecutionType
    {
        OnStateUpdate = 1,
        OnStateEnter =2,       
        OnStateExit = 4,

    }
    public enum vTransitionOutputType
    {
        Default,  TrueFalse
    }
}