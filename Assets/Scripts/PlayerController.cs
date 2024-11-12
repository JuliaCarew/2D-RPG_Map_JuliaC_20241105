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
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal") * tileSize, 0f, 0f);
                Debug.Log("moving horizontally");
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                // Move vertically, snapping to the grid
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical") * tileSize, 0f);
                Debug.Log("moving vertically");
            }
        }
    }
}