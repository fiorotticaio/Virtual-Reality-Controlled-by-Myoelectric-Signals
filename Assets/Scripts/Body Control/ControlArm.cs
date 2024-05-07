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

    /* Debug */
    // private float elbowAngle = 90.0f; // Angle of the elbow
    // private bool change = true;

    private string csvFilePath = "Data/biofeedback-graph-bar-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv";
    private string csvSeparator = ";";
    private string csvContent;
    private string uDataFilePath = "Data/uData.csv";
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

    private float TIME_OF_GAME = 5f; // Time of the game

    const string sharedMemoryName = "SharedMemoryMap";
    const int bufferSize = 2^16;
    private MemoryMappedFile memoryMappedFile;
    private MemoryMappedViewStream memoryMappedViewStream;


    // Start is called before the first frame update
    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        if (!serialPort.IsOpen) serialPort.Open(); // Check that the door is not open

        degreeTexts = new TextMeshProUGUI[] { fortyFiveDegreeText, seventyDegreeText, twentyDegreeText, zeroDegreeText}; // Save the texts in an array

        /* Set the time to 0 */
        time = 0.0f;
        targetAngleTime = 0.0f;

        // CreateSharedMemory();
    }

    // Update is called once per frame
    void Update() {
        // System.Random random = new System.Random();
        // string[] values = new string[6];
        // values[0] = UnityEngine.Random.Range(300, 4000).ToString();
        // values[1] = UnityEngine.Random.Range(150, 2500).ToString();
        // values[2] = random.NextDouble().ToString();
        // values[3] = random.NextDouble().ToString();
        // // values[4] = UnityEngine.Random.Range(0, 90).ToString();
        // /* Debug */
        // values[4] = elbowAngle.ToString();
        // if (!change) {
        //     elbowAngle += 1; // Limita o ângulo mínimo
        //     if (elbowAngle == 90) change = true;
        // } else {
        //     elbowAngle -= 1; // Limita o ângulo máximo
        //     if (elbowAngle == 0) change = false;
        // }
        // values[5] = UnityEngine.Random.Range(0, 7).ToString();

        // /* Debug */
        // string serialData = values[0] + "," + values[1] + "," + values[2] + "," + values[3] + "," + values[4] + "," + values[5]; // Create a string with the values
        // Debug.Log(serialData);

        if (serialPort.IsOpen) {
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            string[] values = serialData.Split(','); // Divide values separated by comma
            float elbowAngle = float.Parse(values[4]); // Convert string to float
            float uf = float.Parse(values[2]); // Convert string to float
            float ue = float.Parse(values[3]); // Convert string to float
            elbowAngle = elbowAngle / 100; // Convert to degrees
            uf = uf / 100; // Convert
            ue = ue / 100; // Convert
            
            Debug.Log(fileCount + " uf: " + uf + " ue: " + ue);
            // Debug.Log(elbowAngle);

            saveUData(uf, ue); // Save the data in a csv file

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

        csvFilePath = "Data/biofeedback-graph-bar-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv"; // Update the csv file path
    }

    // Create shared memory
    void CreateSharedMemory() {
        try {
            memoryMappedFile = MemoryMappedFile.CreateOrOpen(sharedMemoryName, bufferSize);
            memoryMappedViewStream = memoryMappedFile.CreateViewStream();
            Debug.Log("Shared memory created.");
        }
        catch (Exception e) {
            Debug.LogError("Error creating shared memory: " + e.Message);
        }
    }

    // Write data to shared memory
    void writeToSharedMemory(string data) {
        if (memoryMappedViewStream != null) {
            try {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                memoryMappedViewStream.Write(buffer, 0, buffer.Length);
                Debug.Log("Data written to shared memory: " + data);
            }
            catch (Exception e) {
                Debug.LogError("Error writing to shared memory: " + e.Message);
            }
        }
        else {
            Debug.LogError("Shared memory not initialized.");
        }
    }

    // Close shared memory
    void CloseSharedMemory() {
        if (memoryMappedViewStream != null) {
            memoryMappedViewStream.Dispose();
            memoryMappedViewStream = null;
        }
        if (memoryMappedFile != null) {
            memoryMappedFile.Dispose();
            memoryMappedFile = null;
        }
        Debug.Log("Shared memory closed.");
    }


    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
        // CloseSharedMemory();
    }
}