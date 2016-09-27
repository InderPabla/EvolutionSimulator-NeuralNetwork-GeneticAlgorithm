using UnityEngine;
using System.Collections;

public class WorldManager_Test_1 : MonoBehaviour
{
    public GameObject tilePrefab;
    GameObject[,] tiles;

    private int xSize = 150;
    private int ySize = 150;


    //Declare these in your class
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;


    // Use this for initialization
    void Start()
    {
        Resources.UnloadUnusedAssets();
        tiles = new GameObject[ySize, xSize];

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                tiles[y, x] = Instantiate(tilePrefab, new Vector3(x, y, 0), tilePrefab.transform.rotation) as GameObject;
                SpriteRenderer sr = tiles[y, x].GetComponent<SpriteRenderer>();
                float r = Random.Range(0f,1f);
                float g = Random.Range(0f,1f);
                float b = Random.Range(0f,1f);
                sr.color = new Color(r,g,b);
            }
        }


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

    // If application quits makes sure all sockets and streams are closed
    private void OnApplicationQuit()
    {
        Debug.Log("Qutting.");
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                Destroy(tiles[y,x]);

            }
        }

        Resources.UnloadUnusedAssets();
    }
}
