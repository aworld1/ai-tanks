// GameManager.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public GameObject[] redTeamStartingPlayers; // Assign existing red team players in inspector
    public GameObject[] blueTeamStartingPlayers; // Assign existing blue team players in inspector

    public List<Transform> spawnPoints; // Single list containing all spawn points

    // Reference to the TextMeshPro Text element for displaying win messages
    public TMP_Text winText;

    // Lists to keep track of active players on each team
    private List<GameObject> redTeamPlayers = new List<GameObject>();
    private List<GameObject> blueTeamPlayers = new List<GameObject>();

    // Flag to determine if the game has ended
    private bool gameEnded = false;

    void Start()
    {
        ResetPlayers();

        // Initially hide the win text
        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameManager: WinText is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (!gameEnded)
        {
            CheckWinCondition();
        }
    }

    void ResetPlayers()
    {
        redTeamPlayers.Clear();
        blueTeamPlayers.Clear();

        // Reset red team players
        for (int i = 0; i < redTeamStartingPlayers.Length; i++)
        {
            if (redTeamStartingPlayers[i] != null)
            {
                // Get spawn point for this player
                Transform spawnPoint = spawnPoints[i];
                GameObject player = redTeamStartingPlayers[i];
                
                // Reset position and rotation
                player.transform.position = spawnPoint.position;
                player.transform.rotation = Quaternion.identity;

                // Reset player properties
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.Initialize();
                    playerScript.AssignRedTeam();
                    redTeamPlayers.Add(player);
                }
            }
        }

        // Reset blue team players
        for (int i = 0; i < blueTeamStartingPlayers.Length; i++)
        {
            if (blueTeamStartingPlayers[i] != null)
            {
                // Get spawn point for this player
                Transform spawnPoint = spawnPoints[i + redTeamStartingPlayers.Length];
                GameObject player = blueTeamStartingPlayers[i];

                // Reset position and rotation
                player.transform.position = spawnPoint.position;
                player.transform.rotation = Quaternion.identity;

                // Reset player properties
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.Initialize();
                    playerScript.AssignBlueTeam();
                    blueTeamPlayers.Add(player);
                }
            }
        }
    }

    void CheckWinCondition()
    {
        // Remove any null references from the player lists (players that have been destroyed)
        redTeamPlayers.RemoveAll(player => player == null);
        blueTeamPlayers.RemoveAll(player => player == null);

        // Check if Red Team has no players left
        if (redTeamPlayers.Count == 0 && blueTeamPlayers.Count > 0)
        {
            DeclareWinner("Blue");
        }
        // Check if Blue Team has no players left
        else if (blueTeamPlayers.Count == 0 && redTeamPlayers.Count > 0)
        {
            DeclareWinner("Red");
        }
        // Optional: Handle tie if both teams have no players
        else if (redTeamPlayers.Count == 0 && blueTeamPlayers.Count == 0)
        {
            DeclareWinner("No one");
        }
    }

    void DeclareWinner(string winningTeam)
    {
        gameEnded = true;

        // Display the win message using TextMeshPro
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            if (winningTeam == "No one")
            {
                winText.text = "It's a Tie!";
            }
            else
            {
                winText.text = $"{winningTeam} Team Won!";
            }
        }
        else
        {
            Debug.LogWarning("GameManager: WinText is not assigned in the Inspector.");
        }

        // Stop the game by pausing time
        Time.timeScale = 0f;

        // Optionally, disable player inputs or other game functionalities here
    }
}
