using System;
using System.IO;
using System.IO.Ports;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveArm : MonoBehaviour {
    private SerialPort serialPort;
    private string portName = "COM9";
    private int baudRate = 9600;

    public Transform elbowJoint; // The arm that will be controlled
    public Transform graphBar; // The graphic bar that will ilustrate the albow angle

    private string csvFilePath = "Data/mainScene/biofeedback-graph-bar-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv";
    private string csvSeparator = ";";
    private string csvContent;
    private string uDataFilePath = "Data/mainScene/uData.csv";
    private string uDataSeparator = ";";
    private string uDataContent;
    private int fileCount = 0;

    private float time = 0.0f; // Time of the movement
    private float targetAngleTime = 0.0f; // Time to reach the target angle
    private bool startSavingData = false; // Start saving the data
    public TextMeshProUGUI zeroDegreeText;
    public TextMeshProUGUI twentyDegreeText;
    public TextMeshProUGUI fortyFiveDegreeText;
    public TextMeshProUGUI seventyDegreeText;
    public TextMeshProUGUI ninetyDegreeText;
    private TextMeshProUGUI[] degreeTexts;
    private int idx = 0; // Index of the target angle

    private float TIME_OF_GAME = 10f; // Time of the game

    // Debug
    float elbowAngle = 0.0f; // Angle of the elbow


    // Start is called before the first frame update
    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        if (!serialPort.IsOpen) serialPort.Open(); // Check that the door is not open

        degreeTexts = new TextMeshProUGUI[] { fortyFiveDegreeText, seventyDegreeText, twentyDegreeText, zeroDegreeText}; // Save the texts in an array

        /* Set the time to 0 */
        time = 0.0f;
        targetAngleTime = 0.0f;
    }
    

    // Update is called once per frame
    void Update() {
        if (serialPort.IsOpen) {
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            string[] values = serialData.Split(','); // Divide values separated by comma
            // float elbowAngle = float.Parse(values[4]); // Convert string to float
            float uf = float.Parse(values[2]); // Convert string to float
            float ue = float.Parse(values[3]); // Convert string to float
            // elbowAngle = elbowAngle / 100; // Convert to degrees
            uf = uf / 100; // Convert
            ue = ue / 100; // Convert
            
            Debug.Log(elbowAngle);

            saveUData(uf, ue); // Save the data in a csv file to read in python

            if (startSavingData) {
                saveData(elbowAngle, time); // Save the data in a csv file
                if (targetAngleTime >= TIME_OF_GAME) {
                    idx++;
                    degreeTexts[idx-1].color = Color.white; // Back to white the last text
                    degreeTexts[idx].color = Color.red; // Change the color of the text selected to red
                    if (idx == 3) { // End
                        degreeTexts[idx].color = Color.white; // Back to white the last text
                        startSavingData = false; // Stop saving the data
                        idx = 0; // Reset the index
                    }
                    targetAngleTime = 0.0f; // Reset the time to reach the target angle
                }
            } 

            // Debug
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                if (elbowAngle < 90) elbowAngle += 5; // Increase the angle
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (elbowAngle > 0) elbowAngle -= 5; // Decrease the angle
            }
            elbowJoint.localRotation = Quaternion.Euler(0f, elbowAngle, 0f); // Creates a rotation around the X axis based on the mapped angle
            graphBar.localRotation = Quaternion.Euler(0f, 0f, 45-elbowAngle); // Use localRotation for local rotation around the X-axis
        }

        /* Update the time */
        time += Time.deltaTime;
        targetAngleTime += Time.deltaTime; 
    }


    void saveUData(float uf, float ue) {
        uDataContent = fileCount + uDataSeparator + uf.ToString() + uDataSeparator + ue + "\n"; // Create the csv content with the time
        File.AppendAllText(uDataFilePath, uDataContent); // Append the content to the csv file
        fileCount++;
    }
    

    void saveData(float elbowAngle, float time) {
        csvContent = elbowAngle.ToString() + csvSeparator + time + "\n"; // Create the csv content with the time
        File.AppendAllText(csvFilePath, csvContent); // Append the content to the csv file
    }


    public void toggleStartSaveData() {
        startSavingData = !startSavingData;

        /* Reset the time */
        time = 0.0f;
        targetAngleTime = 0.0f;

        degreeTexts[idx].color = Color.red; // Change the color of the text selected to red

        csvFilePath = "Data/mainScene/biofeedback-graph-bar-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv"; // Update the csv file path
    }


    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
    }
}