using UnityEngine;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections;

public class SerialCommunication : MonoBehaviour {
    /* Public variables */
    public string portName = "COM5"; // Appropriate serial port
    public int baudRate = 9600; // The same baud rate set in Arduino
    public Transform rightThigh;
    public Transform rightLeg;
    public Transform rightFoot;

    /* Private variables */
    private SerialPort serialPort;
    private Stopwatch stopwatch; // Used to measure the time of each serial read
    
    // Start is called before the first frame update
    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // Inicialization of the serial port

        /* Setting inicial rotation for right thigh */
        Quaternion rightThighRotation = new Quaternion(-0.99632f, -0.08571f, 0.00000f, 0.00000f);
        rightThigh.localRotation = rightThighRotation;

        /* Setting inicial rotation for right leg */
        Quaternion rightLegRotation = new Quaternion(0.00000f, 0.00000f, -0.29356f, 0.95594f);
        rightLeg.localRotation = rightLegRotation;

        stopwatch = new Stopwatch(); // Initialize the stopwatch

        // StartCoroutine(ReadSerialData()); // Start the coroutine to read the serial data
    }

    /* Test function */
    private IEnumerator ReadSerialData() {
        while (true) {
            if (serialPort.IsOpen) {
                stopwatch.Start(); // Start the stopwatch
                string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
                stopwatch.Stop(); // Stop the stopwatch
                long time = stopwatch.ElapsedMilliseconds;
                UnityEngine.Debug.Log($"{time}");
            }
            yield return null;
        }
    }

    void Update() {
        if (serialPort.IsOpen) {
            stopwatch.Start(); // Start the stopwatch
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            stopwatch.Stop(); // Stop the stopwatch
            long time = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset(); // reset stopwhatch
            UnityEngine.Debug.Log($"{time}");

            string[] values = serialData.Split(','); // Divide the comma-separated values

            if (values.Length == 3) { // Read successfully
                /* Casting string to int */
                int value1 = int.Parse(values[0]);
                int value2 = int.Parse(values[1]);
                int value3 = int.Parse(values[2]);

                /* Using serial values to rotate the elements */
                rightThigh.localRotation = Quaternion.Euler(180f, -5f, value1);
                rightLeg.localRotation = Quaternion.Euler(0, 0, value2);
                rightFoot.localRotation = Quaternion.Euler(180f, 0, value3);

                /* Debug */
                // Debug.Log($"right thigh rotation: {rightThigh.localRotation}");
                // Debug.Log($"right leg rotation: {rightLeg.localRotation}");
                // Debug.Log($"right foot rotation: {rightFoot.localRotation}");
                // UnityEngine.Debug.Log($"{time}: Potentiometer 1: {value1}, Potentiometer 2: {value2}, Potenciometer 3: {value3}");
            }

         }
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
    }
}