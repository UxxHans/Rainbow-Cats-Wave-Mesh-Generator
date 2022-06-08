using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveModelGenerator : MonoBehaviour
{
    public bool debug = true;

    public Vector2Int verticesSize = new Vector2Int(100, 100);
    public Vector2 verticesDistance = Vector2.one;

    public float scale = 15f;
    public float intensity = 5f;
    [Range(0, 5)] public float speed = 0.5f;

    private Vector3[] vertices;
    private Mesh mesh;

    public void Start()
    {
        if (verticesSize.x <= 0 || verticesSize.y <= 0) Debug.LogError("Vertices Size must be set above 0.");

        GenerateWaveMesh();
        printTotalTriangles();
    }

    public void printTotalTriangles()
    {
        int totalTriangles = (verticesSize.x - 1) * (verticesSize.y - 1) * 2;
        Debug.Log("Mesh Generated. Total Triangles: " + totalTriangles);
    }

    public void GenerateWaveMesh()
    {
        mesh = new Mesh();
        vertices = GenerateVertices(verticesSize, verticesDistance);

        mesh.vertices = vertices;
        mesh.triangles = GenerateTriangles(verticesSize);
        mesh.normals = GenerateNormals(verticesSize);
        mesh.uv = GenerateUV(verticesSize);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    public Vector3[] GenerateVertices(Vector2Int verticesSize, Vector2 verticesDistance)
    {
        int totalVertices = verticesSize.x * verticesSize.y;
        Vector3[] vertices = new Vector3[totalVertices];

        for (int x = 0; x < verticesSize.x; x++)
        {
            for (int y = 0; y < verticesSize.y; y++)
            {
                float posX = x * verticesDistance.x;
                float posY = y * verticesDistance.y;
                vertices[y * verticesSize.x + x] = new Vector3(posX, 0, posY);
            }
        }

        return vertices;
    }

    public int[] GenerateTriangles(Vector2Int verticesSize)
    {
        const int VERTICES_PER_TRIANGLE = 3;
        const int TRIANGLES_PER_QUAD = 2;
        const int VERTICES_PER_QUAD = VERTICES_PER_TRIANGLE * TRIANGLES_PER_QUAD;

        int totalQuads = (verticesSize.x - 1) * (verticesSize.y - 1);
        int totalTriangles = totalQuads * TRIANGLES_PER_QUAD;

        int[] triangles = new int[totalTriangles * VERTICES_PER_TRIANGLE];

        int[] QUAD_OFFSETS = new int[VERTICES_PER_QUAD]
        {
            verticesSize.x, 1, 0,
            verticesSize.x, verticesSize.x + 1, 1
        };

        for (int i = 0; i < totalQuads; i++)
        {
            int currentOrigin = i + i / (verticesSize.x - 1);

            for(int t = 0; t < VERTICES_PER_QUAD; t++)
            {
                triangles[i * VERTICES_PER_QUAD + t] = currentOrigin + QUAD_OFFSETS[t];
            }
        }

        return triangles;
    }

    public Vector3[] GenerateNormals(Vector2Int meshResolution)
    {
        Vector3[] normals = new Vector3[meshResolution.x * meshResolution.y];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }

        return normals;
    }

    public Vector2[] GenerateUV(Vector2Int verticesSize)
    {
        int totalVertices = verticesSize.x * verticesSize.y;
        Vector2[] uv = new Vector2[totalVertices];

        for (int i = 0; i < totalVertices; i++)
        {
            uv[i] = new Vector2(i % verticesSize.x / (float)verticesSize.x, i / verticesSize.x / (float)verticesSize.x);
        }

        return uv;
    }

    public void CalculateNoise(Vector2Int verticesSize)
    {
        for (int y = 0; y < verticesSize.y; y++)
        {
            for (int x = 0; x < verticesSize.x; x++)
            {
                float offset = Time.time * speed;

                float xCoord = x * (scale / verticesSize.x);
                float yCoord = y * (scale / verticesSize.x);

                float sample = Mathf.PerlinNoise(xCoord + offset, yCoord + offset) + Mathf.PerlinNoise(xCoord - offset, yCoord - offset);

                Vector3 position = vertices[y * verticesSize.x + x];
                vertices[y * verticesSize.x + x] = new Vector3(position.x, sample * intensity, position.z);
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    public void Update()
    {
        CalculateNoise(verticesSize);
    }

    public void OnDrawGizmos()
    {
        if (!debug) return;

        float dotSize = 0.1f;
        foreach (Vector3 vertice in vertices)
        {
            Gizmos.DrawCube(transform.position + vertice, Vector3.one * dotSize);
        }
    }
}