// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public GameObject redPlayerPrefab; // Declare red team player prefab
    public GameObject bluePlayerPrefab;
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
        //SpawnPlayers();
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
        // Randomly decide which team starts first

        if (Random.value < 0.5f)
        {
            ResetPlayersRedFirst();
        }
        else
        {
            ResetPlayersBlueFirst();
        }
    }

    void ResetPlayersRedFirst()
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
                player.transform.rotation = Quaternion.Euler(0, 0, -90f);

                // Reset player properties
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.Initialize();
                    playerScript.AssignRedTeam();
                    playerScript.gameObject.SetActive(true);
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
                player.transform.rotation = Quaternion.Euler(0, 0, 90f);

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

    void ResetPlayersBlueFirst()
    {
        redTeamPlayers.Clear();
        blueTeamPlayers.Clear();

        // Reset blue team players
        for (int i = 0; i < blueTeamStartingPlayers.Length; i++)
        {
            if (blueTeamStartingPlayers[i] != null)
            {
                // Get spawn point for this player
                Transform spawnPoint = spawnPoints[i];
                GameObject player = blueTeamStartingPlayers[i];

                // Reset position and rotation
                player.transform.position = spawnPoint.position;
                player.transform.rotation = Quaternion.Euler(0, 0, -90f);

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

        // Reset red team players
        for (int i = 0; i < redTeamStartingPlayers.Length; i++)
        {
            if (redTeamStartingPlayers[i] != null)
            {
                // Get spawn point for this player
                Transform spawnPoint = spawnPoints[i + blueTeamStartingPlayers.Length];
                GameObject player = redTeamStartingPlayers[i];
                
                // Reset position and rotation
                player.transform.position = spawnPoint.position;
                player.transform.rotation = Quaternion.Euler(0, 0, 90f);

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
    }

    // void SpawnPlayers()
    // {
    //     // Spawn red team players
    //     foreach (Transform spawnPoint in spawnPoints)
    //     {
    //         GameObject player = Instantiate(redPlayerPrefab, spawnPoint.position, Quaternion.Euler(0, 0, -90f));
    //         Player playerScript = player.GetComponent<Player>();
    //         if (playerScript != null)
    //         {
    //             playerScript.Initialize();
    //             playerScript.AssignRedTeam();
    //             redTeamPlayers.Add(player);
    //         }
    //     }

    //     // // Spawn blue team players
    //     // foreach (Transform spawnPoint in blueTeamSpawnPoints)
    //     // {
    //     //     GameObject player = Instantiate(bluePlayerPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 90f));
    //     //     Player playerScript = player.GetComponent<Player>();
    //     //     if (playerScript != null)
    //     //     {
    //     //         playerScript.Initialize();
    //     //         playerScript.AssignBlueTeam();
    //     //         blueTeamPlayers.Add(player);
    //     //     }
    //     // }
    // }

    void CheckWinCondition()
    {
        // Remove any inactive players from the player lists
        redTeamPlayers.RemoveAll(player => !player.activeInHierarchy);
        blueTeamPlayers.RemoveAll(player => !player.activeInHierarchy);

        // Check if Red Team has no active players left
        if (redTeamPlayers.Count == 0 && blueTeamPlayers.Count > 0)
        {
            DeclareWinner("Blue");
            for (int i = 0; i < blueTeamStartingPlayers.Count; i++)
            {
                blueTeamStartingPlayers[i].Win();
            }
            for (int i = 0; i < redTeamStartingPlayers.Count; i++)
            {
                redTeamStartingPlayers[i].Lose();
            }
           
        }
        // Check if Blue Team has no active players left
        else if (blueTeamPlayers.Count == 0 && redTeamPlayers.Count > 0)
        {
            DeclareWinner("Red");
            for (int i = 0; i < redTeamStartingPlayers.Count; i++)
            {
                redTeamStartingPlayers[i].Win();
            }
            for (int i = 0; i < blueTeamStartingPlayers.Count; i++)
            {
                blueTeamStartingPlayers[i].Lose();
            }
        }
        // Optional: Handle tie if both teams have no active players
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

        // Start coroutine to restart the game after 1 second
        StartCoroutine(RestartGameAfterDelay(0f));
    }

    IEnumerator RestartGameAfterDelay(float delay)
    {
        // Wait for the specified delay in real time
        yield return new WaitForSecondsRealtime(delay);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
