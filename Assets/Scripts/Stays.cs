﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Stays : MonoBehaviour
{

    public float RingLatitude = -40;
    private float TorusRadius;
    public float TubeRadius = 0.001f;
    public int numTubeSides = 12;
    public int numStayLevels = 4;
    public int numStays = 360;
    public int numStaysPerInstance = 60;
    public Material material;

    void Start()
    {
        TorusRadius = Mathf.Cos(RingLatitude * Mathf.PI / 180) / 2;
        RefreshStays();
    }

    public void ComputeCylinderEndpoints(float start_r, float end_r, float start_t, float end_t, float SegmentLength, int segmentsPerSection) {
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

    public void AddCylinders(int instance, int stayIndex, ref int sectionIndex, float section_start_d, float section_end_d, float section_start_t, float section_end_t, float SegmentLength, int segmentsPerSection, int numSegments, Vector3[] vertices, int[] triangleIndices)
    {
        // Pre-Calculate the size in radians of TubeSide
        float staySpacing = 2.0f * Mathf.PI / numStays;
        float tubeSideSize = 0.2f * Mathf.PI / numTubeSides;

        float theta0;
        float theta1;
        float phi0;
        float phi1;
        float x0, y0, z0;
        float x1, y1, z1;
        int segmentIndex;

        // We need to draw each branch (called a section) using a number of segments to that we can add curvature to create a drooping stay.
        // Do do this we linearly interpolate between the start and end of the stay section.
        for (segmentIndex = 0; segmentIndex<segmentsPerSection; segmentIndex++)
        {
            float t0 = section_start_t + (section_end_t - section_start_t) * (segmentIndex + 0) / segmentsPerSection;
            float t1 = section_start_t + (section_end_t - section_start_t) * (segmentIndex + 1) / segmentsPerSection;
            float d0 = section_start_d + (section_end_d - section_start_d) * (segmentIndex + 0) / segmentsPerSection;
            float d1 = section_start_d + (section_end_d - section_start_d) * (segmentIndex + 1) / segmentsPerSection;
            theta0 = (instance * numStaysPerInstance + stayIndex - (numStays/2) + t0) * staySpacing;
            phi0 = TorusRadius * (1.0f - d0 * 0.04f);
            theta1 = (instance * numStaysPerInstance + stayIndex - (numStays / 2) + t1) * staySpacing;
            phi1 = TorusRadius * (1.0f - d1 * 0.04f);

            // Find the current and next segments
            int tubePrimitiveBaseOffset = (stayIndex * numSegments + sectionIndex) * numTubeSides * 2;
            int tubeIndexBaseOffset = (stayIndex * numSegments + sectionIndex) * numTubeSides * 6;

            Vector3 start, end;
            start.x = phi0 * Mathf.Cos(theta0) - TorusRadius;
            start.z = phi0 * Mathf.Sin(theta0);
            start.y = -Mathf.Pow(TorusRadius - phi0, 1.7f) * 10;
            end.x   = phi1 * Mathf.Cos(theta1) - TorusRadius;
            end.z   = phi1 * Mathf.Sin(theta1);
            end.y   = -Mathf.Pow(TorusRadius - phi1, 1.7f) * 10;

            DrawCylinder(start, end, TubeRadius, vertices, triangleIndices, tubePrimitiveBaseOffset, tubeIndexBaseOffset);
            /*
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
                x0 = phi0 * Mathf.Cos(theta0 + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi0) - TorusRadius;
                z0 = phi0 * Mathf.Sin(theta0 + TubeRadius * Mathf.Cos(j * tubeSideSize) / phi0);
                y0 =                           TubeRadius * Mathf.Sin(j * tubeSideSize) - Mathf.Pow(TorusRadius - phi0, 1.7f) * 10;
                x1 = phi1 * Mathf.Cos(theta1 + TubeRadius * Mathf.Cos(m * tubeSideSize) / phi1) - TorusRadius;
                z1 = phi1 * Mathf.Sin(theta1 + TubeRadius * Mathf.Cos(m * tubeSideSize) / phi1);
                y1 =                           TubeRadius * Mathf.Sin(m * tubeSideSize) - Mathf.Pow(TorusRadius - phi1, 1.7f) * 10;

                // As we itterate around the circumference of the tube, add the vertices at each end of the tube
                vertices[iv0] = new Vector3(x0, y0, z0);
                vertices[iv1] = new Vector3(x1, y1, z1);

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
            */
            sectionIndex++;
        }
    }

    public void NewStaySection(int instance, int stayIndex, ref int sectionIndex, int numStayLevels, int currentLevel, float start_r, float end_r, float start_t, float target_t, float end_t, float SegmentLength, int segmentsPerSection, int numSegments, Vector3[] vertices, int[] triangleIndices)
    {
        int new_level;
        float length;
        float new_start_r;
        float new_end_r;
        float new_start_t;
        float new_target_t0;
        float new_target_t1;
        float new_end_t0;
        float new_end_t1;
        float start_d;
        float end_d;

        // convert the r's to distance from the ring
        length = Mathf.Pow(2, numStayLevels) - 1;
        start_d = (length - start_r) / length;
        end_d = (length - end_r) / length;
        segmentsPerSection = (int)Mathf.Pow(2, numStayLevels-1-currentLevel) * 4;
        AddCylinders(instance, stayIndex, ref sectionIndex, start_d, end_d, start_t, end_t, SegmentLength, segmentsPerSection, numSegments, vertices, triangleIndices);

        new_level = currentLevel + 1;
        if (new_level < numStayLevels)
        {
            // Fork the stays
            new_start_r = end_r;
            new_end_r = new_start_r + Mathf.Pow(2, numStayLevels - 1 - new_level);
            new_start_t = end_t;
            new_target_t0 = target_t - 1.0f / Mathf.Pow(2, new_level+1);
            new_target_t1 = target_t + 1.0f / Mathf.Pow(2, new_level+1);
            new_end_t0 = new_start_t + (new_target_t0 - new_start_t) * Mathf.Pow(2, numStayLevels - new_level - 1) / (Mathf.Pow(2, numStayLevels - new_level) - 1);
            new_end_t1 = new_start_t + (new_target_t1 - new_start_t) * Mathf.Pow(2, numStayLevels - new_level - 1) / (Mathf.Pow(2, numStayLevels - new_level) - 1);
            NewStaySection(instance, stayIndex, ref sectionIndex, numStayLevels, new_level, new_start_r, new_end_r, new_start_t, new_target_t0, new_end_t0, SegmentLength, segmentsPerSection, numSegments, vertices, triangleIndices);
            NewStaySection(instance, stayIndex, ref sectionIndex, numStayLevels, new_level, new_start_r, new_end_r, new_start_t, new_target_t1, new_end_t1, SegmentLength, segmentsPerSection, numSegments, vertices, triangleIndices);
        }
    }

    List<GameObject> stayObjects = new List<GameObject>();
    GameObject createStayObject()
    {
        GameObject obj = new GameObject("Stay part " + stayObjects.Count);
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3();
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = new Vector3(1, 1, 1);

        mr.sharedMaterial = material;

        stayObjects.Add(obj);
        return obj;
    }

    void destroyAllStayObjects()
    {
        foreach (GameObject obj in stayObjects)
        {
            Destroy(obj);
        }
        stayObjects.Clear();
    }

    public void RefreshStays()
    {
        destroyAllStayObjects();

        int numInstances = numStays / numStaysPerInstance;
        int sectionIndex;

        float SegmentLength = 1;
        int segmentsPerSection = 1;

        int numSegments = 0;
        for (int currentLevel = 0; currentLevel < numStayLevels; currentLevel++)
        {
            numSegments += (int)Mathf.Pow(2, currentLevel) * (int)Mathf.Pow(2, numStayLevels - 1 - currentLevel) * 4;
        }


        int totalVertices = numStaysPerInstance * numSegments * numTubeSides * 2;
        Vector3[] vertices = new Vector3[totalVertices];

        int totalPrimitives = numStaysPerInstance * numSegments * numTubeSides * 2;
        int totalIndices = totalPrimitives * 3;
        int[] triangleIndices = new int[totalIndices];

        for (int instance = 0; instance < numInstances; instance++)
        {
            GameObject obj = createStayObject();
            Mesh mesh = new Mesh();

            for (int stayIndex = 0; stayIndex < numStaysPerInstance; stayIndex++)
            {
                sectionIndex = 0;
                NewStaySection(
                    instance,
                    stayIndex,
                    ref sectionIndex,
                    numStayLevels,
                    0, // currentLevel
                    0, // start_r
                    Mathf.Pow(2, numStayLevels - 1), // end_r
                    0, // start_t
                    0, // target_t
                    0, // end_t
                    SegmentLength,
                    segmentsPerSection,
                    numSegments,
                    vertices,
                    triangleIndices);

                mesh.vertices = vertices;
                mesh.triangles = triangleIndices;

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                MeshFilter mFilter = obj.GetComponent<MeshFilter>(); // tweaked to Generic
                mFilter.mesh = mesh;
            }
        }
    }

    public void _RefreshStays()
    {
        // Each stay will fork, and then each of those will fork, and this repeats for numStayLevels. So we need "numSegments" which is 1+2+4+8+...2^(n-1)  or 2^n-1 
        int numSegments = 0;
        for (int currentLevel = 0; currentLevel < numStayLevels; currentLevel++)
        {
            numSegments += (int)Mathf.Pow(2, currentLevel) * (int)Mathf.Pow(2, numStayLevels - 1 - currentLevel) * 4;
        }

        // Total vertices - We'll construct a tube for each branch and that tube will have numTubeSides. 
        int totalVertices = numStays * numSegments * numTubeSides * 2;

        // Total primitives
        int totalPrimitives = numStays * numSegments * numTubeSides * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the mesh
        Mesh mesh = new Mesh();

        // Init the vertex and triangle arrays
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangleIndices = new int[totalIndices];

        // Create floats for our xyz coordinates, and angles
        int instance = 0;
        int segmentsPerSection;
        float SegmentLength;
        int sectionIndex;

        segmentsPerSection = 1;
        SegmentLength = 1; // Fix later

        for (int stayIndex = 0; stayIndex < numStays; stayIndex++)
        {
            sectionIndex = 0;
            NewStaySection(
                instance,
                stayIndex,
                ref sectionIndex,
                numStayLevels,
                0, // currentLevel
                0, // start_r
                Mathf.Pow(2, numStayLevels-1), // end_r
                0, // start_t
                0, // target_t
                0, // end_t
                SegmentLength,
                segmentsPerSection,
                numSegments,
                vertices,
                triangleIndices);
        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
    }
}
