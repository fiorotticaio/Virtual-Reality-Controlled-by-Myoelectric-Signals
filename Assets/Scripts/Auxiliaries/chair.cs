using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chair : MonoBehaviour
{
    public GameObject playerStanding, playerSitting;
    public bool interactable, sitting;
    // teste
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = false;
        }
    }
    void Update()
    {
        if(interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                playerSitting.SetActive(true);
                sitting = true;
                playerStanding.SetActive(false);
                interactable = false;
            }
        }
        if(sitting == true)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                playerSitting.SetActive(false);
                playerStanding.SetActive(true);
                sitting = false;
            }
        }
    }
}

