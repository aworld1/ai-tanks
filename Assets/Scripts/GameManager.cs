// GameManager.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public GameObject longRangeShooterPrefab;
    public GameObject shortRangeShooterPrefab;

    public int playersPerTeam = 3;

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
        SpawnPlayers();

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

    void SpawnPlayers()
    {
        string leftSide = "Long";
        string rightSide = "Long";
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Determine team based on spawn point's x position
            Team team = (spawnPoint.position.x < 0) ? Team.Red : Team.Blue;

            // Alternate between long and short range shooters, depending on the side
            GameObject prefab = (team == Team.Red) 
                ? (leftSide == "Long" ? longRangeShooterPrefab : shortRangeShooterPrefab) 
                : (rightSide == "Long" ? longRangeShooterPrefab : shortRangeShooterPrefab);

            // Toggle between "Long" and "Short" for the next player on the same side
            if (team == Team.Red)
            {
                leftSide = (leftSide == "Long") ? "Short" : "Long";
            }
            else
            {
                rightSide = (rightSide == "Long") ? "Short" : "Long";
            }

            // Instantiate the player as a child of the GameManager at the spawn point's position with default rotation
            GameObject player = Instantiate(prefab, spawnPoint.position, Quaternion.identity, this.transform);

            // Assign team-specific properties and configurations
            AssignTeam(player, team, spawnPoint);
        }
    }

    void AssignTeam(GameObject player, Team team, Transform spawnPoint)
    {
        // Retrieve the Player script from the instantiated player prefab
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            // Assign the team to the player
            playerScript.playerTeam = team;

            // Assign the appropriate layer based on the team for collision management
            if (team == Team.Red)
            {
                player.layer = LayerMask.NameToLayer("RedTeam");
                player.name = "Red_Player_" + (spawnPoints.IndexOf(spawnPoint) + 1);
                redTeamPlayers.Add(player);
                playerScript.AssignRedTeam(); // Assign Red team-specific properties
            }
            else if (team == Team.Blue)
            {
                player.layer = LayerMask.NameToLayer("BlueTeam");
                player.name = "Blue_Player_" + (spawnPoints.IndexOf(spawnPoint) + 1);
                blueTeamPlayers.Add(player);
                playerScript.AssignBlueTeam(); // Assign Blue team-specific properties
            }
            else
            {
                Debug.LogWarning("GameManager: Undefined team assignment.");
            }
        }
        else
        {
            Debug.LogError("GameManager: The instantiated prefab does not have a Player script attached.");
            return;
        }

        // Assign team-specific controls based on team and player index
        AssignControls(player, team, spawnPoints.IndexOf(spawnPoint) % playersPerTeam);
    }

    void AssignControls(GameObject player, Team team, int playerIndex)
    {
        Player playerScript = player.GetComponent<Player>();

        if (playerScript == null)
        {
            Debug.LogError("GameManager: Player script not found on the instantiated player.");
            return;
        }

        if (team == Team.Red)
        {
            // Assign controls for Red team based on player index
            switch(playerIndex)
            {
                case 0:
                    playerScript.turnLeftKey = KeyCode.A;
                    playerScript.turnRightKey = KeyCode.D;
                    playerScript.moveForwardKey = KeyCode.W;
                    break;
                case 1:
                    playerScript.turnLeftKey = KeyCode.LeftShift;
                    playerScript.turnRightKey = KeyCode.LeftControl;
                    playerScript.moveForwardKey = KeyCode.Q;
                    break;
                case 2:
                    playerScript.turnLeftKey = KeyCode.Z;
                    playerScript.turnRightKey = KeyCode.X;
                    playerScript.moveForwardKey = KeyCode.C;
                    break;
                default:
                    Debug.LogWarning("GameManager: Red team player index out of range.");
                    break;
            }
        }
        else if (team == Team.Blue)
        {
            // Assign controls for Blue team based on player index
            switch(playerIndex)
            {
                case 0:
                    playerScript.turnLeftKey = KeyCode.J;
                    playerScript.turnRightKey = KeyCode.L;
                    playerScript.moveForwardKey = KeyCode.I;
                    break;
                case 1:
                    playerScript.turnLeftKey = KeyCode.Keypad4;
                    playerScript.turnRightKey = KeyCode.Keypad6;
                    playerScript.moveForwardKey = KeyCode.Keypad5;
                    break;
                case 2:
                    playerScript.turnLeftKey = KeyCode.Comma;
                    playerScript.turnRightKey = KeyCode.Period;
                    playerScript.moveForwardKey = KeyCode.Slash;
                    break;
                default:
                    Debug.LogWarning("GameManager: Blue team player index out of range.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("GameManager: Undefined team for control assignment.");
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
