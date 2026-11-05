using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Lane Movement")]
    [SerializeField] private float laneShiftAmount = 0.1f;

    [Header("Jump Movement")]
    [SerializeField] private float jumpFollowAmount = 0.1f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 0.15f;

    
    [Header("Lane Tilt")]
    [SerializeField] private float maxLaneTilt = 3f;
    [SerializeField] private float tiltSmoothSpeed = 0.12f;

    private float previousPlayerX;
    private float currentTilt;
    private float tiltVelocity;

    private Quaternion startingRotation;

    private Vector3 startingPosition;
    private Vector3 currentVelocity;
    private Vector3 shakeOffset;

    private float shakeIntensity;
    private float shakeTimer;
    private float shakeDuration;

    private float previousJumpHeight;


    private void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        if (playerMovement != null)
            previousPlayerX = playerMovement.transform.position.x;
    }

    private void LateUpdate()
    {
        if (playerMovement == null)
            return;

        UpdateCameraMovement();
        UpdateLandingDetection();
        UpdateShake();
        UpdateLaneTilt();
    }

    private void UpdateCameraMovement()
    {
        PerspectiveSettings settings = PerspectiveSettings.Instance;

        if (settings == null)
            return;

        float middleLaneX = settings.GetPlayerLaneX(1);

        float laneOffset =
            (playerMovement.transform.position.x - middleLaneX)
            * laneShiftAmount;

        float jumpOffset =
            playerMovement.CurrentJumpHeight * jumpFollowAmount;

        Vector3 targetPosition = startingPosition + new Vector3(
            laneOffset,
            jumpOffset,
            0f
        );

        Vector3 basePosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothSpeed
        );

        transform.position = basePosition + shakeOffset;
    }

    private void UpdateLandingDetection()
    {
        float currentJumpHeight = playerMovement.CurrentJumpHeight;

        if (previousJumpHeight > 0.05f &&
            currentJumpHeight <= 0.05f)
        {
            TriggerShake(0.04f, 0.12f);
        }

        previousJumpHeight = currentJumpHeight;
    }

    private void UpdateLaneTilt()
    {
        float currentPlayerX = playerMovement.transform.position.x;
        float movement = currentPlayerX - previousPlayerX;

        float targetTilt = 0f;

        if (Mathf.Abs(movement) > 0.001f)
        {
            targetTilt = -Mathf.Sign(movement) * maxLaneTilt;
        }

        currentTilt = Mathf.SmoothDamp(
            currentTilt,
            targetTilt,
            ref tiltVelocity,
            tiltSmoothSpeed
        );

        transform.rotation = startingRotation *
            Quaternion.Euler(0f, 0f, currentTilt);

        previousPlayerX = currentPlayerX;
    }

    public void TriggerShake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }

    private void UpdateShake()
    {
        if (shakeTimer <= 0f)
        {
            shakeOffset = Vector3.zero;
            return;
        }

        shakeTimer -= Time.deltaTime;

        float strength = shakeIntensity *
            (shakeTimer / shakeDuration);

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        shakeOffset = new Vector3(x, y, 0f) * strength;
    }
}
