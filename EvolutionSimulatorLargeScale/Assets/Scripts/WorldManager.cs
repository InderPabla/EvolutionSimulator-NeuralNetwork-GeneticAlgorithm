using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    public GameObject creaturePrefab;
    public TextMesh tileDataText;

    //private Texture2D tex = null;
    private bool textureLoaded = false;


    private int minCreatureCount = 75;
    private int totalCreatures = 1;

    private List<GameObject> gameArray ;
    private List<Brain> brainArray;

    private const string ACTION_ACTIVATE = "Activate";
    private const string ACTION_SET_MAP = "SetMap";

    //Frame rate variables
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;

    private float sizeX = 150;
    private float sizeY = 150;


    private bool leftMouseDown = false;
    private bool rightMouseDown = false;
    private Vector3 initialMouseLocation;
    private Vector3 initialCamera;

    private Collider2D captureCreatureCollider;
    private Brain captureCreatureBrain;

    private TileMap map;
    int[] brainNetwork = new int[] { 12,15, 8 };

    void Start ()
    {
       
	}

    void Update()
    {
        //CalculateFPS();
        //Debug.Log(m_lastFramerate);
        Debug.Log(brainArray.Count);

        if (Time.timeScale <= 2f)
        {
            
            CameraMovement();
            CaptureCreatureInformation();
        }

       
    }

    public void FixedUpdate()
    {
        map.Apply();
    }

    void CameraMovement()
    {
        Vector3 mouseCoordsScreen = Input.mousePosition;
        Vector3 mouseCoordsWorld = Camera.main.ScreenToWorldPoint(mouseCoordsScreen);

        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDown = true;
            initialMouseLocation = Input.mousePosition;
            initialCamera = Camera.main.transform.position;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightMouseDown = false;
        }

        if (rightMouseDown == true)
        {
            float ratio = (23f/Camera.main.orthographicSize )*25f;
            mouseCoordsScreen = (initialMouseLocation- mouseCoordsScreen) / ratio;
            Vector3 cameraPos = initialCamera + mouseCoordsScreen;
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
   
        
        if (Input.GetMouseButtonUp(0))
        {
            captureCreatureCollider = Physics2D.OverlapCircle(mouseCoordsWorld,2f);
            if (captureCreatureCollider != null)
            {
                captureCreatureBrain = captureCreatureCollider.GetComponent<Creature>().GetBrain();
            }
        }

        TileDataTextPlacement(mouseCoordsWorld);

    }

    private void TileDataTextPlacement(Vector2 mouse)
    {
        if (map.IsValidLocation((int)mouse.x, (int)mouse.y))
        {
            tileDataText.text = map.TileToString((int)mouse.x, (int)mouse.y);
            tileDataText.transform.position = new Vector3((int)mouse.x+0.5f, (int)mouse.y + 0.5f, tileDataText.transform.position.z);
        }
    }

    private void CaptureCreatureInformation()
    {
        if (captureCreatureCollider != null)
        {
            //Debug.Log(captureCreatureBrain.GetOutput()[7]);
            Debug.Log(captureCreatureBrain.GetEnergy());
        }
    }

    void SetTexture(Texture2D tex)
    {
        map = new TileMap(tex, 150,150);


        gameArray = new List<GameObject>();
        brainArray = new List<Brain>();
        for (int i = 0; i < minCreatureCount; i++)
        {
            
            CreateCreature();

            /*gameArray.Add(Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject);
            brainArray.Add(new Brain(brainNetwork, map.GetWorldDeltaTime(), totalCreatures));

            
            gameArray[i].SendMessage(ACTION_ACTIVATE, brainArray[i]);
            gameArray[i].SendMessage(ACTION_SET_MAP, this.map);
            gameArray[i].transform.parent = transform;

            totalCreatures++;*/

        }

        textureLoaded = true;
    }


    public void KillCreature(Brain brain)
    {
        int index = brainArray.IndexOf(brain);
        brainArray.RemoveAt(index);
        gameArray.RemoveAt(index);

        if(brainArray.Count < minCreatureCount)
        {
            CreateCreature();
        }
    }


    public void CreateCreature()
    {
        int[] randomTile = map.RandomFloorTile();
        Vector3 position = new Vector3(randomTile[0] + 0.5f, randomTile[1] + 0.5f, creaturePrefab.transform.position.z);

        gameArray.Add(Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject);
        brainArray.Add(new Brain(brainNetwork, map.GetWorldDeltaTime(), totalCreatures));

        gameArray[gameArray.Count - 1].SendMessage(ACTION_ACTIVATE, brainArray[brainArray.Count - 1]);
        gameArray[gameArray.Count - 1].SendMessage(ACTION_SET_MAP, this.map);
        gameArray[gameArray.Count - 1].transform.parent = transform;

        totalCreatures++;
    }

    public void BirthCreature(Brain parentBrain)
    {
        Vector3 position = gameArray[brainArray.IndexOf(parentBrain)].transform.position; //get position of the parent brain in the world

        Brain childBrain = new Brain(parentBrain, totalCreatures);
        childBrain.Mutate();

        gameArray.Add(Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject);
        brainArray.Add(childBrain);

        gameArray[gameArray.Count - 1].SendMessage(ACTION_ACTIVATE, brainArray[brainArray.Count - 1]);
        gameArray[gameArray.Count - 1].SendMessage(ACTION_SET_MAP, this.map);
        gameArray[gameArray.Count - 1].transform.parent = transform;

        totalCreatures++;
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
