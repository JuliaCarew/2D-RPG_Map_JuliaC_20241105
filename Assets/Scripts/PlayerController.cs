using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public MapBehaviors MapBehaviors;

    public Tilemap myTilemap;
    public Transform movePoint;
    public float tileSize = 0.8f;

    // updating the player sprite
    public TileBase playerTile;
    private TileBase previousTile;

    void Start()
    {
        movePoint.parent = null;  // allows movepoint to dictate player's direction/ can be moved on it's own    
    }
    void Update()
    {
        MovePlayer();
    }
    public bool CanMove(int x, int y)
    {
        // setting new variable to determine current position
        var gridPosition = new Vector3Int(x, y, 0);
        // checking tiles, needs to ref. MapBehaviors _tiles to collide correctly
        TileBase wallTile = MapBehaviors._wall;
        TileBase doorTile = MapBehaviors._door;
        TileBase chestTile = MapBehaviors._chest;
        TileBase enemyTile = MapBehaviors._enemy;
        TileBase noneTile = MapBehaviors._none;    

        // Get the tile at the specified grid position
        TileBase tileAtPosition = myTilemap.GetTile(gridPosition);

        // cannot move on wall, chest, door, or enemy tiles
        if (tileAtPosition == wallTile ||
            tileAtPosition == doorTile ||
            tileAtPosition == chestTile ||
            tileAtPosition == enemyTile)
        {
            Debug.Log($"Cannot move on {tileAtPosition} at: {x}, {y}");
            return false; 
        }
        // you can move on 'none' tiles
        if (tileAtPosition == noneTile) {
            Debug.Log($"CAN move on {tileAtPosition} at: {x}, {y}");
            return true;
        }
        return false; 
    }
    public void MovePlayer()
    {
        // set player's current pos using movePoint & tileSize
        int playerX = Mathf.RoundToInt(movePoint.position.x / tileSize);
        int playerY = Mathf.RoundToInt(movePoint.position.y / tileSize);
        // moving on X, Y
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // can only move vertically/ horizontally, not diagonally
        if (horizontal != 0) {
            vertical = 0;
        }
        // increment target based on player pos
        int targetX = playerX;
        int targetY = playerY;

        if (horizontal > 0) targetX += 1; // move RIGHT
        else if (horizontal < 0) targetX -= 1; // move LEFT

        if (vertical > 0) targetY += 1; // move UP
        else if (vertical < 0) targetY -= 1; // move DOWN

        // Check if the target tile is walkable
        if (CanMove(targetX, targetY))
        {   // stores previous pos as player pos
            int previousX = playerX;
            int previousY = playerY;

            // Update the move point's position using targetX,Y var previously selected
            movePoint.position = new Vector3(targetX * tileSize, targetY * tileSize, movePoint.position.z);

            // Draw the player at the new position
            DrawPlayer(previousX, previousY, targetX, targetY);
        }
    }
    public void DrawPlayer(int previousX, int previousY, int currentX, int currentY)
    {
        TileBase noneTile = MapBehaviors._none;
        // Replace the previous tile with none
        if (myTilemap.HasTile(new Vector3Int(previousX, previousY, 0)))
        {
            myTilemap.SetTile(new Vector3Int(previousX, previousY, 0), noneTile);
        }

        // Place the player tile at the new position
        myTilemap.SetTile(new Vector3Int(currentX, currentY, 0), playerTile);
    }
}