// TODO: passar todos os comentátios para ingles

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Importe a classe SerialPort do UnityEngine

public class MoveArm : MonoBehaviour {
    private SerialPort serialPort;
    public string portName = "COM9"; // Porta serial apropriada
    public int baudRate = 9600; // Taxa de transmissão
    public Transform arm; // Braço

    private float armAngle = 0;
    private bool change = true;

    // Start is called before the first frame update
    void Start() {
        // serialPort = new SerialPort(portName, baudRate);
        // if (!serialPort.IsOpen) serialPort.Open(); // Verifica se a porta não está aberta
        armAngle = 90; // Inicializa o ângulo do braço
    }

    // Update is called once per frame
    void Update() {
        // if (serialPort.IsOpen) {
            // string serialData = serialPort.ReadLine(); // Lê uma linha de dados da porta serial
            // string[] values = serialData.Split(','); // Divide os valores separados por vírgula
            // float armAngle = float.Parse(values[2]); // Converte a string em float
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

        Debug.Log(armAngle);
        

        /* Cria uma rotação em torno do eixo X com base no ângulo mapeado */
        arm.localRotation = Quaternion.Euler(0f, armAngle, 0f); // Use localRotation para rotação local em torno do eixo X
        // Debug.Log(armAngle+100);
    }

    // void OnApplicationQuit() {
    //     if (serialPort.IsOpen) serialPort.Close();
    // }
}