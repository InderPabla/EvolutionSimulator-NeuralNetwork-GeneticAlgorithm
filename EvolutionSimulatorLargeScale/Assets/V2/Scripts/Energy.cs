using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Energy
{

    private bool giveBirth = false;
    private int birthFrameCounter = 0;
    private bool isAlive = true;
    private float initialEnergy;
    private float currentEnergy;
    private float deltaEnergy;
    private float worldDeltaTime;
    private TileMap_V2 map;

    private const float MIN_BRITH_ENERGY = 2f;

    private float maturity = 0f;
    private const float MIN_FIGHT_MATURITY = 0.25f;

    private float life = 1f;
    private float minLife = 5f;
    private float lifeDecerase = 0.7f;
    private float eatDamage = 0f;
    private float veloDamage = 3f;
    private float angDamage = 0.5f;
    private float fightDamage =  0.5f;

    public Energy(float initialEnergy, TileMap_V2 map, float worldDeltaTime, float minLife, float lifeDecerase, float eatDamage, float veloDamage, float angDamage, float fightDamage)
    {
        this.initialEnergy = initialEnergy;
        this.currentEnergy = initialEnergy;
        this.map = map;
        this.worldDeltaTime = worldDeltaTime;
        this.eatDamage = eatDamage;
        this.veloDamage = veloDamage;
        this.angDamage = angDamage;
        this.fightDamage = fightDamage;

        this.minLife = minLife;
        this.lifeDecerase = lifeDecerase;
    }

    public void UpdateCreatureEnergy(int x, int y, float[] output, float groundHue, float mouthHue, Creature_V2 spikeCreature)
    {
        float accelForward = output[0];
        float accelAngular = output[1];
        float eatFood = output[4];
        float birth = output[5];
        float fight = output[6];

        deltaEnergy = 0f;

        deltaEnergy -= (worldDeltaTime / 5f) * (Mathf.Sqrt(Mathf.Max(currentEnergy / initialEnergy, 1f)));  //natural energy loss (creature will die in 5 years)
        deltaEnergy -= (Mathf.Abs(accelForward) * worldDeltaTime) / veloDamage/*5f3f*/; //if creature keep moving at max speed it will die in 5 years
        deltaEnergy -= (Mathf.Abs(accelAngular) * worldDeltaTime) / angDamage/*1f0.5f*/; //if creature keeps turing at max acceleration it will die in 2 years
                                                                        // At worst if the creatures keep turning and moving at max rate it will die in 1.1 year

        int tileType = map.GetTileType(x, y);
        if (tileType == Tile_V2.TILE_WATER)
        {
            deltaEnergy -= worldDeltaTime * 20f;
        }
        else if (eatFood > 0 && tileType != Tile_V2.TILE_INFERT)
        {
            deltaEnergy += map.Eat(x, y) / 1.1f;

            // 1 (cirmum) = 2*pi*r 
            // 1/(pi*2) = r = 0.15915494309189533576888376337251
            // turn r = 1 will make c = 2*pi
            // lenght of two chords combined = 2*sin(theta/2)

            float mouthX = Mathf.Cos(mouthHue * Mathf.PI * 2);
            float mouthY = Mathf.Sin(mouthHue * Mathf.PI * 2);
            float groundX = Mathf.Cos(groundHue * Mathf.PI * 2);
            float groundY = Mathf.Sin(groundHue * Mathf.PI * 2);
            float dist = Mathf.Pow(Mathf.Pow(mouthX - groundX, 2) + Mathf.Pow(mouthY - groundY, 2), 0.5f);

            deltaEnergy -= (dist * worldDeltaTime * eatDamage); //2 is good
        }

        if (fight > 0 && /*maturity > MIN_BIRTH_MATURITY &&*/ spikeCreature != null && maturity> MIN_FIGHT_MATURITY)
        {
            
            float enemyEnergy = spikeCreature.GetEnergy();
            if (enemyEnergy > 0f)
            {
                float energySuck = fightDamage * (Mathf.Max(currentEnergy,0f));

                enemyEnergy -= energySuck;
                if (enemyEnergy < 0f)
                    energySuck = (energySuck + enemyEnergy)+0.005f;

                deltaEnergy += (energySuck / 1.25f);
                spikeCreature.RemoveEnergy(energySuck);
            }

            //deltaEnergy -= (worldDeltaTime) / 1f;
        }


        if (currentEnergy > MIN_BRITH_ENERGY && birth > 0f && birthFrameCounter == 0)
        {
            birthFrameCounter++;
        }
        else if (birthFrameCounter <6 && birthFrameCounter>0)
        {
            birthFrameCounter++;
        }
        else if(birthFrameCounter == 6)
        {
            deltaEnergy -= 1.5f;
            giveBirth = true;
            birthFrameCounter = 0;
        }

        currentEnergy += deltaEnergy;
        if (currentEnergy <= 0f || life <= 0)
            isAlive = false;

        maturity += worldDeltaTime;

        //life -= ((worldDeltaTime /3) / Mathf.Pow((Mathf.Max(currentEnergy, 1f) / initialEnergy), 0.25f));
        life -= ((worldDeltaTime /minLife) / Mathf.Pow(Mathf.Max(currentEnergy, 1f), lifeDecerase));

        //life -= (worldDeltaTime / (5 + (Mathf.Max(currentEnergy, 0f))*2f));
    }


    public float GetEnergy()
    {
        return currentEnergy;
    }

    public float GetDeltaEnergy()
    {
        return deltaEnergy;
    }

    public float GetLife()
    {
        return life;
    }

    public void RemoveEnergy(float energy)
    {
        currentEnergy -= energy;
    }

    public bool GiveBirth()
    {
        return giveBirth;
    }

    public void DoneBirth()
    {
        giveBirth = false;
        birthFrameCounter = 0;
    }

    public bool IsAbleToGiveBirth()
    {
        return birthFrameCounter > 0;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void SetKillEnergy()
    {
        currentEnergy = -10000f;

    }

    public void SetEnergy(float currentEnergy)
    {
        this.currentEnergy = currentEnergy;
    }

    public void SetLife(float life)
    {
        this.life = life;
    }
}
