using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using static System.IO.File;
using System.Diagnostics;

public class WindowGraph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private GameObject originPoint;

    public string portName = "COM6";
    public int baudRate = 9600;
    private SerialPort serialPort;

    public string myosymSceneCode = "AE06"; // Myosym code to send uf and ue

    private string csvFilePath = "Data/secondStep.csv";
    private string csvSeparator = ";";
    private string csvContent;

    /* Patient cocontraction parameters */
    private float mf;
    private float me;
    private float m0;
    private float uf_max;
    private float ue_max;
    private float uf_min;
    private float ue_min;
    private float vel_max;
    private float K_max;
    private float ENV_Freq = 15;


    void Start() {
        graphContainer = transform.Find("Graph Container").GetComponent<RectTransform>(); // Find the Graph Container
        originPoint = CreateCircle(new Vector2(0, 0)); // Create the origin point (0,0)
        originPoint.name = "originPoint"; // Overwrite the name of the origin point

        serialPort = new SerialPort(portName, baudRate);
        if (!serialPort.IsOpen) serialPort.Open(); // Verify if port is not open

        calculateParameterByPython(); // Execute the python script to calculate the parameters
        
        byte[] bytesToSend = { 0xAE, 0x05 }; // EMG normalized values
        serialPort.Write(bytesToSend, 0, bytesToSend.Length);

        /* Create cocontraction points */
        float mfx = findMfx(mf);
        if (mfx == -1) UnityEngine.Debug.Log("Error in findMfx() function");
        float mey = me * 90 > 100 ? 100 : me * 90;
        float m0y = m0 * 70 > 100 ? 100 : m0 * 70;
        GameObject mfPoint = CreateCircle(new Vector2(mfx, 90));
        mfPoint.name = "mfPoint";
        GameObject mePoint = CreateCircle(new Vector2(90, mey));
        mePoint.name = "mePoint";
        GameObject m0Point = CreateCircle(new Vector2(70, m0y));
        m0Point.name = "m0Point";

        /* Create cocontraction lines */
        GameObject connMf = CreateDotConnection(originPoint.GetComponent<RectTransform>().anchoredPosition, 
                            mfPoint.GetComponent<RectTransform>().anchoredPosition);
        connMf.name = "connMf";
        connMf.GetComponent<Image>().color = new Color(1,0,0, .5f); // Set the color of the line (red with 50% opacity)

        GameObject connMe = CreateDotConnection(originPoint.GetComponent<RectTransform>().anchoredPosition,
                            mePoint.GetComponent<RectTransform>().anchoredPosition);
        connMe.name = "connMe";
        connMe.GetComponent<Image>().color = new Color(0,0,1, .5f); // Set the color of the line (blue with 50% opacity)
        
        GameObject connM0 = CreateDotConnection(originPoint.GetComponent<RectTransform>().anchoredPosition,
                            m0Point.GetComponent<RectTransform>().anchoredPosition);
        connM0.name = "connM0";
        connM0.GetComponent<Image>().color = new Color(0,1,0, .5f); // Set the color of the line (green with 50% opacity)
    }

    void Update() {
        /* Clean the last connection and points of the graph (except the origin and mf, me, mo) */
        foreach (Transform child in graphContainer.transform) {
            if (child.name != "originPoint" && child.name != "mfPoint" && child.name != "mePoint" && child.name != "m0Point" &&
                child.name != "connMf" && child.name != "connMe" && child.name != "connM0" && child.name != "Background") {
                Destroy(child.gameObject);
            }
        }

        /* Recive x and y positions from serial (ue and uf) */
        string serialData = serialPort.ReadLine(); // Reads a line of data from the serial port
        string[] values = serialData.Split(','); // Divide the comma-separated values
        CultureInfo culture = new CultureInfo("en-US"); // Culture for float conversion
        float uf = float.Parse(values[0], culture);
        float ue = float.Parse(values[1], culture);

        UnityEngine.Debug.Log("uf: " + uf + "; ue: " + ue); // Show the u value in the console

        saveData(uf, ue); // Save the data in a csv file

        /* Transform 0 to 1 number in 0 to 100 numbers, to plot in the graph */
        float xPosition = ue * 100;
        float yPosition = uf * 100;
        
        // /* Saturating the values */
        if (xPosition > 100) xPosition = 100;
        if (yPosition > 100) yPosition = 100;
        if (xPosition < 0) xPosition = 0;
        if (yPosition < 0) yPosition = 0;

        GameObject newPoint = CreateCircle(new Vector2(xPosition, yPosition));
        GameObject newConn = CreateDotConnection(originPoint.GetComponent<RectTransform>().anchoredPosition, 
                            newPoint.GetComponent<RectTransform>().anchoredPosition);
    }

    float findMfx(float mf) {
        int i = 0;
        for (i = 0; i < 90; i++) { 
            if (mf * i >= 90) return i; 
        }
        return -1; // Error
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false); // Set the parent to the graph container
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        /* Set the appearance of the circle */
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3, 3);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject dotConnection = new GameObject("dotConnection", typeof(Image));
        dotConnection.transform.SetParent(graphContainer, false); // Set the parent to the graph container
        dotConnection.GetComponent<Image>().color = new Color(1,1,1, .5f); // Set the color of the line (white with 50% opacity)
        RectTransform rectTransform = dotConnection.GetComponent<RectTransform>(); 
        Vector2 dir = (dotPositionB - dotPositionA).normalized; // Get the direction of the line
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        /* Set the appearance of the line connection */
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0); 
        rectTransform.sizeDelta = new Vector2(distance, .5f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));

        return dotConnection;
    }

    private void ShowGraph(List<int> valueList) {
        float graphHeight = graphContainer.sizeDelta.y; // Get the height of the graph container
        float yMaximum = 100f; // Max value on the graph
        float xSize = 10f; // Size between each point on the graph

        GameObject lastCircleGameObject = null;
        int i = 0;
        for (i = 0; i < valueList.Count; i++) {
            float xPosition = i * xSize; // Get the x position of the point
            float yPosition = (valueList[i] / yMaximum) * graphHeight; // Get the y position of the point
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null) { // If there is a last circle, create a line between the two
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, 
                                    circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    void saveData(float uf, float ue) {
        if (uf > 2 || ue > 2) return; // Ignore the first values (delay in myosym to change state)
        csvContent = uf.ToString() + csvSeparator + ue.ToString() + "\n";
        File.AppendAllText(csvFilePath, csvContent);
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen) serialPort.Close();
    }

    private void calculateParameterByPython() {
        /* Execute the python script to calculate cocontraction parameters */
        string pythonPath = @"C:\Users\Caio\AppData\Local\Microsoft\WindowsApps\python.exe";
        // string pythonPath = @"C:\Users\buzat\AppData\Local\Programs\Python\Python39\python.exe";
        string scriptPath = @"C:\Users\Caio\UFES\IC\MyoController-LegProsthesis\src\cocontraction.py";
        // string scriptPath = @"C:\Users\buzat\Desktop\OCULUS-QUEST-2\MyoController-LegProsthesis\src\cocontraction.py";
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pythonPath;
        startInfo.Arguments = scriptPath;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        var erros = "";
        var results = "";

        /* Execution */
        using (Process process = Process.Start(startInfo)) {
            erros = process.StandardError.ReadToEnd();
            results = process.StandardOutput.ReadToEnd();
        }

        /* Separate results into mf,me,m0 */
        string[] values = results.Split(',');

        UnityEngine.Debug.Log(results); // Show the u value in the console
        UnityEngine.Debug.Log(erros); // Show the u value in the console

        CultureInfo culture = new CultureInfo("en-US"); // Culture for float conversion
        mf = float.Parse(values[0], culture);
        me = float.Parse(values[1], culture);
        m0 = float.Parse(values[2], culture);
        uf_max = float.Parse(values[3], culture);
        ue_max = float.Parse(values[4], culture);
        uf_min = float.Parse(values[5], culture);
        ue_min = float.Parse(values[6], culture);
        vel_max = float.Parse(values[7], culture);
        K_max = float.Parse(values[8], culture);

        var parametersInFloat = new float[] { mf, me, uf_max, ue_max, uf_min, ue_min, vel_max, K_max, ENV_Freq};
        sendParameterToMyoSym(parametersInFloat);

        /* Save the parameters in the PlayerPrefs to get in the next scene */
        PlayerPrefs.SetFloat("uf_max", uf_max);
        PlayerPrefs.SetFloat("ue_max", ue_max);
        PlayerPrefs.SetFloat("uf_min", uf_min);
        PlayerPrefs.SetFloat("ue_min", ue_min);
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

        serialPort.Write(bytesToSend, 0, bytesToSend.Length); // Send the data
    }
}