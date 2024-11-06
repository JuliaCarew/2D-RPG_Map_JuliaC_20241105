using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

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
    public char door = 'O';
    public char chest = '*';
    public char enemy = '@';
    static char empty = ' ';

    // collision variables
    bool canWalk;
    bool outOfBounts;

    public TextMeshProUGUI tmp;

    void Start()
    {
        LoadPremadeMap();
        GenerateMapString();
    }

    void Update()
    {
        
    }

    // Returns a string representation of a randomly generated map
    string GenerateMapString()
    {
        tmp.text = "";
        // set up multidimensional array for tilemap to be generated
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {   
                // applying rules before generating the map
                TileRules();

                myMap[x, y] = Random.Range(0, 4);
                var mapStringResult = myMap[x, y];

                if (myMap[x, y] == 0)
                {
                    // set char wall
                    tmp.text += wall;
                    Debug.Log("set char wall");
                }
                if (x >=5 && y >= 5 && x <= 15 && y <= 15)
                {
                    // set char empty
                    tmp.text += empty;
                    Debug.Log("set char empty");
                }
                if (myMap[x, y] == 1)
                {
                    // set char door
                    tmp.text += door;
                    Debug.Log("set char door");
                }
                if (myMap[x, y] == 2)
                {
                    // set char chest
                    tmp.text += chest;
                    Debug.Log("set char chest");
                }
                if (myMap[x, y] == 3)
                {
                    // set char enemy
                    tmp.text += enemy;
                    Debug.Log("set char enemy");
                }               
            }
        }
        return($"mapStringResult");
        // need to adjust range so writing to the walls and not everywhere in between
    }
    public int CheckNeighbors()
    {
        // check the surrounding 8 tiles to be used in TileRules()
        int countWall = 0;
        int countDoor = 0;
        int countChest = 0;
        int countEnemy = 0;

        for (int check_x = -1; check_x < 2; check_x++)
        {
            for (int check_y = -1; check_y < 2; check_y++)
            {   // don't count current tile
                if (check_x == 0 && check_y == 0)
                {
                    var tile = new Vector3Int(check_x, check_y);
                    continue;
                }
                // else if ( neighbor char == wall ) -- countWall++;
            }
        }
        return countWall;
        return countDoor;
        return countChest;
        return countWall;
    }
    void TileRules()
    {
        CheckNeighbors();
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
        //if (char wall){
        //  Tile _wall = myMap.GetTile(#);
        //}
        return (" ");
    }

    // Loads a pre-made map from a text asset
    string LoadPremadeMap()
    {
        Debug.Log("reading text file");
        string maptxt = File.ReadAllText("C:/Users/W0517383/Documents/2DMapStrings/TestMap.txt");
        //Read the text from directly from the test.txt file            
        return (maptxt);
    }
}
