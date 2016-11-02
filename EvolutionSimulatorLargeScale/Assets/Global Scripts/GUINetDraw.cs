using UnityEngine;
using System.Collections;

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

    // Use this for initialization
    void Start () {

        texture = new Texture2D(/*(int)((float)Screen.width * 0.25f), Screen.height*/1,1);
        this.screenWidth = (float)Screen.width * 0.25f;
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

        GUI.color = backgroundColor;
        GUI.DrawTexture(new Rect(0, 0, screenWidth, screenHeight), texture);

        
        DrawBrain();
        /*Vector2 pointA = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 pointB = Event.current.mousePosition;
        Drawing.DrawLine(pointA, pointB, Color.red, 2f);*/
    }

    private void DrawBrain() {
        if (brain != null) {
            
            float xMulti = (int)(screenWidth/2f);
            float yMulti = (int)(screenHeight*0.05f);
            Debug.Log(xMulti);

            for (int layerIndex = 0; layerIndex < connections.Length; layerIndex++)
            {
                float currentXPos = ((layerIndex +1) * xMulti);
                float previousXPos = ((layerIndex) * xMulti);
                
                for (int neuronOfLayerIndex = 0; neuronOfLayerIndex < connections[layerIndex].Length; neuronOfLayerIndex++)
                {
                    float nYOffsetCurrentLayer = (yOffset - (((float)connections[layerIndex].Length / 2f) * yMulti)) + 25;
                    float nYOffsetPreviousLayer = (yOffset - (((float)connections[layerIndex][neuronOfLayerIndex].Length / 2f) * yMulti)) + 25;

                    for (int previousLayerNeuronIndex = 0; previousLayerNeuronIndex < connections[layerIndex][neuronOfLayerIndex].Length; previousLayerNeuronIndex++)
                    {
                        Vector2 pointA = new Vector2(currentXPos, (neuronOfLayerIndex* yMulti) + nYOffsetCurrentLayer);
                        Vector2 pointB = new Vector2(previousXPos, (previousLayerNeuronIndex * yMulti) + nYOffsetPreviousLayer);
                        
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

            GUI.color = Color.black;
            float xpos, ypos;
            
            for (int x = 0; x < neurons.Length; x++)
            {
                float numberOfNeuronsInLayer = neurons[x].Length;
                xpos = (x * xMulti);

                float nYOffset = (yOffset - (((float)numberOfNeuronsInLayer / 2f) * yMulti))+ 25;
                for (int y = 0; y < numberOfNeuronsInLayer; y++)
                {
                    ypos = ((y * 25f) + nYOffset);

                    GUI.DrawTexture(new Rect(xpos, ypos, 6f, 6f), texture);
                }
            }

            this.screenWidth = (float)Screen.width * 0.25f;
            this.screenHeight = Screen.height;

        }
    }

    float yOffset = 0f;

    public void SetBrain(Brain_V2 brain)
    {
        this.brain = brain;

        float highest = 0;

        for (int x = 0; x < brain.GetNeurons().Length; x++)
        {
            int numberOfNeuronsInLayer = brain.GetNeurons()[x].Length;

            if (numberOfNeuronsInLayer > highest)
            {
                highest = numberOfNeuronsInLayer;
            }
        }

        highest = ((float)highest /2f)*25f;
        yOffset = highest;

        neurons = brain.GetNeurons();
        connections = brain.GetWeights();
    }

    public void ResetBrain()
    {
        this.brain = null;
        neurons = null;
        connections = null;
    }
}



