using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUINetDraw : MonoBehaviour {
    public int createButtonState = 0;
    public int fastButtonState = 0;
    public int slowButtonState = 0;
    public int visionButtonState = 0;
    public int saveButtonState = 0;
    public int loadButtonState = 0;
    public int drawNetButtonState = 0;
    public bool drawNetState = false;

    private Texture2D texture;
    private Brain_V2 brain;
    private Creature_V2 creature;
    private float screenWidth, screenHeight;
    private float[][] neurons;
    private float[][][] connections;

    private Color backgroundColor;
    private Color deadColor;
    private Color negativeLineColor;
    private Color positiveLineColor;
    private Color neuronColor;

    private float yOffset = 0f;
    private float highest = 0f;
    private List<TreeData> treeDataList;

    private int brainCalculations = 0;
    private int totalNumberOfCreatures = 0;
    private int creatureCount = 0;
    private float playSpeed = 1;

    private string[] inputNames = new string[] {"[Sensor_1_Dist]", "[Sensor_2_Dist]", "[Sensor_3_Dist]", "[Sensor_4_Dist]",
                                                 "[Sensor_1_Hue]", "[Sensor_2_Hue]", "[Sensor_3_Hue]", "[Sensor_4_Hue]","[Spike_Hue]",
                                                 "[Collide?]", "[Collide_Hue]", "[Tile_Hue]", "[Tile_Sat]", "[Left_Hue]", "[Left_Sat]", "[Right_Hue]", "[Right_Sat]",
                                                 /*"[Life]","[Energy]"*/"[Radius]", "[Mem_In_1]","[Mem_In_2]" };

    private string[] outputNames = new string[] {"[Velo_Fwd]","[Velo_Ang]","[MouthHue]","[BodyHue]","[Eat?]","[Birth?]","[Spike_Len]","[Mem_Out_1]","[Mem_Out_2]"};

    // Use this for initialization
    void Start () {
        texture = new Texture2D(1,1);
        this.screenWidth = (float)Screen.width * 0.4f;
        this.screenHeight = Screen.height;

        backgroundColor = Color.grey; backgroundColor.a = 1f;
        negativeLineColor = Color.red; negativeLineColor.a = 1f;
        positiveLineColor = Color.green; positiveLineColor.a = 1f;
        neuronColor = Color.magenta; neuronColor.a = 1f;
        deadColor = new Color(0.5f,0f,0f); deadColor.a = 1f;
    }

    void OnGUI()
    {
        this.screenWidth = (float)Screen.width * 0.4f;
        this.screenHeight = Screen.height;

        if (drawNetButtonState == 2)
        {
            drawNetState = !drawNetState;
        }

        if (creature == null || creature.IsAlive())
            GUI.color = backgroundColor;
        else
            GUI.color = deadColor;

        GUI.DrawTexture(new Rect(0, 0, screenWidth+10f, screenHeight), texture);
        DrawBrain();
        DrawUI();
    }

    private void DrawBrain()
    {
        if (brain != null)
        {

            float xOff = (int)(screenWidth / (neurons.Length - 1));
            float yOff = (int)(screenHeight * 0.025f);
            float rectWidth = (int)(screenWidth * 0.075f);
            float rectHeight = (int)(rectWidth / 1.5f);

            if (drawNetState == true)
            {
                for (int layerIndex = 0; layerIndex < connections.Length; layerIndex++)
                {

                    float currentLayerYRatio = (screenHeight / 2f) / (float)connections[layerIndex].Length;
                    float currentXPos = (int)((layerIndex + 1) * xOff);
                    float previousXPos = (int)((layerIndex) * xOff);

                    for (int neuronOfLayerIndex = 0; neuronOfLayerIndex < connections[layerIndex].Length; neuronOfLayerIndex++)
                    {

                        float previousLayerYRatio = (screenHeight / 2f) / (float)connections[layerIndex][neuronOfLayerIndex].Length;


                        for (int previousLayerNeuronIndex = 0; previousLayerNeuronIndex < connections[layerIndex][neuronOfLayerIndex].Length; previousLayerNeuronIndex++)
                        {
                            Vector2 pointA = new Vector2(currentXPos + (int)(rectWidth / 2f), currentLayerYRatio * neuronOfLayerIndex + yOff + (int)(rectHeight / 2f));
                            Vector2 pointB = new Vector2(previousXPos + (int)(rectWidth / 2f), previousLayerYRatio * previousLayerNeuronIndex + yOff + (int)(rectHeight / 2f));

                            Color lineColor = positiveLineColor;

                            if (connections[layerIndex][neuronOfLayerIndex][previousLayerNeuronIndex] <= 0f)
                            {
                                lineColor = negativeLineColor;
                            }

                            Drawing.DrawLine(pointA, pointB, lineColor, 2f, texture);
                        }
                    }
                }
            }
            else
            {
                Vector2 mousePosition = Event.current.mousePosition;

                for (int x = 0; x < neurons.Length; x++)
                {
                    float numberOfNeuronsInLayer = neurons[x].Length;
                    float yRatio = (screenHeight / 2f) / numberOfNeuronsInLayer;

                    float xpos = x * xOff;
                    float ypos = 0f;
                    for (int y = 0; y < numberOfNeuronsInLayer; y++)
                    {

                        ypos = y * yRatio + yOff;
                        Rect rec = new Rect(xpos, ypos, rectWidth, rectHeight);
                        if (rec.Contains(mousePosition) == true)
                        {
                            if (x == 0)
                            {
                                float nextLayerYRatio = (screenHeight / 2f) / (float)neurons[x+1].Length;
                                float nextXPos = (int)((x + 1) * xOff);
                                float currentXPos = (int)((x) * xOff);

                                for (int z = 0; z < neurons[x + 1].Length; z++)
                                {
                                    Vector2 pointA = new Vector2(rec.x + (int)(rectWidth / 2f), rec.y + (int)(rectHeight / 2f));
                                    Vector2 pointB = new Vector2(nextXPos + (int)(rectWidth / 2f), nextLayerYRatio * z + yOff + (int)(rectHeight / 2f));

                                    Color lineColor = positiveLineColor;

                                    if (connections[x][z][y] <= 0f)
                                    {
                                        lineColor = negativeLineColor;
                                    }

                                    Drawing.DrawLine(pointA, pointB, lineColor, 1f, texture);
                                }
                                break;
                            }
                            else if (x == (neurons.Length - 1))
                            {
                                float nextLayerYRatio = (screenHeight / 2f) / (float)neurons[x - 1].Length;
                                float nextXPos = (int)((x - 1) * xOff);

                                for (int z = 0; z < neurons[x - 1].Length; z++)
                                {
                                    Vector2 pointA = new Vector2(rec.x + (int)(rectWidth / 2f), rec.y + (int)(rectHeight / 2f));
                                    Vector2 pointB = new Vector2(nextXPos + (int)(rectWidth / 2f), nextLayerYRatio * z + yOff + (int)(rectHeight / 2f));

                                    Color lineColor = positiveLineColor;

                                    if (connections[x - 1][y][z] <= 0f)
                                    {
                                        lineColor = negativeLineColor;
                                    }

                                    Drawing.DrawLine(pointA, pointB, lineColor, 1f, texture);
                                }

                                break;
                            }
                            else
                            {
                                float nextLayerYRatio = (screenHeight / 2f) / (float)neurons[x - 1].Length;
                                float nextXPos = (int)((x - 1) * xOff);

                                for (int z = 0; z < neurons[x - 1].Length; z++)
                                {
                                    Vector2 pointA = new Vector2(rec.x + (int)(rectWidth / 2f), rec.y + (int)(rectHeight / 2f));
                                    Vector2 pointB = new Vector2(nextXPos + (int)(rectWidth / 2f), nextLayerYRatio * z + yOff + (int)(rectHeight / 2f));

                                    Color lineColor = positiveLineColor;

                                    if (connections[x - 1][y][z] <= 0f)
                                    {
                                        lineColor = negativeLineColor;
                                    }

                                    Drawing.DrawLine(pointA, pointB, lineColor, 1f, texture);
                                }

                                float nextLayerYRatio2 = (screenHeight / 2f) / (float)neurons[x + 1].Length;
                                float nextXPos2 = (int)((x + 1) * xOff);

                                for (int z = 0; z < neurons[x + 1].Length; z++)
                                {
                                    Vector2 pointA = new Vector2(rec.x + (int)(rectWidth / 2f), rec.y + (int)(rectHeight / 2f));
                                    Vector2 pointB = new Vector2(nextXPos2 + (int)(rectWidth / 2f), nextLayerYRatio2 * z + yOff + (int)(rectHeight / 2f));

                                    Color lineColor = positiveLineColor;

                                    if (connections[x][z][y] <= 0f)
                                    {
                                        lineColor = negativeLineColor;
                                    }

                                    Drawing.DrawLine(pointA, pointB, lineColor, 1f, texture);
                                }
                                break;   
                            }
                        }
                    }
                }

            }

            GUIStyle myStyle = new GUIStyle();
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.fontSize = (int)(screenWidth * 0.025f);

            for (int x = 0; x < neurons.Length; x++)
            {
                float numberOfNeuronsInLayer = neurons[x].Length;
                float yRatio = (screenHeight / 2f) / numberOfNeuronsInLayer;

                float xpos = x * xOff;
                float ypos = 0f;
                for (int y = 0; y < numberOfNeuronsInLayer; y++)
                {
                    ypos = y*yRatio + yOff;
                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(xpos , ypos, rectWidth, rectHeight), texture);
                    myStyle.normal.textColor = Color.black;
                    GUI.Label(new Rect((int)(xpos ), (int)(ypos + (rectHeight / 4f)), rectWidth, rectHeight), neurons[x][y].ToString("0.000") + "", myStyle);

                    /*if (x == 0)
                    {
                        myStyle.normal.textColor = Color.green;
                        GUI.Label(new Rect((int)(xpos + (rectWidth / 0.85f)), (int)(ypos + (rectHeight / 4f)), rectWidth, rectHeight), inputNames[y], myStyle);
                    }
                    else if (x == neurons.Length - 1)
                    {
                        myStyle.normal.textColor = Color.green;
                        GUI.Label(new Rect((int)(xpos - (rectWidth / 0.475f)), (int)(ypos + (rectHeight / 4f)), rectWidth, rectHeight), outputNames[y], myStyle);
                    }*/
                }
            }

            for (int i = 0; i < treeDataList.Count; i++)
            {
                myStyle.normal.textColor = treeDataList[i].color;

                if (i == 0)
                {
                    string state = creature.IsAlive() == true ? "[ALIVE]" : "[DEAD]";

                    string cretureInformation = treeDataList[i].name +" "+ state+
                                                "\n                                                         Parents: [" + creature.GetParentNames() + "]" +
                                                "\n                                                         Child Count: " + creature.GetChildCount() +
                                                "\n                                                         Generation: " + creature.GetGeneration() +
                                                "\n                                                         Time Lived: " + creature.GetTimeLived().ToString("0.000") +
                                                "\n                                                         Life: " + creature.GetLife().ToString("0.000") +
                                                "\n                                                         Energy: " + creature.GetEnergy().ToString("0.000") +
                                                "\n                                                         Delta: " + creature.GetDeltaEnergy();

                    GUI.Label(new Rect(1f, screenHeight / 1.9f + rectHeight + i * ( yOff / 1.5f), rectWidth, rectHeight), cretureInformation, myStyle);
                }
                else
                {
                    GUI.Label(new Rect(1f, screenHeight / 1.9f + rectHeight + i * (yOff / 1.5f), rectWidth, rectHeight), treeDataList[i].name, myStyle);
                }
            }
        }
    }

    public void DrawUI()
    {
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.fontSize = (int)(screenWidth * 0.05f);

        int buttonWidth = (int)(screenWidth * 0.2f);
        int buttonHeight = (int)(buttonWidth / 1.8f);

        //create button
        Rect createButton = new Rect(screenWidth - buttonWidth, screenHeight-buttonHeight, buttonWidth,  buttonHeight);

        if (Input.GetMouseButton(0) && createButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(createButton, texture);
            createButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(createButton, texture);
            createButtonState = 0;
        }

        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(createButton, "Create",myStyle);

        //play speed buttons
        GUI.color = Color.blue;

        Rect fastSpeedButton = new Rect(screenWidth - buttonWidth, screenHeight - buttonHeight*2f - 3, buttonWidth/2.5f, buttonHeight);
        Rect slowSpeedButton = new Rect(screenWidth - buttonWidth + buttonWidth/2.5f +(buttonWidth-(buttonWidth/2.5f * 2f)), screenHeight - buttonHeight*2f - 3, buttonWidth/2.5f, buttonHeight);

        if (Input.GetMouseButton(0) && fastSpeedButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(fastSpeedButton, texture);
            fastButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(fastSpeedButton, texture);
            fastButtonState = 0;
        }

        if (Input.GetMouseButton(0) && slowSpeedButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(slowSpeedButton, texture);
            slowButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(slowSpeedButton, texture);
            slowButtonState = 0;
        }

        //vision button
        Rect visionButton = new Rect(screenWidth - buttonWidth * 2 - 3, screenHeight - buttonHeight*2 - 3, buttonWidth, buttonHeight);
        if (Input.GetMouseButton(0) && visionButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(visionButton, texture);
            visionButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(visionButton, texture);
            visionButtonState = 0;
        }

        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(visionButton, "Vision", myStyle);

        //draw net button
        Rect drawNetButton = new Rect(screenWidth - buttonWidth * 2 - 3, screenHeight - buttonHeight, buttonWidth, buttonHeight);
        if (Input.GetMouseButton(0) && drawNetButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(drawNetButton, texture);
            drawNetButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(drawNetButton, texture);
            drawNetButtonState = 0;
        }

        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(drawNetButton, "Draw\nNet", myStyle);

        //save button
        Rect saveButton = new Rect(screenWidth - buttonWidth * 3 - 6, screenHeight - buttonHeight, buttonWidth, buttonHeight);
        if (Input.GetMouseButton(0) && saveButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(saveButton, texture);
            saveButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(saveButton, texture);
            saveButtonState = 0;
        }

        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(saveButton, "Save", myStyle);

        //load button
        Rect loadButton = new Rect(screenWidth - buttonWidth * 3 - 6, screenHeight - buttonHeight*2 -3, buttonWidth, buttonHeight);
        if (Input.GetMouseButton(0) && loadButton.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(loadButton, texture);
            loadButtonState++;
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(loadButton, texture);
            loadButtonState = 0;
        }

        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(loadButton, "Load", myStyle);


        //UI numbers
        GUI.color = Color.white;
        myStyle.normal.textColor = Color.white;
        GUI.Label(fastSpeedButton, ">>", myStyle);
        GUI.Label(slowSpeedButton, "<<", myStyle);

        myStyle.fontSize = (int)(screenWidth * 0.03f);

        //draw play speed
        Rect playSpeedNumber = new Rect(fastSpeedButton.x - (int)(screenWidth * 0.04f), fastSpeedButton.y-buttonHeight/2f, buttonWidth,buttonHeight);
        GUI.Label(playSpeedNumber, "Speed: "+playSpeed.ToString("0.0"), myStyle);

        //draw total number of creatures 
        Rect totalNumber = new Rect(fastSpeedButton.x - (int)(screenWidth * 0.04f), fastSpeedButton.y - buttonHeight / 2f - (int)(screenWidth * 0.04f), buttonWidth, buttonHeight);
        GUI.Label(totalNumber, "Total: " + totalNumberOfCreatures, myStyle);

        //draw total number of creatures 
        Rect countNumber = new Rect(fastSpeedButton.x - (int)(screenWidth * 0.04f), fastSpeedButton.y - buttonHeight / 2f - (int)(screenWidth * 0.04f*2f), buttonWidth, buttonHeight);
        GUI.Label(countNumber, "Current: " + creatureCount, myStyle);

        //draw total number of creatures 
        Rect calculationsNumber = new Rect(fastSpeedButton.x - (int)(screenWidth * 0.04f), fastSpeedButton.y - buttonHeight / 2f - (int)(screenWidth * 0.04f * 3f), buttonWidth, buttonHeight);
        GUI.Label(calculationsNumber, "Calculations: " + creatureCount*brainCalculations, myStyle);

        //World time
        Rect worldNumber = new Rect(fastSpeedButton.x - (int)(screenWidth * 0.04f), fastSpeedButton.y - buttonHeight / 2f - (int)(screenWidth * 0.04f * 4f), buttonWidth, buttonHeight);
        GUI.Label(worldNumber, "World-Time: " + WolrdManager_V2.WORLD_CLOCK.ToString("0.000"), myStyle);

    }

    public void SetBrain(Brain_V2 brain, List<TreeData> treeDataList, Creature_V2 creature)
    {
        this.brain = brain;
        this.treeDataList = treeDataList;
        this.creature = creature;

        neurons = brain.GetNeurons();
        connections = brain.GetWeights();
    }

    public void ResetBrain()
    {
        brain = null;
        neurons = null;
        connections = null;
        creature = null;
        treeDataList = new List<TreeData>();
    }

    public void SetWorldDrawInformation(int total, int count, float playSpeed, int brainCalculations)
    {
        this.totalNumberOfCreatures = total;
        this.creatureCount = count;
        this.playSpeed = playSpeed;
        this.brainCalculations = brainCalculations;
    }
}



