using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatedRightLegAnimationScript : MonoBehaviour {

    public Transform rightThigh; // Get the left thigh from the unity interface

    private Animator anim;
    private Quaternion kneeRotation;

    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>(); // Get the animator component
    }

    // Update is called once per frame
    void Update() {
        Debug.Log($"Right thigh rotation: {rightThigh.rotation.z}");
        // TODO: change to the sensor trigger
        anim.SetFloat("right thigh rotation", rightThigh.rotation.z); // Set the animation parameter
    }
}
