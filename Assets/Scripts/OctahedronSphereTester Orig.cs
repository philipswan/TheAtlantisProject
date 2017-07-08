﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OctahedronSphereCreator_orig
{
    private static Vector3[] directions = {
        Vector3.left,
        Vector3.back,
        Vector3.right,
        Vector3.forward
    };
    private static void Normalize(Vector3[] vertices, Vector3[] normals)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i] = vertices[i].normalized;
        }
    }
    private static void CreateUV(Vector3[] vertices, Vector2[] uv)
    {
        float previousX = 1f;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            if (v.x == previousX)
            {
                uv[i - 1].x = 1f;
            }
            previousX = v.x;
            Vector2 textureCoordinates;
            textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
            if (textureCoordinates.x < 0f)
            {
                textureCoordinates.x += 1f;
            }
            textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
            uv[i] = textureCoordinates;
        }
        uv[vertices.Length - 4].x = uv[0].x = 0.125f;
        uv[vertices.Length - 3].x = uv[1].x = 0.375f;
        uv[vertices.Length - 2].x = uv[2].x = 0.625f;
        uv[vertices.Length - 1].x = uv[3].x = 0.875f;
    }
    public static Mesh Create(int subdivisions_orig, float radius_orig)
    {
        Vector3[] vertices = {
            Vector3.down, Vector3.down, Vector3.down, Vector3.down,
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.up, Vector3.up, Vector3.up, Vector3.up
        };

        int[] triangles = {
            0, 4, 5,
            1, 5, 6,
            2, 6, 7,
            3, 7, 4,

            8, 5, 4,
            9, 6, 5,
            10, 7, 6,
            11, 4, 7
        };
        Vector3[] normals = new Vector3[vertices.Length];
        Normalize(vertices, normals);

        Vector2[] uv = new Vector2[vertices.Length];
        CreateUV(vertices, uv);

        if (radius_orig != 1f)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= radius_orig;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Octahedron Sphere";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        return mesh;
    }
    private static int CreateVertexLine(
        Vector3 from, Vector3 to, int steps, int v, Vector3[] vertices
      )
    {
        for (int i = 1; i <= steps; i++)
        {
            vertices[v++] = Vector3.Lerp(from, to, (float)i / steps);
        }
        return v;
    }
    private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
    {
        for (int i = 1; i < steps; i++)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;

            triangles[t++] = vBottom++;
            triangles[t++] = vTop++;
            triangles[t++] = vBottom;
        }
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = vTop;
        return t;
    }
    private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
    {
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = ++vBottom;
        for (int i = 1; i <= steps; i++)
        {
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            triangles[t++] = vBottom;

            triangles[t++] = vBottom;
            triangles[t++] = vTop++;
            triangles[t++] = ++vBottom;
        }
        return t;
    }
    private static void CreateOctahedron(Vector3[] vertices, int[] triangles, int resolution)
    {
        int v = 0, vBottom = 0, t = 0;

        for (int i = 0; i < 4; i++)
        {
            vertices[v++] = Vector3.down;
        }

        for (int i = 1; i <= resolution; i++)
        {
            float progress = (float)i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);
            for (int d = 0; d < 4; d++)
            {
                from = to;
                to = Vector3.Lerp(Vector3.down, directions[d], progress);
                t = CreateLowerStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i > 1 ? (i - 1) : 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for (int i = resolution - 1; i >= 1; i--)
        {
            float progress = (float)i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);
            for (int d = 0; d < 4; d++)
            {
                from = to;
                to = Vector3.Lerp(Vector3.up, directions[d], progress);
                t = CreateUpperStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i + 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for (int i = 0; i < 4; i++)
        {
            triangles[t++] = vBottom;
            triangles[t++] = v;
            triangles[t++] = ++vBottom;
            vertices[v++] = Vector3.up;
        }
    }
}

[RequireComponent(typeof(MeshFilter))]
public class OctahedronSphereTester_orig : MonoBehaviour
{

    public int subdivisions_orig = 0;
    public float radius_orig = .01f;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = OctahedronSphereCreator_orig.Create(subdivisions_orig, radius_orig);
    }
}
