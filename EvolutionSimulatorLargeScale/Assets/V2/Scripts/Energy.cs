﻿using UnityEngine;
using System.Collections;

public class Energy  {

    private bool giveBirth = false;
    private bool isAlive = true;
    private float intialEnergy;
    private float currentEnergy;
    private float deltaEnergy;
    private float worldDeltaTime;
    private TileMap_V2 map;
    private float minGiveBirthEnergy;

    private const float MIN_BRITH_ENERGY = 2f;

    private float maturity = 0f;
    private float birthTimer = 0f;
    private const float MIN_BIRTH_MATURITY = 1f; //must wait at least 1 year
    private const float MIN_BIRTH_TIME = 1f; //must wait at least 1 year

    public Energy(float initialEnergy, TileMap_V2 map, float worldDeltaTime)
    {
        this.intialEnergy = initialEnergy;
        this.currentEnergy = initialEnergy;
        this.map = map;
        this.worldDeltaTime = worldDeltaTime;
        this.minGiveBirthEnergy = 2f;
    }

    public void UpdateCreatureEnergy(int x, int y, float[] output, float groundHue)
    {
        float accelForward = output[0];
        float accelAngular = output[1];
        float mouthHue = Mathf.Abs(output[3]);
        float eatFood = output[4];
        float giveBrith = output[5];
        float fight = output[6];

        deltaEnergy = 0f;

        deltaEnergy -= worldDeltaTime/5f;  //natural energy loss (creature will die in 5 years)
        deltaEnergy -= (Mathf.Abs(accelForward)*worldDeltaTime)/5f; //if creature keep moving at max speed it will die in 5 years
        deltaEnergy -= (Mathf.Abs(accelAngular)*worldDeltaTime)/2f; //if creature keeps turing at max acceleration it will die in 2 years
                                                                    // At worst if the creatures keep turning and moving at max rate it will die in 1.1 year

        int tileType = map.GetTileType(x, y);
        if (tileType == Tile_V2.TILE_WATER)
        {
            deltaEnergy -= worldDeltaTime *10f;
        }
        else if (eatFood > 0 && tileType!=Tile_V2.TILE_INFERT)
        {
            deltaEnergy += map.Eat(x,y)/2f;

            // 1 (cirmum) = 2*pi*r 
            // 1/(pi*2) = r = 0.15915494309189533576888376337251
            // turn r = 1 will make c = 2*pi
            // lenght of two chords combined = 2*sin(theta/2)

            float mouthX = Mathf.Cos(mouthHue * Mathf.PI * 2);
            float mouthY = Mathf.Sin(mouthHue * Mathf.PI * 2);
            float groundX = Mathf.Cos(groundHue * Mathf.PI * 2);
            float groundY = Mathf.Sin(groundHue * Mathf.PI * 2);
            float dist = Mathf.Pow(Mathf.Pow(mouthX - groundX, 2) + Mathf.Pow(mouthY - groundY, 2),0.5f);

            deltaEnergy -= (dist * worldDeltaTime *2f);

            /*float damage = Mathf.Abs(mouthHue - groundHue);
            if(damage>0.1f)
                deltaEnergy -= worldDeltaTime * 100f;*/
            //deltaEnergy -= damage * worldDeltaTime*25f;
        }

        

        if (currentEnergy > MIN_BRITH_ENERGY && maturity > MIN_BIRTH_MATURITY)
        {
            if (birthTimer > MIN_BIRTH_TIME)
            {
                deltaEnergy -= 1f;
                birthTimer = 0f;
                giveBirth = true;
            }

            birthTimer += worldDeltaTime;
        }

        

        currentEnergy += deltaEnergy;
        if (currentEnergy <= 0f)
            isAlive= false;

        maturity += worldDeltaTime;
    }


    public float GetEnergy()
    {
        return currentEnergy;
    }

    public bool GiveBirth()
    {
        return giveBirth;
    }

    public void GiveBirth(bool state)
    {
        giveBirth = state;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void SetKillEnergy()
    {
        currentEnergy = -10000f;

    }
}
