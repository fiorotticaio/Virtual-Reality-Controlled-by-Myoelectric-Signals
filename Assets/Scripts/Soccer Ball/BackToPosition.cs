using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPosition : MonoBehaviour {

    private Rigidbody ballRigidbody; // Rigidbody component
    public float positionX; // X position
    public float positionY; // Y position
    public float positionZ; // Z position
    public float lateralSpeed;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start() {
        ballRigidbody = GetComponent<Rigidbody>(); // Get the rigidbody component
    }

    // Update is called once per frame
    void Update() {
        /* Making the ball have a small lateral movement to be able to shoot */
        if (ballRigidbody.velocity.z == 0) { // Check if the ball is moving forward
            float lateralMovement = Mathf.Sin(Time.time) * lateralSpeed; // If not, move the ball sideways
            float newSpeed = Mathf.Clamp(ballRigidbody.velocity.x + lateralMovement, -maxSpeed, maxSpeed); // Limit the speed
            ballRigidbody.velocity = new Vector3(newSpeed, 0f, ballRigidbody.velocity.z); // Apply the new speed
        }
    }

    public void setPosition() {
        Vector3 newPosition = new Vector3(positionX, positionY, positionZ); // Values got in Unity 
        transform.position = newPosition; // Add in the self element
        ballRigidbody.velocity = Vector3.zero; // Reset the velocity
    }
}