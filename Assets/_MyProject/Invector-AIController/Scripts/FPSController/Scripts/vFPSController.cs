using UnityEngine;
using Invector;
using Invector.vCharacterController;
using UnityEngine.SceneManagement;

/// <summary>
/// This FPS Controller is based in the FPSWalkerEnhanced and you can find the full version here: http://wiki.unity3d.com
/// We simplify the controller as much as possible and modify the core to attend our purposes which is simply to walk around and see the AI in action.
/// </summary>

[RequireComponent(typeof(CharacterController))]
[vClassHeader("vFPS Simple Controller", "Simple FPS Controller added as bonus just to see the AI in action.")]
public class vFPSController : vMonoBehaviour
{
    [vEditorToolbar("Controller Settings")]
    public float walkSpeed = 6.0f;
    public float runSpeed = 11.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;   

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    private float antiBumpFactor = .75f;
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    private int antiBunnyHopFactor = 1;
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private bool crouch;
    private int jumpTimer;

    [vEditorToolbar("Camera Settings")]
    public AnimationCurve cameraWalkCurve;
    public Camera _camera;
    public Transform cameraPivot;
    public float cameraHeight = 1f;
    public float cameraWalkEffectSpeed = 0.5f;
    public float cameraSensitivityX = 1f;
    public float cameraSensitivityY = 1f;

    [vEditorToolbar("Inputs")]
    public string horizontalInput = "Horizontal";
    public string verticalInput = "Vertical";

    [Header("Camera Input")]
    public string rotateCameraXInput = "Mouse X";
    public string rotateCameraYInput = "Mouse Y";
    float inputX, inputY;
    float cameraWalkNormalizedTime;
    float cameraWalkProgress;    

    [vEditorToolbar("Events")]
    public UnityEngine.Events.UnityEvent onAttack;
    public UnityEngine.Events.UnityEvent onStep;

    void Start()
    {
        _camera = Camera.main;
        controller = GetComponent<CharacterController>();
         myTransform = transform;
        speed = walkSpeed;        
        jumpTimer = antiBunnyHopFactor;
    }

    void Update()
    {
        AttackInput();
        MoveController();
        RotationControl();
        CameraWalkEffect();
    } 

    void MoveController()
    {
        inputX = Input.GetAxis(horizontalInput);
        inputY = Input.GetAxis(verticalInput);
        // If both horizontal and vertical are used simultaneously, so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        if (grounded)
        {
            // increase speed if sprintInput is pressed
            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            crouch = Input.GetKey(KeyCode.C);

            if (crouch)
            {
                controller.height = 1f;
                controller.center = Vector3.up * 0.5f;
                cameraHeight = 0.95f;
            }
            else
            {
                controller.height = 2f;
                controller.center = Vector3.up;
                cameraHeight = 1.5f;
            }
                

            moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetKeyDown(KeyCode.Space))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else
        {
            // If air control is allowed, check movement but don't touch the y component            
            moveDirection.x = inputX * speed * inputModifyFactor;
            moveDirection.z = inputY * speed * inputModifyFactor;
            moveDirection = myTransform.TransformDirection(moveDirection);            
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    void AttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) onAttack.Invoke();
    }

    void RotationControl()
    {
        var characterEuler = transform.eulerAngles;
        var cameraEuler = _camera.transform.localEulerAngles;
        characterEuler.y += Input.GetAxis(rotateCameraXInput) * cameraSensitivityX;
        transform.eulerAngles = characterEuler;
        cameraEuler.x -= Input.GetAxis(rotateCameraYInput) * cameraSensitivityY;
        cameraEuler = cameraEuler.NormalizeAngle();
        cameraEuler.x = Mathf.Clamp(cameraEuler.x, -45, 45);
        _camera.transform.localEulerAngles = cameraEuler;
    }

    void CameraWalkEffect()
    {
        cameraWalkNormalizedTime += Time.deltaTime * (Mathf.Abs(Mathf.Clamp(inputX + inputY, -1, 1))) * speed * cameraWalkEffectSpeed;
        cameraWalkProgress = cameraWalkNormalizedTime % 1;
        var stepProgress = cameraHeight * (1 + cameraWalkCurve.Evaluate(cameraWalkProgress));
        var position = new Vector3(cameraPivot.transform.localPosition.x, 0, cameraPivot.transform.localPosition.z) + Vector3.up * stepProgress;
        cameraPivot.transform.localPosition = position;
    }   

    public void ReloadScene()
    {
        SceneManager.LoadScene("AI_Examples");
    }
}