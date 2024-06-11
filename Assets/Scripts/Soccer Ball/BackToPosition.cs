using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPosition : MonoBehaviour {

    private Rigidbody ballRigidbody; // Rigidbody component
    public float positionX1; // X position 1
    public float positionY1; // Y position 2 
    public float positionZ1; // Z position 3
    public float positionX2; // X position 4
    public float positionY2; // Y position 5
    public float positionZ2; // Z position 6
    public float lateralSpeed;
    public float maxSpeed;

    private bool changePosition = false;

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
        /* Each time the object goes to a diff place */
        if (changePosition) {
            changePosition = false;
            Vector3 newPosition = new Vector3(positionX1, positionY1, positionZ1); // Values got in Unity 
            transform.position = newPosition; // Add in the self element
            ballRigidbody.velocity = Vector3.zero; // Reset the velocity
        } else {
            changePosition = true;
            Vector3 newPosition = new Vector3(positionX2, positionY2, positionZ2); // Values got in Unity 
            transform.position = newPosition; // Add in the self element
            ballRigidbody.velocity = Vector3.zero; // Reset the velocity
        }
    }
}