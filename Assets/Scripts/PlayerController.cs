using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Tilemap myTilemap;
    public float movementSpeed = 5f;
    public Transform movePoint;
    public float tileSize = 0.8f;

    // Layer definitions for checking tiles
    public TileBase wallTile;
    public TileBase chestTile;
    public TileBase enemyTile;
    public TileBase borderTile; // For invisible borders around the screen

    // Map boundaries 
    public int minX = -1;
    public int maxX = 16;
    public int minY = -1;
    public int maxY = 16;

    void Start()
    {
        movePoint.parent = null;
        // Snap movePoint to nearest grid position
        movePoint.position = new Vector3(
            Mathf.Round(transform.position.x / tileSize) * tileSize,
            Mathf.Round(transform.position.y / tileSize) * tileSize,
            transform.position.z);
    }
    void Update()
    {
        MovePlayer();
    }
    public void MovePlayer()
    {    
        // Move Player's sprite towards movePoint
        transform.position = Vector3.MoveTowards
            (transform.position, movePoint.position, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            // Player has reached the movePoint, can issue next move command
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Move horizontally, snapping to the grid
                movePoint.position += 
                new Vector3(Input.GetAxisRaw("Horizontal") * tileSize, 0f, 0f);
                //Debug.Log("moving horizontally");
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                // Move vertically, snapping to the grid
                movePoint.position += 
                new Vector3(0f, Input.GetAxisRaw("Vertical") * tileSize, 0f);
                //Debug.Log("moving vertically");
            }
        }
    }

    // Function to check if the player can move to the target position
    public bool CanMoveTo(Vector3 targetPosition)
    {
        // Convert world position to grid (cell) position
        Vector3Int cellPosition = myTilemap.WorldToCell(targetPosition);
        TileBase tileAtTarget = myTilemap.GetTile(cellPosition);

        // Check if the position is outside the map boundaries
        if (cellPosition.x < minX || cellPosition.x > maxX || cellPosition.y < minY || cellPosition.y > maxY)
        {
            return false;  // Prevent moving beyond the screen border
        }

        // Check if there's a wall, chest, or enemy tile
        if (tileAtTarget == wallTile || tileAtTarget == chestTile || tileAtTarget == enemyTile || tileAtTarget == borderTile)
        {
            Debug.Log($"Collision detected at {cellPosition} with {tileAtTarget}");
            return false;  // Prevent moving into a blocked tile
        }

        // If no collision, return true to allow movement
        return true;
    }
}