using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using TMPro;
using static System.IO.File;

public class IsolateMuscleGraph : MonoBehaviour {

    public Transform extendGraphBar;
    public Transform flexGraphBar;
    public Transform extendGraphLimit;
    public Transform flexGraphLimit;
    
    private float L0positionYExtendGraphLimit = 0.4f;
    private float L1positionYExtendGraphLimit = 0.3f;
    private float L2positionYExtendGraphLimit = 0.2f;
    private float L3positionYExtendGraphLimit = 0.1f;

    private int level = 0; // 0 (initial), 1, 2 or 3
    private bool levelComplete = false; // Flag to save time just once in the csv file

    public TextMeshProUGUI textTime;
    private float time; // Time of the simulation
    private float[] timeLevels = new float[3]; // Time to complete each level

    public string portName = "COM6"; // Appropriate serial port
    public int baudRate = 9600;
    private SerialPort serialPort;

    private float ENV_Freq = 30;

    private string csvFilePathFlex = "Data/thirdStep/flexion.csv";
    private string csvFilePathExt = "Data/thirdStep/extension.csv";
    private string csvSeparator = ";";
    private string csvContent;

    private bool toggleFlexExt = true; // True: flexion; False: extension
    public TextMeshProUGUI graphTitle;
    
    private float minGraphPositionY = -115.9f;
    private float maxGraphPositionY = -38f;
    private float minValueCh1 = 2f; // Default value
    private float maxValueCh1 = 20f; // Default value
    private float minValueCh2 = 2f; // Default value
    private float maxValueCh2 = 20f; // Default value


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
        
        graphTitle.text = "MCV Flexion"; // Initialize graph title

        /* Initialize time array */
        int i = 0;
        for (i = 0; i < timeLevels.Length; i++) timeLevels[i] = 0.0f;

        /* Get the parameters of second step in player prefs */
        if (PlayerPrefs.HasKey("uf_max")) maxValueCh1 = PlayerPrefs.GetFloat("uf_max") - 5; // 5 is a margin
        if (PlayerPrefs.HasKey("uf_min")) minValueCh1 = PlayerPrefs.GetFloat("uf_min") + 1;
        if (PlayerPrefs.HasKey("ue_max")) maxValueCh2 = PlayerPrefs.GetFloat("ue_max") - 5;
        if (PlayerPrefs.HasKey("ue_min")) minValueCh2 = PlayerPrefs.GetFloat("ue_min") + 1;
    }

    // Update is called once per frame
    void Update() {
        handlePositionOfLimitsBar();

        if (serialPort.IsOpen) {
            string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
            string[] values = serialData.Split(','); // Divide the comma-separated values (expected: "uf,ue")
            CultureInfo culture = new CultureInfo("en-US"); // Culture for float conversion
            float ch1 = float.Parse(values[0], culture); // Converts the string to float
            float ch2 = float.Parse(values[1], culture); // Converts the string to float

            Debug.Log("ch1: " + ch1 + "; ch2: " + ch2); // Show the u value in the console

            if (toggleFlexExt) { // Saving flexion data
                handleFlexGraphBar(ch1);
                handleExtendGraphBar(ch2);
            } else { // Saving extension data
                handleFlexGraphBar(ch2);
                handleExtendGraphBar(ch1);
            }
        }

        if (level != 0 && !levelComplete && completedTheLevel()) {
            if      (level == 1) timeLevels[0] = time;
            else if (level == 2) timeLevels[1] = time;
            else if (level == 3) timeLevels[2] = time;
            saveData();
        }

        /* Update text time */
        time += Time.deltaTime; // Increment the time
        textTime.text = time.ToString("F1"); // Show the time with 1 decimal places
    }

    void handlePositionOfLimitsBar() {
        switch (level) {
            case 0: // Initial
                extendGraphLimit.position = new Vector3(
                    extendGraphLimit.position.x,
                    L0positionYExtendGraphLimit,
                    extendGraphLimit.position.z
                );
                break;

            case 1: // Easy
                extendGraphLimit.position = new Vector3(
                    extendGraphLimit.position.x,
                    L1positionYExtendGraphLimit,
                    extendGraphLimit.position.z
                );
                break;

            case 2: // Medium
                extendGraphLimit.position = new Vector3(
                    extendGraphLimit.position.x,
                    L2positionYExtendGraphLimit,
                    extendGraphLimit.position.z
                );
                break;

            case 3: // Hard
                extendGraphLimit.position = new Vector3(
                    extendGraphLimit.position.x,
                    L3positionYExtendGraphLimit,
                    extendGraphLimit.position.z
                );
                break;

            default: // Should never happen
                break;
        }
    }

    bool completedTheLevel() {
        /* If the y position of the extend graph matches with the extend limit 
        and the y position of the flex graph matches with the flex limit*/
        if (
            flexGraphBar.position.y >= (flexGraphLimit.position.y-0.5f) &&
            extendGraphBar.position.y <= (extendGraphLimit.position.y-0.5f)
        ) {
            levelComplete = true;
            extendGraphBar.GetComponent<Renderer>().material.color = Color.green; // Change the color of the bar
            flexGraphBar.GetComponent<Renderer>().material.color = Color.green; // Change the color of the bar
            return true;
        }
        return false;
    }

    void saveData() { // Save the time to complete each level into a csv file
        if (toggleFlexExt) { // Saving flexion data
            csvContent = timeLevels[0].ToString() + csvSeparator + timeLevels[1].ToString() + csvSeparator + timeLevels[2].ToString() + "\n";
            File.AppendAllText(csvFilePathFlex, csvContent);
        } else { // Saving extension data
            csvContent = timeLevels[0].ToString() + csvSeparator + timeLevels[1].ToString() + csvSeparator + timeLevels[2].ToString() + "\n";
            File.AppendAllText(csvFilePathExt, csvContent);
        }
    }

    public void goToNextLevel() {
        levelComplete = false; // Reset the completed level flag
        time = 0.0f; // Reset the time
        extendGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        flexGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        if (level < 3) level++;
    }

    public void goToPreviousLevel() {
        levelComplete = false; // Reset the completed level flag
        time = 0.0f; // Reset the time
        extendGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        flexGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        if (level > 1) level--;
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
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

    public void toggleFlexExtMode() {
        toggleFlexExt = !toggleFlexExt;

        /* Reset variables */
        level = 1;
        levelComplete = false;
        time = 0.0f;
        extendGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        flexGraphBar.GetComponent<Renderer>().material.color = Color.blue; // Reset the color of the bar
        timeLevels[0] = 0.0f;
        timeLevels[1] = 0.0f;
        timeLevels[2] = 0.0f;

        /* Changing max and min values to mach with the change of the graphs */
        if (PlayerPrefs.HasKey("ue_max")) maxValueCh1 = PlayerPrefs.GetFloat("ue_max") - 5; // 5 is a margin
        if (PlayerPrefs.HasKey("ue_min")) minValueCh1 = PlayerPrefs.GetFloat("ue_min") + 1;
        if (PlayerPrefs.HasKey("uf_max")) maxValueCh2 = PlayerPrefs.GetFloat("uf_max") - 5;
        if (PlayerPrefs.HasKey("uf_min")) minValueCh2 = PlayerPrefs.GetFloat("uf_min") + 1;

        if (toggleFlexExt) graphTitle.text = "MVC Flexion";
        else               graphTitle.text = "MVC Extension";
        
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
}