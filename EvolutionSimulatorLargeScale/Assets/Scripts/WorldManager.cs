using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {

    public GameObject creaturePrefab;

    private Texture2D tex = null;
    private bool textureLoaded = false;
  

    private int creatureCount = 100;
    private GameObject[] gameArray ;
    private Brain[] brainArray;

    private const string ACTION_ACTIVATE = "Activate";
    private const string ACTION_SET_TEXTURE = "SetTexture";

    //Frame rate variables
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;

    private float sizeX = 150;
    private float sizeY = 150;


    private bool mouseDown = false;
    private Vector3 initialMouseLocation;
    private Vector3 initialCamera;

    TileMap map;
    void Start ()
    {
       
	}

    void Update()
    {
       CalculateFPS();
       Debug.Log(m_lastFramerate);


        CameraMovement();
        tex.Apply();
    }


    void CameraMovement()
    {
        Vector3 mouseCoords = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            initialMouseLocation = Input.mousePosition;
            initialCamera = Camera.main.transform.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

        if (mouseDown == true)
        {
            float ratio = (23f/Camera.main.orthographicSize )*25f;
            mouseCoords = (initialMouseLocation- mouseCoords)/ ratio;
            Vector3 cameraPos = initialCamera + mouseCoords;
            cameraPos.z = -111;

            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            Camera.main.orthographicSize -= 1f;
            if (Camera.main.orthographicSize < 2f)
                Camera.main.orthographicSize = 2f;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += 1f;
        }
    }

    void FixedUpdate ()
    {
        /*if (textureLoaded == true)
        {
            
            Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
            }
        }*/
        

    }

    void SetTexture(Texture2D tex)
    {
        this.tex = tex;
        map = new TileMap(this.tex.GetPixels(0,0,150,150),150,150);


        gameArray = new GameObject[creatureCount];
        brainArray = new Brain[creatureCount];
        for (int i = 0; i < creatureCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(0f, sizeX), Random.Range(0f, sizeY), creaturePrefab.transform.position.z);
            gameArray[i] = Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject;
            brainArray[i] = new Brain(new int[] { 12, 12, 12, 12 });
            gameArray[i].SendMessage(ACTION_ACTIVATE, brainArray[i]);
            gameArray[i].SendMessage(ACTION_SET_TEXTURE, this.tex);
        }

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

        
    }
}
