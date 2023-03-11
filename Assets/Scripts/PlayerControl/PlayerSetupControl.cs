using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerSetupControl : MonoBehaviour
{
    private int PlayerIndex, selectedCharIndex;
    private PlayerConfiguration playerConfig;
    [SerializeField] private Animator anim;
    [SerializeField] private Image statusIcon;
    [SerializeField] private Sprite ReadyIcon, WaitingIcon;
    [SerializeField] private TextMeshProUGUI PlayerText;
    [SerializeField] private GameObject Arrows;
    private bool inputEnabled = false;
    private string selectedCharacterName;

    void Update()
    {
        UINavigation();
    }

    private void UINavigation()
    {
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
            return;
        }

        if (previousIndex == selectedCharIndex)
            return;
        selectedCharacterName = GameManager.instance.UnlockedCharacters[selectedCharIndex];
        SetCharacter(selectedCharacterName);
    }

    public void SetPlayer(PlayerConfiguration config)
    {
        playerConfig = config;
        PlayerIndex = config.PlayerIndex;
        PlayerText.text = $"P{PlayerIndex + 1}";
        PlayerText.color = GameManager.instance.PlayerColors[PlayerIndex];

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
}
