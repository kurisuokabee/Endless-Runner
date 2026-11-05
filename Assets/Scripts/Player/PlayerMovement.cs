
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] private float laneMoveDuration = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 1f;

    private int currentLane = 1;

    private float baseY;
    private float jumpTimer;
    private bool isJumping;

    public float CurrentJumpHeight { get; private set; }

    public bool IsJumping => isJumping;
    public float BaseY => baseY;

    private float laneMoveTimer;
    private float laneStartX;
    private float laneTargetX;
    private bool isMovingLane;

    private void Start()
    {
        PerspectiveSettings settings = PerspectiveSettings.Instance;

        if (settings == null)
            return;

        baseY = settings.PlayerPosition.y;

        transform.position = new Vector3(
            settings.GetPlayerLaneX(1),
            baseY,
            transform.position.z
        );
    }

    private void Update()
    {
        HandleLaneInput();
        HandleJumpInput();
        MoveToLane();
        UpdateJump();
    }

    private void HandleLaneInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            StartLaneMovement();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            StartLaneMovement();
        }
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isJumping)
        {
            isJumping = true;
            jumpTimer = 0f;
        }
    }

    private void MoveToLane()
    {
        if (!isMovingLane)
            return;

        PerspectiveSettings settings = PerspectiveSettings.Instance;
        if (settings == null) return;

        laneMoveTimer += Time.deltaTime;

        float progress = Mathf.Clamp01(laneMoveTimer / laneMoveDuration);
        float easedProgress = EaseInOutQuad(progress);

        float newX = Mathf.Lerp(
            laneStartX,
            laneTargetX,
            easedProgress
        );

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );

        if (progress >= 1f)
        {
            isMovingLane = false;
        }
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f
            ? 2f * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }
    private void StartLaneMovement()
    {
        PerspectiveSettings settings = PerspectiveSettings.Instance;
        if (settings == null) return;

        laneStartX = transform.position.x;
        laneTargetX = settings.GetPlayerLaneX(currentLane);

        laneMoveTimer = 0f;
        isMovingLane = true;
    }

    private void UpdateJump()
    {
        if (!isJumping)
        {
            CurrentJumpHeight = 0f;
        }
        else
        {
            jumpTimer += Time.deltaTime;

            float progress = jumpTimer / jumpDuration;

            if (progress >= 1f)
            {
                progress = 1f;
                isJumping = false;
            }

            CurrentJumpHeight = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
        }

        Vector3 position = transform.position;
        position.y = baseY + CurrentJumpHeight;
        transform.position = position;
    }

}
