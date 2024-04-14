using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandupRightLegAnimationScript : MonoBehaviour
{
    public Transform leftThigh; // Get the left thigh from the unity interface

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); // Get the animator component
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("left thigh rotation", leftThigh.rotation.z); // Set the animation parameter
    }
}