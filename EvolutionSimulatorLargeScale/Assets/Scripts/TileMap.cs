using UnityEngine;
using System.Collections;

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
                }
                else if (r == 1 && g == 1 && b == 1)
                {
                    tiles[y, x].type = Tile.TILE_INFERTIAL;
                    tiles[y, x].maxEnergy = 0f;
                }
                else if (r == 1 && g == 0 && b == 0)
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
                }
                else
                {
                    tiles[y, x].type = Tile.TILE_ORANGE;
                    tiles[y, x].detail.b = 1f;
                    tiles[y, x].maxEnergy = 100f;
                }

                
                tiles[y, x].currentEnergy = tiles[y, x].maxEnergy;
                //Color c = tiles[y, x].detail.ToColor();
                //if (i == 0)
                //Debug.Log(j+" "+tiles[i, j].detail.h+" "+ tiles[i, j].detail.s+" "+ tiles[i, j].detail.b+" -------- "+c.r+" "+ c.g +" "+ c.b+" ------ "+r+" "+g+" "+b);

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
                /*if (tiles[y, x].type != Tile.TILE_INFERTIAL && tiles[y, x].type != Tile.TILE_WATER)
                {
                    tiles[y, x].detail.s -= 0.005f;
                    tiles[y, x].detail.b -= 0.001f;

                    if (tiles[y, x].detail.s < 0)
                    {
                        tiles[y, x].detail.s = 0;
                    }
                    if (tiles[y, x].detail.b < 0.75f)
                    {
                        tiles[y, x].detail.b = 0.75f;
                    }
                }
               
               
                texture.SetPixel(x, y, tiles[y, x].detail.ToColor());*/

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

    public Color GetColor(int x, int y)
    {
        if (x < 0 || x > sizeX || y < 0 || y > sizeY)
            return Color.black;
        else 
            return texture.GetPixel(x,y);
    }

    public float Eat(int x,int y)
    {
        float energy = 0;

        if (tiles[y, x].currentEnergy > 0)
        {
            tiles[y, x].currentEnergy -= 10f;

            if (tiles[y, x].currentEnergy < 0)
                tiles[y, x].currentEnergy = 0;

            energy = 10f;
        }

        return energy;


        /*tiles[y, x].detail.s -= 0.1f;
       if (tiles[y, x].detail.s < 0)
           tiles[y, x].detail.s = 0;
       texture.SetPixel(x,y, tiles[y, x].detail.ToColor());*/


        /*if (tiles[y, x].type != Tile.TILE_INFERTIAL && tiles[y, x].type != Tile.TILE_WATER)
        {


            if (tiles[y, x].detail.s > 0)
            {
                tiles[y, x].detail.s -= 0.1f;
                tiles[y, x].detail.b -= 0.02f;
                energy = 10f;
            }


            if (tiles[y, x].detail.s < 0)
            {
                tiles[y, x].detail.s = 0;
            }

            if (tiles[y, x].detail.b < 0.75f)
            {
                tiles[y, x].detail.b = 0.75f;
            }

        }*/


    }

   
}
