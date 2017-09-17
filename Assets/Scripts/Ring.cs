/**
 * Based on a script by Steffen (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in $primitives_966_104.zip, originally named "Primitives.cs")
 *
 * Editted by Michael Zoller on December 6, 2015.
 * It was shortened by about 30 lines (and possibly sped up by a factor of 2) by consolidating math & loops and removing intermediate Collections.
 */
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Ring : MonoBehaviour {

    public float RingLatitude = -40;
    public float TubeRadius = 0.001f;
    public int numSegments = 100;
    public int numTubes = 12;
    private float TorusRadius;
    //public var material : Material;

    void Start() {
        TorusRadius = Mathf.Cos(RingLatitude * Mathf.PI / 180) / 2;
        RefreshRing();
    }

    public void RefreshRing() {
        // Total vertices
        int totalVertices = numSegments * numTubes;

        // Total primitives
        int totalPrimitives = totalVertices * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the mesh
        Mesh mesh = new Mesh();

        // Init the vertex and triangle arrays
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangleIndices = new int[totalIndices];

        // Calculate size of a segment and a tube
        float tubeSize = 2 * Mathf.PI / (float)numTubes;

        // Create floats for our xyz coordinates
        float x, y, z;

        // Begin loop that fills in both arrays
        for (int i = 0; i < numSegments; i++)
        {
            // Find next (or first) segment offset
            int n = (i + 1) % numSegments; // changed segmentList.Count to numSegments

            // Find the current and next segments
            int currentTubeOffset = i * numTubes;
            int nextTubeOffset = n * numTubes;
            float normi = (float)i / numSegments * Mathf.PI;
            float funci = 0.5f - (Mathf.Cos(normi) / 2.0f);
            float angle = funci * 2 * Mathf.PI;

            for (int j = 0; j < numTubes; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % numTubes; // changed currentTube.Count to numTubes

                // Find the 4 vertices that make up a quad
                int iv1 = currentTubeOffset + j;
                int iv2 = currentTubeOffset + m;
                int iv3 = nextTubeOffset + m;
                int iv4 = nextTubeOffset + j;

                // Calculate X, Y, Z coordinates.

                x = (TorusRadius + TubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Cos(angle) - TorusRadius;
                z = (TorusRadius + TubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Sin(angle);
                y = TubeRadius * Mathf.Sin(j * tubeSize);

                // Add the vertex to the vertex array
                vertices[iv1] = new Vector3(x, y, z);

                // "Draw" the first triangle involving this vertex
                triangleIndices[iv1 * 6] = iv1;
                triangleIndices[iv1 * 6 + 1] = iv2;
                triangleIndices[iv1 * 6 + 2] = iv3;
                // Finish the quad
                triangleIndices[iv1 * 6 + 3] = iv3;
                triangleIndices[iv1 * 6 + 4] = iv4;
                triangleIndices[iv1 * 6 + 5] = iv1;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
        Renderer renderer = GetComponent<MeshRenderer>();
        Material mat = Resources.Load("earthMat") as Material;
        renderer.material = mat;
        print(mat);
    }

}
