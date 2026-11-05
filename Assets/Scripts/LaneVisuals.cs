using UnityEngine;

public class LaneVisuals : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color laneColor = Color.white;
    [SerializeField] private float lineWidth = 0.04f;
    [SerializeField] private int sortingOrder = -10;

    private LineRenderer[] laneLines;
    private Material lineMaterial;

    private void Start()
    {
        CreateLines();
        UpdateLines();
    }


    private void CreateLines()
    {
        laneLines = new LineRenderer[4];

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogWarning("Could not find Sprites/Default shader.");
            return;
        }

        lineMaterial = new Material(shader);

        for (int i = 0; i < laneLines.Length; i++)
        {
            GameObject lineObject = new GameObject("Lane Line " + i);
            lineObject.transform.SetParent(transform);

            LineRenderer line = lineObject.AddComponent<LineRenderer>();

            line.useWorldSpace = true;
            line.positionCount = 2;

            line.startWidth = lineWidth;
            line.endWidth = lineWidth;

            line.startColor = laneColor;
            line.endColor = laneColor;

            line.material = lineMaterial;
            line.sortingOrder = sortingOrder;

            laneLines[i] = line;
        }
    }

    private void UpdateLines()
    {
        PerspectiveSettings settings = PerspectiveSettings.Instance;

        if (settings == null || laneLines == null)
            return;

        for (int i = 0; i < laneLines.Length; i++)
        {
            if (laneLines[i] == null)
                continue;

            Vector2 top = settings.VanishingPoint;
            Vector2 bottom = settings.GetLaneBoundaryPosition(i, 1f);

            laneLines[i].SetPosition(0, top);
            laneLines[i].SetPosition(1, bottom);
        }
    }

    private void OnDestroy()
    {
        if (lineMaterial != null)
            Destroy(lineMaterial);
    }
}