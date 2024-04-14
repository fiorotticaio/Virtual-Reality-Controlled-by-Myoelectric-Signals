using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO.MemoryMappedFiles;
using System.IO;
using System;
using System.Threading;
using System.Globalization;
using TMPro;

public class ReadKneeAngleFromSerial : MonoBehaviour {
    private SerialPort serialPort;
    public string portName = "COM6"; // Appropriate serial port
    private SerialPort serialPortVT; // Serial port for the PCB_VT
    public string portNameVT = "COM10"; // Appropriate serial port
    public int baudRate = 9600; // The baud at this moment, actualy doesn't matter, only the freq in myosym
    
    private float ENV_Freq = 30;

    public Transform graphBar; // The graphic bar that will ilustrate the knee angle
    public Transform kneeJoint; // The leg that will rotate

    private string csvFilePath = "Data/mainScene/mainScene" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv";
    private string csvSeparator = ";";
    private string csvContent;

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

    private float TIME_OF_GAME = 20f; // Time of the game

    // Start is called before the first frame update
    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        if (!serialPort.IsOpen) serialPort.Open(); // Verify if port is not open
        // serialPortVT = new SerialPort(portNameVT, baudRate);     
        // if (!serialPortVT.IsOpen) serialPortVT.Open(); // Verify if port is not open   

        /* Setting only the data sending frequency */
        var parametersInFloat = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, ENV_Freq };
        sendParameterToMyoSym(parametersInFloat);

        byte[] bytesToSend = { 0xAE, 0x06 }; // Cocontraction values
        serialPort.Write(bytesToSend, 0, bytesToSend.Length);

        degreeTexts = new TextMeshProUGUI[] { fortyFiveDegreeText, seventyDegreeText, twentyDegreeText, zeroDegreeText}; // Save the texts in an array

        /* Set the time to 0 */
        time = 0.0f;
        targetAngleTime = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        if (serialPort.IsOpen) {
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            string[] values = serialData.Split(','); // Divide the comma-separated values
            CultureInfo culture = new CultureInfo("en-US"); // Culture for float conversion
            float kneeAngle = float.Parse(values[0], culture); // Get the knee angle
            float kneeImpedance = float.Parse(values[1], culture); // Get the knee impedance
            
            Debug.Log(kneeAngle + ", " + kneeImpedance);

            // sendAngleToVT(kneeAngle); // Send the knee angle to the PCB_VT system

            handleGraphBarColor(kneeImpedance); // Change the color of the graph bar to ilustrate the impedance

            if (startSavingData) {
                saveData(kneeAngle, time); // Save the data in a csv file
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

            /* Rotating the leg and graph */
            kneeJoint.localRotation = Quaternion.Euler(0f, 0f, kneeAngle); // Use localRotation for local rotation around the X-axis
            graphBar.localRotation = Quaternion.Euler(0f, 0f, 45-kneeAngle); // Use localRotation for local rotation around the X-axis
        }
        /* Update the time */
        time += Time.deltaTime;
        targetAngleTime += Time.deltaTime;
    }

    void handleGraphBarColor(float kneeImpedance) {
        float blueTransparency = kneeImpedance / 30f; // Calculate the new blue transparency (0-30 -> 0-1)
        Color newBlue = new Color(0, 0, 0.2f + blueTransparency, 1f); // Create a new color with the new blue transparency
        graphBar.GetComponent<Renderer>().material.color = newBlue; // Update object material color
    }

    void saveData(float kneeAngle, float time) {
        csvContent = kneeAngle.ToString() + csvSeparator + time + "\n"; // Create the csv content with the time
        File.AppendAllText(csvFilePath, csvContent); // Append the content to the csv file
    }

    void sendParameterToMyoSym(float[] parametersInFloat) {
        /* Create a byte array and copy the floats into it */
        var parametersInBytes = new byte[parametersInFloat.Length * 4];
        Buffer.BlockCopy(parametersInFloat, 0, parametersInBytes, 0, parametersInBytes.Length);

        /* Command bytes of Myosym */
        byte[] MyoSymCommand = { 0xAE, 0x07 }; // Send data

        /* Concatenate the command and data array */
        byte[] bytesToSend = new byte[MyoSymCommand.Length + parametersInBytes.Length];
        Array.Copy(MyoSymCommand, bytesToSend, MyoSymCommand.Length);
        Array.Copy(parametersInBytes, 0, bytesToSend, MyoSymCommand.Length, parametersInBytes.Length);

        UnityEngine.Debug.Log(bytesToSend.Length);

        serialPort.Write(bytesToSend, 0, bytesToSend.Length); // Send the data
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
    }

    public void toggleStartSaveData() {
        startSavingData = !startSavingData;

        /* Reset the time */
        time = 0.0f;
        targetAngleTime = 0.0f;

        degreeTexts[idx].color = Color.red; // Change the color of the text selected to red

        csvFilePath = "Data/mainScene/mainScene" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv"; // Update the csv file path
    }

    void sendAngleToVT(float kneeAngle) {
        byte[] kneeAngleInBytes = BitConverter.GetBytes(kneeAngle); // Create a byte array and copy the float into it

        byte[] bluePillCommand = { 0xAE, 0x02 }; // Command bytes of Myosym

        /* Concatenate the command and data array */
        byte[] bytesToSend = new byte[bluePillCommand.Length + kneeAngleInBytes.Length];
        Array.Copy(bluePillCommand, bytesToSend, bluePillCommand.Length);
        Array.Copy(kneeAngleInBytes, 0, bytesToSend, bluePillCommand.Length, kneeAngleInBytes.Length);

        // UnityEngine.Debug.Log(bytesToSend.Length);

        serialPortVT.Write(bytesToSend, 0, bytesToSend.Length); // Send the data
    }
}