using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2.0f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;

    void Start()
    { // getting the rigidbody component of the current gameobject
        rb = GetComponent<Rigidbody2D>();
    }
    // movement functions
    void Update()
    { // specify what direction we will move in depending on button pressed
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    // physics functions
    void FixedUpdate()
    {
        rb.velocity = movementDirection * movementSpeed;
    }
}
