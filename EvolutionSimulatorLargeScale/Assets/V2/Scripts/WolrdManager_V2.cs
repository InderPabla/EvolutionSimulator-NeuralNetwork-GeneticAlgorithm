using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Creature_V2
{
    public Transform trans = null;
    public LineRenderer leftLine = null;
    public LineRenderer rightLine = null;
    public Brain_V2 brain = null;
    public HSBColor bodyColor;
    public Vector3 bodyPos;
    public Vector3 leftPos;
    public Vector3 rightPos;
    public float angle;
    public float energy = 100f;
    public float size = 0.5f;
    public Creature_V2(Transform trans, LineRenderer leftLine, LineRenderer rightLine, Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, float angle)
    {
        this.trans = trans;
        this.leftLine = leftLine;
        this.rightLine = rightLine;
        this.brain = brain;
        this.bodyPos = bodyPos;
        this.bodyColor = bodyColor;
        this.leftPos = leftPos;
        this.rightPos = rightPos;
        this.angle = angle;
    }
}
public class WolrdManager_V2 : MonoBehaviour
{

    public GameObject creaturePrefab;
    public GameObject linePrefab;

    
    private int sizeX = 150;
    private int sizeY = 150;
    private int minCreatureCount = 1000;
    private int totalCreaturesCount = 0;

    private int[] brainNetwork = new int[] { 8, 12, 8 };
    // Output
    // Index 0: forward acceleration
    // Index 1: Turn acceleration
    // Index 2: Body Hue

    private int playSpeed = 1;
    private float worldTime = 0.001f;
    private float year = 0f;
    private bool textureLoaded = false;
    

    private TileMap_V2 map_v2;

    /*List<Transform> creatureTransformList;
    List<Brain_V2> creatureBrainList;
    List<Vector3[]> creatureVectorList;*/
    List<Creature_V2> creatureList;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (textureLoaded == true)
        {
            /*for (int itteration = 0; itteration < playSpeed; itteration++)
            {
                for (int creatureIndex = 0; creatureIndex < minCreatureCount; creatureIndex++)
                {
                    Brain_V2 brain = creatureBrainList[creatureIndex];
                    Vector3[] vs = creatureVectorList[creatureIndex];

                    HSBColor tileColor = map_v2.GetColor((int)vs[0].x,(int)vs[0].y);

                    //HSBColor leftSideColor = map_v2.GetColor( vs[0].x, (int)vs[0].y);

                    float[] output = brain.feedforward(new float[] { tileColor.h, tileColor.s, tileColor.b});

                    float angle = vs[1].z-90f;
                    if (angle > 180)
                        angle = (360f - angle)*-1f;
                    Vector3 newUnit = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad) ,0f);

                    vs[0] += newUnit * output[0] * worldTime * 10f;
                    vs[1] += new Vector3(0, 0, output[1] * worldTime * 100f);

                    map_v2.Eat((int)vs[0].x, (int)vs[0].y);

                }

                year += worldTime;
            }


            for (int creatureIndex = 0; creatureIndex < minCreatureCount; creatureIndex++)
            {
                Brain_V2 brain = creatureBrainList[creatureIndex];
                Vector3[] vs = creatureVectorList[creatureIndex];
                Transform trans = creatureTransformList[creatureIndex];
                float[] output = brain.GetOutput();
                float hue = output[2] < 0 ? (1f + output[2]):output[2];

                trans.GetComponent<Renderer>().material.color = new HSBColor(hue, 1f,1f).ToColor();

                trans.position = vs[0];
                trans.eulerAngles = vs[1];
            }*/


            float creatureCount = creatureList.Count;
            for (int itteration = 0; itteration < playSpeed; itteration++)
            {

                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {
                    Brain_V2 brain = creatureList[creatureIndex].brain;
                    Vector3 bodyPos = creatureList[creatureIndex].bodyPos;
                    Vector3 leftPos = creatureList[creatureIndex].leftPos;
                    Vector3 rightPos = creatureList[creatureIndex].rightPos;
                    float angle = creatureList[creatureIndex].angle;


                   
                    //map_v2.RemoveCreatureFromTileList((int)bodyPos.x, (int)bodyPos.y, creatureIndex);

                    HSBColor bodyTileColor = map_v2.GetColor((int)bodyPos.x, (int)bodyPos.y);
                    HSBColor leftTileColor = map_v2.GetColor((int)leftPos.x, (int)bodyPos.y);
                    HSBColor rightTileColor = map_v2.GetColor((int)rightPos.x, (int)rightPos.y);

                    /*if (leftColl != null)
                    {
                        leftTileColor = HSBColor.FromColor(leftColl.GetComponent<Renderer>().material.color);
                        
                        Debug.Log("COLOR "+leftTileColor);
                    }

                    if (rightColl != null)
                    {
                        rightTileColor = HSBColor.FromColor(rightColl.GetComponent<Renderer>().material.color);
                    }*/

                    

                    float[] output = brain.feedforward(new float[] { bodyTileColor.h, bodyTileColor.s, });



                    /* int bodyTileType = map_v2.GetTileType((int)bodyPos.x, (int)bodyPos.y);
                     float bodyTileEnergy = map_v2.GetTileEnergy((int)bodyPos.x, (int)bodyPos.y) / 100f;

                     int leftTileType = map_v2.GetTileType((int)leftPos.x, (int)leftPos.y);
                     float leftTileEnergy = map_v2.GetTileEnergy((int)leftPos.x, (int)leftPos.y) / 100f;

                     int rightTileType = map_v2.GetTileType((int)rightPos.x, (int)rightPos.y);
                     float rightTileEnergy = map_v2.GetTileEnergy((int)rightPos.x, (int)rightPos.y) / 100f;

                     

                     float[] output = brain.feedforward(new float[] { bodyTileType, bodyTileEnergy, leftTileType, leftTileEnergy, rightTileType, rightTileEnergy });*/

                    //body position calculation
                    float unitAngle = angle - 90f;
                    if (unitAngle > 180)
                        unitAngle = (360f - unitAngle) * -1f;
                    Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);
                    Vector3 displace = newUnit * output[0] * worldTime * 10f;

                    bodyPos += displace;
                    float mag = 1.05f;
                    //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
                    //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);
                    float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad);
                    float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad);

