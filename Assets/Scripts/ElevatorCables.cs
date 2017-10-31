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
    public GameObject climber_prefab;
    //public GameObject top_terminal_prefab;
    public Transform ring_edge_transform;
    public float ringLatitude = -40;
    public float tubeRadius = 0.00001f;
    public int numTubeSides = 8;
    public int numElevatorCables = 90;
    public float ringAltitude = .0003f;
    public Waypoints Keymanager;
    private float tetheredRingRadius;

    void Start()
    {
        tetheredRingRadius = Mathf.Cos(ringLatitude * Mathf.PI / 180) / 2;
        RefreshElevatorCables();
    }

    public void DrawCylinder(Vector3 start, Vector3 end, float Radius, Vector3[] vertices, int[] triangleIndices, int tubePrimitiveBaseOffset, int tubeIndexBaseOffset)
    {
        Vector3 v1 = (end - start).normalized;
        Vector3 v2 = Vector3.Cross(v1, Vector3.up).normalized * Radius;
        Vector3 v3 = Vector3.Cross(v1, v2).normalized * Radius;
        float r;

        //Debug.DrawRay(transform.TransformPoint(start), transform.TransformDirection(v2 * 1000), Color.red, 100);
        //Debug.DrawRay(transform.TransformPoint(start), transform.TransformDirection(v3 * 1000), Color.green, 100);
        //Debug.DrawRay(transform.TransformPoint(end), transform.TransformDirection(v2 * 1000), Color.yellow, 100);
        //Debug.DrawRay(transform.TransformPoint(end), transform.TransformDirection(v3 * 1000), Color.blue, 100);

        for (int j = 0; j < numTubeSides; j++)
        {
            // Find next (or first) vertex offset
            int m = (j + 1) % numTubeSides; // changed currentTube.Count to numTubeSides

            // Find the 4 vertices that make up a quad
            int iv0 = tubePrimitiveBaseOffset + j * 2 + 0;
            int iv1 = tubePrimitiveBaseOffset + j * 2 + 1;
            int iv2 = tubePrimitiveBaseOffset + m * 2 + 0;
            int iv3 = tubePrimitiveBaseOffset + m * 2 + 1;

            r = j * 2.0f * Mathf.PI / numTubeSides;
            Vector3 v4 = Mathf.Cos(r) * v2 + Mathf.Sin(r) * v3;
            Vector3 v5 = start + v4;
            Vector3 v6 = end + v4;

            vertices[iv0] = v5;
            vertices[iv1] = v6;

            // As we itterate around the circumference of the tube, "Draw" the two triangles that make each tube face
            int ti = tubeIndexBaseOffset + j * 2 * 3;
            // Triangle 0
            triangleIndices[ti + 0] = iv0;
            triangleIndices[ti + 1] = iv2;
            triangleIndices[ti + 2] = iv1;
            // Triangle 1
            triangleIndices[ti + 3] = iv1;
            triangleIndices[ti + 4] = iv2;
            triangleIndices[ti + 5] = iv3;
        }
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
        float x2 = 0, y2 = 0, z2 = 0;
        float x3 = 0, y3 = 0, z3 = 0;
        float x4 = 0, y4 = 0, z4 = 0;
        float theta;
        float phi0;
        float phi1;

        for (int i = 0; i < numElevatorCables; i++)
        {
            theta = i * elevatorCableSpacing;
            phi0 = tetheredRingRadius;
            phi1 = tetheredRingRadius - ringAltitude * Mathf.Cos(ringLatitude * Mathf.Deg2Rad);

            // Find the current and next segments
            int tubePrimitiveBaseOffset = i * numTubeSides * 2;
            int tubeIndexBaseOffset = i * numTubeSides * 6;

            x2 = phi1 * Mathf.Cos(theta + tubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1) - tetheredRingRadius;
            z2 = phi1 * Mathf.Sin(theta + tubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1);
            y2 = tubeRadius * Mathf.Sin(0 * tubeSideSize);
            x3 = phi1 * Mathf.Cos(theta + tubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1) - tetheredRingRadius;
            z3 = phi1 * Mathf.Sin(theta + tubeRadius * Mathf.Cos(0 * tubeSideSize) / phi1);
            y3 = tubeRadius * Mathf.Sin(0 * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);
            x4 = phi1 * Mathf.Cos(theta + tubeRadius * Mathf.Cos(1 * tubeSideSize) / phi1) - tetheredRingRadius;
            z4 = phi1 * Mathf.Sin(theta + tubeRadius * Mathf.Cos(1 * tubeSideSize) / phi1);
            y4 = tubeRadius * Mathf.Sin(1 * tubeSideSize) - ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);

            // Find the current and next segments

            Vector3 cabletop, cablebot, cableleft;
            cabletop.x = phi0 * Mathf.Cos(theta) - tetheredRingRadius;
            cabletop.z = phi0 * Mathf.Sin(theta);
            cabletop.y = -Mathf.Pow(tetheredRingRadius - phi0, 1.7f) * 10;
            cablebot.x = phi1 * Mathf.Cos(theta) - tetheredRingRadius;
            cablebot.z = phi1 * Mathf.Sin(theta);
            cablebot.y = -ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);
            cableleft.x = phi0 * Mathf.Cos(theta) - tetheredRingRadius;
            cableleft.z = phi0 * Mathf.Sin(theta);
            cableleft.y = -ringAltitude * Mathf.Sin(ringLatitude * Mathf.Deg2Rad);

            DrawCylinder(cabletop, cablebot, tubeRadius, vertices, triangleIndices, tubePrimitiveBaseOffset, tubeIndexBaseOffset);

            var cabletop_old = new Vector3(x2, y2, z2);
            var cablebot_old = new Vector3(x3, y3, z3);
            var cableleft_old = new Vector3(x4, y4, z4);

            //var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //go.transform.parent = this.transform; //ring_edge_transform;
            //go.transform.localPosition = new Vector3(x2, y2, z2);
            //go.transform.localScale = Vector3.one * 1e-3f;

            if (i <= 4)
            {
                // Add an box to represent the ring terminal at the top of each stay 
                //var acc_top = Instantiate(top_terminal_prefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                //acc_top.transform.localPosition = Vector3.Lerp(cablebot, cabletop, 0.95f);
                //acc_top.transform.localScale = Vector3.one * 4e-5f;
                ////acc_top.transform.localRotation = new Quaternion(0, -90, 0,  0); //This doesn't work
                //acc_top.transform.LookAt(this.transform.TransformPoint(cableleft_old), this.transform.TransformVector(cabletop_old - cablebot_old));

                // Add an capsule somewher along the length of each stay 
                var acc_mid = Instantiate(climber_prefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                acc_mid.transform.localPosition = Vector3.Lerp(cablebot, cabletop, 0.85f);
                acc_mid.transform.localScale = Vector3.one * 1e-6f;
                // acc_mid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                acc_mid.transform.LookAt(this.transform.TransformPoint(cableleft_old), this.transform.TransformVector(cabletop_old - cablebot_old));

                // Add an aircraft carrier to represent the surface terminal at the bottom of each stay 
                var acc_bot = Instantiate(carrier_prefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                acc_bot.transform.localPosition = new Vector3(x3, y3, z3);
                //acc_bot.transform.localPosition = Vector3.Lerp(cablebot, cabletop, 0.2f); // Why doesn't this work???
                acc_bot.transform.localScale = Vector3.one * 3e-7f;
                acc_bot.transform.LookAt(this.transform.TransformPoint(cableleft_old), this.transform.TransformVector(cabletop_old - cablebot_old));
                //acc_bot.transform.LookAt(this.transform.TransformPoint(cableleft), this.transform.TransformVector(cabletop - cablebot));

                //acc.transform.rotation = Quaternion.LookRotation(this.transform.TransformVector(Vector3.Cross(cableleft - cablebot, cabletop - cablebot)), this.transform.TransformVector(cabletop - cablebot));
                //acc.transform.LookAt(this.transform.TransformVector(Vector3.Cross(cableleft - cablebot, cabletop - cablebot)), this.transform.TransformVector(cabletop - cablebot));

                if (i == 0)
                {
                    // We're going to set a waypoint so that we can navigate to here later using animation scripts
                    //Transform temp = Keymanager.GetComponent<Waypoints>().waypoints[1];
                    Keymanager.waypoints[1].position = -acc_bot.transform.position;
                    Keymanager.waypoints[1].rotation = acc_bot.transform.rotation;
                    Keymanager.waypoints[1].localScale = Vector3.one * 10.0f;
                }
            }


        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
    }
}
