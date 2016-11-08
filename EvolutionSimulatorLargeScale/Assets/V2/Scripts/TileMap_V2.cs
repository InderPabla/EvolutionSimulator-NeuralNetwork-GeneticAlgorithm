using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tile_V2
{

    public const int TILE_FERT = 0;
    public const int TILE_INFERT = 1;
    public const int TILE_WATER = -1;

    public int type;
    public HSBColor detail;
    public float maxEnergy;
    public float currentEnergy;

    public List<Creature_V2> creatureListOnTile = new List<Creature_V2>();

    public float lastUpdated = 0;
    public bool selected = false;
}

public class TileMap_V2
{
    private Tile_V2[,] tiles;
    private Texture2D texture;
    private int sizeX;
    private int sizeY;
    float worldDeltaTime = 0.001f; //each year last
    private float maxEnergyGrownOnTile = 0.8f;
    private float climate = 3f; //1 is excellent climate for growth, 0 means nothing will grow, and below zero, vegetation starts to die
    private List<int[]> floorTiles = new List<int[]>();

    private float seedSoilFracture = 1f;
    private float seedSoilColor = 0.5f;
    private float seedWater = 1425f;
    private float seedSoil = 134f;
    private float seedSoilPower = 1.25f;
    private float seedFirt = 8925;
    private float seedSoilFirt = 5f;
    private float seedSoilFirtPower = 0.3f;

    public TileMap_V2(Texture2D tex, int sizeX, int sizeY, float climate, float worldDeltaTime, float seedSoilFracture, float seedSoilColor, float seedWater, float seedSoil, float seedSoilPower, float seedFirt, float seedSoilFirt, float seedSoilFirtPower)
    {
        this.texture = tex;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.climate = climate;
        this.worldDeltaTime = worldDeltaTime;

        tiles = new Tile_V2[sizeY, sizeX];

        /*float seedSoilFracture = 1f;
        float seedSoilColor = 0.5f;

        float seedWater = 1425f;
        float seedSoil = 134f;

        float seedSoilPower = 1.25f;

        float seedFirt = 8925;
        float seedSoilFirt = 5f;
        float seedSoilFirtPower = 0.3f;*/

        this.seedSoilFracture = seedSoilFracture;
        this.seedSoilColor = seedSoilColor;
        this.seedWater = seedWater;
        this.seedSoil = seedSoil;
        this.seedSoilPower = seedSoilPower;
        this.seedFirt = seedFirt;
        this.seedSoilFirt = seedSoilFirt;
        this.seedSoilFirtPower = seedSoilFirtPower;

        GenerateTile(seedSoilFracture, seedSoilColor, seedWater,seedSoil,seedSoilPower,seedFirt, seedSoilFirt, seedSoilFirtPower);

        

        texture.Apply();
    }

