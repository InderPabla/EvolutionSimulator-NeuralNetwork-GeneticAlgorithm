using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUINetDraw : MonoBehaviour {
    private Texture2D texture;
    private Brain_V2 brain;
    private float screenWidth, screenHeight;
    private float[][] neurons;
    private float[][][] connections;

    private Color backgroundColor;
    private Color negativeLineColor;
    private Color positiveLineColor;
    private Color neuronColor;

    private float yOffset = 0f;
    private float highest = 0f;
    private List<TreeData> treeDataList;

    // Use this for initialization
    void Start () {

        texture = new Texture2D(/*(int)((float)Screen.width * 0.25f), Screen.height*/1,1);
        this.screenWidth = (float)Screen.width * 0.4f;
        this.screenHeight = Screen.height;

        backgroundColor = Color.grey; backgroundColor.a = 1f;
        negativeLineColor = Color.red; negativeLineColor.a = 1f;
        positiveLineColor = Color.green; positiveLineColor.a = 1f;
        neuronColor = Color.magenta; neuronColor.a = 1f;

        /*for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.grey);
            }
        }
        
        texture.Apply();*/
    }

    // Update is called once per frame
    void Update () {
        
    }

    void OnGUI()
    {
        this.screenWidth = (float)Screen.width * 0.4f;
        this.screenHeight = Screen.height;

        GUI.color = backgroundColor;
        GUI.DrawTexture(new Rect(0, 0, screenWidth+10f, screenHeight), texture);
        DrawBrain();
        DrawButton();

        /*Vector2 pointA = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 pointB = Event.current.mousePosition;
        Drawing.DrawLine(pointA, pointB, Color.red, 2f);*/
    }

    private void DrawBrain() {
        if (brain != null) {
            
            float xMulti = (int)(screenWidth/(neurons.Length-1));
            float yMulti = (int)(screenHeight* 0.025f);

            for (int layerIndex = 0; layerIndex < connections.Length; layerIndex++)
            {
                float currentXPos = ((layerIndex +1) * xMulti);
                float previousXPos = ((layerIndex) * xMulti);
                
                for (int neuronOfLayerIndex = 0; neuronOfLayerIndex < connections[layerIndex].Length; neuronOfLayerIndex++)
                {
                    float nYOffsetCurrentLayer = (yOffset - (((float)connections[layerIndex].Length / 2f) * yMulti)) + yMulti;
                    float nYOffsetPreviousLayer = (yOffset - (((float)connections[layerIndex][neuronOfLayerIndex].Length / 2f) * yMulti)) + yMulti;

                    for (int previousLayerNeuronIndex = 0; previousLayerNeuronIndex < connections[layerIndex][neuronOfLayerIndex].Length; previousLayerNeuronIndex++)
                    {
                        Vector2 pointA = new Vector2(currentXPos +12f, (neuronOfLayerIndex* yMulti) + nYOffsetCurrentLayer + 7f);
                        Vector2 pointB = new Vector2(previousXPos + 12f, (previousLayerNeuronIndex * yMulti) + nYOffsetPreviousLayer + 7f);
                        
                        Color lineColor = positiveLineColor;

                        if (connections[layerIndex][neuronOfLayerIndex][previousLayerNeuronIndex] <= 0f)
                        {
                            lineColor = negativeLineColor;
                        }

                        /*float width = Mathf.Abs(connections[layerIndex][neuronOfLayerIndex][previousLayerNeuronIndex]);
                        if (width > 1f)
                            width = 1f;

                        Color lineColor = new Color(1f-width,1f-width,1f-width,1f);*/
                        Drawing.DrawLine(pointA, pointB, lineColor, 1f,texture);
                    }
                }
            }
 
            float xpos, ypos;
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.fontSize = (int)(screenWidth*0.025f);

            for (int x = 0; x < neurons.Length; x++)
            {
                float numberOfNeuronsInLayer = neurons[x].Length;
                xpos = (x * xMulti);
                float nYOffset = (yOffset - (((float)numberOfNeuronsInLayer / 2f) * yMulti)) + yMulti;

                for (int y = 0; y < numberOfNeuronsInLayer; y++)
                {
                    ypos = (y * (yMulti)) + nYOffset;

                    GUI.color = Color.white; 
                    GUI.DrawTexture(new Rect(xpos, ypos, 30f, 14f), texture); 
                    GUI.Label(new Rect(xpos, ypos, 100f, 20f), neurons[x][y].ToString("#.###") +"", myStyle);    
                }
            }

            for (int i = 0; i < treeDataList.Count; i++) {
                myStyle.normal.textColor = treeDataList[i].color;
                GUI.Label(new Rect(10f, (yOffset * 2f) + (i*15f) + 15f, 100f, 20f), treeDataList[i].name, myStyle);
            }
            yOffset = ((float)highest / 2f) * (screenHeight * 0.025f);
        }
    }

    public void DrawButton()
    {
        Rect button = new Rect(screenWidth - 100f, screenHeight-50, 100f,50f);
        if (Input.GetMouseButton(0) && button.Contains(Event.current.mousePosition))
        {
            GUI.color = Color.red;
            GUI.DrawTexture(button, texture);
        }
        else
        {

            GUI.color = Color.blue;
            GUI.DrawTexture(button, texture);
        }

    }

    public void SetBrain(Brain_V2 brain, List<TreeData> treeDataList)
    {
        this.brain = brain;
        this.treeDataList = treeDataList;
        //float highest = 0;

        for (int x = 0; x < brain.GetNeurons().Length; x++)
        {
            int numberOfNeuronsInLayer = brain.GetNeurons()[x].Length;

            if (numberOfNeuronsInLayer > highest)
            {
                highest = numberOfNeuronsInLayer;
            }
        }

        //highest = ((float)highest /2f)*25f;
        yOffset = ((float)highest / 2f) * (screenHeight * 0.025f); 

        neurons = brain.GetNeurons();
        connections = brain.GetWeights();
        
    }

    public void ResetBrain()
    {
        brain = null;
        neurons = null;
        connections = null;
        treeDataList = new List<TreeData>();
    }
}



