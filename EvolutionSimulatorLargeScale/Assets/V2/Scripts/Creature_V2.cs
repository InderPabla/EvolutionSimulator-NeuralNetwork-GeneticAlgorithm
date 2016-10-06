using UnityEngine;
using System.Collections;
using System;

public class Creature_V2 : IEquatable<Creature_V2>
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

    public float angle;

    public float energy = 100f;
    public float size = 0.5f;
    public int[] tileDetail = new int[2];

    int ID = -1;

    public Creature_V2(int ID, Transform trans, LineRenderer leftLine, LineRenderer rightLine, Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, float angle)
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
    }

    public bool Equals(Creature_V2 other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }
}

