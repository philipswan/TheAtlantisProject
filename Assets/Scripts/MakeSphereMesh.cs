using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

/*
private static class IcoSphere
{
    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }

    public class MakeSphereMesh : MonoBehaviour
    //public static void Create()
    {
        // Init the mesh
        Mesh mesh = new Mesh();

        //MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        //Mesh mesh = filter.mesh;
        //mesh.Clear();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
        int index = 0;

        int recursionLevel = 3;
        float radius = 1f;

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));


        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        mesh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        mesh.triangles = triList.ToArray();
        mesh.uv = new Vector2[vertices.Length];

        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
            normales[i] = vertList[i].normalized;

        mesh.normals = normales;

        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}
*/
public class MakeSphereMesh : MonoBehaviour {


    public float TorusRadius = .23f;
    public float TubeRadius = 0.00001f;
    public int numTubeSides = 8;
    public int numElevatorCables = 90;
    public float ringAltitude = .000001f;
    public float ringLatitude = -60;

    void Start()
    {
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        RefreshSphere();
    }

    public void RefreshSphere()
    {
        // Total vertices - We'll construct a tube for each branch and that tube will have numTubeSides. 
        int totalVertices = numElevatorCables * numTubeSides * 2;

        // Total primitives
        int totalPrimitives = numElevatorCables * numTubeSides * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the mesh
        Mesh mesh = new Mesh();

        // Init the vertex and triangle arrays
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangleIndices = new int[totalIndices];

        // Pre-Calculate the size in radians of TubeSide
        float elevatorCableSpacing = 2 * Mathf.PI / (float)numElevatorCables;
        float tubeSideSize = 2.0f * Mathf.PI / (float)numTubeSides;

        // Create floats for our xyz coordinates, and angles
        float x0, y0, z0;
        float x1, y1, z1;
        float theta;
        float phi0;
        float phi1;

        for (int i = 0; i < numElevatorCables; i++)
        {
            theta = i * elevatorCableSpacing;
            phi0 = TorusRadius;
            phi1 = TorusRadius - ringAltitude * Mathf.Cos(ringLatitude * Mathf.PI / 180.0f);

            // Find the current and next segments
            int tubePrimitiveBaseOffset = i * numTubeSides * 2;
            int tubeIndexBaseOffset = i * numTubeSides * 6;

            for (int j = 0; j < numTubeSides; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % numTubeSides; // changed currentTube.Count to numTubeSides

                // Find the 4 vertices that make up a quad
                int iv0 = tubePrimitiveBaseOffset + j * 2 + 0;
                int iv1 = tubePrimitiveBaseOffset + j * 2 + 1;
                int iv2 = tubePrimitiveBaseOffset + m * 2 + 0;
                int iv3 = tubePrimitiveBaseOffset + m * 2 + 1;

                // Calculate X, Y, Z coordinates.
                x0 = phi0 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi0) - TorusRadius;
                z0 = phi0 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi0);
                y0 = TubeRadius * Mathf.Sin(j * tubeSideSize);
                x1 = phi1 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(m * tubeSideSize) / phi1) - TorusRadius;
                z1 = phi1 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(m * tubeSideSize) / phi1);
                y1 = TubeRadius * Mathf.Sin(m * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.PI / 180.0f);

                // As we itterate around the circumference of the tube, add the vertices at each end of the tube
                vertices[iv0] = new Vector3(x0, y0, z0);
                vertices[iv1] = new Vector3(x1, y1, z1);

                // As we itterate around the circumference of the tube, "Draw" the two triangles that make each tube face
                int ti = tubeIndexBaseOffset + j * 2 * 3;
                // Triangle 0
                triangleIndices[ti + 0] = iv0;
                triangleIndices[ti + 1] = iv2;
                triangleIndices[ti + 2] = iv3;
                // Triangle 1
                triangleIndices[ti + 3] = iv3;
                triangleIndices[ti + 4] = iv1;
                triangleIndices[ti + 5] = iv0;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;

        Renderer renderer = GetComponent<Renderer>();
        renderer.material = Resources.Load("TestMaterial") as Material;
        //renderer.material.SetTextureScale("_MainTex", new Vector2(.001000f, .001000f));

    }
}
