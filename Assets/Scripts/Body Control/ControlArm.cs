using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MoveArm : MonoBehaviour {
    private SerialPort serialPort;
    public string portName = "COM9";
    public int baudRate = 9600; 
    public Transform arm;

    private float armAngle = 90;
    private bool change = true;

    // Start is called before the first frame update
    void Start() {
        // serialPort = new SerialPort(portName, baudRate);
        // if (!serialPort.IsOpen) serialPort.Open(); // Check that the door is not open
    }

    // Update is called once per frame
    void Update() {
        // if (serialPort.IsOpen) {
            // string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            // string[] values = serialData.Split(','); // Divide values separated by comma
            // float armAngle = float.Parse(values[2]); // Convert string to float
            // armAngle = armAngle / 100; // Convert to degrees
            
            // Debug.Log(serialData);
            // Debug.Log(armAngle);

        // }

        /* Debug */
        if (!change) {
            armAngle += 1; // Limita o ângulo mínimo
            if (armAngle == 90) change = true;
        } else {
            armAngle -= 1; // Limita o ângulo máximo
            if (armAngle == 0) change = false;
        }

        arm.localRotation = Quaternion.Euler(0f, armAngle, 0f); // Creates a rotation around the X axis based on the mapped angle
    }

    // void OnApplicationQuit() {
    //     if (serialPort.IsOpen) serialPort.Close();
    // }
}