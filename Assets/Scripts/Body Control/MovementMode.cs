using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMode : MonoBehaviour {
    /* Public variables */
    public Transform leg; // Get the leg (left or right) from the unity interface
    public float moveSpeed = 5.0f;

    /* Private variables */
    private Quaternion inicialRotation;

    // Start is called before the first frame update
    void Start() {
        inicialRotation = leg.rotation; // Save the inicial rotation 
    }

    // Update is called once per frame
    void Update() {
        /* Gets the rotation angle in degrees around the Y axis (vertical axis) */
        float rotationAngle = Quaternion.Angle(inicialRotation, leg.rotation);

        /* Making decisions based on rotation angle */
        if (rotationAngle > 50.0f) transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime); // Forward movement
        /* else, stop */
    }
}
