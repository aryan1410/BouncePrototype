using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class DashedOutline2D : MonoBehaviour
{
    public Vector2 size = Vector2.one;
    public Color color = new Color(0.55f, 0.55f, 0.55f, 1f);
    [Min(1)] public int horizontalDashCount = 20;
    [Min(1)] public int verticalDashCount = 4;
    [Range(0.1f, 0.9f)] public float dashFill = 0.55f;
    [Min(0.001f)] public float thickness = 0.04f;
    public int sortingOrder = 1;

    private static Material sharedMaterial;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void OnEnable()
    {
        SyncWithPlatform();
        Rebuild();
    }

    void OnValidate()
    {
        SyncWithPlatform();
        Rebuild();
    }

    void Update()
    {
        if (SyncWithPlatform())
            Rebuild();
    }

    public void SetVisible(bool visible)
    {
        EnsureComponents();
        meshRenderer.enabled = visible;
    }

    private void Rebuild()
    {
        EnsureComponents();
        if (mesh == null)
        {
            mesh = new Mesh { name = "Dashed Outline Mesh" };
            mesh.hideFlags = HideFlags.DontSave;
        }

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        AddDashedEdge(new Vector2(-halfWidth, -halfHeight), new Vector2(halfWidth, -halfHeight), horizontalDashCount, vertices, triangles);
        AddDashedEdge(new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight), verticalDashCount, vertices, triangles);
        AddDashedEdge(new Vector2(halfWidth, halfHeight), new Vector2(-halfWidth, halfHeight), horizontalDashCount, vertices, triangles);
        AddDashedEdge(new Vector2(-halfWidth, halfHeight), new Vector2(-halfWidth, -halfHeight), verticalDashCount, vertices, triangles);

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        var colors = new Color[vertices.Count];
        for (int i = 0; i < colors.Length; i++) colors[i] = color;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sortingOrder = sortingOrder;
    }

    private void AddDashedEdge(Vector2 start, Vector2 end, int dashCount, List<Vector3> vertices, List<int> triangles)
    {
        Vector2 direction = end - start;
        Vector2 normal = new Vector2(-direction.y, direction.x).normalized * (thickness * 0.5f);

        for (int i = 0; i < dashCount; i++)
        {
            float segmentStart = (float)i / dashCount;
            float segmentEnd = (i + dashFill) / dashCount;
            Vector2 a = Vector2.Lerp(start, end, segmentStart);
            Vector2 b = Vector2.Lerp(start, end, segmentEnd);
            int index = vertices.Count;
            vertices.Add(a - normal);
            vertices.Add(a + normal);
            vertices.Add(b + normal);
            vertices.Add(b - normal);
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 2);
            triangles.Add(index);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
        }
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

    private bool SyncWithPlatform()
    {
        if (transform.parent == null)
            return false;

        BoxCollider2D platformCollider = transform.parent.GetComponent<BoxCollider2D>();
        if (platformCollider == null)
            return false;

        Vector3 parentScale = transform.parent.lossyScale;
        if (Mathf.Approximately(parentScale.x, 0f) || Mathf.Approximately(parentScale.y, 0f))
            return false;

        Vector3 inverseScale = new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f);
        Vector2 platformSize = Vector2.Scale(
            platformCollider.size,
            new Vector2(Mathf.Abs(parentScale.x), Mathf.Abs(parentScale.y)));
        Vector3 platformOffset = platformCollider.offset;
        bool changed = transform.localPosition != platformOffset
                       || transform.localRotation != Quaternion.identity
                       || transform.localScale != inverseScale
                       || size != platformSize;

        transform.localPosition = platformOffset;
        transform.localRotation = Quaternion.identity;
        transform.localScale = inverseScale;
        size = platformSize;
        return changed;
    }
}
