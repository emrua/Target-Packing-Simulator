using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;       // Timer Text (TextMeshProUGUI)
    public TextMeshProUGUI scoreText;       // Score Text (TextMeshProUGUI)
    public GameObject endScreen;            // End Screen UI
    public TextMeshProUGUI timeLeftText;    // Time Left Text on End Screen (TextMeshProUGUI)
    public TextMeshProUGUI finalScoreText;  // Final Score Text on End Screen (TextMeshProUGUI)

    public GameObject[] levelButtons;       // Array for Level Buttons
    public OrderQueueManager orderQueueManager; // Reference to OrderQueueManager
    public Transform packingAreaParent;     // Assign PackingArea

    private float timeRemaining;            // Time left for the level

    public bool timerStopped;
    private int score = 0;                  // Current score
    private int currentLevel = 1;           // Default to level 1

    public GameObject startScreen;
    public GameObject instructionScreen;
    public GameObject loseScreen;
    public GameObject winScreen;

    private void Start()
    {
        Debug.Log("GameManager: Start called.");
        
        // Ensure the OrderQueueManager is assigned
        if (orderQueueManager == null)
        {
            orderQueueManager = GameObject.Find("Content")?.GetComponent<OrderQueueManager>();
            if (orderQueueManager == null)
            {
                Debug.LogError("OrderQueueManager not found! Ensure the Content GameObject has the OrderQueueManager script.");
                return;
            }
        }

        SetupLevel(currentLevel); // Start the first level
    }

    private void Update()
    {
        // Check if the timer is active and decrement timeRemaining
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Reduce the timer by the time between frames
            UpdateTimerUI(); // Update the UI to reflect the new time

            if (timeRemaining <= 0)
            {
                timeRemaining = 0; // Clamp the timer to 0
                 LoseGame();
            }
        }
    }



    public void SetupLevel(int level)
        {
        Debug.Log($"SetupLevel called with level {level}.");

        if (orderQueueManager == null)
        {
            Debug.LogError("OrderQueueManager is null! Cannot set up the level.");
            return;
        }

        // Reset the score when starting a new level
        score = 0;
        UpdateScoreUI();

        // Clear the packing area
        ClearPackingArea();

        // Clear the order queue
        orderQueueManager.ClearOrderQueue();

        // Set up the new level
        switch (level)
        {
            case 1:
                timeRemaining = 90f;
                orderQueueManager.PopulateOrderQueue(7);
                break;
            case 2:
                timeRemaining = 60f;
                orderQueueManager.PopulateOrderQueue(10);
                break;
            case 3:
                timeRemaining = 45f;
                orderQueueManager.PopulateOrderQueue(15);
                break;
            default:
                Debug.LogError("Invalid level selected!");
                return;
        }

        UpdateTimerUI();
    }


    private void ClearPackingArea()
    {
        Debug.Log("Clearing packing area...");
        foreach (Transform child in packingAreaParent)
        {
            Destroy(child.gameObject); // Destroy all child objects in PackingArea
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void NextLevel()
    {
        // Increment the current level
        currentLevel++;

        // Check if the next level exists
        if (currentLevel > levelButtons.Length)
        {
            Debug.Log("No more levels! Returning to main menu or restarting the game.");
            currentLevel = 1; // Reset to level 1 or handle as needed
            SceneManager.LoadScene("MainMenu"); // Load your MainMenu scene (adjust the name accordingly)
        }
        else
        {
            Debug.Log($"Loading Level {currentLevel}...");
            SetupLevel(currentLevel); // Set up the next level
            endScreen.SetActive(false); // Hide the end screen
        }
    }

    public void FinishBox()
{
    // Stop the timer
    timeRemaining = Mathf.Max(0, timeRemaining);

    // Calculate final score with bonus points
    int bonusPoints = Mathf.FloorToInt(timeRemaining) * 5; // 5 points per second left
    score += bonusPoints;

    // Update end screen UI
    finalScoreText.text = $"Final Score: {score}";
    timeLeftText.text = $"Time Left: {Mathf.CeilToInt(timeRemaining)}s";

    // Check if the player has completed the final level (Win Condition)
    if (currentLevel == levelButtons.Length) // Assuming levelButtons.Length represents the total number of levels
    {
        Debug.Log("Game Won!");
        winScreen.SetActive(true); // Show the win screen
    }
    else if (timeRemaining == 0) // Lose condition when time runs out
    {
        Debug.Log("Game Lost!");
        loseScreen.SetActive(true); // Show the lose screen
    }
    else
    {
        // Enable or disable the Next Level button for other levels
        Button nextLevelButton = endScreen.transform.Find("NextLevelButton").GetComponent<Button>();
        if (currentLevel < levelButtons.Length)
        {
            nextLevelButton.gameObject.SetActive(true); // Enable the button
        }
        else
        {
            nextLevelButton.gameObject.SetActive(false); // Hide if no more levels
        }

        endScreen.SetActive(true); // Show the end screen for other levels
    }
}


    public void RestartGame()
    {
        winScreen.SetActive(false); // Hide the win screen
        SetupLevel(1); 
    }

    public void ResetGame()
    {
        Debug.Log("ResetGame: Resetting the game...");

        // Iterate through the PackingArea children in reverse order to avoid skipping items
        for (int i = packingAreaParent.childCount - 1; i >= 0; i--)
        {
            Transform child = packingAreaParent.GetChild(i);
            Debug.Log($"ResetGame: Returning {child.name} to the queue.");

            // Move the child back to the queue
            child.SetParent(orderQueueManager.Content);
            child.localPosition = Vector3.zero; // Reset position in the queue
        }

        // Reset the score
        score = 0;
        UpdateScoreUI(); // Update the score UI

        Debug.Log("ResetGame: Game reset completed.");
    }


    public void StartGame()
    {
        Debug.Log("Game Started");
        startScreen.SetActive(false); // Hide the start screen
        SetupLevel(1); // Start the first level
    }

    
    public void ShowInstructions()
    {
        Debug.Log("Showing Instructions");
        instructionScreen.SetActive(true);
        startScreen.SetActive(false); // Hide the start screen
    }

    public void HideInstructions()
    {
        Debug.Log("Hiding Instructions");
        instructionScreen.SetActive(false);
        startScreen.SetActive(true); // Return to the start screen
    }

    public void LoseGame()
    {
        Debug.Log("Game Lost!");
        loseScreen.SetActive(true); // Show the lose screen
        timerStopped = true; // Stop further updates
    }

    public void RetryLevel()
    {
        Debug.Log("Retrying Level");
        loseScreen.SetActive(false); // Hide the lose screen
        SetupLevel(currentLevel); // Restart the current level
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}