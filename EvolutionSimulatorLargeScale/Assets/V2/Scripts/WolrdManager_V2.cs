using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class WolrdManager_V2 : MonoBehaviour
{

    public GameObject creaturePrefab;
    public GameObject linePrefab;

    private int sizeX = 150;
    private int sizeY = 150;
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

    private int playSpeed = 1;
    private int playSpeedVisual = 200;
    private float worldDeltaTime = 0.001f;
    private float year = 0f;
    private bool textureLoaded = false;
    

    private TileMap_V2 map_v2;

    private List<Creature_V2> creatureList;
	
	// Update is called once per frame
	void Update ()
    {
        if (textureLoaded == true)
        {

            float creatureCount = creatureList.Count;
            for (int itteration = 0; itteration < playSpeed; itteration++)
            {
                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {
                    creatureList[creatureIndex].UpdateCreaturePhysics();
                }

                year += worldDeltaTime;
            }


            if (playSpeed < playSpeedVisual)
            {
                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {

                    creatureList[creatureIndex].UpdateRender();
                }
            }

            map_v2.Apply(playSpeed, playSpeed < playSpeedVisual);
            Debug.Log(year);
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
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = new Vector3(randomTile[0] + 0.5f, randomTile[1] + 0.5f, creaturePrefab.transform.position.z);
        Vector3 leftPos = /*bodyPosition + new Vector3(-0.46f/2f, 0.95f / 2f, 0f);*/ Vector3.zero;
        Vector3 rightPos = /*bodyPosition + new Vector3(0.46f / 2f, 0.95f / 2f, 0f);*/ Vector3.zero;
        GameObject creatureGameObject = Instantiate(creaturePrefab, bodyPosition, creaturePrefab.transform.rotation) as GameObject;
        GameObject leftLineGameObject = Instantiate(linePrefab) as GameObject;
        GameObject rightLineGameObject = Instantiate(linePrefab) as GameObject;
        LineRenderer leftLine = leftLineGameObject.GetComponent<LineRenderer>();
        LineRenderer rightLine = rightLineGameObject.GetComponent<LineRenderer>();
        leftLine.SetWidth(0.02f,0.02f);
        rightLine.SetWidth(0.02f, 0.02f);
        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount);
        Creature_V2 creature = new Creature_V2(totalCreaturesCount,creatureGameObject.transform, leftLine, rightLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos,0.5f, 0f, worldDeltaTime, creatureGameObject.transform.localScale.x/2f, 1f,map_v2);
        creatureList.Add(creature);
        totalCreaturesCount++;
    }
}
