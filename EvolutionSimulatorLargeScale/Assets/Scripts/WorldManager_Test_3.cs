using UnityEngine;
using System.Collections;

public class WorldManager_Test_3 : MonoBehaviour
{
    MeshFilter meshFilter;
    int mapSizeX = 100;
    int mapSizeY = 100;
    Vector2[] UVArray;
    Mesh mesh;


    //Declare these in your class
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;
    private int texWidth = 100;
    private int texHeight = 100;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        BuildMesh();
    }

    public void BuildTexture()
    {
        Texture2D tex = new Texture2D(texWidth, texHeight);

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                float r = Random.Range(0f, 1f);
                float g = Random.Range(0f, 1f);
                float b = Random.Range(0f, 1f);
                Color c = new Color(r, g, b, 1);
                tex.SetPixel(x, y, c);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        MeshRenderer meshRender = GetComponent<MeshRenderer>();
        meshRender.sharedMaterials[0].mainTexture = tex;
    }

    public void BuildMesh()
    {
        int numTiles = mapSizeX * mapSizeY;
        int numTriangles = numTiles * 6;
        int numVerts = numTiles * 4;

        Vector3[] vertices = new Vector3[numVerts];
        UVArray = new Vector2[numVerts];

        int x, y, iVertCount = 0;
        for (x = 0; x < mapSizeX; x++)
        {
            for (y = 0; y < mapSizeY; y++)
            {
                vertices[iVertCount + 0] = new Vector3(x, y, 0);
                vertices[iVertCount + 1] = new Vector3(x + 1, y, 0);
                vertices[iVertCount + 2] = new Vector3(x + 1, y + 1, 0);
                vertices[iVertCount + 3] = new Vector3(x, y + 1, 0);
                iVertCount += 4;
            }
        }

        int[] triangles = new int[numTriangles];

        int iIndexCount = 0; iVertCount = 0;
        for (int i = 0; i < numTiles; i++)
        {
            triangles[iIndexCount + 0] += (iVertCount + 0);
            triangles[iIndexCount + 1] += (iVertCount + 1);
            triangles[iIndexCount + 2] += (iVertCount + 2);
            triangles[iIndexCount + 3] += (iVertCount + 0);
            triangles[iIndexCount + 4] += (iVertCount + 2);
            triangles[iIndexCount + 5] += (iVertCount + 3);

            iVertCount += 4; iIndexCount += 6;
        }

        mesh = new Mesh();
        //mesh.MarkDynamic(); if you intend to change the vertices a lot, this will help.
        mesh.vertices = vertices;
        Debug.Log(vertices.Length);
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;

        UpdateMesh(); //I put this in a separate method for my own purposes.
        BuildTexture();
    }


    //Note, the example UV entries I have are assuming a tile atlas 
    //with 16 total tiles in a 4x4 grid.

    public void UpdateMesh()
    {
        int iVertCount = 0;

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                UVArray[iVertCount + 0] = new Vector2(0, 0); //Top left of tile in atlas
                UVArray[iVertCount + 1] = new Vector2(.25f, 0); //Top right of tile in atlas
                UVArray[iVertCount + 2] = new Vector2(.25f, .25f); //Bottom right of tile in atlas
                UVArray[iVertCount + 3] = new Vector2(0, .25f); //Bottom left of tile in atlas
                iVertCount += 4;
            }
        }

        meshFilter.mesh.uv = UVArray;
    }



    // Update is called once per frame
    void Update()
    {
        CalculateFPS();
        Debug.Log(m_lastFramerate);
    }

    private void CalculateFPS()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }

}