    private void GenerateTile(float seedSoilFracture, float seedSoilColor, float seedWater, float seedSoil, float seedSoilPower, float seedFertil, float seedSoilFirt, float seedSoilFirtPower)
    {
        for (int x = 0; x < sizeY; x++)
        {
            for (int y = 0; y < sizeX; y++)
            {
                tiles[y, x] = new Tile_V2();
                float climateType = 0.6f;
                float type = 1f;
                float firt = 0.8f;

                HSBColor color = new HSBColor();

                float climateX = seedSoil + x / 100f * 10f;
                float climateY = seedSoil + y / 100f * 10f;
                float waterX = seedWater + x / 100f * 15f;
                float waterY = seedWater + y / 100f * 15f;
                float fertX = seedFertil + x / 100f * 13f;
                float fertY = seedFertil + y / 100f * 13f;

                climateType = Mathf.Pow(Mathf.Min(Mathf.Abs(Mathf.PerlinNoise(climateX * seedSoilColor, climateY * seedSoilColor)),1f), seedSoilPower);
                type = Mathf.Min(Mathf.Abs(Mathf.PerlinNoise(waterX * seedSoilFracture, waterY * seedSoilFracture )), 1f);
                firt = Mathf.Pow(Mathf.Min(Mathf.Abs(Mathf.PerlinNoise(fertX * seedSoilFirt, fertY * seedSoilFirt)), 1f), seedSoilFirtPower);

                if (type > 0.4f)
                {
                    tiles[y, x].currentEnergy = firt;
                    tiles[y, x].type = Tile_V2.TILE_FERT;
                }
                else
                {
                    tiles[y, x].currentEnergy = 0f;
                    tiles[y, x].type = Tile_V2.TILE_WATER;
                }

                if (tiles[y, x].type == Tile_V2.TILE_FERT)
                {
                    color.h = Mathf.Max(0f, climateType);
                    color.s = tiles[y, x].currentEnergy;
                    color.b = 1f - (0.25f - (color.s * 0.25f));
                    tiles[y, x].maxEnergy = tiles[y, x].currentEnergy;
                    floorTiles.Add(new int[] { x, y });

                }
                else if (tiles[y, x].type == Tile_V2.TILE_INFERT)
                {
                    color = HSBColor.HSBCOLOR_WHITE;
                    tiles[y, x].currentEnergy = 0f;
                    tiles[y, x].maxEnergy = 0f;
                }
                else if (tiles[y, x].type == Tile_V2.TILE_WATER)
                {
                    color = HSBColor.HSBCOLOR_BLACK;
                    tiles[y, x].currentEnergy = 0f;
                    tiles[y, x].maxEnergy = 0f;
                }
                tiles[y, x].detail = color;
                //texture.SetPixel(x, y, tiles[y, x].detail.ToColor());

                /*int centerX = sizeX / 2;
                int centerY = sizeY / 2;
                bool isIn = Mathf.Pow(Mathf.Pow(centerX-x,2f)+ Mathf.Pow(centerY -y,2f),0.5f)<30f;

                if (isIn)
                {
                    tiles[y, x].type = Tile_V2.TILE_FERT;
                    tiles[y, x].currentEnergy = 0.8f;
                }
                else;
                {
                    tiles[y, x].type = Tile_V2.TILE_WATER;
                    tiles[y, x].currentEnergy = 0f;
                }*/
            }
        }
    }

