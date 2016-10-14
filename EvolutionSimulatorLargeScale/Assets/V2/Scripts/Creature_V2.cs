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

    public Vector3 bodyPos;
    public Vector3 leftPos;
    public Vector3 rightPos;

    public int[] tileDetail = new int[2];

    public float angle;

    public float initialEnergy = 100f;
    public float currentEnergy = 100f;

    public float initialRadius = 0.5f;
    public float currentRadius = 0.5f;
    public float deltaEnergy = 0f;

    public int ID = -1;
    public float worldDeltaTime = 0.001f;

    private TileMap_V2 map;
    public Creature_V2(int ID, Transform trans, LineRenderer leftLine, LineRenderer rightLine, 
                       Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, 
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy, TileMap_V2 map) 
                       : base(initialRadius,bodyPos,angle,0f,0f,1f,(1f/100f),worldDeltaTime)
    {
        
        this.ID = ID;
        this.trans = trans;
        this.leftLine = leftLine;
        this.rightLine = rightLine;
        this.brain = brain;
        this.bodyPos = bodyPos;
        this.bodyColor = bodyColor;
        this.leftPos = leftPos;
        this.rightPos = rightPos;
        this.angle = angle;
        this.worldDeltaTime = worldDeltaTime;
        this.initialEnergy = initialEnergy;
        this.initialRadius = initialRadius;
        this.map = map;
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

    public void Update() {

       /* Vector3 bodyPos = this.bodyPos;
        Vector3 leftPos = this.leftPos;
        Vector3 rightPos = this.rightPos;
        int[] tileDetail = this.tileDetail;
        float angle = this.angle;
        float bodyRadius = currentRadius;*/

        float[] previousOutput = brain.GetOutput();

        HSBColor bodyTileColor = map.GetColor((int)bodyPos.x, (int)bodyPos.y);
        HSBColor leftTileColor = map.GetColor((int)leftPos.x, (int)bodyPos.y);
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
                    Vector3 bodyPosOfCreature = creature.bodyPos;
                    float creatureSizeSqr = Mathf.Pow(creature.currentRadius, 2f);
                    float distanceBetweenPointAndCircle = Mathf.Pow((bodyPosOfCreature.x - leftPos.x), 2f) + Mathf.Pow((bodyPosOfCreature.y - leftPos.y), 2f);

                    //inter section occoures
                    if (distanceBetweenPointAndCircle <= creatureSizeSqr)
                    {
                        leftTileColor = creature.bodyColor;
                    }
                    break;
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
                    Vector3 bodyPosOfCreature = creature.bodyPos;
                    float creatureSizeSqr = Mathf.Pow(creature.currentRadius, 2f);
                    float distanceBetweenPointAndCircle = Mathf.Pow((bodyPosOfCreature.x - rightPos.x), 2f) + Mathf.Pow((bodyPosOfCreature.y - rightPos.y), 2f);

                    //inter section occoures
                    if (distanceBetweenPointAndCircle <= creatureSizeSqr)
                    {
                        rightTileColor = creature.bodyColor;
                    }
                    break;
                }
            }
        }

        float[] output = brain.feedforward(new float[] { bodyTileColor.s, bodyTileColor.h, leftTileColor.h, leftTileColor.s, rightTileColor.h, rightTileColor.s, currentRadius, previousOutput[7], previousOutput[8] });

        float forwardAccel = output[0];
        float rotationalAccel = output[1];
        float bodyHue = output[2];
        float mouthHue = output[3];
        float eatFood = output[4];
        float giveBrith = output[5];
        float fight = output[6];
        this.bodyColor = new HSBColor(bodyHue, 1f, 1f);
        this.mouthColor = new HSBColor(mouthHue, 1f, 1f);

        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);

        //body position calculation
        float unitAngle = angle - 90f;
        if (unitAngle > 180)
            unitAngle = (360f - unitAngle) * -1f;
        Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);
        Vector3 displace = newUnit * forwardAccel * worldDeltaTime * 10f;

        bodyPos += displace;
        float mag = 1.05f;
        //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
        //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);
        float leftAngle = (((angle + 90f) + 25f) * Mathf.Deg2Rad);
        float rightAngle = (((angle + 90f) - 25f) * Mathf.Deg2Rad);

        //left position calculation
        leftPos = bodyPos + new Vector3(mag * Mathf.Cos(leftAngle), mag * Mathf.Sin(leftAngle), 0f);

        //right position calculation
        rightPos = bodyPos + new Vector3(mag * Mathf.Cos(rightAngle), mag * Mathf.Sin(rightAngle), 0f);

        //angle calculation
        angle += rotationalAccel * worldDeltaTime * 100f;

        //copy tile detail
        tileDetail[0] = (int)bodyPos.x;
        tileDetail[1] = (int)bodyPos.y;

        map.Eat((int)bodyPos.x, (int)bodyPos.y);

        map.AddCreatureToTileList(tileDetail[0], tileDetail[1], this);
    }
}

