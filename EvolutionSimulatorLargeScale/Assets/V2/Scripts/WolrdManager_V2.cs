using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolrdManager_V2 : MonoBehaviour
{

    public GameObject creaturePrefab;


    
    private int sizeX = 150;
    private int sizeY = 150;
    private int minCreatureCount = 100;
    private int totalCreaturesCount = 0;
    private int[] brainNetwork = new int[] { 8, 12, 8 };
    private int playSpeed = 1;
    private float worldTime = 0.001f;
    private float year = 0f;
    private bool textureLoaded = false;
    

    private TileMap_V2 map_v2;
    List<Transform> creatureTransformList;
    List<Brain_V2> creatureBrainList;
    List<Vector3[]> creatureVectorList;
    

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (textureLoaded == true)
        {
            for (int itteration = 0; itteration < playSpeed; itteration++)
            {
                for (int creatureIndex = 0; creatureIndex < minCreatureCount; creatureIndex++)
                {
                    Brain_V2 brain = creatureBrainList[creatureIndex];
                    Vector3[] vs = creatureVectorList[creatureIndex];

                    HSBColor tileColor = map_v2.GetColor((int)vs[0].x,(int)vs[0].y);

                    float[] output = brain.feedforward(new float[] { tileColor.h, tileColor.s, tileColor.b});
                    

                    Transform trans = creatureTransformList[creatureIndex];

                    //Vector3 unit = new Vector3((Mathf.Cos(vs[1].z+90f) * Mathf.Deg2Rad), Mathf.Sin((vs[1].z + 90f) * Mathf.Deg2Rad) ,0f);

                    vs[0] += trans.up * output[0] * worldTime * 10f;
                    vs[1] += new Vector3(0, 0, output[1] * worldTime * 100f);

                    map_v2.Eat((int)vs[0].x, (int)vs[0].y);
                    /*Transform trans = creatureTransformList[creatureIndex];
                    
                    trans.position += trans.up * output[0] * worldTime;
                    trans.eulerAngles += new Vector3(0,0, output[1] * worldTime);*/
                }

                year += worldTime;
            }


            for (int creatureIndex = 0; creatureIndex < minCreatureCount; creatureIndex++)
            {
                Vector3[] vs = creatureVectorList[creatureIndex];
                Transform trans = creatureTransformList[creatureIndex];

                trans.position = vs[0];
                trans.eulerAngles = vs[1];
            }
            map_v2.Apply(playSpeed);
            Debug.Log(year);
        }
    }

    public void SetTexture(Texture2D tex) {
        map_v2 = new TileMap_V2(tex, sizeX, sizeY);
        //map_v2.Apply();

        creatureTransformList = new List<Transform>();
        creatureBrainList = new List<Brain_V2>();
        creatureVectorList = new List<Vector3[]>();

        for (int i = 0; i < minCreatureCount; i++) {
            CreateCreature();
        }

        textureLoaded = true;
    }

    public void CreateCreature()
    {
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 position = new Vector3(randomTile[0] + 0.5f, randomTile[1] + 0.5f, creaturePrefab.transform.position.z);

        GameObject creature = Instantiate(creaturePrefab, position, creaturePrefab.transform.rotation) as GameObject;
        creatureTransformList.Add(creature.transform);
        creatureBrainList.Add(new Brain_V2(brainNetwork, worldTime, totalCreaturesCount));

        Vector3[] vs = new Vector3[] { creature.transform.position, creature.transform.eulerAngles};
        creatureVectorList.Add(vs);

        totalCreaturesCount++;
    }
}
