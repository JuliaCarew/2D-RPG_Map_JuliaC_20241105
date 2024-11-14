using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject chestPrefab;
    public GameObject enemyPrefab;
    public GameObject nonePrefab;

    public List<string> Spawnables;

    public string wall = "#";
    public string door = "O";
    public string chest = "*";
    public string enemy = "@";
    public string none = " ";

    public int wallCount;
    public int doorCount;
    public int chestCount;
    public int enemyCount;
    public int noneCount;

    public Vector2 BottomLeft,TopRight;

    private Dictionary<string, GameObject> prefabMap;

    // Start is called before the first frame update
    void Start()
    {
        prefabMap = new Dictionary<string, GameObject>
        {
        { wall, wallPrefab },
        { door, doorPrefab },
        { chest, chestPrefab },
        { enemy, enemyPrefab },
        { none, nonePrefab }
        };

        GenerateMapString();
    }
    string GenerateMapString()
    {
        Spawnables = new List<string> { wall, door, chest, enemy, none };

        // Spawn objects based on their individual spawn counts
        for (int i = 0; i < wallCount; i++)
        {
            SpawnRandom(wall);
        }
        for (int i = 0; i < doorCount; i++)
        {
            SpawnRandom(door);
        }
        for (int i = 0; i < chestCount; i++)
        {
            SpawnRandom(chest);
        }
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnRandom(enemy);
        }
        for (int i = 0; i < noneCount; i++)
        {
            SpawnRandom(none);
        }
        return " ";
    }
    public void SpawnRandom(string mapString)
    {
        Vector2 pos = new Vector2(
            Random.Range(BottomLeft.x, TopRight.x),
            Random.Range(BottomLeft.y, TopRight.y)
        );

        // Get the corresponding prefab from the dictionary
        if (prefabMap.TryGetValue(mapString, out GameObject prefab))
        {
            GameObject g = Instantiate(prefab, pos, Quaternion.identity);
            g.transform.parent = transform;
        }
        else
        {
            Debug.LogError("No prefab found for map string: " + mapString);
        }
    }
    public void CheckOverlap()
    {

    }
}
