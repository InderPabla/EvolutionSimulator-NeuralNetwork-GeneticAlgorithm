using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{

    public TextMesh text;
    public Transform left;
    public Transform right;

    private Brain brain;
    private Rigidbody2D rBody;
    private Material mat;
    private Color color;
    private TileMap map;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        mat = GetComponent<Renderer>().material;
        Invoke("Fire", 1f);
    }

    void Activate(Brain brain)
    {
        this.brain = brain;
        text.text = brain.GetName(); 
    }

    void SetMap(TileMap map)
    {
        this.map = map;
    }

    void LateUpdate()
    {
        FixTextPosition();

        CheckForDeath();

    }

    public void FixedUpdate()
    {


        


    }
    

    public void Fire()
    {
        Color floorBodyColor, floorLeftColor, floorRightColor;

        floorBodyColor = map.GetColor((int)transform.position.x, (int)transform.position.y);
        floorLeftColor = map.GetColor((int)left.position.x, (int)left.position.y);
        floorRightColor = map.GetColor((int)right.position.x, (int)right.position.y);

        Collider2D leftCol = Physics2D.OverlapPoint(left.position);
        Collider2D rightCol = Physics2D.OverlapPoint(left.position);

        if (leftCol != null)
        {
            floorLeftColor = leftCol.GetComponent<Renderer>().material.color;
        }

        if (rightCol != null)
        {
            floorRightColor = rightCol.GetComponent<Renderer>().material.color;
        }

        float[] previousOutput = brain.GetOutput();

        float[] input = new float[] {floorBodyColor.r,floorBodyColor.g,floorBodyColor.b,
                                     floorLeftColor.r,floorLeftColor.g,floorLeftColor.b,
                                     floorRightColor.r,floorRightColor.g,floorRightColor.b,
                                      brain.GetSize(),previousOutput[5],previousOutput[6]};
        float[] output = brain.feedforward(input);

        rBody.velocity = rBody.transform.up * output[0] * 10f;
        rBody.angularVelocity = output[1] * 100f;



        output = brain.GetOutput();

        float r = output[2] < 0 ? 1f + output[2] : output[2];
        float g = output[3] < 0 ? 1f + output[3] : output[3];
        float b = output[4] < 0 ? 1f + output[4] : output[4];
        mat.color = new Color(r, g, b);

        if (output[7] > 0)
        {
            Vector3 pos = transform.position;
            float energy = map.Eat((int)transform.position.x, (int)transform.position.y);
            brain.Eat(energy);
        }

        brain.Move(output[0]);

        brain.NaturalEnergyLoss();

        Invoke("Fire", 0.1f);
    }

    void FixTextPosition()
    {
        Vector3 pos = transform.position;
        pos.y += -1;
        text.transform.position = pos;

        text.transform.eulerAngles = Vector3.zero;
    }

    void CheckForDeath()
    {

        if (brain.GetEnergy() < 0)
        {
            Destroy(gameObject);
        }
    }




}
