using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Creature_V2 : CustomCircleCollider, IEquatable<Creature_V2>
{
    private Transform trans = null; //Transform of this object
    private LineRenderer leftLine = null;
    private LineRenderer rightLine = null;

    private Brain_V2 brain = null;

    private HSBColor bodyColor;
    private HSBColor mouthColor;

    private Vector3 leftPos;
    private Vector3 rightPos;

    private int[] tileDetail = new int[2];

    private float initialRadius = 0.06f;
    private float currentRadius = 0.06f;

    private int ID = -1;
    private float worldDeltaTime = 0.001f;

    private TileMap_V2 map;
    private Material bodyMaterial;
    private Material mouthMaterial;

    private float sensorSize;
    private Energy energy;
    private WolrdManager_V2 world;

    public Creature_V2(int ID, Transform trans, LineRenderer leftLine, LineRenderer rightLine, 
                       Brain_V2 brain, HSBColor bodyColor, Vector3 bodyPos, Vector3 leftPos, Vector3 rightPos, float sensorSize,
                       float angle, float worldDeltaTime, float initialRadius, float initialEnergy, TileMap_V2 map, WolrdManager_V2 world) 
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
        this.sensorSize = sensorSize;
        this.worldDeltaTime = worldDeltaTime;
        this.initialRadius = initialRadius;
        this.map = map;
        this.world = world;

        //energyDensity = 1f/(Mathf.PI * initialRadius * initialRadius);

        this.bodyMaterial = trans.GetComponent<Renderer>().material;
        this.mouthMaterial = trans.GetChild(0).GetComponent<Renderer>().material;

        this.energy = new Energy(initialEnergy, map, worldDeltaTime);
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
        this.bodyColor = new HSBColor(bodyHue,1f,1f);
        this.mouthColor = new HSBColor(mouthHue, 1f, 1f); 

        map.RemoveCreatureFromTileList(tileDetail[0], tileDetail[1], this);

        base.UpdateColliderPhysics(accelForward,accelAngular);

        UpdateSensors();

        //copy tile detail
        tileDetail[0] = (int)base.position.x;
        tileDetail[1] = (int)base.position.y;

        map.AddCreatureToTileList(tileDetail[0], tileDetail[1], this);


        energy.UpdateCreatureEnergy(tileDetail[0], tileDetail[1], output);

        // Creature is dead ;( D: :( -_-  ;_;
        if (energy.IsAlive() == false)
        {
            world.RemoveCreature(this);
            GameObject.Destroy(trans.gameObject);
        }
        else if (energy.GiveBirth() == true) {
            energy.GiveBirth(false);
            world.CreateCreature(this);
        }
    }

    private void UpdateSensors()
    {
        float leftAngle = (((base.rotation + 90f) + 25f) * Mathf.Deg2Rad);
        float rightAngle = (((base.rotation + 90f) - 25f) * Mathf.Deg2Rad);
        //float leftAngle = (((angle+90f)+25f) * Mathf.Deg2Rad) + ((Mathf.PI/10f)*output[2]);
        //float rightAngle = (((angle+90f)-25f) * Mathf.Deg2Rad)+ ((Mathf.PI/10f) *output[3]);

        //left and right position calculation
        leftPos = base.position + new Vector3(sensorSize * Mathf.Cos(leftAngle), sensorSize * Mathf.Sin(leftAngle), 0f);
        rightPos = base.position + new Vector3(sensorSize * Mathf.Cos(rightAngle), sensorSize * Mathf.Sin(rightAngle), 0f);
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
        trans.eulerAngles = new Vector3(0f, 0f, rotation);
        rotation = trans.eulerAngles.z; //resets it back to 0-360
    }

    public Brain_V2 GetBrain()
    {
        return brain;
    }

}

