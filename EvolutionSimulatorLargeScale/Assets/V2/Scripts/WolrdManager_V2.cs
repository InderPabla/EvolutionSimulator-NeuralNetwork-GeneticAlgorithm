using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class WolrdManager_V2 : MonoBehaviour
{

    public GameObject creaturePrefab;
    public GameObject linePrefab;
    public TextMesh tileDataText;
    public AncestryTreeMaker ancestryTree;
    public GUINetDraw netDrawer;
    public bool runInBackground = false;

    private int sizeX = 100;
    private int sizeY = 100;
    private int minCreatureCount = 60;
    private int totalCreaturesCount = 0;

    private int[] brainNetwork = new int[] {20, 40, 9};  //16, 16 ,16 ,9
    // Output
    // Index 0: Forward acceleration
    // Index 1: Turn acceleration
    // Index 2: Body Hue
    // Index 3: Mouth Hue
    // Index 4: Eat Food
    // Index 5: Give Birth
    // Index 6: Fight Mode
    // Index 7: Memory Output 1
    // Index 8: Memory Output 2

    // Input (Sensors can detect tile OR creature under the sensor)
    // Index 0: Body Tile Hue
    // Index 1: Body Tile Saturation
    // Index 2: Left Sensor Hue
    // Index 3: Left Sensor Saturation
    // Index 4: Right Sensor Hue
    // Index 5: Right Sensor Saturation
    // Index 6: Body Size
    // Index 7: Memory Input 1
    // Index 8: Memory Input 2

    public int playSpeed = 1;
    public int slowFactor = 1;

    private int slowFactorCounter = 0;
    private int playSpeedVisual = 11;
    private float worldDeltaTime = 0.001f;
    public static float WORLD_CLOCK = 0f;
    private bool textureLoaded = false;

    private TileMap_V2 map_v2;

    private List<Creature_V2> creatureList;

    private bool rightMouseDown;
    private bool leftMouseDown;
    private Vector3 initialMousePosition;
    private Vector3 finalMousePosition;
    private Vector3 initialCameraPosition;
    private int brainCalculations = 0;
    private bool visionState = false;

    // Update is called once per frame
    void Update ()
    {

        if (textureLoaded == true)
        {
            if (slowFactorCounter >= slowFactor)
            {
                for (int itteration = 0; itteration < playSpeed; itteration++)
                {
                    for (int creatureIndex = 0; creatureIndex < creatureList.Count; creatureIndex++)
                    {
                        creatureList[creatureIndex].UpdateCreaturePhysics();
                    }

                    WORLD_CLOCK += worldDeltaTime;
                }


                if (playSpeed < playSpeedVisual)
                {
                    float creatureCount = creatureList.Count;
                    for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                    {

                        creatureList[creatureIndex].UpdateRender(visionState);
                    }
                }

                map_v2.Apply(playSpeed, playSpeed < playSpeedVisual);
                slowFactorCounter = 0;
            }
            slowFactorCounter++;
        }

        CameraMovement();
        GUIStates();
    }

    void GUIStates()
    {
        if (creatureList != null)
        {
            netDrawer.SetWorldDrawInformation(totalCreaturesCount, creatureList.Count, playSpeed>1?playSpeed:(float)playSpeed/(float)slowFactor, brainCalculations);
        }

        if (netDrawer.createButtonState == 2)
        {
            MakeWorld();
        }
        else if (netDrawer.fastButtonState == 2)
        {
            if (slowFactor > 1)
            {
                slowFactor = slowFactor / 2;
                if (slowFactor <= 0)
                {
                    slowFactor = 1;
                }
            }
            else
            {
                playSpeed = playSpeed * 2;
            }

        }
        else if (netDrawer.slowButtonState == 2 || (netDrawer.slowButtonState > 0 && playSpeed > 4))
        {

            if (playSpeed > 1)
            {
                playSpeed = playSpeed / 2;

                if (playSpeed <= 0)
                {
                    playSpeed = 1;
                }
            }
            else
            {
                slowFactor = slowFactor * 2;
            }
        }
        else if (netDrawer.visionButtonState == 2)
        {
            visionState = !visionState;
        }
        else if (netDrawer.saveButtonState == 2)
        {
            SaveWorld();
        }
    }

    private void SaveWorld()
    {
        string filename = "world_snapshot.lses";
        StreamWriter writer = new StreamWriter(filename);

        for (int i = 0; i < creatureList.Count; i++)
        {
            //float neurons[] 
        }

        writer.Close();
    }

    void CameraMovement()
    {
        Vector3 mouseCoordsScreen = Input.mousePosition;
        Vector3 mouseCoordsWorld = Camera.main.ScreenToWorldPoint(mouseCoordsScreen);

        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDown = true;
            initialMousePosition = Input.mousePosition;
            initialCameraPosition = Camera.main.transform.position;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightMouseDown = false;
        }

        if (rightMouseDown == true)
        {
            float ratio = (23f / Camera.main.orthographicSize) * 25f;
            mouseCoordsScreen = (initialMousePosition - mouseCoordsScreen) / ratio;
            Vector3 cameraPos = initialCameraPosition + mouseCoordsScreen;
            cameraPos.z = -111;

            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            Camera.main.orthographicSize -= (Camera.main.orthographicSize/23f)*2f;
            if (Camera.main.orthographicSize < 0.01f)
                Camera.main.orthographicSize = 0.01f;


            Vector3 cameraPos = Camera.main.transform.position;
            Vector2 posChange = Vector2.Lerp(cameraPos,mouseCoordsWorld,0.1f);
            cameraPos = posChange;
            cameraPos.z = -111f;
            Camera.main.transform.position = cameraPos;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += (Camera.main.orthographicSize / 23f) * 3f;
            //Camera.main.orthographicSize += 2f;
        }


        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
            initialMousePosition = mouseCoordsWorld;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
            finalMousePosition = mouseCoordsWorld;

            int x1 = (int)initialMousePosition.x;
            int y1 = (int)initialMousePosition.y;
            int x2 = (int)finalMousePosition.x;
            int y2 = (int)finalMousePosition.y;

            for (int y = y2; y <= y1; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    map_v2.SetSelected(x, y);
                }
            }
        }

        TileDataTextPlacement(mouseCoordsWorld);  

        ButtonActionCheck();
    }

    private void TileDataTextPlacement(Vector2 mouse)
    {
        if (map_v2.IsValidLocation((int)mouse.x, (int)mouse.y))
        {
            tileDataText.text = map_v2.TileToString((int)mouse.x, (int)mouse.y);
            tileDataText.transform.position = new Vector3((int)mouse.x + 0.5f, (int)mouse.y + 0.5f, tileDataText.transform.position.z);
        }
    }

    private void ButtonActionCheck()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            OnDeletePress();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpacePress();
        }
    }

    public void OnDeletePress()
    {
        map_v2.DeleteAllBodiesOnSelected();
    }

    public void OnSpacePress()
    {
        List<Creature_V2> allSelectedCreatures = map_v2.GetAllBodiesOnSelected();
        ancestryTree.ResetAllNodes();
        netDrawer.ResetBrain();

        if (allSelectedCreatures.Count > 0)
        {
            ancestryTree.MakeTree(allSelectedCreatures);
            netDrawer.SetBrain(ancestryTree.GetSelectedCreature().GetBrain(), ancestryTree.GetTreeDataList(),ancestryTree.GetSelectedCreature());
        }
    }

    public void MakeWorld()
    {
        if (textureLoaded == false)
        {
            brainCalculations = new Brain_V2(brainNetwork,-1).GetCalculations();

            creatureList = new List<Creature_V2>();

            for (int i = 0; i < minCreatureCount; i++)
            {
                CreateCreature();
            }

            textureLoaded = true;
        }
    }

    public void SetTexture(Texture2D tex)
    {
        Application.runInBackground = this.runInBackground;
        map_v2 = new TileMap_V2(tex, sizeX, sizeY);
    }

    public void CreateCreature()
    {
        float energy = 1f;
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = new Vector3(randomTile[0] + 0.5f, randomTile[1] + 0.5f, creaturePrefab.transform.position.z);
        Vector3 leftPos = /*bodyPosition + new Vector3(-0.46f/2f, 0.95f / 2f, 0f);*/ Vector3.zero;
        Vector3 rightPos = /*bodyPosition + new Vector3(0.46f / 2f, 0.95f / 2f, 0f);*/ Vector3.zero;
        GameObject creatureGameObject = Instantiate(creaturePrefab, bodyPosition, creaturePrefab.transform.rotation) as GameObject;
        GameObject leftLineGameObject = Instantiate(linePrefab) as GameObject;
        GameObject rightLineGameObject = Instantiate(linePrefab) as GameObject;
        leftLineGameObject.transform.parent = creatureGameObject.transform;
        rightLineGameObject.transform.parent = creatureGameObject.transform;

        LineRenderer leftLine = leftLineGameObject.GetComponent<LineRenderer>();
        LineRenderer rightLine = rightLineGameObject.GetComponent<LineRenderer>();
        leftLine.SetWidth(0.02f,0.02f);
        rightLine.SetWidth(0.02f, 0.02f);

        GameObject spikeLineGameObject = Instantiate(linePrefab) as GameObject;
        spikeLineGameObject.transform.parent = creatureGameObject.transform;
        LineRenderer spikeLine = spikeLineGameObject.GetComponent<LineRenderer>();
        spikeLine.SetWidth(0.02f, 0.02f);

        LineRenderer[] lineSensor = new LineRenderer[4];
        for (int i = 0; i < lineSensor.Length; i++)
        {
            GameObject newLine = Instantiate(linePrefab) as GameObject;
            newLine.transform.parent = creatureGameObject.transform;
            lineSensor[i] = newLine.GetComponent<LineRenderer>();
            lineSensor[i].SetWidth(0.02f, 0.02f);
        }

        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount);
        creatureGameObject.transform.GetChild(1).GetComponent<TextMesh>().text = brain.GetName();

        Creature_V2 creature = new Creature_V2(totalCreaturesCount,0,creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos,0.5f, UnityEngine.Random.Range(0f,360f), worldDeltaTime, creatureGameObject.transform.localScale.x/2f, energy, map_v2, this);
        creatureList.Add(creature);
        totalCreaturesCount++;
    }

    public void CreateCreature(Creature_V2 parent)
    {
        float energy = 1f;
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = parent.position - (parent.trans.up * 2f * parent.GetRadius());
        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;
        GameObject creatureGameObject = Instantiate(creaturePrefab, bodyPosition, creaturePrefab.transform.rotation) as GameObject;
        GameObject leftLineGameObject = Instantiate(linePrefab) as GameObject;
        GameObject rightLineGameObject = Instantiate(linePrefab) as GameObject;
        leftLineGameObject.transform.parent = creatureGameObject.transform;
        rightLineGameObject.transform.parent = creatureGameObject.transform;

        LineRenderer leftLine = leftLineGameObject.GetComponent<LineRenderer>();
        LineRenderer rightLine = rightLineGameObject.GetComponent<LineRenderer>();
        leftLine.SetWidth(0.02f, 0.02f);
        rightLine.SetWidth(0.02f, 0.02f);

        LineRenderer[] lineSensor = new LineRenderer[4];
        for (int i = 0; i < lineSensor.Length; i++) {
            GameObject newLine = Instantiate(linePrefab) as GameObject;
            newLine.transform.parent = creatureGameObject.transform;
            lineSensor[i] = newLine.GetComponent<LineRenderer>();
            lineSensor[i].SetWidth(0.02f, 0.02f);
        }

        GameObject spikeLineGameObject = Instantiate(linePrefab) as GameObject;
        spikeLineGameObject.transform.parent = creatureGameObject.transform;
        LineRenderer spikeLine = spikeLineGameObject.GetComponent<LineRenderer>();
        spikeLine.SetWidth(0.02f, 0.02f);


        Brain_V2 brain = new Brain_V2(parent.GetBrain(), totalCreaturesCount);
        brain.Mutate();
        creatureGameObject.transform.GetChild(1).GetComponent<TextMesh>().text = brain.GetName();

        Creature_V2 creature = new Creature_V2(totalCreaturesCount, parent.GetGeneration()+1,creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f, 0f, 0f), bodyPosition, leftPos, rightPos, 0.5f, UnityEngine.Random.Range(0f, 360f), worldDeltaTime, creatureGameObject.transform.localScale.x / 2f, energy, map_v2, this);
        creatureList.Add(creature);
        totalCreaturesCount++;

        parent.AddChildren(creature);
    }

    public void RemoveCreature(Creature_V2 creature)
    {
        creatureList.Remove(creature);
        if (creatureList.Count < minCreatureCount)
        {
            CreateCreature();
        }
    }
}
