using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ikLegsMovement : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Degrees per second
    public float rotationAngle = 30.0f; // Degrees
    public float rotationDirection = 1.0f; // 1.0f or -1.0f
    public Transform leftLegIKTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) {
            /* Move the target foward, lifting the leg */
            leftLegIKTarget.Translate(Vector3.forward * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            /* Move the target backward, lowering the leg */
            leftLegIKTarget.Translate(Vector3.forward * rotationDirection * -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            /* Move the target up, throwing the leg forward */
            leftLegIKTarget.Translate(Vector3.up  * rotationDirection * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            /* Move the target down, throwing the leg backward */
            leftLegIKTarget.Translate(Vector3.up * rotationDirection * -rotationSpeed * Time.deltaTime);
        }
    }
}