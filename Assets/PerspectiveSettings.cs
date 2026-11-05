
using UnityEngine;

public class PerspectiveSettings : MonoBehaviour
{
    public static PerspectiveSettings Instance { get; private set; }

    [Header("Perspective Settings")]
    [SerializeField] private Vector2 vanishingPoint = new Vector2(0f, 4f);
    [SerializeField] private Vector2 endPosition = new Vector2(0f, -4f);
    [SerializeField] private float laneWidth = 2f;

    public Vector2 VanishingPoint => vanishingPoint;
    public Vector2 EndPosition => endPosition;
    public float LaneWidth => laneWidth;

    
    public Vector2 PlayerPosition => Vector2.Lerp(
        vanishingPoint,
        endPosition,
        0.5f
    );

    public float GetLaneX(int laneIndex)
    {
        return endPosition.x + (laneIndex - 1) * laneWidth;
    }

    public float GetLaneBoundaryX(int boundaryIndex)
    {
        return endPosition.x + (boundaryIndex - 1.5f) * laneWidth;
    }

    public Vector2 GetLanePosition(int laneIndex, float progress)
    {
        Vector2 target = new Vector2(
            GetLaneX(laneIndex),
            endPosition.y
        );

        return Vector2.Lerp(
            vanishingPoint,
            target,
            progress
        );
    }
    
   public float GetPlayerLaneX(int laneIndex)
    {
        return GetLanePosition(laneIndex, 0.5f).x;
    }

    public Vector2 GetLaneBoundaryPosition(int boundaryIndex, float progress)
    {
        float boundaryX = GetLaneBoundaryX(boundaryIndex);

        Vector2 target = new Vector2(
            boundaryX,
            endPosition.y
        );

        return vanishingPoint +
            (target - vanishingPoint) * progress;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
