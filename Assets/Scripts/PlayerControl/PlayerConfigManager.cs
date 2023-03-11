using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class PlayerConfigManager : MonoBehaviour
{
    public static PlayerConfigManager instance { get; private set; }
    private List<PlayerConfiguration> playerConfigs = new List<PlayerConfiguration>();
    public List<PlayerConfiguration> PlayerConfigs => playerConfigs;

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public void TryGameStart()
    {
        if (playerConfigs.All(p => p.IsReady == true))
        {
            Debug.Log("Starting Minigame...");
            foreach(PlayerConfiguration config in playerConfigs) {
                config.Input.transform.SetParent(null);
                DontDestroyOnLoad(config.Input.gameObject);
            }
            SceneManager.LoadScene("MiniGame");
        }
    }

    public void HandlePlayerJoin(PlayerInput player_input)
    {
        if (!playerConfigs.Any(p => p.PlayerIndex == player_input.playerIndex)) {
            playerConfigs.Add(new PlayerConfiguration(player_input));
            player_input.transform.SetParent(GameObject.FindWithTag("MainLayout").transform);
            player_input.gameObject.GetComponent<PlayerSetupControl>().SetPlayer(playerConfigs[player_input.playerIndex]);

            Debug.Log($"Player {player_input.playerIndex + 1} Joined.");
        }
    }
    public bool PressKey(PlayerInput pi, string input_tag)
    {
        return pi.actions[input_tag].triggered;
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput player_input) {
        PlayerIndex = player_input.playerIndex;
        Input = player_input;
    }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public string CharacterType { get; set; }
}

public class InputType
{
    public const string LEFT = "LEFT"; public const string RIGHT = "RIGHT";
    public const string UP = "UP"; public const string DOWN = "DOWN";
    public const string SOUTHBUTTON = "SOUTHBUTTON";
    public const string EASTBUTTON = "EASTBUTTON";
    public const string NORTHBUTTON = "NORTHBUTTON";
    public const string WESTBUTTON = "WESTBUTTON";
}
