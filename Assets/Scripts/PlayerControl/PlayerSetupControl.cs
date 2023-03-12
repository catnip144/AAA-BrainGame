using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerSetupControl : MonoBehaviour
{
    private int selectedCharIndex;
    private PlayerConfiguration playerConfig;

    [SerializeField] private Animator anim;
    public Animator PlayerAnim => anim;

    [SerializeField] private Image statusIcon;
    [SerializeField] private Sprite ReadyIcon, WaitingIcon;
    [SerializeField] private TextMeshProUGUI PlayerText;
    [SerializeField] private GameObject Arrows;

    private bool inputEnabled = false;
    private bool isGameMode = false;
    public bool IsGameMode => isGameMode;
    public string SelectedCharacterName {get; private set; }

    void Update()
    {
        UINavigation();
    }

    private void UINavigation()
    {
        if ((isGameMode) || (!inputEnabled))
            return;

        int previousIndex = selectedCharIndex;

        if (PlayerConfigManager.instance.PressKey(playerConfig.Input, InputType.LEFT))
        {
            selectedCharIndex++;
            if (selectedCharIndex >= GameManager.instance.UnlockedCharacters.Count)
                selectedCharIndex = 0;
        }
        else if (PlayerConfigManager.instance.PressKey(playerConfig.Input, InputType.RIGHT))
        {
            selectedCharIndex--;
            if (selectedCharIndex < 0)
                selectedCharIndex = GameManager.instance.UnlockedCharacters.Count - 1;
        }
        else if (PlayerConfigManager.instance.PressKey(playerConfig.Input, InputType.SOUTHBUTTON))
        {
            ReadyPlayer();
            isGameMode = true;
            return;
        }

        if (previousIndex == selectedCharIndex) return;

        SelectedCharacterName = GameManager.instance.UnlockedCharacters[selectedCharIndex];
        SetCharacter(SelectedCharacterName);
    }

    public void SetPlayer(PlayerConfiguration config)
    {
        playerConfig = config;
        PlayerText.text = $"P{config.PlayerIndex + 1}";
        PlayerText.color = GameManager.instance.PlayerColors[config.PlayerIndex];

        Invoke("EnableInput", 0.4f);
    }
    private void EnableInput()
    {
        inputEnabled = true;
    }
    private void SetCharacter(string character_type)
    {
        if (!inputEnabled)
            return;
        
        playerConfig.CharacterType = character_type;
        anim.Play(character_type);
    }
    private void ReadyPlayer()
    {
        if (!inputEnabled)
            return;
        inputEnabled = false;
        
        playerConfig.IsReady = true;
        //anim.Play($"{selectedCharacterName}_Ready");
        statusIcon.sprite = ReadyIcon;
        Arrows.SetActive(false);

        PlayerConfigManager.instance.TryGameStart();
    }

    public void SetPlayerStatus(bool hasChosenAnswer)
    {
        if (hasChosenAnswer)
            statusIcon.sprite = ReadyIcon;
        else
            statusIcon.sprite = WaitingIcon;
    }
}
