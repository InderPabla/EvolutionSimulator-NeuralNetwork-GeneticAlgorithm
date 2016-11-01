using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Creature_V2 : CustomCircleCollider, IEquatable<Creature_V2>, IComparable
{
    private Transform trans = null; //Transform of this object
    private Transform textTrans = null;
    private Transform fightTrans = null; 
    private LineRenderer leftLine = null;
    private LineRenderer rightLine = null;
    private LineRenderer spikeLine = null;
    private LineRenderer[] sensorLine;

    private Brain_V2 brain = null;

    private HSBColor bodyColor;
    private HSBColor mouthColor;

    private Vector3 leftPos;
    private Vector3 rightPos;
    private Vector3[] sensorPos = new Vector3[4];
    private float[] sensorValue = new float[4];
    private float spikeLength = 0f;
    private Vector3 spikePos;

    private int[] tileDetail = new int[2];

    private float initialRadius;
    //private float currentRadius = 0.04f;

    private int ID = -1;
    private int generation;
    private float worldDeltaTime = 0.001f;

    private TileMap_V2 map;
    private Material bodyMaterial;
    private Material mouthMaterial;

    private float sensorSize;
    private Energy energy;
    private WolrdManager_V2 world;

    private List<Creature_V2> children;
    private int childCount = 0;
    private bool isNode = false;

    public Creature_V2(int ID, int generation, Transform trans, LineRenderer leftLine, LineRenderer rightLine, LineRenderer[] sensorLine, LineRenderer spikeLine,
                       Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, float sensorSize,
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy, TileMap_V2 map, WolrdManager_V2 world) 
                       : base(initialRadius,bodyPos,angle,0f,0f,1f,worldDeltaTime)
    {
        
        this.ID = ID;
        this.generation = generation;
        this.trans = trans;
        
        this.leftLine = leftLine;
        this.rightLine = rightLine;
        this.sensorLine = sensorLine;
        this.spikeLine = spikeLine;

        this.brain = brain;
        this.bodyColor = bodyColor;
        this.leftPos = leftPos;
        this.rightPos = rightPos;
        this.sensorSize = sensorSize;
        this.worldDeltaTime = worldDeltaTime;
        this.initialRadius = initialRadius;
        this.map = map;
        this.world = world;
        
        this.textTrans = trans.GetChild(1).GetComponent<TextMesh>().transform;
        this.fightTrans = trans.GetChild(3);


        for (int i = 0; i < sensorPos.Length; i++)
        {
            sensorPos[i] = Vector3.zero;
        }

        this.spikeLength = 0f;
        this.spikePos = Vector3.zero;

        this.children = new List<Creature_V2>();
        //energyDensity = 1f/(Mathf.PI * initialRadius * initialRadius);

        this.bodyMaterial = trans.GetComponent<Renderer>().material;
        this.mouthMaterial = trans.GetChild(0).GetComponent<Renderer>().material;

        this.energy = new Energy(initialEnergy, map, worldDeltaTime);

        this.bodyColor = new HSBColor(UnityEngine.Random.Range(0f,1f),1f,1f);
        this.mouthColor = new HSBColor(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
    }

    public bool Equals(Creature_V2 other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }

    /**/

  

    public void UpdateCreaturePhysics()
    {

        float[] previousOutput = brain.GetOutput();
        
        HSBColor bodyTileColor = map.GetColor((int)base.position.x, (int)base.position.y);
        HSBColor leftTileColor = map.GetColor((int)leftPos.x, (int)leftPos.y);
        HSBColor rightTileColor = map.GetColor((int)rightPos.x, (int)rightPos.y);

        /*List<Creature_V2> creatureListAtLeftTile = map.ExistCreatureAtTile((int)leftPos.x, (int)leftPos.y);
        List<Creature_V2> creatureListAtRightTile = map.ExistCreatureAtTile((int)rightPos.x, (int)rightPos.y);

        //check right sensor collsision
        if (creatureListAtLeftTile != null)
        {
            for (int i = 0; i < creatureListAtLeftTile.Count; i++)
            {
                Creature_V2 creature = creatureListAtLeftTile[i];
                if (!creature.Equals(this))
                {
                    if (creature.CollisionCheckWithPoint(leftPos) )
                    {
                        leftTileColor = creature.bodyColor;
                        break;
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
                        break;
                    }

                }
            }
        }*/

        bool spikeTargetFound = false;
        Creature_V2 spikeCreature = null;
        List<Creature_V2> creatureListAtTile = map.ExistCreaturesNearTile((int)base.position.x, (int)base.position.y);


        for (int i = 0; i < sensorPos.Length; i++)
        {
            //List<Creature_V2> creatureListAtTile = map.ExistCreaturesBetweenTiles((int)base.position.x, (int)base.position.y, (int)sensorPos[i].x, (int)sensorPos[i].y);
            sensorValue[i] = -1f;

            if (creatureListAtTile != null)
            {
                for (int j = 0; j < creatureListAtTile.Count; j++)
                {
                    Creature_V2 creature = creatureListAtTile[j];
                    if (!creature.Equals(this))
                    {
                        if (spikeTargetFound == false)
                        {
                            if (creature.IsLineIntersectingWithCircle(base.position, spikePos) == true)
                            {
                                spikeCreature = creature;
                                spikeTargetFound = true;
                            }
                        }

                        if (creature.IsLineIntersectingWithCircle(base.position, sensorPos[i]) == true)
                        {
                            sensorValue[i] = creature.bodyColor.h;
                            break;
                        }
                    }
                }
            }

            /*List<Creature_V2> creatureListAtTile1 = map.ExistCreatureAtTile((int)base.position.x, (int)base.position.y);
            List<Creature_V2> creatureListAtTile2 = map.ExistCreatureAtTile((int)sensorPos[i].x, (int)sensorPos[i].y);
            sensorValue[i] = -1f;

            if (creatureListAtTile1 != null)
            {
                for (int j = 0; j < creatureListAtTile1.Count; j++)
                {
                    Creature_V2 creature = creatureListAtTile1[j];
                    if (!creature.Equals(this))
                    {
                        if (spikeTargetFound == false)
                        {
                            if (creature.IsLineIntersectingWithCircle(base.position, spikePos) == true)
                            {
                                spikeCreature = creature;
                                spikeTargetFound = true;
                            }
                        }

                        if (creature.IsLineIntersectingWithCircle(base.position,sensorPos[i]) == true)
                        {
                            sensorValue[i] = creature.bodyColor.h;
                            break;
                        }
                    }
                }
            }

            if (creatureListAtTile2 != null && sensorValue[i] == -1f)
            {
                for (int j = 0; j < creatureListAtTile2.Count; j++)
                {
                    Creature_V2 creature = creatureListAtTile2[j];
                    if (!creature.Equals(this))
                    {
                        if (spikeTargetFound == false)
                        {
                            if (creature.IsLineIntersectingWithCircle(base.position, spikePos) == true)
                            {
                                spikeCreature = creature;
                                spikeTargetFound = true;
                            }
                        }

                        if (creature.IsLineIntersectingWithCircle(base.position,sensorPos[i]) == true)
                        {
                            sensorValue[i] = creature.bodyColor.h;
                            break;
                        }

                    }
                }
            }*/
        }



        List<Creature_V2> creatureListAtBodyTile = map.ExistCreaturesNearPrecisionTile(base.position.x, base.position.y, radius/**2f*/);
        float hueAverage = -1f;
        float isCollision = -1;

        //check for left sensor collision
        if (creatureListAtBodyTile != null)
        {
            for (int i = 0; i < creatureListAtBodyTile.Count; i++)
            {
                Creature_V2 creature = creatureListAtBodyTile[i];
                if (!creature.Equals(this))
                {
                    if (creature.CollisionCheckWithCircle(radius /** 2f*/, base.position) == true)
                    {
                        hueAverage += creature.GetBodyHue();
                        isCollision++;
                    }
                }
            }
        }

        if (isCollision >= 0)
        {
            isCollision++;
            hueAverage = hueAverage / isCollision;
        }


        float[] output = brain.Feedforward(new float[] {sensorValue[0], sensorValue[1], sensorValue[2], sensorValue[3],
            isCollision,hueAverage, bodyTileColor.h, bodyTileColor.s, leftTileColor.h, leftTileColor.s, rightTileColor.h, rightTileColor.s, energy.GetEnergy(), previousOutput[7], previousOutput[8] });

        float accelForward = output[0];
        float accelAngular = output[1];
        float bodyHue = output[2];
        float mouthHue = output[3];

        if (output[6] >= 0)
            spikeLength = output[6];
        else
            spikeLength = 0;

        if (bodyHue>= 0)
            this.bodyColor.h = bodyHue /*= new HSBColor(bodyHue,1f,1f)*/;

        if (mouthHue >= 0)
            this.mouthColor.h = mouthHue /* = new HSBColor(mouthHue, 1f, 1f)*/; 

        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);

        base.UpdateColliderPhysics(accelForward,accelAngular);

        UpdateSensors();

        //copy tile detail
        tileDetail[0] = (int)base.position.x;
        tileDetail[1] = (int)base.position.y;

        map.AddCreatureToTileList(tileDetail[0], tileDetail[1], this);


        energy.UpdateCreatureEnergy(tileDetail[0], tileDetail[1], output, bodyTileColor.h, mouthColor.h, /*collisions*/spikeCreature);

        if (energy.GetEnergy() < 1f)
            base.radius = initialRadius - ((1f-energy.GetEnergy())*(initialRadius/2));
        else
            base.radius = initialRadius+(energy.GetEnergy()*0.0025f);

        // Creature is dead ;( D: :( -_-  ;_;
        if (energy.IsAlive() == false)
        {
            Kill();
        }
        else if (energy.GiveBirth() == true)
        {
            energy.GiveBirth(false);
            world.CreateCreature(this);
        }

    }

    // Find the points of intersection.
    private int FindLineCircleIntersections(
        float cx, float cy, float radius,
        Vector2 point1, Vector2 point2,
        out Vector2 intersection1, out Vector2 intersection2)
    {
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
        C = (point1.x - cx) * (point1.x - cx) +
            (point1.y - cy) * (point1.y - cy) -
            radius * radius;

        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        else if (det == 0)
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 1;
        }
        else
        {
            // Two solutions.
            t = (float)((-B + Math.Sqrt(det)) / (2 * A));
            intersection1 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);
            t = (float)((-B - Math.Sqrt(det)) / (2 * A));
            intersection2 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);
            return 2;
        }
    }

    private void UpdateSensors()
    {
        float fixedRotation = base.rotation + 90f;
        float leftAngle = (((fixedRotation) + 25f) * Mathf.Deg2Rad);
        float rightAngle = (((fixedRotation) - 25f) * Mathf.Deg2Rad);
        //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
        //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);

        //left and right position calculation
        leftPos = base.position + new Vector3(sensorSize * Mathf.Cos(leftAngle), sensorSize * Mathf.Sin(leftAngle), 0f);
        rightPos = base.position + new Vector3(sensorSize * Mathf.Cos(rightAngle), sensorSize * Mathf.Sin(rightAngle), 0f);

        float startRotation = fixedRotation - 16.875f;
        float addRotation = 11.25f;
        for (int i = 0; i < sensorPos.Length; i++)
        {
            sensorPos[i] = base.position + new Vector3(sensorSize * Mathf.Cos(startRotation* Mathf.Deg2Rad) *2.5f, sensorSize * Mathf.Sin(startRotation* Mathf.Deg2Rad) * 2.5f, 0f);
            startRotation += addRotation;
        }

        spikePos = base.position + new Vector3(sensorSize * Mathf.Cos(fixedRotation * Mathf.Deg2Rad) * spikeLength * 2.5f, sensorSize * Mathf.Sin(fixedRotation * Mathf.Deg2Rad) * spikeLength * 2.5f, 0f);

    }

    public void UpdateRender()
    {
        float[] output = brain.GetOutput();
        leftLine.SetPosition(0, position);
        leftLine.SetPosition(1, leftPos);
        rightLine.SetPosition(0, position);
        rightLine.SetPosition(1, rightPos);

        for (int i = 0; i < sensorLine.Length; i++)
        {
            sensorLine[i].SetPosition(0, position);
            sensorLine[i].SetPosition(1, sensorPos[i]);
        }

        /*for (int i = 0; i < sensorTrigger.Length; i++)
        {
            if (sensorTrigger[i] == true)
            {
                sensorLine[i].SetColors(Color.red, Color.red);
            }
            else
            {
                sensorLine[i].SetColors(Color.white, Color.white);
            }
        }*/


        if (output[4] > 0)
        {
            leftLine.SetColors(Color.black,Color.black);
            rightLine.SetColors(Color.black, Color.black);
        }
        else
        {
            leftLine.SetColors(Color.white,Color.white);
            rightLine.SetColors(Color.white, Color.white);
        }

        /*if (output[6] > 0)
        {
            //outlineTrans.localScale = new Vector3(2f, 2f, 1f);
            fightTrans.localScale = new Vector3(1.6f, 1.6f, 1f);
            fightTrans.GetComponent<Renderer>().material.color = new Color(1f,0f,0f,0.5f);
        }
        else
        {
            fightTrans.localScale = new Vector3(1.075f, 1.075f, 1f);
            fightTrans.GetComponent<Renderer>().material.color = Color.black;
        }*/

        spikeLine.SetPosition(0, position);
        spikeLine.SetPosition(1, spikePos);
        spikeLine.SetColors(Color.red,Color.red);

        bodyMaterial.color = bodyColor.ToColor();
        mouthMaterial.color = mouthColor.ToColor();
        trans.position = position;

        trans.eulerAngles = new Vector3(0f, 0f, rotation);
        rotation = trans.eulerAngles.z; //resets it back to 0-360

        textTrans.eulerAngles = new Vector3(0, 0, 0f);
        textTrans.position = position + new Vector3(0f, 0.4f, 0f);

        if (isNode == true)
        {
            trans.localScale = new Vector3(radius * 12f, radius * 12f, 1f);
            textTrans.localScale = new Vector3(4f,4f,4f);
        }
        else
        {
            trans.localScale = new Vector3(radius*2f, radius*2f, 1f);
            textTrans.localScale = new Vector3(1f, 1f, 1f);
        }


    }

    public Brain_V2 GetBrain()
    {
        return brain;
    }

    public float GetBodyHue()
    {
        return bodyColor.h;
    }

    public void KillWithEnergy()
    {
        energy.SetKillEnergy();
    }

    public void RemoveEnergy(float energyDamage)
    {
        energy.RemoveEnergy(energyDamage);
    }

    public float GetEnergy()
    {
       return energy.GetEnergy();
    }

    public bool IsAlive()
    {
        return energy.IsAlive();
    }

    public void Kill()
    {
        //map.AddEnergyToTile(tileDetail[0], tileDetail[1],0.25f);
        children.Clear();
        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);
        world.RemoveCreature(this);
        GameObject.Destroy(trans.gameObject);
    }

    public int GetGeneration()
    {
        return generation;
    }

    public void AddChildren(Creature_V2 child)
    {
        children.Add(child);
        childCount++;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        Creature_V2 otherCreature = obj as Creature_V2;
        if (otherCreature != null)
            return this.ID.CompareTo(otherCreature.ID);
        else
            throw new ArgumentException("Object is not a Temperature");
    }

    public int GetID()
    {
        return ID;
    }

    public List<Creature_V2> GetChildren()
    {
        return children;
    }

    public int GetChildCount()
    {
        return childCount;
    }

    public string GetName()
    {
        return brain.GetName();
    }

    public void SetIsNode(bool isNode)
    {
        this.isNode = isNode;
    }

    public bool IsNode()
    {
        return this.isNode;
    }



}


