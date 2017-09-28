using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTest : MonoBehaviour {

    void Start()
    {
        float size = 1f;
        Vector3[] vertices = {
            new Vector3(0, size, 0),
            new Vector3(0, 0, 0),
            new Vector3(size, size, 0),
            new Vector3(size, 0, 0),

            new Vector3(0, 0, size),
            new Vector3(size, 0, size),
            new Vector3(0, size, size),
            new Vector3(size, size, size),

            new Vector3(size, 0, 0),
            new Vector3(size, 0, size),
            new Vector3(size, size, 0),
            new Vector3(size, size, size),

            new Vector3(0, size, 0),
            new Vector3(0, size, size),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, size),
        };

        int[] triangles = {
            0, 2, 1, // front
			1, 2, 3,
            4, 5, 6, // back
			5, 7, 6,
            13, 11, 12, //top
			11, 10, 12,
            1, 3, 4, //bottom
			3, 5, 4,
            8, 10, 9,// left
			10, 11, 9,
            12, 14, 13,// right
			14, 15, 13
        };


        Vector2[] uvs = {
            new Vector2(0f, 0.33f),
            new Vector2(0.33f, 0.33f),
            new Vector2(0f, 0f),
            new Vector2(0.33f, 0f),

            new Vector2(0.66f, 0.33f),
            new Vector2(0.66f, 0f),
            new Vector2(1f, 0.33f),
            new Vector2(1f, 0f),

            new Vector2(0f, 0.66f),
            new Vector2(0f, 0.33f),
            new Vector2(0.33f, 0.66f),
            new Vector2(0.33f, 0.33f),

            new Vector2(0.66f, 0.66f),
            new Vector2(0.66f, 0.33f),
            new Vector2(1f, 0.66f),
            new Vector2(1f, 0.33f)
        };

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        transform.Translate(new Vector3(-0.5f, -0.5f, -0.5f));
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
