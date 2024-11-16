using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using TMPro;
using Random = UnityEngine.Random;

public class MapBehaviors : MonoBehaviour
{
    // tilemap & tile sprite variables
    [Header("Tilemap & Tile Variables")]
    public Tilemap myTilemap;
    public TileBase _wall;
    public TileBase _door;
    public TileBase _chest;
    public TileBase _enemy;
    public TileBase _none;

    // Tile variables
    [Header("Tile String Characters")]
    public string wall = "#";
    public string door = "O";
    public string chest = "*";
    public string enemy = "@";
    public string none = " ";

    // tile counters (to be used in TileRules & CheckNeighbors)
    int wallCount;
    int doorCount;
    int chestCount;
    int enemyCount;
    int noneCount;   

    // map grid (20X20)
    public int[,] myMap = new int[15, 15];
    
    // mapping each spawnable object to an integer (0-5) as a method - Edit: VERY USEFUL
    private Dictionary<string, int> GetTileMapping()
    {
        return new Dictionary<string, int>
        {
             { wall, 1 },
             { door, 2 },
             { chest, 3 },
             { enemy, 4 },
             { none, 0 }
        };
    }
    // set all tiles to none, for RefreshMap()
    void InitializeMap()
    {
        var tileMapping = GetTileMapping();
        for (int x = 0; x < myMap.GetLength(0); x++)
        {
            for (int y = 0; y < myMap.GetLength(1); y++)
            {
                myMap[x, y] = tileMapping[none];  
            }
        }
    }
    // Load Map Button
    public void LoadMap()
    {
        myTilemap.ClearAllTiles();
        LoadPremadeMap();
    }
    // Generate Map button
    public void RefreshMap()
    {      
        myTilemap.ClearAllTiles();
        InitializeMap(); // maybe is irrelevant since ClearAllTiles?
        ConvertMapToTilemap();
        // reset counts
        wallCount = 0;
        doorCount = 0;
        chestCount = 0;
        enemyCount = 0; 
        noneCount = 0;
    }
    // Returns a string representation of a randomly generated map
    // USED IN: ConvertMapToTilemap()
    string GenerateMapString()
    {
        var tileMapping = GetTileMapping();
        string mapStringResult = " ";

        // Getting Random Positions
        for (int y = 0; y < myMap.GetLength(1); y++)
        {
            for (int x = 0; x < myMap.GetLength(0); x++)
            {   // random gen, ints are assigned in TileMapping
                int randomTileGen = Random.Range(0, 5); 
                myMap[x, y] = randomTileGen;

                TileRules(x,y); // !! enforcing rules AFTER random generation & BEFORE writing the chars !!
            }
        }

        // Placing the assigned chars in the map array
        for (int y = 0; y < myMap.GetLength(1); y++)
        {
            for (int x = 0; x < myMap.GetLength(0); x++)
            {
                int tileType = myMap[x, y];

                if (tileType == tileMapping[wall]) {
                    mapStringResult += "#";
                    //Debug.Log("generate wall");
                }
                else if (tileType == tileMapping[door]) {
                    mapStringResult += "O";
                    //Debug.Log("generate door");
                }
                else if (tileType == tileMapping[chest]) {
                    mapStringResult += "*";
                    //Debug.Log("generate chest");
                }
                else if (tileType == tileMapping[enemy]) {
                    mapStringResult += "@"; 
                    //Debug.Log("generate enemy");
                }
                else if (tileType == tileMapping[none]) {
                    mapStringResult += " ";
                    //Debug.Log("generate none");
                }
            }
            //mapStringResult += "\n"; // map was checkered @ one point, didn't help :/
        }
        return mapStringResult;
    }
    // vars in the method represent myMap, x,y co-ords, tile checking, and tile the rule is distancing from
    // USED IN: TileRules()
    bool CheckNeighbors(int[,] map, int x, int y, int tileType, int distance)
    {   // since certain tile rules need more than 8 tiles checked, use distance for array bounds
        for (int check_x = -distance; check_x < distance; check_x++)
        {
            for (int check_y = -distance; check_y < distance; check_y++)
            {
                if (check_x == 0 && check_y == 0) // skip the current tile (0,0)
                    continue;

                int neighborX = x + check_x;
                int neighborY = y + check_y;

                // check if the neighbor is within the bounds of the map
                if (neighborX >= 0 && neighborX < map.GetLength(0) &&
                    neighborY >= 0 && neighborY < map.GetLength(1))
                {
                    if (map[neighborX, neighborY] == tileType) // check tile type (referenced in rules)
                        return true;
                }
            }
        }
        return false;
    }
    // !! TILE RULES !! //
    // Creates placement rules & maximum placements depending on the tile    
    void TileRules(int x, int y)
    {
        var tileMapping = GetTileMapping();
        int tileType = myMap[x, y];

        // Apply different rules based on the tile type
        if (tileType == tileMapping[wall]) {
            WallRules(x, y);
        }
        else if (tileType == tileMapping[door]) {
            DoorRules(x, y);
        }
        else if (tileType == tileMapping[chest]) {
            ChestRules(x, y);
        }
        else if (tileType == tileMapping[enemy]) {
            EnemyRules(x, y);
        }
        // none tiles don't need rules applied
        // check final tile counts after enforcing rules
        Debug.Log($"Final " +
        $" wall count: {wallCount}, " +
        $" door count: {doorCount}, " +
        $" chest count: {chestCount}, " +
        $" enemy count: {enemyCount}, " +
        $" none count: {noneCount}");
    }
    //  !!  WALLS   !! //
    // Walls must be on the borders of the map array
    void WallRules(int x, int y)
    {
        //Debug.Log($"Running WallRules at ({x}, {y})");

        var tileMapping = GetTileMapping();
        
        if (x <= 0 || x == myMap.GetLength(0) -2 || y <= 0 || y == myMap.GetLength(1) -2 ) {
            // Wall is allowed on the border
            if (myMap[x, y] != tileMapping[wall]) 
            {
                //Debug.Log($"Placed wall at border ({x}, {y})");
                myMap[x, y] = tileMapping[wall];
                wallCount++;
            }
        }
        else { // Remove walls that are not on the borders
            if (myMap[x, y] == tileMapping[wall]) 
            {
                //Debug.Log($"Removed wall from non-border position ({x}, {y})");
                myMap[x, y] = tileMapping[none];
                wallCount--;
            }
        }
    }
    //  !!  DOORS   !! //
    // Doors always need to be within 1 tile of walls and 10 tiles from the nearest door
    // the ___Rules() methods are structured the same, so comments apply :)
    void DoorRules(int x, int y)
    {
        //Debug.Log($"Running DoorRules at ({x}, {y})");

        var tileMapping = GetTileMapping();

        if (doorCount >= 2) { // if count reaches the maximum, set to none
            myMap[x, y] = tileMapping[none]; 
            return;
        }
        // using CheckNeighbos to specifically set the rules for this tile using booleans
        bool nextToWall = CheckNeighbors(myMap, x, y, tileMapping[wall], 1);
        bool farFromOtherDoors = !CheckNeighbors(myMap, x, y, tileMapping[door], 10);

        if (nextToWall && farFromOtherDoors) { // if the door follows the rules it it placed & count goes up
            if (myMap[x, y] != tileMapping[door])
            {
                //Debug.Log($"Placed door at ({x}, {y})");
                myMap[x, y] = tileMapping[door];
                doorCount++;
            }
        }
        else { // if door doesn't follow rules replace with none & count goes down
            if (myMap[x, y] == tileMapping[door]) 
            {
                //Debug.Log($"ERROR - Replaced door with none at {x}, {y}");
                myMap[x, y] = tileMapping[none];
                doorCount--;
            }
        }        
    }
    //  !!  CHESTS   !! //
    // Chests must be at least 2 tiles away from doors, and 10 tiles from nearest chest
    void ChestRules(int x, int y)
    {
        //Debug.Log($"Running ChestRules at ({x}, {y})");

        var tileMapping = GetTileMapping();

        if (chestCount >= 3) {
            myMap[x, y] = tileMapping[none];
            return;
        }

        bool farFromDoor = !CheckNeighbors(myMap, x, y, tileMapping[door], 2);
        bool farFromOtherChests = !CheckNeighbors(myMap, x, y, tileMapping[chest], 10);

        if (farFromDoor && farFromOtherChests) {
            if (myMap[x, y] != tileMapping[chest]) 
            {
                //Debug.Log($"Placed chest at ({x}, {y})");
                myMap[x, y] = tileMapping[chest];
                chestCount++;
            }
        }
        else {
            if (myMap[x, y] == tileMapping[chest]) 
            {
                //Debug.Log($"ERROR - Replaced chest with none at {x}, {y}");
                myMap[x, y] = tileMapping[none];
                chestCount--;
            }
        }      
    }
    //  !!  ENEMIES   !! //
    // Enemies need to be 1 tile away from nearest occupied tile of any kind 
    void EnemyRules(int x, int y)
    {
        //Debug.Log($"Running EnemyRules at ({x}, {y})");

        var tileMapping = GetTileMapping();

        if (enemyCount >= 5){
            myMap[x, y] = tileMapping[none];
            return;
        }

        bool farFromOccupied =
                           !CheckNeighbors(myMap, x, y, tileMapping[door], 3) &&
                           !CheckNeighbors(myMap, x, y, tileMapping[chest], 1) &&
                           !CheckNeighbors(myMap, x, y, tileMapping[enemy], 3);
        if (farFromOccupied) {
            if (myMap[x, y] != tileMapping[enemy]) 
            {
                //Debug.Log($"Placed enemy at ({x}, {y})");
                myMap[x, y] = tileMapping[enemy];
                enemyCount++;
            }
        }
        else {
            if (myMap[x, y] == tileMapping[enemy]) // Remove invalid enemies
            {
                //Debug.Log($"ERROR - Replaced enemy with none at {x}, {y}");
                myMap[x, y] = tileMapping[none];
                enemyCount--;
            }
        }
    }
    // Converts a map string into Tilemap using assigned public variables with _
    // (taken from the GenerateMapString() method)
    string ConvertMapToTilemap()
    {   // take the data after string is generated, use chars as ref
        string mapData = GenerateMapString();

        for (int x = 0; x < myMap.GetLength(1); x++)
        {
            for (int y = 0; y < myMap.GetLength(0); y++)
            {
                int index = x + y * myMap.GetLength(0);
                char currentChar = mapData[index];              

                // checking for assigned chars and replacing with assigned tiles              
                if (currentChar == '#') {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _wall);
                    //Debug.Log($"Set tile to _wall at ({x}, {y})");
                }
                if (currentChar == 'O') {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _door);
                    //Debug.Log($"Set tile to _door at ({x}, {y})");
                }
                if (currentChar == '*') {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _chest);
                    //Debug.Log($"Set tile to _chest at ({x}, {y})");
                }
                if (currentChar == '@') {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _enemy);
                    //Debug.Log($"Set tile to _enemy at ({x}, {y})");
                }
                if (currentChar == ' ') {
                    myTilemap.SetTile(new Vector3Int(x, y, 0), _none);
                    //Debug.Log($"Set tile to _none at ({x}, {y})");
                }
            }
        }
        return ("Tilemap Converted");
    }
    // Loads a pre-made map from a text asset
    string LoadPremadeMap()
    {
        Debug.Log("reading text file");
        string pathToFile = ($"{Application.dataPath}/2DMapStrings/TestMap.txt"); // in the Unity Assets folder, then path
        string[] myLines = System.IO.File.ReadAllLines(pathToFile); // create string from all idv. lines read

        for (int y = 0; y < myLines.Length; y++)
        {
            string myLine = myLines[y]; // so each line gets read in proper order one-by-one
            for (int x = 0; x < myLine.Length; x++)
            {   // on x axis, so accross the line to idv. char, read & assign each one
                char myChar = myLine[x];
                if (x < myMap.GetLength(0)
                 && y < myMap.GetLength(1))
                {
                    myMap[x, y] = myChar;
                    // since ConvertMapToTilemap() references GenerateMapString() for it's data, I just convert directly
                    if (myChar == '#')
                    {
                        myTilemap.SetTile(new Vector3Int(x, y, 0), _wall);
                    }
                    if (myChar == 'O')
                    {
                        myTilemap.SetTile(new Vector3Int(x, y, 0), _door);
                    }
                    if (myChar == '*')
                    {
                        myTilemap.SetTile(new Vector3Int(x, y, 0), _chest);
                    }
                    if (myChar == '@')
                    {
                        myTilemap.SetTile(new Vector3Int(x, y, 0), _enemy);
                    }
                    if (myChar == ' ')
                    {
                        myTilemap.SetTile(new Vector3Int(x, y, 0), _none);
                    }
                }
            }
        }
        return ($"{myLines}");
    }
}