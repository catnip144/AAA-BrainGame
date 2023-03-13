using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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
            Debug.Log("미니게임 준비 중...");
            DOTween.Rewind("ShowBoard");
            DOTween.Play("ShowBoard");

            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);

        foreach(PlayerConfiguration config in playerConfigs) {
            config.PlayerSetup.SetPlayerStatus(false);
            config.Input.transform.SetParent(null);
            DontDestroyOnLoad(config.Input.gameObject);
        }
        DOTween.Rewind("HideBoard");
        DOTween.Play("HideBoard");

        var process = SceneManager.LoadSceneAsync("MiniGame");
        process.completed += (AsyncOperation operation) =>
        {
            GameManager.instance.NextMiniGame();
        };
    }

    public void HandlePlayerJoin(PlayerInput player_input)
    {
        if (!playerConfigs.Any(p => p.PlayerIndex == player_input.playerIndex)) {
            PlayerConfiguration player = new PlayerConfiguration(player_input);

            playerConfigs.Add(player);
            player_input.transform.SetParent(GameObject.FindWithTag("MainLayout").transform);
            player.PlayerSetup.SetPlayer(playerConfigs[player_input.playerIndex]);

            Debug.Log($"Player {player_input.playerIndex + 1}이 참여했습니다.");
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
        InputDetect = player_input.gameObject.GetComponent<PlayerInputDetect>();
        PlayerSetup = player_input.gameObject.GetComponent<PlayerSetupControl>();
    }
    public PlayerInput Input { get; set; }
    public PlayerInputDetect InputDetect { get; set; }
    public PlayerSetupControl PlayerSetup { get; set; }
    public int PlayerScore { get; set; }
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
