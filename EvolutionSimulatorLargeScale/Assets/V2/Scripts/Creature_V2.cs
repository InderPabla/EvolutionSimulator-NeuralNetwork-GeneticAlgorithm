using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Creature_V2 : CustomCircleCollider, IEquatable<Creature_V2>, IComparable
{
    public Transform trans = null; //Transform of this object
    private TextMesh textMesh = null;
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
    private float timeLived = 0f;

    private TileMap_V2 map;
    private Material bodyMaterial;
    private Material mouthMaterial;

    private float sensorSize;
    private Energy energy;
    private WolrdManager_V2 world;

    private List<Creature_V2> children;
    private int childCount = 0;
    private bool isNode = false;
    private string parentNames = "";

    public Creature_V2(int ID, int generation, Transform trans, LineRenderer leftLine, LineRenderer rightLine, LineRenderer[] sensorLine, LineRenderer spikeLine,
                       Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, float sensorSize,
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy, float currentEnergy, float life, float minLife, float lifeDecerase, float veloForward, float veloAngular,
                       TileMap_V2 map, WolrdManager_V2 world, String parentNames) 
                       : base(initialRadius,bodyPos,angle,veloForward,veloAngular,1f,worldDeltaTime)
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
        this.parentNames = parentNames;

        this.textMesh = trans.GetChild(1).GetComponent<TextMesh>();
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

        this.energy = new Energy(initialEnergy, map, worldDeltaTime, minLife, lifeDecerase);
        this.energy.SetEnergy(currentEnergy);
        this.energy.SetLife(life);

        this.bodyColor = new HSBColor(UnityEngine.Random.Range(0f,1f),1f,1f);
        this.mouthColor = new HSBColor(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
    }

    public bool Equals(Creature_V2 other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }

    public void UpdateCreaturePhysics()
    {

        float[] previousOutput = brain.GetOutput();
        
        HSBColor bodyTileColor = map.GetColor((int)base.position.x, (int)base.position.y);
        HSBColor leftTileColor = map.GetColor((int)leftPos.x, (int)leftPos.y);
        HSBColor rightTileColor = map.GetColor((int)rightPos.x, (int)rightPos.y);

        bool spikeTargetFound = false;
        float spikeValue = -1;
        Creature_V2 spikeCreature = null;
        List<Creature_V2> creatureListAtTile = map.ExistCreaturesNearTile((int)base.position.x, (int)base.position.y);
        float[] sensorDistance = new float[4];

        for (int i = 0; i < sensorPos.Length; i++)
        {
            sensorValue[i] = -1f;
            sensorDistance[i] = -1f;

            if (creatureListAtTile != null)
            {
                for (int j = 0; j < creatureListAtTile.Count; j++)
                {
                    Creature_V2 creature = creatureListAtTile[j];
                    if (!creature.Equals(this))
                    {
                        float distance;
                        if (spikeTargetFound == false)
                        {
                            distance = creature.IsLineIntersectingWithCircle(base.position, spikePos);
                            if (distance != -1f)
                            {
                                spikeCreature = creature;
                                spikeValue = spikeCreature.bodyColor.h;
                                spikeTargetFound = true;
                            }
                        }

                        distance = creature.IsLineIntersectingWithCircle(base.position, sensorPos[i]);

                        if (distance != -1f)
                        {
                            sensorValue[i] = creature.bodyColor.h;
                            sensorDistance[i] = distance;
                            break;
                        }
                    }
                }
            }
        }

        List<Creature_V2> creatureListAtBodyTile = map.ExistCreaturesNearPrecisionTile(base.position.x, base.position.y, base.radius);
        List<Creature_V2> creaturesInBirthRange = new List<Creature_V2>();
        float hueAverage = -1f;
        float isCollision = -1f;

        //check for left sensor collision
        if (creatureListAtBodyTile != null)
        {
            for (int i = 0; i < creatureListAtBodyTile.Count; i++)
            {
                Creature_V2 creature = creatureListAtBodyTile[i];
                if (!creature.Equals(this))
                {
                    if (creature.CollisionCheckWithCircle(radius, base.position) == true)
                    {
                        hueAverage += creature.bodyColor.h;
                        isCollision++;
                        creaturesInBirthRange.Add(creature);
                    }
                    else if (creature.CollisionCheckWithCircle(radius*2f, base.position))
                    {
                        creaturesInBirthRange.Add(creature);
                    }
                }
            }
        }

        if (isCollision >= 0)
        {
            isCollision++;
            hueAverage = hueAverage / isCollision;
            isCollision = 1f;
        }


        float[] output = brain.Feedforward(new float[] {sensorDistance[0],sensorDistance[1], sensorDistance[2], sensorDistance[3],
            sensorValue[0], sensorValue[1], sensorValue[2], sensorValue[3], spikeValue,
            isCollision, hueAverage, bodyTileColor.h, bodyTileColor.s, leftTileColor.h, leftTileColor.s, rightTileColor.h, rightTileColor.s,
            base.radius, previousOutput[7], previousOutput[8] });

        float accelForward = output[0];
        float accelAngular = output[1];
        float bodyHue = output[2];
        float mouthHue = output[3];

        if (output[6] >= 0)
            spikeLength = output[6];
        else
            spikeLength = 0;

        if (bodyHue>= 0)
            this.bodyColor.h = bodyHue;

        if (mouthHue >= 0)
            this.mouthColor.h = mouthHue; 

        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);

        base.UpdateColliderPhysics(accelForward,accelAngular);

        UpdateSensors();

        //copy tile detail
        tileDetail[0] = (int)base.position.x;
        tileDetail[1] = (int)base.position.y;

        map.AddCreatureToTileList(tileDetail[0], tileDetail[1], this);

        energy.UpdateCreatureEnergy(tileDetail[0], tileDetail[1], output, bodyTileColor.h, mouthColor.h, spikeCreature);

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
            energy.DoneBirth();
            world.CreateCreature(this);
        }
        else if (energy.IsAbleToGiveBirth() == true)
        {
            for (int i = 0; i < creaturesInBirthRange.Count; i++)
            {
                Creature_V2 creature = creaturesInBirthRange[i];
                if (creature.energy.IsAbleToGiveBirth())
                {
                    energy.RemoveEnergy(1f);
                    creature.energy.RemoveEnergy(1f);
                    energy.DoneBirth();
                    creature.energy.DoneBirth();

                    world.CreateCreature(this, creature);
                    break;
                }
                else
                {
                    energy.RemoveEnergy(1.25f);
                    creature.energy.RemoveEnergy(0.25f);

                    energy.DoneBirth();
                    creature.energy.DoneBirth();

                    world.CreateCreature(this, creature);
                    break;
                }
            }
        }
        

        timeLived += worldDeltaTime;
    }


    private void UpdateSensors()
    {
        float fixedRotation = base.rotation + 90f;
        float leftAngle = (((fixedRotation) + 25f) * Mathf.Deg2Rad);
        float rightAngle = (((fixedRotation) - 25f) * Mathf.Deg2Rad);

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

    public void UpdateRender(bool visionVisual)
    {
        /*if (parentNames.Contains("@"))
        {
            Debug.Log(parentNames+"  "+(int)position.x+","+(int)position.y);
        }*/

        float[] output = brain.GetOutput();

        leftLine.SetPosition(0, position);
        leftLine.SetPosition(1, leftPos);
        rightLine.SetPosition(0, position);
        rightLine.SetPosition(1, rightPos);

        if (output[4] > 0)
        {
            leftLine.SetColors(Color.black, Color.black);
            rightLine.SetColors(Color.black, Color.black);
        }
        else
        {
            leftLine.SetColors(Color.white, Color.white);
            rightLine.SetColors(Color.white, Color.white);
        }


        if (visionVisual)
        {
            for (int i = 0; i < sensorLine.Length; i++)
            {
                sensorLine[i].SetPosition(0, position);
                sensorLine[i].SetPosition(1, sensorPos[i]);
            }

            for (int i = 0; i < sensorValue.Length; i++)
            {
                if (sensorValue[i] >= 0)
                {
                    sensorLine[i].SetColors(Color.red, Color.red);
                }
                else
                {
                    sensorLine[i].SetColors(Color.white, Color.white);
                }
            }
        }
        else
        {
            for (int i = 0; i < sensorLine.Length; i++)
            {
                sensorLine[i].SetPosition(0, Vector2.zero);
                sensorLine[i].SetPosition(1, Vector2.zero);
            }
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

        textMesh.transform.eulerAngles = new Vector3(0, 0, 0f);
        textMesh.transform.position = position + new Vector3(0f, 0.4f, 0f);

        if (isNode == true)
        {
            //trans.localScale = new Vector3(radius * 12f, radius * 12f, 1f);
            //textTrans.localScale = new Vector3(4f,4f,4f);

            textMesh.transform.localScale = new Vector3(2f, 2f, 2f);
            textMesh.color = Color.red;
        }
        else
        {
            trans.localScale = new Vector3(radius*2f, radius*2f, 1f);
            textMesh.transform.localScale = new Vector3(1f, 1f, 1f);
            textMesh.color = Color.white;
        }


    }

    public Brain_V2 GetBrain()
    {
        return brain;
    }

    public float GetLife()
    {
        return energy.GetLife();
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

    public float GetDeltaEnergy()
    {
        return energy.GetDeltaEnergy();
    }

    public float GetTimeLived()
    {
        return timeLived;
    }

    public bool IsAlive()
    {
        return energy.IsAlive();
    }

    public String GetParentNames()
    {
        return parentNames;
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


