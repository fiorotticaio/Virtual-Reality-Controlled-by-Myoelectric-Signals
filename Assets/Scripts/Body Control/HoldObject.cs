using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObject : MonoBehaviour {

    public AudioClip holdAudio;

    // Execute just when the contact happens 
    void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("ObjectToHold")) {
            AudioSource.PlayClipAtPoint(holdAudio, transform.position); // Play the audio 
        }
    }

    // Execute while the contact is happening
    void OnTriggerStay(Collider other) {
        /* Checks whether the collision involves the object that must be held */
        if (other.transform.CompareTag("ObjectToHold")) {
            /* Holding the object */
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;
        }
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}
}
