using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class WolrdManager_V2 : MonoBehaviour
{

    public GameObject creaturePrefab;
    public GameObject linePrefab;
    public TextMesh tileDataText;

    private int sizeX = 100;
    private int sizeY = 100;
    private int minCreatureCount = 100;
    private int totalCreaturesCount = 0;

    private int[] brainNetwork = new int[] { 9, 12, 9 };
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

    private int playSpeed = 3;
    private int playSpeedVisual = 200;
    private float worldDeltaTime = 0.001f;
    private float year = 0f;
    private bool textureLoaded = false;
    

    private TileMap_V2 map_v2;

    private List<Creature_V2> creatureList;


    private bool rightMouseDown;
    private Vector3 initialMousePosition;
    private Vector3 initialCameraPosition;

    // Update is called once per frame
    void Update ()
    {
        if (textureLoaded == true)
        {

            //float creatureCount = creatureList.Count;
            for (int itteration = 0; itteration < playSpeed; itteration++)
            {
                for (int creatureIndex = 0; creatureIndex < creatureList.Count; creatureIndex++)
                {
                    creatureList[creatureIndex].UpdateCreaturePhysics();
                }

                year += worldDeltaTime;
            }


            if (playSpeed < playSpeedVisual)
            {
                float creatureCount = creatureList.Count;
                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {

                    creatureList[creatureIndex].UpdateRender();
                }
            }


            CameraMovement();
            map_v2.Apply(playSpeed, playSpeed < playSpeedVisual);
            Debug.Log(year);
        }
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
            if (Camera.main.orthographicSize < 1f)
                Camera.main.orthographicSize = 1f;


            Vector3 cameraPos = Camera.main.transform.position;
            Vector2 posChange = Vector2.Lerp(cameraPos,mouseCoordsWorld,0.1f);
            cameraPos = posChange;
            cameraPos.z = -111f;
            Camera.main.transform.position = cameraPos;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += 1f;
        }

        TileDataTextPlacement(mouseCoordsWorld);
    }

    private void TileDataTextPlacement(Vector2 mouse)
    {
        if (map_v2.IsValidLocation((int)mouse.x, (int)mouse.y))
        {
            tileDataText.text = map_v2.TileToString((int)mouse.x, (int)mouse.y);
            tileDataText.transform.position = new Vector3((int)mouse.x + 0.5f, (int)mouse.y + 0.5f, tileDataText.transform.position.z);
        }
    }

    public void SetTexture(Texture2D tex)
    {
        map_v2 = new TileMap_V2(tex, sizeX, sizeY);

        creatureList = new List<Creature_V2>();

        for (int i = 0; i < minCreatureCount; i++)
        {
            CreateCreature();
        }

        textureLoaded = true;
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
        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount);
        Creature_V2 creature = new Creature_V2(totalCreaturesCount,creatureGameObject.transform, leftLine, rightLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos,0.5f, UnityEngine.Random.Range(0f,360f), worldDeltaTime, creatureGameObject.transform.localScale.x/2f, energy, map_v2, this);
        creatureList.Add(creature);
        totalCreaturesCount++;
    }

    public void CreateCreature(Creature_V2 parent)
    {
        float energy = 1f;
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = parent.position;
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
        Brain_V2 brain = new Brain_V2(parent.GetBrain(), totalCreaturesCount);
        brain.Mutate();
        Creature_V2 creature = new Creature_V2(totalCreaturesCount, creatureGameObject.transform, leftLine, rightLine, brain, new HSBColor(1f, 0f, 0f), bodyPosition, leftPos, rightPos, 0.5f, UnityEngine.Random.Range(0f, 360f), worldDeltaTime, creatureGameObject.transform.localScale.x / 2f, energy, map_v2, this);
        creatureList.Add(creature);
        totalCreaturesCount++;
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
