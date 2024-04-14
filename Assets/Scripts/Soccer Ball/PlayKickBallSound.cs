using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayKickBallSound : MonoBehaviour
{
    public AudioSource kickBallSound;
    // Start is called before the first frame update
    void Start()
    {
        kickBallSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Ball") {
            kickBallSound.Play();
        }
    }
}
