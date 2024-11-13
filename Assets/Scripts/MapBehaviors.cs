using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class MapBehaviors : MonoBehaviour
{
    // tilemap & tile variables
    public Tilemap myTilemap;
    public TileBase _wall;
    public TileBase _door;
    public TileBase _chest;
    public TileBase _enemy;
    public TileBase _none;

    // tile counters (to be used in TileRules & CheckNeighbors)
    private int doorCount = 0;
    private int chestCount = 0;
    private int enemyCount = 0;

    // positions of tiles (to be used in TileRules & CheckNeighbors)
    private List<Vector2Int> doorPositions = new List<Vector2Int>();
    private List<Vector2Int> chestPositions = new List<Vector2Int>();
    private List<Vector2Int> enemyPositions = new List<Vector2Int>();
    private List<Vector2Int> wallPositions = new List<Vector2Int>();

    // map grid (20X20)
    int[,] myMap = new int[15, 15];


    // collision variables
    bool canWalk;
    bool outOfBounts;

    public TextMeshProUGUI tmp;   

    void Start()
    {      
        //LoadPremadeMap();
        ConvertMapToTilemap();
    }

    // Returns a string representation of a randomly generated map
    // USED IN: ConvertMapToTilemap()
    string GenerateMapString()
    {
        string mapStringResult = " ";

        // Tile variables
        string wall = "#";
        string door = "O";
        string chest = "*";
        string enemy = "@";
        string none = " ";

        // set up multidimensional array for tilemap to be generated
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                // randomly assign tiles (0 - wall, 1 - door, 2 - chest, 3 - enemy)
                myMap[x, y] = Random.Range(0, 5);

                // applying tile rules 
                TileRules(x, y);

                if (myMap[x, y] == 0)
                {
                    // set char wall
                    mapStringResult += wall;
                    Debug.Log("generate wall");
                }
                if (myMap[x, y] == 1)
                {
                    // set char door
                    mapStringResult += door;
                    Debug.Log("generate door");
                }
                if (myMap[x, y] == 2)
                {
                    // set char chest
                    mapStringResult += chest;
                    Debug.Log("generate chest");
                }
                if (myMap[x, y] == 3)
                {
                    // set char enemy
                    mapStringResult += enemy;
                    Debug.Log("generate enemy");
                }
                if (myMap[x, y] == 4)
                {
                    // set char enemy
                    mapStringResult += none;
                    Debug.Log("generate none");
                }
            }
        }
        return mapStringResult;
        // need to adjust range so
        // writing to the walls and not everywhere in between
    }

    // vars in the method represent myMap, x,y co-ords, tile checking, and tile the rule is distancing from
    // USED IN: TileRules()
    bool CheckNeighbors(int[,] map, int x, int y, int tileType, int distance)
    {
        // check the surrounding 8 tiles to be used in TileRules()      
        for (int check_x = -distance; check_x < distance; check_x++)
        {
            for (int check_y = -distance; check_y < distance; check_y++)
            {   // don't count current tile
                if (check_x == 0 && check_y == 0)
                {
                    continue;
                }
                int neighborX = x + check_x;
                int neighborY = y + check_y;

                if (neighborX >= 0 && neighborX < 15 && neighborY >= 0 && neighborY < 15)
                {
                    if (map[neighborX, neighborY] == tileType)
                    {
                        return true;  // A matching tile was found nearby
                    }
                }
            }
        }     
        return false;
    }
    // Creates placement rules & maximum placements depending on the tile
    // USED IN: GenerateMapString()
    void TileRules(int x, int y)
    {
        Vector2Int currentPos = new Vector2Int(x, y);
        //  !!  WALLS   !! //
        // Walls must be on the borders of the map array
        if (myMap[x, y] == 0)
        {
            if (x == 0 || x == 16 || y == 0 || y == 16)
            {
                // Wall is allowed on the border
                wallPositions.Add(currentPos);
                Debug.Log($"Placed wall at the border ({x}, {y})");
            }
            else
            {
                myMap[x, y] = 4;  // 4 for empty space
                Debug.Log($"Removed wall from non-border position ({x}, {y})");
            }
        }
        //  !!  DOORS   !! //
        // Doors always need to be within 1 tile of walls and there only one
        if (doorCount < 1 && myMap[x, y] == 1)
        {
            bool nextToWall = CheckNeighbors(myMap, x, y, 0, 1); // 0 - wall / 1 - distance

            if (nextToWall)
            {
                doorCount++;
                doorPositions.Add(currentPos);
                Debug.Log($"Placed door at {x}, {y}");
            }
            else
            {
                myMap[x, y] = 4;  // Place an empty space if rule not met
                Debug.Log($"ERROR - Replaced door with none at {x}, {y}");
            }
        }
        //  !!  CHESTS   !! //
        // Chests must be at least 2 tiles away from doors, and within 1 tile of a wall 
        if (chestCount < 2 && myMap[x, y] == 2)
        {
            bool nextToWall = CheckNeighbors(myMap, x, y, 0, 1);   // 0 - wall / 1 - distance
            bool farFromDoor = !CheckNeighbors(myMap, x, y, 1, 2); // 1 - door / 2 - distance

            if (nextToWall && farFromDoor)
            {
                chestCount++;
                chestPositions.Add(currentPos);
                Debug.Log($"Placed chest at {x}, {y}");
            }
            else
            {
                myMap[x, y] = 4;  // Place an empty space if rule not met
                Debug.Log($"ERROR - Replaced chest with none at {x}, {y}");
            }
        }
        //  !!  ENEMIES   !! //
        // Enemies need to be 2 tiles away from nearest occupied tile of any kind 
        if (enemyCount < 3 && myMap[x, y] == 3)
        {
            bool farFromOccupied = !CheckNeighbors(myMap, x, y, 0, 2) && // 0 - wall / 2 - distance
                                   !CheckNeighbors(myMap, x, y, 1, 2) && // 1 - door / 2 - distance
                                   !CheckNeighbors(myMap, x, y, 2, 2) && // 2 - chest / 2 - distance
                                   !CheckNeighbors(myMap, x, y, 3, 2);   // 3 - enemy / 2 - distance

            if (farFromOccupied)
            {
                enemyCount++;
                enemyPositions.Add(currentPos);
                Debug.Log($"Placed enemy at {x}, {y}");
            }
            else
            {
                myMap[x, y] = 4;  // Place an empty space if rule not met
                Debug.Log($"ERROR - Replaced enemy with none at {x}, {y}");
            }
        }
        // wall positions
        if (myMap[x, y] == 0)
        {
            wallPositions.Add(currentPos);
            Debug.Log($"Placed wall at {x}, {y}");
        }
    }

    // Converts a map string into Tilemap using assigned public variables with _
    // (taken from the GenerateMapString() method)
    string ConvertMapToTilemap()
    {
        string mapData = GenerateMapString();
        int width = 15;
        int height = 15;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;
                char currentChar = mapData[index];

                // checking for assigned chars and replacing with assigned tiles              
                if (currentChar == '#')
                {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _wall);
                    Debug.Log($"Set tile to _wall at ({x}, {y})");
                }
                if (currentChar == 'O')
                {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _door);
                    Debug.Log($"Set tile to _door at ({x}, {y})");
                }
                if (currentChar == '*')
                {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _chest);
                    Debug.Log($"Set tile to _chest at ({x}, {y})");
                }
                if (currentChar == '@')
                {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _enemy);
                    Debug.Log($"Set tile to _enemy at ({x}, {y})");
                }
                if (currentChar == ' ')
                {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _none);
                    Debug.Log($"Set tile to _none at ({x}, {y})");
                }
            }
        }
        return ("Tilemap Converted");
    }
    // Loads a pre-made map from a text asset
    string LoadPremadeMap()
    {
        Debug.Log("reading text file");

        tmp.text = "";
        //var premadeMapResult;

        //change the path for different users
        string path = @".\Assets\2DMapStrings\TestMap.txt";

        string maptxt = File.ReadAllText(path);

        return ($"maptxt");       
    }
}
/*  FLOW
    On START - only ConvertMapToTilemap() runs
    In that process...
    First GenerateMapString() runs
        Where TileRules() is called
    In TileRules(), 
        CheckNeighbors() is called before implementing rules
    The Map String is then taken and used in the ConvertMapToTilemap() method
    Order is: CheckNeighnors() - TileRules() - GenerateMapString() - ConvertMapToTilemap()
*/