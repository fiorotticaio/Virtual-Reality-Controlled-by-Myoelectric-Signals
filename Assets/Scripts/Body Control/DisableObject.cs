using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}

    public void ToggleActiveState() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}