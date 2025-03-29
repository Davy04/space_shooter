using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioSource footstepSound;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [SerializeField] private float sprintTransitSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight = 2f;

    private float verticalVelocity;
    private float currentSpeed;
    private float currentSpeedMultiplier = 1f;
    private float xRotation;

    [Header("Camera Bob Settings")]
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private float bobAmplitude = 0.1f;
    [SerializeField] private float bobSmoothing = 10f;

    private CinemachineBasicMultiChannelPerlin noiseComponent;

    [Header("Recoil")]
    private Vector3 targetRecoil = Vector3.zero;
    private Vector3 currentRecoil = Vector3.zero;
    private float bobTimer = 0f;

    [Header("Footstep Settings")]
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private float stepInterval = 0.5f;

    private float nextStepTimer = 0;

    [Header("SFX")]
    [SerializeField] private AudioClip[] groundFootsteps;
    [SerializeField] private AudioClip[] grassFootsteps;
    [SerializeField] private AudioClip[] gravelFootsteps;

    [Header("Input")]
    [SerializeField] private float mouseSensitivity = 100f;
    private float moveInput;
    private float turnInput;
    private float mouseX;
    private float mouseY;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        noiseComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        InputManagement();
        Movement();
        PlayFootstepSound();
    }

    private void LateUpdate()
    {
        CameraBob();
    }

    private void Movement()
    {
        GroundMovement();
        Turn();
    }

    private void GroundMovement()
    {
        // Suavização dos inputs de movimento
        float smoothMoveInput = Mathf.Lerp(0, moveInput, 0.5f);
        float smoothTurnInput = Mathf.Lerp(0, turnInput, 0.5f);

        Vector3 move = new Vector3(smoothTurnInput, 0, smoothMoveInput);
        move = virtualCamera.transform.TransformDirection(move);

        // Controle de sprint
        currentSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ?
            Mathf.Lerp(currentSpeedMultiplier, sprintSpeedMultiplier, sprintTransitSpeed * Time.deltaTime) :
            Mathf.Lerp(currentSpeedMultiplier, 1f, sprintTransitSpeed * Time.deltaTime);

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * currentSpeedMultiplier, sprintTransitSpeed * Time.deltaTime);
        move *= currentSpeed;

        // Aplicação da gravidade e pulo
        move.y = VerticalForceCalculation();

        controller.Move(move * Time.deltaTime);
    }

    private void Turn()
    {
        // Suavização do input do mouse
        float smoothMouseX = Mathf.Lerp(0, mouseX, 0.5f) * mouseSensitivity * Time.deltaTime;
        float smoothMouseY = Mathf.Lerp(0, mouseY, 0.5f) * mouseSensitivity * Time.deltaTime;

        xRotation -= smoothMouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        virtualCamera.transform.localRotation = Quaternion.Euler(xRotation + currentRecoil.y, currentRecoil.x, 0f);
        transform.Rotate(Vector3.up * smoothMouseX);
    }

    public void ApplyRecoil(GunData gunData)
    {
        float recoilX = Random.Range(-gunData.maxRecoil.x, gunData.maxRecoil.x) * gunData.recoilAmount;
        float recoilY = Random.Range(-gunData.maxRecoil.y, gunData.maxRecoil.y) * gunData.recoilAmount;

        targetRecoil += new Vector3(recoilX, recoilY, 0);

        currentRecoil = Vector3.MoveTowards(currentRecoil, targetRecoil, Time.deltaTime * gunData.recoilSpeed);
    }

    public void ResetRecoil(GunData gunData)
    {
        currentRecoil = Vector3.MoveTowards(currentRecoil, Vector3.zero, Time.deltaTime * gunData.resetRecoilSpeed);
        targetRecoil = Vector3.MoveTowards(targetRecoil, Vector3.zero, Time.deltaTime * gunData.resetRecoilSpeed);
    }

    private void CameraBob()
    {
        float targetAmplitude = 0f;
        float targetFrequency = 0f;

        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(controller.velocity.magnitude / (moveSpeed * sprintSpeedMultiplier));
            targetAmplitude = bobAmplitude * currentSpeedMultiplier * speedFactor;
            targetFrequency = bobFrequency * currentSpeedMultiplier * speedFactor;
        }

        // Suavização do camera bob
        noiseComponent.m_AmplitudeGain = Mathf.Lerp(
            noiseComponent.m_AmplitudeGain,
            targetAmplitude,
            bobSmoothing * Time.deltaTime);

        noiseComponent.m_FrequencyGain = Mathf.Lerp(
            noiseComponent.m_FrequencyGain,
            targetFrequency,
            bobSmoothing * Time.deltaTime);
    }

    private void PlayFootstepSound()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            if (Time.time >= nextStepTimer)
            {
                AudioClip[] footstepClips = DetermineAudioClips();

                if (footstepClips != null && footstepClips.Length > 0)
                {
                    AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
                    footstepSound.PlayOneShot(clip);
                }

                nextStepTimer = Time.time + (stepInterval / currentSpeedMultiplier);
            }
        }
    }

    private AudioClip[] DetermineAudioClips()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f, terrainLayerMask))
        {
            return hit.collider.tag switch
            {
                "Grass" => grassFootsteps,
                "Gravel" => gravelFootsteps,
                _ => groundFootsteps,
            };
        }
        return groundFootsteps;
    }

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // Pequena força para manter no chão

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        return verticalVelocity;
    }

    private void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}