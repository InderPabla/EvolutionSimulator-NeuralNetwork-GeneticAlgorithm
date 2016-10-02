using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{

    public TextMesh text;
    public Transform left;
    public Transform right;
    public Transform nose;

    private Brain brain;
    private Rigidbody2D rBody;
    private Material bodyMat;
    private Material noseMat;
    private Color color;
    private TileMap map;

    //Frame rate variables
    private int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    private float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;


    private const string ACTION_KILL_CREATURE = "KillCreature";
    private const string ACTION_BIRTH_CREATURE = "BirthCreature";

    public float currentSpeed;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        bodyMat = GetComponent<Renderer>().material;
        noseMat = nose.GetComponent<Renderer>().material;
        //Invoke("Fire", 0f);
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

        HSBColor floorBodyColor, floorLeftColor, floorRightColor;

        floorBodyColor = map.GetColor((int)transform.position.x, (int)transform.position.y);
        floorLeftColor = map.GetColor((int)left.position.x, (int)left.position.y);
        floorRightColor = map.GetColor((int)right.position.x, (int)right.position.y);

        Collider2D leftCol = Physics2D.OverlapPoint(left.position);
        Collider2D rightCol = Physics2D.OverlapPoint(left.position);

        if (leftCol != null)
        {
            floorLeftColor = HSBColor.FromColor(leftCol.GetComponent<Renderer>().material.color);
        }

        if (rightCol != null)
        {
            floorRightColor = HSBColor.FromColor(rightCol.GetComponent<Renderer>().material.color);
        }

        float[] previousOutput = brain.GetOutput();

        float[] input = new float[] {floorBodyColor.h,floorBodyColor.s,floorBodyColor.b,
                                     floorLeftColor.h,floorLeftColor.s,floorLeftColor.b,
                                     floorRightColor.h,floorRightColor.s,floorRightColor.b,
                                      brain.GetSize(),previousOutput[5],previousOutput[6]};
        float[] output = brain.feedforward(input);

        Vector2 veloCheck = rBody.velocity;
        veloCheck += (Vector2)transform.up * output[0] * 10f * Time.fixedDeltaTime;
        if(veloCheck.magnitude<10f)
            rBody.velocity = veloCheck;

        rBody.angularVelocity = output[1] * 100f;




        //output = brain.GetOutput();

        float h1 = output[2] < 0 ? 1f + output[2] : output[2];
        float h2 = output[3] < 0 ? 1f + output[3] : output[3];

        bodyMat.color = new HSBColor(h1, 1f, 1f).ToColor();
        noseMat.color = new HSBColor(h2, 1f, 1f).ToColor();

        //Energy calculations
        brain.ResetDeltaEnergy();

        // Eat food?
        if (output[7] > 0)
        {
            Vector3 pos = transform.position;
            float energy = map.Eat((int)transform.position.x, (int)transform.position.y);
            brain.Eat(energy); //eat energy
        }

        if (output[4] > 0 && brain.GetEnergy() > 255f)
        {
            brain.BirthEnergyLoss();
            BirthCreature();
        }

        if (map.GetTileType((int)transform.position.x, (int)transform.position.y) == Tile.TILE_WATER)
        {
            brain.NaturalEnergyLoss(10f);
        }
        else
        {
            brain.NaturalEnergyLoss(1f); //natural body energy loss over time (starvation)
        }

        brain.Move(output[0]); //use energy porpotional to the accelration
        

        brain.ApplyDeltaEnergy(); //apply delta

        //Calculate new size of creature
        float size = brain.CalculateSize();
        transform.localScale = new Vector3(size, size, size);
    }

    /*public void Fire()
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

        drag = (output[0] / topSpeed);
        rBody.velocity += ((Vector2)transform.up * output[0] *10f - (drag * rBody.velocity)) * Time.fixedDeltaTime;
        rBody.angularVelocity = output[1] * 100f;


        //rBody.velocity += ((Vector2)rBody.transform.up * output[0]);
        //rBody.angularVelocity = output[1] * 100f;


        Invoke("Fire", 0.1f);
    }*/

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
            transform.parent.SendMessage(ACTION_KILL_CREATURE, brain);
            Destroy(gameObject);
        }
    }


    public void BirthCreature()
    {
        transform.parent.SendMessage(ACTION_BIRTH_CREATURE, brain);
    }

    private void CalculateFPS()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }


    }

    public Brain GetBrain()
    {
        return brain;
    }


}
