using UnityEngine;
using System.Collections;

public class GUINetDraw : MonoBehaviour {
    private Texture2D texture;
    private Brain_V2 brain;

    // Use this for initialization
    void Start () {
        texture = new Texture2D((int)((float)Screen.width * 0.25f), Screen.height);

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
        
        /*GUI.color = Color.grey;
        GUI.DrawTexture(new Rect(0, 0, texture.width, texture.height), texture);

        
        DrawBrain();*/
        /*Vector2 pointA = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 pointB = Event.current.mousePosition;
        Drawing.DrawLine(pointA, pointB, Color.red, 2f);*/
    }

    private void DrawBrain() {
        if (brain != null) {
            GUI.color = Color.cyan;
            float[][] neurons = brain.GetNeurons();
            float[][][] connections = brain.GetWeights();

            float xpos, ypos;
        
            for (int x = 0; x < neurons.Length; x++) {
                float numberOfNeuronsInLayer = neurons[x].Length;
                xpos = (x * 100f);
                for (int y = 0; y < numberOfNeuronsInLayer; y++)
                {
                    ypos = ((y * 15f) +25f);
                    
                    GUI.DrawTexture(new Rect(xpos,ypos,2f,2f),texture);
                }
            }

            //GUI.color = Color.red;
            for (int layerIndex = 0; layerIndex < connections.Length; layerIndex++)
            {
                float currentXPos = ((layerIndex +1) * 100f);
                float previousXPos = ((layerIndex) * 100f);
                for (int neuronOfLayerIndex = 0; neuronOfLayerIndex < connections[layerIndex].Length; neuronOfLayerIndex++)
                {

                    for (int previousLayerNeuronIndex = 0; previousLayerNeuronIndex < connections[layerIndex][neuronOfLayerIndex].Length; previousLayerNeuronIndex++)
                    {
                        Vector2 pointA = new Vector2(currentXPos, (neuronOfLayerIndex*15f) + 25f);
                        Vector2 pointB = new Vector2(previousXPos, (previousLayerNeuronIndex * 15f) + 25f);
                        Color lineColor = Color.green;

                        if (connections[layerIndex][neuronOfLayerIndex][previousLayerNeuronIndex] <= 0f)
                        {
                            lineColor = Color.red;
                        }
                        Drawing.DrawLine(pointA, pointB, lineColor, 1f,texture);
                    }
                }
            }

        }
    }

    public void SetBrain(Brain_V2 brain)
    {
        this.brain = brain;
    } 
}



