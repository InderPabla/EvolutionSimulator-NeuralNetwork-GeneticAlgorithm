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

    private int playSpeed = 128;
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
                    Brain_V2 brain = creatureList[creatureIndex].brain;
                    Vector3 bodyPos = creatureList[creatureIndex].bodyPos;
                    Vector3 leftPos = creatureList[creatureIndex].leftPos;
                    Vector3 rightPos = creatureList[creatureIndex].rightPos;
                    int[] tileDetail = creatureList[creatureIndex].tileDetail;
                    float angle = creatureList[creatureIndex].angle;
                    float bodyRadius = creatureList[creatureIndex].currentRadius;
                    float[] previousOutput = creatureList[creatureIndex].brain.GetOutput();

                    HSBColor bodyTileColor = map_v2.GetColor((int)bodyPos.x, (int)bodyPos.y);
                    HSBColor leftTileColor = map_v2.GetColor((int)leftPos.x, (int)bodyPos.y);
                    HSBColor rightTileColor = map_v2.GetColor((int)rightPos.x, (int)rightPos.y);

                    List<Creature_V2> creatureListAtLeftTile = map_v2.ExistCreatureAtTile((int)leftPos.x, (int)leftPos.y);
                    List<Creature_V2> creatureListAtRightTile = map_v2.ExistCreatureAtTile((int)rightPos.x, (int)rightPos.y);

                    //check right sensor collsision
                    if (creatureListAtLeftTile != null)
                    {
                        for (int i = 0; i < creatureListAtLeftTile.Count; i++)
                        {
                            Creature_V2 creature = creatureListAtLeftTile[i];
                            if (!creature.Equals(creatureList[creatureIndex]))
                            {
                                Vector3 bodyPosOfCreature = creature.bodyPos;
                                float creatureSizeSqr = Mathf.Pow(creature.currentRadius, 2f);
                                float distanceBetweenPointAndCircle = Mathf.Pow((bodyPosOfCreature.x - leftPos.x), 2f) + Mathf.Pow((bodyPosOfCreature.y - leftPos.y), 2f);

                                //inter section occoures
                                if (distanceBetweenPointAndCircle <= creatureSizeSqr)
                                {
                                    leftTileColor = creature.bodyColor;
                                }
                                break;
                            }
                        }
                    }

                    //check for left sensor collision
                    if (creatureListAtRightTile != null)
                    {
                        for (int i = 0; i < creatureListAtRightTile.Count; i++)
                        {
                            Creature_V2 creature = creatureListAtRightTile[i];
                            if (!creature.Equals(creatureList[creatureIndex]))
                            {
                                Vector3 bodyPosOfCreature = creature.bodyPos;
                                float creatureSizeSqr = Mathf.Pow(creature.currentRadius, 2f);
                                float distanceBetweenPointAndCircle = Mathf.Pow((bodyPosOfCreature.x - rightPos.x), 2f) + Mathf.Pow((bodyPosOfCreature.y - rightPos.y), 2f);

                                //inter section occoures
                                if (distanceBetweenPointAndCircle <= creatureSizeSqr)
                                {
                                    rightTileColor = creature.bodyColor;
                                }
                                break;
                            }
                        }
                    }

                    float[] output = brain.feedforward(new float[] {bodyTileColor.s, bodyTileColor.h, leftTileColor.h, leftTileColor.s, rightTileColor.h, rightTileColor.s, bodyRadius, previousOutput[7], previousOutput[8] });

                    float forwardAccel = output[0];
                    float rotationalAccel = output[1];
                    float bodyHue = output[2];
                    float mouthHue = output[3];
                    float eatFood = output[4];
                    float giveBrith = output[5];
                    float fight = output[6];
                    HSBColor bodyColor = new HSBColor(bodyHue,1f,1f);
                    HSBColor mouthColor = new HSBColor(mouthHue, 1f, 1f);

                    map_v2.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], creatureList[creatureIndex]);

                    //body position calculation
                    float unitAngle = angle - 90f;
                    if (unitAngle > 180)
                        unitAngle = (360f - unitAngle) * -1f;
                    Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);
                    Vector3 displace = newUnit * forwardAccel * worldDeltaTime * 10f;

                    bodyPos += displace;
                    float mag = 1.05f;
                    //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
                    //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);
                    float leftAngle = (((angle + 90f) + 25f) * Mathf.Deg2Rad);
                    float rightAngle = (((angle + 90f) - 25f) * Mathf.Deg2Rad);

                    //left position calculation
                    leftPos = bodyPos + new Vector3(mag * Mathf.Cos(leftAngle), mag * Mathf.Sin(leftAngle), 0f);

                    //right position calculation
                    rightPos = bodyPos + new Vector3(mag * Mathf.Cos(rightAngle), mag * Mathf.Sin(rightAngle), 0f);

                    //angle calculation
                    angle += rotationalAccel * worldDeltaTime * 100f;

                    //copy tile detail
                    tileDetail[0] = (int)bodyPos.x;
                    tileDetail[1] = (int)bodyPos.y;

                    creatureList[creatureIndex].bodyPos = bodyPos;
                    creatureList[creatureIndex].leftPos = leftPos;
                    creatureList[creatureIndex].rightPos = rightPos;
                    creatureList[creatureIndex].angle = angle;
                    creatureList[creatureIndex].tileDetail = tileDetail;
                    creatureList[creatureIndex].mouthColor = mouthColor;
                    creatureList[creatureIndex].bodyColor = bodyColor;

                    map_v2.Eat((int)bodyPos.x, (int)bodyPos.y);

                    map_v2.AddCreatureToTileList(tileDetail[0], tileDetail[1], creatureList[creatureIndex]);
                }

                year += worldDeltaTime;
            }


            if (playSpeed < playSpeedVisual)
            {
                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {
                    HSBColor bodyColor = creatureList[creatureIndex].bodyColor;
                    Vector3 bodyPos = creatureList[creatureIndex].bodyPos;
                    Vector3 leftPos = creatureList[creatureIndex].leftPos;
                    Vector3 rightPos = creatureList[creatureIndex].rightPos;

                    LineRenderer leftLine = creatureList[creatureIndex].leftLine;
                    LineRenderer rightLine = creatureList[creatureIndex].rightLine;
                    leftLine.SetPosition(0, bodyPos);
                    leftLine.SetPosition(1, leftPos);
                    rightLine.SetPosition(0, bodyPos);
                    rightLine.SetPosition(1, rightPos);

                    Transform trans = creatureList[creatureIndex].trans;
                    trans.GetComponent<Renderer>().material.color = bodyColor.ToColor();
                    trans.position = creatureList[creatureIndex].bodyPos;
                    trans.eulerAngles = new Vector3(0f, 0f, creatureList[creatureIndex].angle);
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
        Vector3 leftPos = bodyPosition + new Vector3(-0.46f, 0.95f, 0f);
        Vector3 rightPos = bodyPosition + new Vector3(0.46f, 0.95f, 0f);
        GameObject creatureGameObject = Instantiate(creaturePrefab, bodyPosition, creaturePrefab.transform.rotation) as GameObject;
        GameObject leftLineGameObject = Instantiate(linePrefab) as GameObject;
        GameObject rightLineGameObject = Instantiate(linePrefab) as GameObject;
        LineRenderer leftLine = leftLineGameObject.GetComponent<LineRenderer>();
        LineRenderer rightLine = rightLineGameObject.GetComponent<LineRenderer>();
        leftLine.SetWidth(0.05f,0.05f);
        rightLine.SetWidth(0.05f, 0.05f);
        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount);
        Creature_V2 creature = new Creature_V2(totalCreaturesCount,creatureGameObject.transform, leftLine, rightLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos, 0f, worldDeltaTime, creatureGameObject.transform.localScale.x/2f, 100f);
        creatureList.Add(creature);


        totalCreaturesCount++;
    }
}
