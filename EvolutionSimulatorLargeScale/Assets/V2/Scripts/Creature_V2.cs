using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Creature_V2 : CustomCircleCollider, IEquatable<Creature_V2>
{
    public Transform trans = null; //Transform of this object
    public LineRenderer leftLine = null;
    public LineRenderer rightLine = null;

    public Brain_V2 brain = null;

    public HSBColor bodyColor;
    public HSBColor mouthColor;

    public Vector3 leftPos;
    public Vector3 rightPos;

    public int[] tileDetail = new int[2];

    public float initialEnergy = 100f;
    public float currentEnergy = 100f;

    public float initialRadius = 0.06f;
    public float currentRadius = 0.06f;
    public float deltaEnergy = 0f;

    public int ID = -1;
    public float worldDeltaTime = 0.001f;

    private TileMap_V2 map;
    private Material bodyMaterial;
    private Material mouthMaterial;

    public Creature_V2(int ID, Transform trans, LineRenderer leftLine, LineRenderer rightLine, 
                       Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, 
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy, TileMap_V2 map) 
                       : base(initialRadius,bodyPos,angle,0f,0f,1f,worldDeltaTime)
    {
        
        this.ID = ID;
        this.trans = trans;
        this.leftLine = leftLine;
        this.rightLine = rightLine;
        this.brain = brain;
        this.bodyColor = bodyColor;
        this.leftPos = leftPos;
        this.rightPos = rightPos;
        this.worldDeltaTime = worldDeltaTime;
        this.initialEnergy = initialEnergy;
        this.initialRadius = initialRadius;
        this.map = map;

        this.bodyMaterial = trans.GetComponent<Renderer>().material;
        this.mouthMaterial = trans.GetChild(0).GetComponent<Renderer>().material;
    }

    public bool Equals(Creature_V2 other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }


    public float GetEnergy()
    {
        return currentEnergy;
    }

    //return creature size
    public float GetRadius()
    {
        return currentRadius;
    }

    public void NaturalEnergyLoss(float factor)
    {
        deltaEnergy -= (((Time.fixedDeltaTime * worldDeltaTime * 100f) * (currentRadius / initialRadius)) * factor);
    }

    public void ResetDeltaEnergy()
    {
        deltaEnergy = 0f;
    }


    public void Eat(float energy)
    {
        //this.deltaEnergy += (energy - ((Time.deltaTime * 100f)/2f));
        this.deltaEnergy += (energy / 2f);
    }


    public float CalculateSize()
    {
        if (currentEnergy < initialEnergy)
        {
            currentRadius = initialRadius;
        }
        else
        {
            currentRadius = initialRadius + (currentEnergy / initialEnergy) * 0.014f;
        }

        return currentRadius;
    }


    public void ApplyDeltaEnergy()
    {
        currentEnergy += deltaEnergy;
    }

    public void BirthEnergyLoss()
    {
        deltaEnergy -= 150f;
    }

    public void UpdatePhysics()
    {

        float[] previousOutput = brain.GetOutput();
        
        HSBColor bodyTileColor = map.GetColor((int)base.position.x, (int)base.position.y);
        HSBColor leftTileColor = map.GetColor((int)leftPos.x, (int)leftPos.y);
        HSBColor rightTileColor = map.GetColor((int)rightPos.x, (int)rightPos.y);

        List<Creature_V2> creatureListAtLeftTile = map.ExistCreatureAtTile((int)leftPos.x, (int)leftPos.y);
        List<Creature_V2> creatureListAtRightTile = map.ExistCreatureAtTile((int)rightPos.x, (int)rightPos.y);

        //check right sensor collsision
        if (creatureListAtLeftTile != null)
        {
            for (int i = 0; i < creatureListAtLeftTile.Count; i++)
            {
                Creature_V2 creature = creatureListAtLeftTile[i];
                if (!creature.Equals(this))
                {
                    if (creature.CollisionCheckWithPoint(leftPos))
                    {
                        leftTileColor = creature.bodyColor;
                    }
                }
            }
        }

        //check for left sensor collision
        if (creatureListAtRightTile != null)
        {
            for (int i = 0; i < creatureListAtRightTile.Count; i++)
            {
                Creature_V2 creature = creatureListAtRightTile[i];
                if (!creature.Equals(this))
                {
                    if (creature.CollisionCheckWithPoint(rightPos))
                    {
                        rightTileColor = creature.bodyColor;
                    }

                }
            }
        }

        float[] output = brain.feedforward(new float[] { bodyTileColor.s, bodyTileColor.h, leftTileColor.h, leftTileColor.s, rightTileColor.h, rightTileColor.s, currentRadius, previousOutput[7], previousOutput[8] });

        float accelForward = output[0];
        float accelAngular = output[1];
        float bodyHue = output[2];
        float mouthHue = output[3];
        float eatFood = output[4];
        float giveBrith = output[5];
        float fight = output[6];
        this.bodyColor = new HSBColor(bodyHue,1f,1f);
        this.mouthColor = new HSBColor(mouthHue, 1f, 1f); 

        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);


        //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
        //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);

        
        base.UpdatePhysics(accelForward,accelAngular);

        UpdateSensors();

        //copy tile detail
        tileDetail[0] = (int)base.position.x;
        tileDetail[1] = (int)base.position.y;

        map.Eat(tileDetail[0], tileDetail[1]);

        map.AddCreatureToTileList(tileDetail[0], tileDetail[1], this);
    }

    private void UpdateSensors()
    {
        float leftAngle = (((base.angle + 90f) + 25f) * Mathf.Deg2Rad);
        float rightAngle = (((base.angle + 90f) - 25f) * Mathf.Deg2Rad);

        //left and right position calculation
        float mag = 1.05f;
        leftPos = base.position + new Vector3(mag * Mathf.Cos(leftAngle), mag * Mathf.Sin(leftAngle), 0f);
        rightPos = base.position + new Vector3(mag * Mathf.Cos(rightAngle), mag * Mathf.Sin(rightAngle), 0f);
    }

    public void UpdateRender()
    {
        leftLine.SetPosition(0, position);
        leftLine.SetPosition(1, leftPos);
        rightLine.SetPosition(0, position);
        rightLine.SetPosition(1, rightPos);

        bodyMaterial.color = bodyColor.ToColor();
        mouthMaterial.color = mouthColor.ToColor();
        trans.position = position;
        trans.eulerAngles = new Vector3(0f, 0f, angle);
        angle = trans.eulerAngles.z; //resets it back to 0-360

       
    }


}

