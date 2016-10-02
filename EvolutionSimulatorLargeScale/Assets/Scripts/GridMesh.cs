using UnityEngine;
using System.Collections;

public class GridMesh : MonoBehaviour {
    public GameObject linePrefab;

    private GameObject[] gameLineHori;
    private GameObject[] gameLineVert;

    private LineRenderer[] renLineHori;
    private LineRenderer[] renLineVert;

    // Use this for initialization
    void Start () {
        gameLineHori = new GameObject[150];
        renLineHori = new LineRenderer[150];

        gameLineVert = new GameObject[150];
        renLineVert = new LineRenderer[150];

        for (int i = 0; i < 150; i++) {
            gameLineHori[i] = Instantiate(linePrefab) as GameObject;
            gameLineHori[i].transform.parent = transform;

            renLineHori[i] = gameLineHori[i].GetComponent<LineRenderer>();

            renLineHori[i].SetPosition(0, new Vector3(0, i, -1));
            renLineHori[i].SetPosition(1, new Vector3(150, i, -1));

            renLineHori[i].SetWidth(0.1f, 0.1f) ;

            //renLineHori[i].material = new Material(Shader.Find("Particles/Multiply"));
            renLineHori[i].SetColors(Color.black, Color.black);

            gameLineVert[i] = Instantiate(linePrefab) as GameObject;
            gameLineVert[i].transform.parent = transform;

            renLineVert[i] = gameLineVert[i].GetComponent<LineRenderer>();

            renLineVert[i].SetPosition(0, new Vector3(i, 0, -1));
            renLineVert[i].SetPosition(1, new Vector3(i, 150, -1));

            renLineVert[i].SetWidth(0.1f, 0.1f);

            //renLineVert[i].material = new Material(Shader.Find("Particles/Multiply"));
            renLineVert[i].SetColors(Color.black, Color.black);
            
        }
        

    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 150; i++)
        {

            float cameraSize = Camera.main.orthographicSize;
            float width = 0.1f;
            if (cameraSize <23)
            {
                width = 0.1f-(0.05f * ((23 - cameraSize) / 23));
            }

            renLineHori[i].SetWidth(width, width);
            renLineVert[i].SetWidth(width, width);

        }
    }
}
