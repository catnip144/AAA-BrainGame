using System;
using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance { get; private set; }
    public void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    private bool isTutorial = false;
    private Action<MiniGameName> proceedMiniGame;
    private MiniGameName gameName;

    void Update()
    {
        if (isTutorial && CheckUnderstood()) {
            isTutorial = false;
            proceedMiniGame?.Invoke(gameName);
            Debug.Log("미니게임으로 이동합니다.");
        }
    }
    public void ShowTutorial(Action<MiniGameName> proceed_minigame, MiniGameName game_name)
    {
        proceedMiniGame = proceed_minigame;
        gameName = game_name;
        isTutorial = true;
        Debug.Log("튜토리얼 중...");
    }

    private bool CheckUnderstood()
    {
        for (int i = 0; i < PlayerConfigManager.instance.PlayerConfigs.Count; i++) {
            if (PlayerConfigManager.instance.PressKey(PlayerConfigManager.instance.PlayerConfigs[i].Input, InputType.SOUTHBUTTON))
                return true;
        }
        return false;
    }
}
