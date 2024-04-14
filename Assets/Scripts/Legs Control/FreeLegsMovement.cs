using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLegsMovement : MonoBehaviour
{
    public float rotationSpeed = 250.0f; // Degrees per second
    private float rotationDirection = 1.0f; // 1.0f or -1.0f

    public float translationSpeed = 1.0f; // Meters per second

    public Transform rightThigh;
    public Transform rightLeg;
    public Transform rightFoot;

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {
        movementControl();        
    }

    void movementControl() {
        /* Right thig */ 
        if (Input.GetKey(KeyCode.UpArrow)) { // Move forward
            rightThigh.Rotate(Vector3.forward * rotationDirection * -rotationSpeed * Time.deltaTime); // Rotate the righth thigh foward
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            rightThigh.Rotate(Vector3.forward * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            rightThigh.Rotate(Vector3.up  * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            rightThigh.Rotate(Vector3.up * rotationDirection * -rotationSpeed * Time.deltaTime);
        }

        /* Right leg */
        //TODO: Change the Input.GetKey(KeyCode.T) to the sensor trigger
        if (Input.GetKey(KeyCode.T) && rightLeg.rotation.z < 0.71f) { // 0.71f is the maximun
            rightLeg.Rotate(Vector3.forward * rotationDirection * -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.G) && rightLeg.rotation.z > 0.51f) { // 0.51f is the minimun
            rightLeg.Rotate(Vector3.forward * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.F)) {
            rightLeg.Rotate(Vector3.up * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.H)) {
            rightLeg.Rotate(Vector3.up * rotationDirection * -rotationSpeed * Time.deltaTime);
        }

        /* Right foot */
        if (Input.GetKey(KeyCode.U)) {
            rightFoot.Rotate(Vector3.forward * rotationDirection * -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.I)) {
            rightFoot.Rotate(Vector3.forward * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.O)) {
            rightFoot.Rotate(Vector3.up * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.P)) {
            rightFoot.Rotate(Vector3.up * rotationDirection * -rotationSpeed * Time.deltaTime);
        }
    }
}