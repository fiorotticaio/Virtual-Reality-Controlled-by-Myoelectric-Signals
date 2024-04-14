using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using TMPro;
using static System.IO.File;

public class BarGraphControlByu : MonoBehaviour {
    public Transform extendGraphBar;
    public Transform flexGraphBar;

    public string portName = "COM6"; // Appropriate serial port
    public int baudRate = 9600;
    private SerialPort serialPort;

    public TextMeshProUGUI textTimeExtend; // Text to show the time of the movement
    public TextMeshProUGUI textTimeFlex; // Text to show the time of the movement
    private float time; // Time of the movement

    private string csvFilePathExtension = "Data/firstStep/extension.csv";
    private string csvFilePathFlexion = "Data/firstStep/flexion.csv";
    private string csvSeparator = ";";
    private string csvContent;

    private bool startSaveExtensionData = false, startSaveFlexionData = false;

    private float minGraphPositionY = -130f;
    private float maxGraphPositionY = -40f;
    private float minValueCh1 = 2f;
    private float maxValueCh1 = 20f;
    private float minValueCh2 = 2f;
    private float maxValueCh2 = 20f;

    private float ENV_Freq = 30;


    // Start is called before the first frame update
    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        if (!serialPort.IsOpen) serialPort.Open(); // Verify if port is not open

        /* Setting only the data sending frequency */
        var parametersInFloat = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, ENV_Freq};
        sendParameterToMyoSym(parametersInFloat);

        byte[] bytesToSend = { 0xAE, 0x04 }; // EMG values
        serialPort.Write(bytesToSend, 0, bytesToSend.Length);

        time = 0.0f; // Initialize time
    }

    // Update is called once per frame
    void Update() {
        if (serialPort.IsOpen) {
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            string[] values = serialData.Split(','); // Divide the comma-separated values (expected: "ch1,ch2")
            CultureInfo culture = new CultureInfo("en-US"); // Culture for float conversion
            float ch1 = float.Parse(values[0], culture); // Converts the string to float
            float ch2 = float.Parse(values[1], culture); // Converts the string to float

            Debug.Log("ch1: " + ch1 + "; ch2: " + ch2); // Show the u value in the console

            if (startSaveExtensionData) saveExtensionData(ch1, ch2); // Save the extension data in a csv file
            if (startSaveFlexionData) saveFlexionData(ch1, ch2); // Save the flexion data in a csv file

            handleExtendGraphBar(ch2);
            handleFlexGraphBar(ch1);
        }

        /* Update the time */
        time += Time.deltaTime; // Increment the time
        textTimeExtend.text = time.ToString("F1"); // Show the time with 1 decimal places
        textTimeFlex.text = time.ToString("F1"); // Show the time with 1 decimal places
    }

    void saveExtensionData(float ch1, float ch2) {
        csvContent = ch1.ToString() + csvSeparator + ch2.ToString() + "\n";
        File.AppendAllText(csvFilePathExtension, csvContent);
    }

    void saveFlexionData(float ch1, float ch2) {
        csvContent = ch1.ToString() + csvSeparator + ch2.ToString() + "\n";
        File.AppendAllText(csvFilePathFlexion, csvContent);
    }

    void handleFlexGraphBar(float ch1) {
        Vector3 flexGraphPosition = flexGraphBar.localPosition; // Get current position

        /* Map ch1 values trough -96 and -22 */
        float mappedPositionY = minGraphPositionY + (ch1 * (maxGraphPositionY - minGraphPositionY) / (maxValueCh1 - minValueCh1));

        /* Handle saturation */
        if (mappedPositionY > maxGraphPositionY) mappedPositionY = maxGraphPositionY;
        if (mappedPositionY < minGraphPositionY) mappedPositionY = minGraphPositionY;

        flexGraphPosition.y = mappedPositionY;
        flexGraphBar.localPosition = flexGraphPosition; // Update position
    }

    void handleExtendGraphBar(float ch2) {
        Vector3 extendGraphPosition = extendGraphBar.localPosition; // Get current position

        /* Map ch2 valuer trough -96 and -22 */
        float mappedPositionY = minGraphPositionY + (ch2 * (maxGraphPositionY - minGraphPositionY) / (maxValueCh2 - minValueCh2));

        /* Handle saturation */
        if (mappedPositionY > maxGraphPositionY) mappedPositionY = maxGraphPositionY;
        if (mappedPositionY < minGraphPositionY) mappedPositionY = minGraphPositionY;

        extendGraphPosition.y = mappedPositionY;
        extendGraphBar.localPosition = extendGraphPosition; // Update position
    }

    public void toggleSaveExtensionData() {
        startSaveExtensionData = !startSaveExtensionData;
        time = 0.0f; // Reset time
        csvContent = ""; // Reset csv content
    }

    public void toggleSaveFlexionData() {
        startSaveFlexionData = !startSaveFlexionData;
        time = 0.0f; // Reset time
        csvContent = ""; // Reset csv content
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
}