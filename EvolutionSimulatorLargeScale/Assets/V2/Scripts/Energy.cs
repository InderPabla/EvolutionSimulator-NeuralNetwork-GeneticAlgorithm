using UnityEngine;
using System.Collections;

public class Energy  {

    private bool giveBirth = false;
    private float intialEnergy;
    private float currentEnergy;
    private float deltaEnergy;
    private float worldDeltaTime;
    private TileMap_V2 map;
    private float minGiveBirthEnergy;

    public Energy(float initialEnergy, TileMap_V2 map, float worldDeltaTime)
    {
        this.intialEnergy = initialEnergy;
        this.currentEnergy = initialEnergy;
        this.map = map;
        this.worldDeltaTime = worldDeltaTime;
        this.minGiveBirthEnergy = 2f;
    }

    public void UpdateCreatureEnergy(int x, int y, float[] output)
    {
        float accelForward = output[0];
        float accelAngular = output[1];
        float mouthHue = output[3];
        float eatFood = output[4];
        float giveBrith = output[5];
        float fight = output[6];

        if (eatFood > 0) {
            map.Eat(x,y);
        }
    }


    public float GetEnergy()
    {
        return currentEnergy;
    }

    public void NaturalEnergyLoss(float factor)
    {
        //deltaEnergy -= (((Time.fixedDeltaTime * worldDeltaTime * 100f) * (currentRadius / initialRadius)) * factor);
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

    public void ApplyDeltaEnergy()
    {
        currentEnergy += deltaEnergy;
    }

    public void BirthEnergyLoss()
    {
        deltaEnergy -= 150f;
    }

    public bool GiveBirth()
    {
        return giveBirth;
    }
}
