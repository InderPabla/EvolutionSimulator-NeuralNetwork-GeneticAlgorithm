using UnityEngine;
using System.Collections;

struct Tile
{
    public const int TILE_RED = 0;
    public const int TILE_GREEN = 0;
    public const int TILE_BLUE = 0;
    public const int TILE_ORANGE = 0;
    public const int TILE_PURPLE = 0;
    public const int TILE_INFERTIAL = 0;
    public const int TILE_WATER = 0;

    public int type;
    public float fertility;
}
public class TileMap
{

    Tile[,] tiles;

    public TileMap(Color[] texColor, int sizeX, int sizeY)
    {
        tiles = new Tile[sizeY, sizeX];
        int colorIndex = 0;
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                tiles[i, j].fertility = 0;
                float r = texColor[colorIndex].r;
                float g = texColor[colorIndex].g;
                float b = texColor[colorIndex].b;

                if (r == 0 && g == 0 && b == 0)
                    tiles[i, j].type = Tile.TILE_WATER;
                else if (r == 1 && g == 1 && b == 1)
                    tiles[i, j].type = Tile.TILE_INFERTIAL;
                else if (r == 1 && g == 0 && b == 0)
                    tiles[i, j].type = Tile.TILE_RED;
                else if (r == 0 && g == 1 && b == 0)
                    tiles[i, j].type = Tile.TILE_GREEN;
                else if (r == 0 && g == 0 && b == 1)
                    tiles[i, j].type = Tile.TILE_BLUE;
                else if (r == b)
                    tiles[i, j].type = Tile.TILE_PURPLE;
                else
                    tiles[i, j].type = Tile.TILE_ORANGE;

                colorIndex++;
            }
        }
    }
   
}
