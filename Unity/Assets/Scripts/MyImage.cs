using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyImage : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] Texture2D texture;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    VertexHelper vertexHelper;
    Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        vertexHelper = new VertexHelper();

        InitMesh();
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.material.color = color;
        meshRenderer.material.mainTexture = texture;
    }

    void InitMesh()
    {
        mesh = new Mesh();
        vertexHelper.AddVert(new Vector3(0, 0), color, new Vector2(0, 0));
        vertexHelper.AddVert(new Vector3(0, 100), color, new Vector2(0, 1));
        vertexHelper.AddVert(new Vector3(100, 100), color, new Vector2(1, 1));
        vertexHelper.AddVert(new Vector3(100, 0), color, new Vector2(1, 0));

        vertexHelper.AddTriangle(0, 1, 2);
        vertexHelper.AddTriangle(2, 3, 0);

        vertexHelper.FillMesh(mesh);
        meshFilter.mesh = mesh;
    }
}
