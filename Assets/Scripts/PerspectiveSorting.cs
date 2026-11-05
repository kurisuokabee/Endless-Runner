using UnityEngine;
using UnityEngine.Rendering;

public class PerspectiveSorting : MonoBehaviour
{
    [SerializeField] private int backSortingOrder = 0;
    [SerializeField] private int frontSortingOrder = 100;

    [SerializeField] private PlayerMovement playerMovement;
    private SortingGroup sortingGroup;
    PerspectiveSettings settings;
    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();

        settings = PerspectiveSettings.Instance;
    }

    private void LateUpdate()
    {
        float sortingY = transform.position.y;

        if (playerMovement != null)
            sortingY = playerMovement.BaseY;

        float progress = Mathf.InverseLerp(
            settings.VanishingPoint.y,
            settings.EndPosition.y,
            sortingY
        );

        sortingGroup.sortingOrder = Mathf.RoundToInt(
            Mathf.Lerp(
                backSortingOrder,
                frontSortingOrder,
                progress
            )
        );
    }
}
