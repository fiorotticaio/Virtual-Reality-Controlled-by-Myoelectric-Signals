using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayInteraction : MonoBehaviour {
    public float distance = 4f;
    private LineRenderer lineRenderer;
    private bool secondaryButtonIsPressed = false;

    // Start is called before the first frame update
    void Start() {
        lineRenderer = GetComponent<LineRenderer>(); // Gets the LineRenderer component
        
        /* Set LineRenderer's starting points accordingly to the foot position */
        lineRenderer.SetPosition(0, transform.position); 
        lineRenderer.SetPosition(1, transform.position + transform.forward * distance); 
        lineRenderer.enabled = false; // Initially, the LineRenderer is disabled
    }

    // Update is called once per frame
    void Update() {
        Vector3 newForward = new Vector3(0f, transform.forward.y, 0.99f); // Create a new forward vector that points
                                                                          // always forward, but can move up and down
        Ray ray = new Ray(transform.position, newForward); // Create the real ray interactor accordingly to the foot position
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red); // Show the ray in the scene view (game mode)
        
        RaycastHit hitInfo;
        /* Check if the ray hit something */
        if (Physics.Raycast(ray, out hitInfo, distance)) {
            GameObject hitObject = hitInfo.collider.gameObject; // Get the object that was hit
            /* Handle hits accordingly to the type (tag) of the object  */            
            if (hitObject.CompareTag("Button")) handleButtonHit(hitObject);
            if (hitObject.CompareTag("Toggle")) handleToggleHit(hitObject);
            if (hitObject.CompareTag("ColorBlock")) handleColorBlockHit(hitObject);
        }

        /* Updates the LineRenderer's position to match the radius (when it is active) */
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * distance);
    }

    void handleButtonHit(GameObject hitObject) {
        Button button = hitObject.GetComponent<Button>(); // Get the button
        if (button != null) {
            button.Select(); // Hover
            if (Input.GetKeyDown(KeyCode.C)) button.onClick.Invoke(); // Click
        }
    }

    void handleToggleHit(GameObject hitObject) {
        Toggle toggle = hitObject.GetComponent<Toggle>(); // Get the toggle
        if (toggle != null) {
            toggle.Select(); // Hover
            if (Input.GetKeyDown(KeyCode.C)) toggle.isOn = !toggle.isOn; // Click
        }
    }

    /* For Color Game */
    void handleColorBlockHit(GameObject hitObject) {
        Renderer renderer = hitObject.GetComponent<Renderer>(); // Get the renderer
        if (renderer != null) {
            if (secondaryButtonIsPressed) {
                hitObject.tag = "Untagged"; // Change the tag
            }
        }
    }

    public void ToggleLineRender() {
        lineRenderer.enabled = !lineRenderer.enabled;
    }

    public void pressSecondaryButton() {
        secondaryButtonIsPressed = true;
    }

    public void releaseSecondaryButton() {
        secondaryButtonIsPressed = false;
    }
}