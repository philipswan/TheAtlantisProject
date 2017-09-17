/**
 * Based on a script by Steffen (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in $primitives_966_104.zip, originally named "Primitives.cs")
 *
 * Editted by Michael Zoller on December 6, 2015.
 * It was shortened by about 30 lines (and possibly sped up by a factor of 2) by consolidating math & loops and removing intermediate Collections.
 */
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ElevatorCables : MonoBehaviour
{

    public GameObject carrier_prefab;
    public Transform ring_edge_transform;
    public float ringLatitude = -40;
    public float TubeRadius = 0.00001f;
    public int numTubeSides = 8;
    public int numElevatorCables = 90;
    public float ringAltitude = .0003f;
    private float TorusRadius;

    void Start()
    {
        TorusRadius = Mathf.Cos(ringLatitude * Mathf.PI / 180) / 2;
        RefreshElevatorCables();
    }

    public void RefreshElevatorCables()
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
        float elevatorCableSpacing = 2.0f * Mathf.PI / (float)numElevatorCables;
        float tubeSideSize = 2.0f * Mathf.PI / (float)numTubeSides;

        // Create floats for our xyz coordinates, and angles
        float x0 = 0, y0 = 0, z0 = 0;
        float x1 = 0, y1 = 0, z1 = 0;
        float x2 = 0, y2 = 0, z2 = 0;
        float x3 = 0, y3 = 0, z3 = 0;
        float x4 = 0, y4 = 0, z4 = 0;
        float theta;
        float phi0;
        float phi1;

        for (int i = 0; i < numElevatorCables; i++)
        {
            theta = i * elevatorCableSpacing;
            phi0 = TorusRadius;
            phi1 = TorusRadius - ringAltitude * Mathf.Cos(ringLatitude * Mathf.Deg2Rad);

            // Find the current and next segments
            int tubePrimitiveBaseOffset = i * numTubeSides * 2;
            int tubeIndexBaseOffset = i * numTubeSides * 6;

            x2 = phi1 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1) - TorusRadius;
            z2 = phi1 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1);
            y2 = TubeRadius * Mathf.Sin(0 * tubeSideSize);
            x3 = phi1 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1) - TorusRadius;
            z3 = phi1 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1);
            y3 = TubeRadius * Mathf.Sin(0 * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);
            x4 = phi1 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(1 * tubeSideSize) / phi1) - TorusRadius;
            z4 = phi1 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(1 * tubeSideSize) / phi1);
            y4 = TubeRadius * Mathf.Sin(1 * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);

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
                x1 = phi1 * Mathf.Cos(theta + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi1) - TorusRadius;
                z1 = phi1 * Mathf.Sin(theta + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi1);
                y1 = TubeRadius * Mathf.Sin(j * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);

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


            //var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //go.transform.parent = this.transform; //ring_edge_transform;
            //go.transform.localPosition = new Vector3(x2, y2, z2);
            //go.transform.localScale = Vector3.one * 1e-3f;

            var acc = Instantiate(carrier_prefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
            acc.transform.localPosition = new Vector3(x3, y3, z3);
            acc.transform.localScale = Vector3.one * 1e-6f;
            var cabletop = new Vector3(x2, y2, z2);
            var cablebot = new Vector3(x3, y3, z3);
            var cableleft = new Vector3(x4, y4, z4);
            //acc.transform.rotation = Quaternion.LookRotation(this.transform.TransformVector(Vector3.Cross(cableleft - cablebot, cabletop - cablebot)), this.transform.TransformVector(cabletop - cablebot));
            //acc.transform.LookAt(this.transform.TransformVector(Vector3.Cross(cableleft - cablebot, cabletop - cablebot)), this.transform.TransformVector(cabletop - cablebot));
            acc.transform.LookAt(this.transform.TransformPoint(cableleft), this.transform.TransformVector(cabletop-cablebot));
        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
    }
}