    public void Apply(float playSpeed, bool visual)
    {

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {

                if (tiles[y, x].type != Tile_V2.TILE_INFERT && tiles[y, x].type != Tile_V2.TILE_WATER)
                {

                    float missedFramed = (WolrdManager_V2.WORLD_CLOCK - tiles[y, x].lastUpdated) / worldDeltaTime;

                    if (climate > 0)
                    {
                        if (tiles[y, x].currentEnergy < tiles[y, x].maxEnergy)
                        {
                            tiles[y, x].currentEnergy += climate * worldDeltaTime * missedFramed/*playSpeed*/;
                            if (tiles[y, x].currentEnergy > tiles[y, x].maxEnergy)
                            {
                                tiles[y, x].currentEnergy = tiles[y, x].maxEnergy;
                            }
                        }
                    }
                    else if (climate < 0)
                    {
                        if (tiles[y, x].currentEnergy > 0f)
                        {
                            tiles[y, x].currentEnergy += climate * worldDeltaTime * missedFramed /*playSpeed*/;

                            if (tiles[y, x].currentEnergy <0)
                            {
                                tiles[y, x].currentEnergy = 0f;
                            }
                        }
                    }

                    if (tiles[y, x].currentEnergy < 0f)
                        tiles[y, x].currentEnergy = 0f;
                    else if (tiles[y, x].currentEnergy > 1f)
                        tiles[y, x].currentEnergy = 1f;

                    float saturationToEnergyRatio = tiles[y, x].currentEnergy;
                    tiles[y, x].detail.s = saturationToEnergyRatio;
                    tiles[y, x].detail.b = 1f - (0.25f - (saturationToEnergyRatio * 0.25f));


                    if (tiles[y, x].selected == false)
                        texture.SetPixel(x, y, tiles[y, x].detail.ToColor());
                    else
                        texture.SetPixel(x, y, Color.grey);

                    tiles[y, x].lastUpdated = WolrdManager_V2.WORLD_CLOCK;
                }
                else
                {
                    texture.SetPixel(x, y, tiles[y, x].detail.ToColor());
                }
            }
        }

       
        if (visual == true)
        {
            texture.Apply();
        }
    }

    public float GetWorldDeltaTime()
    {
        return worldDeltaTime;
    }

    public HSBColor GetColor(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
            return tiles[y, x].detail;

        return HSBColor.FromColor(Color.black);

    }
    
    public float Eat(int x, int y)
    {
        float energy = 0;

        if (IsValidLocation(x, y) == true)
        {
            if (tiles[y, x].currentEnergy > 0)
            {
                float missedFramed = (WolrdManager_V2.WORLD_CLOCK - tiles[y, x].lastUpdated)/worldDeltaTime;

                if (tiles[y, x].currentEnergy < tiles[y, x].maxEnergy)
                {
                    tiles[y, x].currentEnergy += climate * worldDeltaTime * missedFramed;
                    if (tiles[y, x].currentEnergy > tiles[y, x].maxEnergy)
                    {
                        tiles[y, x].currentEnergy = tiles[y, x].maxEnergy;
                    }
                }


                energy = worldDeltaTime *5f;

                tiles[y, x].currentEnergy -= energy;

                if (tiles[y, x].currentEnergy < 0)
                {
                    energy += tiles[y, x].currentEnergy;
                    tiles[y, x].currentEnergy = 0;
                }

                tiles[y, x].lastUpdated = WolrdManager_V2.WORLD_CLOCK;
            }
        }

        return energy;
    }

    public bool IsValidLocation(int x, int y)
    {
        if (x < 0 || x > sizeX - 1 || y < 0 || y > sizeY - 1)
            return false;

        return true;
    }

    public int[] RandomFloorTile()
    {
        return floorTiles[UnityEngine.Random.Range(0, floorTiles.Count)];
        //return new int[] {UnityEngine.Random.Range(0,sizeY), UnityEngine.Random.Range(0, sizeX) };
    }

    public int GetTileType(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
        {
            return tiles[y, x].type;
        }
        return Tile_V2.TILE_WATER;
    }

    /*public float GetTileEnergy(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
        {
            return tiles[y, x].currentEnergy;
        }
        return 0f;
    }*/
    
    public void RemoveCreatureFromTileList(int x, int y, Creature_V2 creature)
    {
        if (IsValidLocation(x, y) == true)
        {
            int index = tiles[y, x].creatureListOnTile.IndexOf(creature);
            if (index != -1)
            {
                tiles[y, x].creatureListOnTile.RemoveAt(index);
            }
        }
    }

    public void AddCreatureToTileList(int x, int y, Creature_V2 creature)
    {
        if (IsValidLocation(x, y) == true)
        {
            tiles[y, x].creatureListOnTile.Add(creature);
        }
    }

    // search in a 3x by 3x grid, (8 searches)
    public List<Creature_V2> ExistCreaturesNearTile(int x, int y)
    {
        List<Creature_V2> creatureIndexList = new List<Creature_V2>();
        if (IsValidLocation(x, y) == true)
        {
            for (int i = y - 1; i <= y + 1; i++)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (IsValidLocation(j, i) == true)
                    {
                        creatureIndexList.AddRange(tiles[i, j].creatureListOnTile);
                    }
                }
            }
        }

        return creatureIndexList;
    }

    // search in based on float coords
    public List<Creature_V2> ExistCreaturesNearPrecisionTile(float x, float y, float radius)
    {
        List<Creature_V2> creatureIndexList = new List<Creature_V2>();


        if (IsValidLocation((int)x, (int)y) == true)
        {
            int x1 = (int)(x - radius);
            int x2 = (int)(x + radius);
            int y1 = (int)(y - radius);
            int y2 = (int)(y + radius);

            for (int i = y1; i < y2+1; i++)
            {
                for (int j = x1; j < x2 + 1; j++)
                {
                    if (IsValidLocation(j, i) == true)
                    {
                        List<Creature_V2> list = tiles[i, j].creatureListOnTile;

                        creatureIndexList.AddRange(tiles[i, j].creatureListOnTile);
                    }
                }
            }
        }

        return creatureIndexList;
    }

    public List<Creature_V2> ExistCreaturesBetweenTiles(int x1, int y1, int x2, int y2)
    {
        List<Creature_V2> creatureIndexList = new List<Creature_V2>();


        if (IsValidLocation((int)x1, (int)y1) == true && IsValidLocation((int)x2, (int)y2) == true)
        {
            if (x1 > x2)
            {
                int temp = x2;
                x2 = x1;
                x1 = temp;
            }

            if (y1 > y2)
            {
                int temp = y2;
                y2 = y1;
                y1 = temp;
            }

            for (int i = y1; i < y2 + 1; i++)
            {
                for (int j = x1; j < x2 + 1; j++)
                {
                    if (IsValidLocation(j, i) == true)
                    {
                        List<Creature_V2> list = tiles[i, j].creatureListOnTile;

                        creatureIndexList.AddRange(tiles[i, j].creatureListOnTile);
                    }
                }
            }

        }

        return creatureIndexList;
    }



    //search in only the given grid, (1 search)
    public List<Creature_V2> ExistCreatureAtTile(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
        {
            return tiles[y, x].creatureListOnTile;
        }
        return null;
    }

    public string TileToString(int x, int y)
    {
        return x + "," + y + "\nE: " + String.Format("{0:###.00}", tiles[y, x].currentEnergy*100f) + "\nC: " + String.Format("{0:###.00}", climate + "\nT: "+tiles[y,x].type);
    }

    public bool IsSelected(int x, int y)
    {
        return tiles[y, x].selected;
    }

    public void SetSelected(int x, int y)
    {
        if(IsValidLocation(x,y) == true)
            tiles[y, x].selected = true ;
    }

    public void DeleteAllBodiesOnSelected()
    {
        for (int x = 0; x < sizeY; x++)
        {
            for (int y = 0; y < sizeX; y++)
            {
                if (tiles[y, x].selected == true)
                {
                    tiles[y, x].selected = false;

                    for (int i = 0; i < tiles[y, x].creatureListOnTile.Count; i++)
                    {
                        tiles[y, x].creatureListOnTile[i].KillWithEnergy();
                        //tiles[y, x].creatureListOnTile.RemoveAt(i);
                    }
                }
            }
        }
    }

    public List<Creature_V2> GetAllBodiesOnSelected()
    {
        List<Creature_V2> selectedCreatures = new List<Creature_V2>();
        for (int x = 0; x < sizeY; x++)
        {
            for (int y = 0; y < sizeX; y++)
            {
                if (tiles[y, x].selected == true)
                {
                    tiles[y, x].selected = false;
                    for (int i = 0; i < tiles[y, x].creatureListOnTile.Count; i++)
                    {
                        selectedCreatures.Add(tiles[y, x].creatureListOnTile[i]);
                    }
                }
            }
        }
        return selectedCreatures;
    }

    public void AddEnergyToTile(int x, int y, float energy)
    {
        if (IsValidLocation(x, y) == true && tiles[y,x].type == Tile_V2.TILE_FERT)
        {
            tiles[y, x].currentEnergy += energy;
            if (tiles[y, x].currentEnergy > 1f)
                tiles[y, x].currentEnergy = 1f;
        }
    }
    
}
