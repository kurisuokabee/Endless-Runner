
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Hit Detection")]
    [SerializeField] private float hitDistance = 0.6f;
    [SerializeField] private float minimumJumpHeight = 0.5f;

    private int laneIndex;
    private float moveSpeed;
    private float progress;

    private float minimumScale;
    private float maximumScale;

    private Vector3 startingScale;

    private PlayerMovement playerMovement;
    private PlayerHP playerHP;

    private bool hasHit;
    private bool hasPassedPlayer;

    private void Awake()
    {
        startingScale = transform.localScale;
    }

    public void Initialize(
        int lane,
        float speed,
        float minScale,
        float maxScale,
        PlayerMovement movement,
        PlayerHP hp)
    {
        laneIndex = lane;
        moveSpeed = speed;

        minimumScale = minScale;
        maximumScale = maxScale;

        playerMovement = movement;
        playerHP = hp;

        progress = 0f;
        hasHit = false;
        hasPassedPlayer = false;

        UpdatePerspective();
    }

    private void Update()
    {
        float previousProgress = progress;

        MoveObstacle();


        UpdatePerspective();
        CheckHit(previousProgress);

        if (progress >= 1f)
            Destroy(gameObject);
    }

    private void MoveObstacle()
    {
        float acceleration = Mathf.Pow(progress, 2f);

        float currentSpeed = moveSpeed * (0.15f + acceleration);

        progress += currentSpeed * Time.deltaTime;
        progress = Mathf.Clamp01(progress);
    }
    
   
    private void UpdatePerspective()
    {
        PerspectiveSettings settings = PerspectiveSettings.Instance;

        if (settings == null)
            return;

        Vector2 position = settings.GetLanePosition(laneIndex, progress);

        transform.position = new Vector3(
            position.x,
            position.y,
            0f
        );

        float scaleMultiplier = Mathf.Lerp(
            minimumScale,
            maximumScale,
            progress
        );

        transform.localScale = startingScale * scaleMultiplier;
    }

    
    private void CheckHit(float previousProgress)
    {
        if (hasHit || hasPassedPlayer ||
            playerMovement == null || playerHP == null)
            return;

        PerspectiveSettings settings = PerspectiveSettings.Instance;

        if (settings == null)
            return;

        // Get the obstacle lane's X position at the player's perspective depth.
        float playerLaneX = settings.GetLanePosition(laneIndex, 0.5f).x;

        // Check whether the player is in this lane.
        if (Mathf.Abs(playerLaneX - playerMovement.transform.position.x) > hitDistance)
            return;

        // Check if the obstacle crossed the player's ground position this frame.
        float previousY = settings.GetLanePosition(laneIndex, previousProgress).y;
        float currentY = settings.GetLanePosition(laneIndex, progress).y;

        if (previousY > playerMovement.BaseY &&
            currentY <= playerMovement.BaseY)
        {
            hasPassedPlayer = true;

            if (playerMovement.CurrentJumpHeight >= minimumJumpHeight)
                return;

            hasHit = true;
            playerHP.TakeDamage();

            Debug.Log("Player hit by obstacle!");
        }
    }


}
