using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class ShapeRenderer2D : MonoBehaviour
{
    public enum ShapeType
    {
        Triangle,
        Square,
        Ellipse
    }

    public ShapeType shape = ShapeType.Square;
    public Color color = Color.white;
    public Vector2 size = Vector2.one;
    public int sortingOrder;

    private static Material sharedMaterial;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            Rebuild();
        }
    }

    void OnEnable()
    {
        Rebuild();
    }

    void OnValidate()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        EnsureComponents();
        if (mesh == null)
        {
            mesh = new Mesh { name = "Runtime Shape Mesh" };
            mesh.hideFlags = HideFlags.DontSave;
        }

        Vector2 halfSize = size * 0.5f;
        Vector3[] vertices;
        int[] triangles;

        if (shape == ShapeType.Triangle)
        {
            vertices = new[]
            {
                new Vector3(-halfSize.x, -halfSize.y),
                new Vector3(halfSize.x, -halfSize.y),
                new Vector3(0f, halfSize.y)
            };
            triangles = new[] { 0, 1, 2 };
        }
        else if (shape == ShapeType.Square)
        {
            vertices = new[]
            {
                new Vector3(-halfSize.x, -halfSize.y),
                new Vector3(halfSize.x, -halfSize.y),
                new Vector3(halfSize.x, halfSize.y),
                new Vector3(-halfSize.x, halfSize.y)
            };
            triangles = new[] { 0, 1, 2, 0, 2, 3 };
        }
        else
        {
            const int segmentCount = 32;
            vertices = new Vector3[segmentCount + 1];
            triangles = new int[segmentCount * 3];
            vertices[0] = Vector3.zero;

            for (int i = 0; i < segmentCount; i++)
            {
                float angle = i * Mathf.PI * 2f / segmentCount;
                vertices[i + 1] = new Vector3(
                    Mathf.Cos(angle) * halfSize.x,
                    Mathf.Sin(angle) * halfSize.y);

                int triangleIndex = i * 3;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i == segmentCount - 1 ? 1 : i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        var colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++) colors[i] = color;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sortingOrder = sortingOrder;
    }

    private void EnsureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (sharedMaterial == null)
        {
            Shader shader = Shader.Find("Bounce/FlatColor2D");
            sharedMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
        }

        meshRenderer.sharedMaterial = sharedMaterial;
    }
}
