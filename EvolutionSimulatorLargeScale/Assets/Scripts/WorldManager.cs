using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {

    private Texture2D tex = null;
    private bool textureLoaded = false;
    private bool mouseDown = false;

    //Declare these in your class
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;

    public GameObject creaturePrefab;
    int size = 100;
    private GameObject[] gCre = new GameObject[100];
    private Rigidbody2D[] rCre = new Rigidbody2D[100];
    private Brain[] bCre = new Brain[100];

    void Start ()
    {
        for (int i = 0; i < size; i++)
        {
            Vector3 position = new Vector3(Random.Range(0f, 100f), Random.Range(0f,100f), creaturePrefab.transform.position.z);
            gCre[i] = Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject;

            rCre[i] = gCre[i].GetComponent<Rigidbody2D>();
            bCre[i] = new Brain(new int[] { 1, 10, 10, 2 });
        }
	}
	
	void Update ()
    {
        if (textureLoaded == true)
        {
            /*Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
            }

            if (mouseDown == true)
            {
                if (mouseCoords.x < 100f && mouseCoords.x > 0 && mouseCoords.y < 100f && mouseCoords.y > 0)
                {
                    int x = (int)mouseCoords.x;
                    int y = (int)mouseCoords.y;
                    tex.SetPixel(x, y, Color.red);
                    tex.Apply();
                }
            }*/


            for (int i = 0; i < size; i++)
            {
                float[] input = new float[] { 2 };
                float[] output = bCre[i].feedforward(input);


                rCre[i].velocity = rCre[i].transform.up * output[0];
                rCre[i].angularVelocity = output[1] * 100f;
            }



            //CalculateFPS();
        }
    }

    void SetTexture(Texture2D tex)
    {
        this.tex = tex;
        textureLoaded = true;
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

        Debug.Log(m_lastFramerate);
    }
}
