using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class ControlGraphicBar : MonoBehaviour {
    public Transform rightKneeJoint;
    private float lastRotarion = 0.0f;

    // Start is called before the first frame update
    void Start() {
        lastRotarion = rightKneeJoint.rotation.z;
    }

    // Update is called once per frame
    void Update() {        
        Vector3 scale = transform.localScale; // Get current scale

        if (rightKneeJoint.rotation.z > lastRotarion) { // Extension - graphic bar grows
            lastRotarion = rightKneeJoint.rotation.z; // Update last rotation
            scale.y = scale.y + 2f; // Increase scale (2: experimentally)
        } else if (rightKneeJoint.rotation.z < lastRotarion) { // Flexion - graphic bar shrinks
            lastRotarion = rightKneeJoint.rotation.z; // Update last rotation
            scale.y = scale.y - 2f; // Decrease scale (2: experimentally)
        }

        /* Checking max / min scale values */
        if (scale.y > 180)    scale.y = 180;
        else if (scale.y < 4) scale.y = 4;

        transform.localScale = scale; // Update scale
    }
}