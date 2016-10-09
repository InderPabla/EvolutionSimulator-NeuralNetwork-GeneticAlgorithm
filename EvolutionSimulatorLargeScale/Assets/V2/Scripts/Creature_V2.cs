using UnityEngine;
using System.Collections;
using System;

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

    public Creature_V2(int ID, Transform trans, LineRenderer leftLine, LineRenderer rightLine, Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, 
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy) : base(initialRadius,bodyPos,angle,0f,0f,1f,(1f/100f),worldDeltaTime)
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
}

