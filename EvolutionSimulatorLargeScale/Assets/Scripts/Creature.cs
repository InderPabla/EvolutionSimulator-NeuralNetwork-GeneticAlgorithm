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
    private Texture2D tex;

    
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        mat = GetComponent<Renderer>().material;
        Invoke("Fire",1f);
    }

    void Activate(Brain brain)
    {
        this.brain = brain;
        text.text = brain.GetName();
    }

    void SetTexture(Texture2D tex)
    {
        this.tex = tex;
    }

    void LateUpdate()
    {
        FixTextPosition();
    }

    private Color GetTileColor(Vector2 pos)
    {
        if (pos.x > 150f || pos.x < 0 || pos.y > 150f || pos.x < 0)
        {
            return Color.black;
        }
        else
        {
            return tex.GetPixel((int)pos.x, (int)pos.y);
        }

    }

    public void Fire()
    {
        Color floorBodyColor, floorLeftColor, floorRightColor;

        floorBodyColor = GetTileColor(transform.position);
        floorLeftColor = GetTileColor(left.position);
        floorRightColor = GetTileColor(right.position);

        float[] input = new float[] {floorBodyColor.r,floorBodyColor.g,floorBodyColor.b,
                                     floorLeftColor.r,floorLeftColor.g,floorLeftColor.b,
                                     floorRightColor.r,floorRightColor.g,floorRightColor.b,
                                      3.43f,-0.34f,-0.894f};
        float[] output = brain.feedforward(input);

        rBody.velocity = rBody.transform.up * output[3] * 10f;
        rBody.angularVelocity = output[4] * 100f;

        float r = output[0] < 0 ? 1f + output[0] : output[0];
        float g = output[1] < 0 ? 1f + output[1] : output[1];
        float b = output[2] < 0 ? 1f + output[2] : output[2];
        mat.color = new Color(r,g,b);

        floorBodyColor.g += 0.01f;
        tex.SetPixel((int)transform.position.x, (int)transform.position.y,floorBodyColor);
        
        Invoke("Fire", 0.1f);
    }

    void FixTextPosition()
    {
        Vector3 pos = transform.position;
        pos.y += -1;
        text.transform.position = pos;

        text.transform.eulerAngles = Vector3.zero;
    }




}
