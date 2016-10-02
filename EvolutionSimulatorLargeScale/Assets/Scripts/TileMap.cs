using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

struct Tile
{
    public const int TILE_RED = 0;
    public const int TILE_GREEN = 1;
    public const int TILE_BLUE = 2;
    public const int TILE_ORANGE = 3;
    public const int TILE_PURPLE = 4;
    public const int TILE_INFERTIAL = 5;
    public const int TILE_WATER = 6;

    public int type;
    public HSBColor detail;
    public float maxEnergy;
    public float currentEnergy;

}
public class TileMap
{

    Tile[,] tiles;
    Texture2D texture;
    int sizeX;
    int sizeY;
    float worldDeltaTime = 0.033333333f; //each year last
    float climate = 0.1f; //1 is excellent climate for growth, 0 means nothing will grow, and below zero, vegetation starts to die
    
    List<int[]> floorTiles = new List<int[]>();

    public TileMap(Texture2D tex, int sizeX, int sizeY)
    {
        Color[] texColor = tex.GetPixels(0,0,sizeX,sizeY);
        this.texture = tex;
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        tiles = new Tile[sizeY, sizeX];
        int colorIndex = 0;
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                float r = texColor[colorIndex].r;
                float g = texColor[colorIndex].g;
                float b = texColor[colorIndex].b;

                tiles[y, x].detail = new HSBColor(texColor[colorIndex]);

                if (r == 0 && g == 0 && b == 0)
                {
                    tiles[y, x].type = Tile.TILE_WATER;
                    tiles[y, x].maxEnergy = 0f;
                    tiles[y, x].detail.b = 1f;
                }
                else if (r == 1 && g == 1 && b == 1)
                {
                    tiles[y, x].type = Tile.TILE_INFERTIAL;
                    tiles[y, x].maxEnergy = 0f;
                    tiles[y, x].detail.b = 1f;

                }
                else
                { 
                    if (r == 1 && g == 0 && b == 0)
                    {
                        tiles[y, x].type = Tile.TILE_RED;
                        tiles[y, x].detail.b = 1f;
                        tiles[y, x].maxEnergy = 100f;
                    }
                    else if (r == 0 && g == 1 && b == 0)
                    {
                        tiles[y, x].type = Tile.TILE_GREEN;
                        tiles[y, x].detail.b = 1f;
                        tiles[y, x].maxEnergy = 100f;
                    }
                    else if (r == 0 && g == 0 && b == 1)
                    {
                        tiles[y, x].type = Tile.TILE_BLUE;
                        tiles[y, x].detail.b = 1f;
                        tiles[y, x].maxEnergy = 100f;
                    }
                    else if (r == b)
                    {
                        tiles[y, x].type = Tile.TILE_PURPLE;
                        tiles[y, x].detail.b = 1f;
                        tiles[y, x].maxEnergy = 100f;
                        tiles[y, x].detail = new HSBColor(0.7777778f, 1f, 1f);
                    }
                    else
                    {
                        tiles[y, x].type = Tile.TILE_ORANGE;
                        tiles[y, x].detail.b = 1f;
                        tiles[y, x].maxEnergy = 100f;
                    }
                    floorTiles.Add(new int[] {x,y});
                }

                
                tiles[y, x].currentEnergy = tiles[y, x].maxEnergy;

                colorIndex++;
            }
        }
    }


    bool check = false;

    public void Apply()
    {
        for (int y = 0; y < sizeY;  y++)
        {
            for (int x = 0; x < sizeX; x++)
            {

                tiles[y, x].currentEnergy += climate*Time.deltaTime*10f;

                if (tiles[y, x].currentEnergy > tiles[y, x].maxEnergy)
                    tiles[y, x].currentEnergy = tiles[y, x].maxEnergy;
                else if(tiles[y, x].currentEnergy < 0f)
                    tiles[y, x].currentEnergy = 0f;

                if (tiles[y, x].type != Tile.TILE_INFERTIAL && tiles[y, x].type != Tile.TILE_WATER)
                {
                    float saturationToEnergyRatio = tiles[y, x].currentEnergy / tiles[y, x].maxEnergy;
                    tiles[y, x].detail.s = saturationToEnergyRatio;
                    tiles[y, x].detail.b = 1f-(0.25f-(saturationToEnergyRatio * 0.25f));

                    texture.SetPixel(x, y, tiles[y, x].detail.ToColor());
                }
            }
        }

        texture.Apply();
    }

    public float GetWorldDeltaTime()
    {
        return worldDeltaTime;
    }

    public HSBColor GetColor(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
            //return texture.GetPixel(x, y);
            return tiles[y, x].detail;

        return HSBColor.FromColor(Color.black);
            
    }

    public float Eat(int x,int y)
    {
        float energy = 0;

        if (IsValidLocation(x, y) == true)
        {
            if (tiles[y, x].currentEnergy > 0)
            {
                energy = Time.deltaTime * 100f;

                tiles[y, x].currentEnergy -= energy;

                if (tiles[y, x].currentEnergy < 0)
                {
                    energy += tiles[y, x].currentEnergy;
                    tiles[y, x].currentEnergy = 0;
                }
                
                        
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

    public string TileToString(int x, int y)
    {
        return "X: " + x + "\nY: " + y + "\nE: " + String.Format("{0:###.00}", tiles[y, x].currentEnergy) + "\nC: " + String.Format("{0:###.00}", climate);
    }

    public int[] RandomFloorTile()
    {
        return floorTiles[UnityEngine.Random.Range(0,floorTiles.Count)];
    }

    public int GetTileType(int x, int y)
    {
        if (IsValidLocation(x, y) == true)
        {
            return tiles[y,x].type;
        }
        return Tile.TILE_WATER;
    }
}
