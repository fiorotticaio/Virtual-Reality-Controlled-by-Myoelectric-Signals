using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.IO.MemoryMappedFiles;

// EXECUTE TOGETHER WITH PYTHON SCRIPT

/* Movements according to feature value
* 0 - Rest
* 1 - Knee extension
* 2 - Knee flexion
*/

public class MakeFeatureExtracted : MonoBehaviour {
    const string memoryMapName = "SharedMemoryMap"; // Shared memory map name
    const int bufferSize = sizeof(float); // Buffer size in bytes to store the float value
    private MemoryMappedFile mmf; // Object to shared memory file
    private MemoryMappedViewAccessor accessor; // Object to access shared memory

    public Transform rightLeg;
    public float rightLegRotationSpeed = 200.0f; // Degrees per second


    // Start is called before the first frame update
    void Start() {
        mmf = MemoryMappedFile.CreateOrOpen(memoryMapName, bufferSize); // Creates or opens shared memory file with 
                                                                        // the defined name and buffer size
        accessor = mmf.CreateViewAccessor(); // Creates the accessor to read the value from the shared buffer
    }

    // Update is called once per frame
    void Update() {
        if (mmf != null && accessor != null) {
            /* Read the value from the shared buffer as a float, passing the offset as 0 */
            accessor.Read(0, out float value); // The value variable is declared within the Read method
           
            Debug.Log("Value recived from Myosym: " + value);
            
            if (value == 0) {} // Do nothing
            if (value == 1) makeKneeExtension();
            if (value == 2) makeKneeFlexion();
        }
    }

    void makeKneeExtension() {
        if (rightLeg.rotation.z < 0.71f) { // 0.71f is the maximun rotation
            rightLeg.Rotate(Vector3.forward * -rightLegRotationSpeed * Time.deltaTime);
        }
    }

    void makeKneeFlexion() {
        if (rightLeg.rotation.z > 0.51f) { // 0.51f is the minimun rotation
            rightLeg.Rotate(Vector3.forward * rightLegRotationSpeed * Time.deltaTime);
        }
    }
}