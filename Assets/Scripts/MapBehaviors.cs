using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBehaviors : MonoBehaviour
{
    // tilemap & tile variables
    public Tilemap myTilemap;
    public Tile _wall;
    public Tile _door;
    public Tile _chest;
    public Tile _enemy;

    public int[,] myMap = new int[20, 20];   

    // Tile char variables
    public char wall = '#';
    public char door = '0';
    public char chest = '*';
    public char enemy = '@';

    // collision variables
    bool canWalk;
    bool outOfBounts;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // Returns a string representation of a randomly generated map
    int GenerateMapString(int width, int height)
    { // set up multidimensional array for tilemap to be generated
        for (int x = 0; x < myMap.GetLength(0); x++)
        {
            for (int y = 0; y < myMap.GetLength(1); y++)
            {
                //TileRules();
                myMap[x, y] = Random.Range(0, 4);

                //if (myMap[x, y] == 0) // set char wall
                //if (myMap[x, y] == 1) // set char door
                //if (myMap[x, y] == 2) // set char chest
                //if (myMap[x, y] == 3) // set char enemy
            }
        }
        return(width,height);
    }
    // ?? don't need nested for loop because i will get width/height as a result of reading the map ??
    // ?? don't need to convert the chars to their tiles until next method ??
    // !! this method generates a random map based on the characters given, using the tile rules !!
    public void CheckNeighbor()
    {
        // check the surrounding 8 tiles to be used in TileRules()
    }
    public void TileRules()
    {
        // Doors always need to be within 1 tile of walls 
        // Chests must be at least 2 tiles away from doors, and within 1 tile of a wall 
        // Enemies need to be 2 tiles away from nearest occupied tile of any kind 

        // Maximum of 1 door per map
        // Maximum of 2 chests per map
        // Maximum of 3 enemies per map
    }

    // Converts a map string into Unity Tilemap
    string ConvertMapToTilemap(string mapData)
    {
        return (" ");
    }

    // Loads a pre-made map from a text asset
    string LoadPremadeMap(string mapFilePath)
    {
        return (" ");
    }
}
