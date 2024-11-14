using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using TMPro;
using Random = UnityEngine.Random;

public class MapBehaviors : MonoBehaviour
{
    // tilemap & tile variables
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
    [Header("Tile Count")]
    public int wallCount;
    public int doorCount;
    public int chestCount;
    public int enemyCount;
    public int noneCount;

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

    public List<string> Spawnables;

    public void LoadMap()
    {
        myTilemap.ClearAllTiles();
        LoadPremadeMap();
    }
    public void RefreshMap()
    {
        myTilemap.ClearAllTiles();
        ConvertMapToTilemap();

    }
    // Returns a string representation of a randomly generated map
    // USED IN: ConvertMapToTilemap()
    string GenerateMapString()
    {
        Spawnables = new List<string> { wall, door, chest, enemy, none };

        // mapping each spawnable object to an integer (0-5)
        Dictionary<string, int> tileMapping = new Dictionary<string, int>
        {
             { wall, 1 },
             { door, 2 },
             { chest, 3 },
             { enemy, 4 },
             { none, 0 }
        };

        // move check neighbors here and tile rles to their respective tiles ??
        string mapStringResult = " ";

        // Getting Random Positions
        for (int y = 0; y < myMap.GetLength(0); y++)
        {
            for (int x = 0; x < myMap.GetLength(1); x++)
            {
                Debug.Log($"Current tile: ({x}, {y})");

                // (0 - wall, 1 - door, 2 - chest, 3 - enemy, 4 - none)
                int randomTileGen = Random.Range(0, 5);
                myMap[x, y] = randomTileGen;

                TileRules(x, y);
            }
        }  
        // Placing the assigned chars in the map array
        for (int y = 0; y < myMap.GetLength(0); y++)
        {
            for (int x = 0; x < myMap.GetLength(1); x++)
            {
                int tileType = myMap[x, y];

                if (tileType == tileMapping[wall])
                {
                    mapStringResult += "#";
                    Debug.Log("generate wall");
                }
                else if (tileType == tileMapping[door])
                {
                    mapStringResult += "O";
                    Debug.Log("generate door");
                }
                else if (tileType == tileMapping[chest])
                {
                    mapStringResult += "*";
                    Debug.Log("generate chest");
                }
                else if (tileType == tileMapping[enemy])
                {
                    mapStringResult += "@"; 
                    Debug.Log("generate enemy");
                }
                else if (tileType == tileMapping[none])
                {
                    mapStringResult += " ";
                    Debug.Log("generate none");
                }
            }
            //mapStringResult += "\n";
        }
        return mapStringResult;
    }
    // vars in the method represent myMap, x,y co-ords, tile checking, and tile the rule is distancing from
    // USED IN: TileRules()
    bool CheckNeighbors(int[,] map, int x, int y, int tileType, int distance)
    {
        for (int check_x = -1; check_x < 2; check_x++)
        {
            for (int check_y = -1; check_y < 2; check_y++)
            { // so the loop doesnt count the current cell/ itself
                if (check_y == 0 && check_x == 0)
                {
                    continue;
                }
                int checkX = x + check_x;
                int checkY = y + check_y;

                // Ensure the coordinates are within bounds
                if (checkX >= 0 && checkX < map.GetLength(0) &&
                    checkY >= 0 && checkY < map.GetLength(1))
                {
                    // Check if the neighboring tile matches the tileType
                    if (map[checkX, checkY] == tileType)
                    {
                        return true;
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
        Dictionary<string, int> tileMapping = new Dictionary<string, int>
        {
             { wall, 1 },
             { door, 2 },
             { chest, 3 },
             { enemy, 4 },
             { none, 0 }
        };

        Vector2Int currentPos = new Vector2Int(x, y);

        void WallRules(int x, int y)
        {
            //  !!  WALLS   !! //
            // Walls must be on the borders of the map array
            if (wallCount >= 20) return;

            if (x == 0 || x == myMap.GetLength(0) - 1 || y == 0 || y == myMap.GetLength(1) - 1)
            {
                // Wall is allowed on the border
                if (myMap[x, y] != tileMapping[wall]) // Place wall if it doesn't already exist here
                {
                    myMap[x, y] = tileMapping[wall];
                    wallPositions.Add(currentPos);
                    wallCount++;
                    Debug.Log($"Placed wall at border ({x}, {y}). Total walls: {wallCount}");
                }
            }
            else
            {
                myMap[x, y] = tileMapping[none];
                Debug.Log($"Removed wall from non-border position ({x}, {y})");
            }            
        }

        void DoorRules(int x, int y)
        {
            //  !!  DOORS   !! //
            // Doors always need to be within 1 tile of walls and there only one
            if (doorCount >= 2) return;

            if (myMap[x, y] == tileMapping[door])
            {
                bool nextToWall = CheckNeighbors(myMap, x, y, tileMapping[wall], 1); // 1 - distance

                if (nextToWall)
                {
                    doorCount++;
                    doorPositions.Add(currentPos);
                    Debug.Log($"Placed door at ({x}, {y}). Total doors: {doorCount}");
                }
                else
                {
                    myMap[x, y] = tileMapping[none];  // Place an empty space if rule not met
                    Debug.Log($"ERROR - Replaced door with none at {x}, {y}");
                }
            }
        }
        void ChestRules(int x, int y)
        {
            //  !!  CHESTS   !! //
            // Chests must be at least 2 tiles away from doors, and within 1 tile of a wall 
            if (chestCount >= 3) return;

            if (myMap[x, y] == tileMapping[chest])
            {
                bool nextToWall = CheckNeighbors(myMap, x, y, tileMapping[wall], 1);   // 1 - distance
                bool farFromDoor = !CheckNeighbors(myMap, x, y, tileMapping[door], 2); // 2 - distance

                if (nextToWall && farFromDoor)
                {
                    chestCount++;
                    chestPositions.Add(currentPos);
                    Debug.Log($"Placed chest at ({x}, {y}). Total chests: {chestCount}");
                }
                else
                {
                    myMap[x, y] = tileMapping[none];  // Place an empty space if rule not met
                    Debug.Log($"ERROR - Replaced chest with none at {x}, {y}");
                }
            }
        }
        void EnemyRules(int x, int y)
        {
            //  !!  ENEMIES   !! //
            // Enemies need to be 1 tile away from nearest occupied tile of any kind 
            if (enemyCount >= 4) return;

            if (myMap[x, y] == tileMapping[enemy])
            {
                bool farFromOccupied = !CheckNeighbors(myMap, x, y, tileMapping[wall], 1) && // 2 - distance
                                       !CheckNeighbors(myMap, x, y, tileMapping[door], 1) && // 2 - distance
                                       !CheckNeighbors(myMap, x, y, tileMapping[chest], 1) && //2 - distance
                                       !CheckNeighbors(myMap, x, y, tileMapping[enemy], 1);   // 2 - distance
                if (farFromOccupied)
                {
                    enemyCount++;
                    enemyPositions.Add(currentPos);
                    Debug.Log($"Placed enemy at ({x}, {y}). Total enemies: {enemyCount}");
                }
                else
                {
                    myMap[x, y] = tileMapping[none];  // Place an empty space if rule not met
                    Debug.Log($"ERROR - Replaced enemy with none at {x}, {y}");
                }
            }
        }

        WallRules(x, y);   // Walls must be placed first and only on borders
        DoorRules(x, y);   // Doors come next, near walls
        ChestRules(x, y);  // Chests next, near walls but far from doors
        EnemyRules(x, y);  // Enemies last, far from all other occupied tiles
    }

    // Converts a map string into Tilemap using assigned public variables with _
    // (taken from the GenerateMapString() method)
    string ConvertMapToTilemap()
    {
        string mapData = GenerateMapString();

        for (int x = 0; x < myMap.GetLength(0); x++)
        {
            for (int y = 0; y < myMap.GetLength(1); y++)
            {
                //int index = x + (myMap.GetLength(1) - y - 1) * myMap.GetLength(0);
                int index = x + y * myMap.GetLength(0);
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
        string pathToFile = ($"{Application.dataPath}/2DMapStrings/TestMap.txt");
        string[] myLines = System.IO.File.ReadAllLines(pathToFile);

        for (int y = 0; y < myLines.Length; y++)
        {
            string myLine = myLines[y];
            for (int x = 0; x < myLine.Length; x++)
            {
                char myChar = myLine[x];
                if (x < myMap.GetLength(0)
                 && y < myMap.GetLength(1))
                {
                    myMap[x, y] = myChar;

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