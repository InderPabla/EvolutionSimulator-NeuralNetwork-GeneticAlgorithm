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

    private int[] brainNetwork;  
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

    private int mutationNumber = 1000;
    private int mutationSign = 1;
    private int mutationRandom = 1;
    private int mutationIncrease = 1;
    private int mutationDecrease = 1;
    private int mutationWeakerParentFactor = 1;

    private float climate = 10;
    private float minLife = 5;
    private float lifeDecrease = 0.7f; 

    int inputNeurons = 20;
    int outputNeurons = 9;

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
        else if (netDrawer.loadButtonState == 2)
        {
            LoadWorld();
        }
    }

    private void SaveWorld()
    {
        string filename = "world_snapshot.lses";

        StreamWriter writer = new StreamWriter(filename);

        writer.Write(brainNetwork.Length+" ");
        for (int i = 0; i < brainNetwork.Length; i++)
        {
            writer.Write(brainNetwork[i]+" "); 
        }

        writer.Write(creatureList.Count + " "); 
        for (int i = 0; i < creatureList.Count; i++)
        {
            Creature_V2 creature = creatureList[i];
            Brain_V2 brain = creature.GetBrain();
            //float[][] neurons = brain.GetNeurons();
            float[][][] connections = brain.GetWeights();

            writer.Write(creature.GetName() + " " 
                + creature.GetParentNames() + " " 
                + creature.GetEnergy() + " "
                + creature.GetLife() + " "
                + creature.position.x + " "
                + creature.position.y + " "
                + creature.rotation + " "
                + creature.veloForward + " "
                + creature.veloAngular + " ");

            for (int j = 0; j < connections.Length; j++)
            {
                for (int k = 0; k < connections[j].Length; k++)
                {
                    for (int l = 0; l < connections[j][k].Length; l++)
                    {
                        writer.Write(connections[j][k][l] + " ");
                    }  
                }
            }

        }

        writer.Close();
    }


    private void LoadWorld()
    {
        if (textureLoaded == false)
        {
            textureLoaded = true;
            string filename = "world_snapshot.lses";  
            StreamReader reader = new StreamReader(filename);
            creatureList = new List<Creature_V2>();

            string[] readAll = reader.ReadToEnd().Split(' ');
            int actualLength = readAll.Length - 1;
            int index = 0;

            //make brain network
            int brainLength = int.Parse(readAll[0]);
            brainNetwork = new int[brainLength];
            index++;

            for (int i = 0; i < brainLength; i++, index++)
            {
                brainNetwork[i] = int.Parse(readAll[index]);
            }
            brainCalculations = new Brain_V2(brainNetwork, -1,0,0,0,0,0,0).GetCalculations();

            int numberOfCreatures = int.Parse(readAll[index]);
            index++;

            for (int creatureIndex = 0; creatureIndex < numberOfCreatures; creatureIndex++)
            {
                string name = readAll[index];index++;
                string parnetNames = readAll[index];index++;
                float energy = float.Parse(readAll[index]); index++;
                float life = float.Parse(readAll[index]); index++;
                Vector2 position = new Vector2(float.Parse(readAll[index]), float.Parse(readAll[index+1])); index += 2;
                float rotation = float.Parse(readAll[index]); index++;
                float veloForward = float.Parse(readAll[index]); index++;
                float veloAngular = float.Parse(readAll[index]); index++;
                float[][][] weights;

                //Weights Initilization
                List<float[][]> weightsList = new List<float[][]>();

                for (int i = 1; i < brainNetwork.Length; i++)
                {
                    List<float[]> layerWeightsList = new List<float[]>(); //layer weights list

                    int neuronsInPreviousLayer = brainNetwork[i - 1];

                    for (int j = 0; j < brainNetwork[i]; j++)
                    {
                        float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                        for (int k = 0; k < neuronsInPreviousLayer; k++)
                        {
                            neuronWeights[k] = float.Parse(readAll[index]); index++;
                        }

                        layerWeightsList.Add(neuronWeights);
                    }
                    weightsList.Add(layerWeightsList.ToArray());
                }

                weights = weightsList.ToArray(); //convert list to array

                CreateCreature(energy, life, veloForward, veloAngular, name, parnetNames, position, rotation, weights);
            }

            reader.Close();
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
            brainCalculations = new Brain_V2(brainNetwork,-1,0,0,0,0,0,0).GetCalculations();

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

        string filename = "world_hyper_parameters.txt";
        StreamReader reader = new StreamReader(filename);

        string[] neuralLayerLine = reader.ReadLine().Split(' ');
        string[] minCreatureCountLine = reader.ReadLine().Split(' ');
        string[] maxVisualSpeedLine = reader.ReadLine().Split(' ');
        string[] climateLine = reader.ReadLine().Split(' ');
        string[] minLifeLine = reader.ReadLine().Split(' ');
        string[] lifeDecreaseLine = reader.ReadLine().Split(' ');
        string[] worldDeltaTimeLine = reader.ReadLine().Split(' ');
        string[] mutationNumberLine = reader.ReadLine().Split(' ');
        string[] mutationWeakerParentFactorLine = reader.ReadLine().Split(' ');
        string[] mutationSignLine = reader.ReadLine().Split(' ');
        string[] mutationRandomLine = reader.ReadLine().Split(' ');
        string[] mutationIncreaseLine = reader.ReadLine().Split(' ');
        string[] mutationDecreaseLine = reader.ReadLine().Split(' ');

        //make brain network 
        int layerLength = (neuralLayerLine.Length - 1);
        brainNetwork = new int[layerLength + 2];
        brainNetwork[0] = inputNeurons;
        brainNetwork[brainNetwork.Length - 1] = outputNeurons;
        for (int i = 1; i < neuralLayerLine.Length; i++)
        {
            brainNetwork[i] = int.Parse(neuralLayerLine[i]);
        }

        minCreatureCount = int.Parse(minCreatureCountLine[1]);
        playSpeedVisual = int.Parse(maxVisualSpeedLine[1]);
        worldDeltaTime = float.Parse(worldDeltaTimeLine[1]);
        mutationNumber = int.Parse(mutationNumberLine[1]);
        mutationWeakerParentFactor = int.Parse(mutationWeakerParentFactorLine[1]);
        mutationSign = int.Parse(mutationSignLine[1]);
        mutationRandom = int.Parse(mutationRandomLine[1]);
        mutationIncrease = int.Parse(mutationIncreaseLine[1]);
        mutationDecrease = int.Parse(mutationDecreaseLine[1]);

        climate = float.Parse(climateLine[1]);
        minLife = float.Parse(minLifeLine[1]);
        lifeDecrease = float.Parse(lifeDecreaseLine[1]);

        reader.Close();


        map_v2 = new TileMap_V2(tex, sizeX, sizeY, climate, worldDeltaTime);
    }

    public void CreateCreature()
    {
        float energy = 1f;
        float life = 1f;
        float veloForward = 0f;
        float veloAngular = 0f;

        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = new Vector3(randomTile[0] + 0.5f, randomTile[1] + 0.5f, creaturePrefab.transform.position.z);
        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;
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

        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount,mutationNumber,mutationSign,mutationRandom,mutationIncrease,mutationDecrease,mutationWeakerParentFactor);
        creatureGameObject.transform.GetChild(1).GetComponent<TextMesh>().text = brain.GetName();

        Creature_V2 creature = new Creature_V2(totalCreaturesCount,0,creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f,0f,0f), bodyPosition, leftPos, rightPos,0.5f, UnityEngine.Random.Range(0f,360f), worldDeltaTime, creatureGameObject.transform.localScale.x/2f, energy,energy,life, minLife, lifeDecrease,veloForward,veloAngular, map_v2, this, "WORLD");
        creatureList.Add(creature);
        totalCreaturesCount++;
    }

    public void CreateCreature(Creature_V2 parent)
    {
        float energy = 1f;
        float life = 1f;
        float veloForward = 0f;
        float veloAngular = 0f;
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = parent.position - (parent.trans.up * 2f * parent.GetRadius());
        bodyPosition.z = creaturePrefab.transform.position.z;
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

        Creature_V2 creature = new Creature_V2(totalCreaturesCount, parent.GetGeneration()+1,creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f, 0f, 0f), bodyPosition, leftPos, rightPos, 0.5f, UnityEngine.Random.Range(0f, 360f), worldDeltaTime, creatureGameObject.transform.localScale.x / 2f, energy, energy, life, minLife, lifeDecrease, veloForward, veloAngular, map_v2, this, parent.GetName());
        creatureList.Add(creature);
        totalCreaturesCount++;

        parent.AddChildren(creature);
    }

    public void CreateCreature(Creature_V2 parent1, Creature_V2 parent2)
    {
        float energy = 1f;
        float life = 1f;
        float veloForward = 0f;
        float veloAngular = 0f;
        int[] randomTile = map_v2.RandomFloorTile();
        Vector3 bodyPosition = parent1.position - (parent1.trans.up * 2f * parent1.GetRadius());
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
        for (int i = 0; i < lineSensor.Length; i++)
        {
            GameObject newLine = Instantiate(linePrefab) as GameObject;
            newLine.transform.parent = creatureGameObject.transform;
            lineSensor[i] = newLine.GetComponent<LineRenderer>();
            lineSensor[i].SetWidth(0.02f, 0.02f);
        }

        GameObject spikeLineGameObject = Instantiate(linePrefab) as GameObject;
        spikeLineGameObject.transform.parent = creatureGameObject.transform;
        LineRenderer spikeLine = spikeLineGameObject.GetComponent<LineRenderer>();
        spikeLine.SetWidth(0.02f, 0.02f);

        Creature_V2 strongerParent = parent1.GetEnergy() > parent2.GetEnergy() ? parent1 : parent2;
        Creature_V2 weakerParent = parent1.GetEnergy() > parent2.GetEnergy() ? parent2 : parent1;

        Brain_V2 brain = new Brain_V2(strongerParent.GetBrain(), totalCreaturesCount);
        brain.Mutate(strongerParent.GetBrain().GetWeights(), weakerParent.GetEnergy()/strongerParent.GetEnergy());
        creatureGameObject.transform.GetChild(1).GetComponent<TextMesh>().text = brain.GetName();

        string parentNames = strongerParent.GetName() + "@" + weakerParent.GetName();

        Creature_V2 creature = new Creature_V2(totalCreaturesCount, strongerParent.GetGeneration() + 1, creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f, 0f, 0f), bodyPosition, leftPos, rightPos, 0.5f, UnityEngine.Random.Range(0f, 360f), worldDeltaTime, creatureGameObject.transform.localScale.x / 2f, energy, energy, life, minLife, lifeDecrease, veloForward,veloAngular, map_v2, this, parentNames);
        creatureList.Add(creature);
        totalCreaturesCount++;

        parent1.AddChildren(creature);
        parent2.AddChildren(creature);
    }


    public void CreateCreature(float currentEnergy, float life, float veloForward, float veloAngular, string name, string parentNames, Vector3 bodyPosition, float angle, float[][][] weights)
    {
        float energy = 1f;

        int[] randomTile = map_v2.RandomFloorTile();

        bodyPosition.z = creaturePrefab.transform.position.z;

        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos =  Vector3.zero;
        GameObject creatureGameObject = Instantiate(creaturePrefab, bodyPosition, creaturePrefab.transform.rotation) as GameObject;
        GameObject leftLineGameObject = Instantiate(linePrefab) as GameObject;
        GameObject rightLineGameObject = Instantiate(linePrefab) as GameObject;
        leftLineGameObject.transform.parent = creatureGameObject.transform;
        rightLineGameObject.transform.parent = creatureGameObject.transform;

        LineRenderer leftLine = leftLineGameObject.GetComponent<LineRenderer>();
        LineRenderer rightLine = rightLineGameObject.GetComponent<LineRenderer>();
        leftLine.SetWidth(0.02f, 0.02f);
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

        Brain_V2 brain = new Brain_V2(brainNetwork, totalCreaturesCount, weights, name, mutationNumber, mutationSign, mutationRandom, mutationIncrease, mutationDecrease, mutationWeakerParentFactor);
        creatureGameObject.transform.GetChild(1).GetComponent<TextMesh>().text = brain.GetName();

        Creature_V2 creature = new Creature_V2(totalCreaturesCount, 0, creatureGameObject.transform, leftLine, rightLine, lineSensor, spikeLine, brain, new HSBColor(1f, 0f, 0f), bodyPosition, leftPos, rightPos, 0.5f, angle, worldDeltaTime, creatureGameObject.transform.localScale.x / 2f, energy, currentEnergy, life, minLife, lifeDecrease, veloForward, veloAngular, map_v2, this, parentNames);
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