                    //left position calculation
                    leftPos = bodyPos + new Vector3(mag * Mathf.Cos(leftAngle), mag * Mathf.Sin(leftAngle), 0f);

                    //right position calculation
                    rightPos = bodyPos + new Vector3(mag * Mathf.Cos(rightAngle), mag * Mathf.Sin(rightAngle), 0f);

                    //angle calculation
                    angle += output[1] * worldTime * 100f;

                    creatureList[creatureIndex].bodyPos = bodyPos;
                    creatureList[creatureIndex].leftPos = leftPos;
                    creatureList[creatureIndex].rightPos = rightPos;
                    creatureList[creatureIndex].angle = angle;
 
                    map_v2.Eat((int)bodyPos.x, (int)bodyPos.y);


                    
                    /*map_v2.AddCreatureToTileList((int)bodyPos.x, (int)bodyPos.y, creatureIndex);
                    List<List<int>> creatureListOnNearTiles = map_v2.ExistCreatureNearTile((int)bodyPos.x, (int)bodyPos.y);
                    for (int i = 0; i < creatureListOnNearTiles.Count; i++)
                    {
                        List<int> creatureListOnTile = creatureListOnNearTiles[i];
                        for (int j = 0; j < creatureListOnTile.Count; j++)
                        {
                            int index = creatureListOnTile[j];
                            if (index != creatureIndex)
                            {
                                
                            }
                        }
                    }*/


                }

                year += worldTime;
            }


            for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
            {
                Brain_V2 brain = creatureList[creatureIndex].brain;
                float[] output = brain.GetOutput();
                float hue = output[2] < 0 ? (1f + output[2]) : output[2];

                Transform trans = creatureList[creatureIndex].trans;
                trans.GetComponent<Renderer>().material.color = new HSBColor(hue, 1f, 1f).ToColor();
                trans.position = creatureList[creatureIndex].bodyPos;


                Vector3 bodyPos = creatureList[creatureIndex].bodyPos;
                Vector3 leftPos = creatureList[creatureIndex].leftPos;
                Vector3 rightPos = creatureList[creatureIndex].rightPos;
                LineRenderer leftLine = creatureList[creatureIndex].leftLine;
                LineRenderer rightLine = creatureList[creatureIndex].rightLine;
                leftLine.SetPosition(0,bodyPos);
                leftLine.SetPosition(1,leftPos);
                rightLine.SetPosition(0, bodyPos);
                rightLine.SetPosition(1, rightPos);

                trans.eulerAngles = new Vector3(0f,0f, creatureList[creatureIndex].angle);
            }

            map_v2.Apply(playSpeed);
            //Debug.Log(year);
        }
    }

    public void SetTexture(Texture2D tex)
    {
        map_v2 = new TileMap_V2(tex, sizeX, sizeY);

        creatureList = new List<Creature_V2>();
        /*creatureTransformList = new List<Transform>();
        creatureBrainList = new List<Brain_V2>();
        creatureVectorList = new List<Vector3[]>();*/

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
        Brain_V2 brain = new Brain_V2(brainNetwork, worldTime, totalCreaturesCount);

        Creature_V2 creature = new Creature_V2(creatureGameObject.transform, leftLine, rightLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos, 0f);
        creatureList.Add(creature);
        /*GameObject creature = Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject;
        creatureTransformList.Add(creature.transform);
        creatureBrainList.Add(new Brain_V2(brainNetwork, worldTime, totalCreaturesCount));

        Vector3[] vs = new Vector3[] { creature.transform.position, creature.transform.eulerAngles};
        creatureVectorList.Add(vs);*/

        totalCreaturesCount++;
    }
}
