using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class ColorGame : MonoBehaviour {
    /* Getting the blocks that will be changing color */
    public Transform block1;
    public Transform block2;
    public Transform colorIndicator;
    public Image backgroundImage;

    public TextMeshProUGUI scorePointsText;
    private int scorePoints;
    private float time;

    public AudioSource rightAnswerSound;
    public AudioSource wrongAnswerSound;

    /* Defining the colors */
    static Color red = new Color(1.0f, 0.0f, 0.0f);
    static Color green = new Color(0.0f, 1.0f, 0.0f);
    static Color blue = new Color(0.0f, 0.0f, 1.0f);
    static Color yellow = new Color(1.0f, 1.0f, 0.0f);
    static Color[] colors = {red, green, blue, yellow};

    private bool isFirstTime; // Flag to check if it is the first time the game is being played
    private bool startGame = false; // Flag to check if the game has started

    private string csvFilePath = "Data/mainScene/colorGame" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv";
    private string csvContent;

    private float TIME_OF_GAME = 10f; // Time of the game


    // Start is called before the first frame update
    void Start() {
        scorePoints = 0; // Set the score points to 0
        isFirstTime = true; // Flag to check if it is the first time the game is being played
        time = 0.0f; // Initialize time
        backgroundImage.color = new Color(0.5f, 0.5f, 0.5f); // Set the color of the game background gray
    }

    // Update is called once per frame
    void Update() {
        scorePointsText.text = scorePoints.ToString(); // Update the score points text

        Color indicatorColor;
        if (isFirstTime) {
            indicatorColor = setColors(); // Set the colors for the first time
            isFirstTime = false; // Set the flag to false
        } else {
            indicatorColor = colorIndicator.GetComponent<Renderer>().material.color; // Get the color of the color indicator
        }

        /* Check the tag of all blocks */
        if (block1.CompareTag("Untagged")) { // Was clicked
            if (block1.GetComponent<Renderer>().material.color == indicatorColor) { // Was the right one
                rightAnswerSound.Play();
                indicatorColor = setColors(); // Set new colors
                scorePoints++; // Increment the score points
            } else {
                wrongAnswerSound.Play();
                block1.tag = "ColorBlock"; // Set the tag back to ColorBlock
            }
        }

        if (block2.CompareTag("Untagged")) { // Was clicked
            if (block2.GetComponent<Renderer>().material.color == indicatorColor) { // Was the right one
                rightAnswerSound.Play();
                indicatorColor = setColors(); // Set new colors
                scorePoints++; // Increment the score points
            } else {
                wrongAnswerSound.Play();
                block2.tag = "ColorBlock"; // Set the tag back to ColorBlock
            }
        }

        if (startGame && time >= TIME_OF_GAME) { // If the time game is over (50 seconds)
            startGame = false; // Set the flag to false
            saveData(scorePoints); // Save the score points
            backgroundImage.color = new Color(0.5f, 0.5f, 0.5f); // Set the color of the game background gray
        } else if (time < TIME_OF_GAME) { // If the game is still running or has not started
            time += Time.deltaTime; // Increment the time
        }
    }

    Color setColors() {
        int colorIndex = UnityEngine.Random.Range(0, 2); // Random number between 0 and 1
        colorIndicator.GetComponent<Renderer>().material.color = colors[colorIndex]; // Set the color of the color indicator
        Color indicatorColor = colors[colorIndex]; // Get the color of the color indicator

        /* Sort the color vector randonly */
        int[] colorOrder = {0, 1};
        for (int i = 0; i < 2; i++) {
            int temp = colorOrder[i];
            int randomIndex = UnityEngine.Random.Range(i, 2);
            colorOrder[i] = colorOrder[randomIndex];
            colorOrder[randomIndex] = temp;
        }

        /* Set the color of the blocks in order */
        block1.GetComponent<Renderer>().material.color = colors[colorOrder[0]];
        block2.GetComponent<Renderer>().material.color = colors[colorOrder[1]];

        /* Set the tags of the blocks */
        block1.tag = "ColorBlock";
        block2.tag = "ColorBlock";

        return indicatorColor;
    }

    void saveData(int scorePoints) {
        csvContent = scorePoints.ToString(); // Create the csv content with the time
        File.AppendAllText(csvFilePath, csvContent); // Append the content to the csv file
    }

    public void toggleStartGame() {
        startGame = !startGame;
        scorePoints = 0; // Set the score points to 0
        time = 0.0f; // Initialize time
        backgroundImage.color = new Color(0.5449448f, 0.735849f, 0.6624243f); // Set the color of the game background green
        csvFilePath = "Data/mainScene/colorGame" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".csv"; // Update the csv file path
    }
}