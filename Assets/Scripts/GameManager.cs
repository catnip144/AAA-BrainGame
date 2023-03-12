using System.IO;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] List<string> unlockedCharacters = new List<string>();
    public List<string> UnlockedCharacters => unlockedCharacters;
    [SerializeField] List<string> lockedCharacters = new List<string>();

    public List<Color32> PlayerColors = new List<Color32>();
    
    private int currentMiniGameIndex;
    private GameObject currentMinigame;
    private MiniGameName miniGameName;
    [SerializeField] private MiniGameSet allMiniGameSet;
    [SerializeField] private List<OptionBundle> MiniGame2_Options = new List<OptionBundle>();

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public void SetMiniGameType(MiniGameName gameType, Action continueGame)
    {
        miniGameName = gameType;
        List<OptionBundle> possibleOptions = new List<OptionBundle>();

        switch (miniGameName) {
            case MiniGameName.MiniGame2:
                possibleOptions = MiniGame2_Options;
                break;
            
            default:
                break;
        }
        AnswerManager.instance.SetPossibleImages(possibleOptions);
        AnswerManager.instance.SetNextGameAction(continueGame);
    }

    public void NextMiniGame()
    {
        AnswerManager.instance.TotalCanvas.alpha = 1;

        if (currentMiniGameIndex == 5) {
            // Show Results
            currentMiniGameIndex = 0;
            return;
        }

        List<MiniGameName> miniGameList = new List<MiniGameName>();

        switch (currentMiniGameIndex) {
            case 0:
                miniGameList = allMiniGameSet.SenseType;
                break;
            case 1:
                miniGameList = allMiniGameSet.MemorizeType;
                break;
            case 2:
                miniGameList = allMiniGameSet.AnalyzeType;
                break;
            case 3:
                miniGameList = allMiniGameSet.CalculateType;
                break;
            case 4:
                miniGameList = allMiniGameSet.VisualizeType;
                break;
            default:
                break;
        }
        int randomIndex = Random.Range(0, miniGameList.Count);
        MiniGameName minigame = miniGameList[randomIndex];

        Destroy(currentMinigame);
        // Instantiate MiniGame Prefab
        // Get Interface. Function
        // SetMiniGameType(minigame, )

        // interface 구현된 prefab 쪽에서
        // 튜토리얼 자동으로 보여줄 것이고
        // 튜토리얼 확인 누를 시 문제를 실행
        // 문제 실행이 끝나면 AnswerManager SetCorrectAnswer(answer) 호출

        currentMiniGameIndex += 1;
    }
}

public enum MiniGameName {
    // SenseType
    // MemorizeType
    // AnalyzeType
    CrocGame, // CalculateType
    MiniGame2 // VisualizeType
}

[System.Serializable]
public class OptionBundle
{
    public string imageTag;
    public Sprite optionImage;
}

[System.Serializable]
public class MiniGameSet
{
    public List<MiniGameName> SenseType = new List<MiniGameName>();
    public List<MiniGameName> MemorizeType = new List<MiniGameName>();
    public List<MiniGameName> AnalyzeType = new List<MiniGameName>();
    public List<MiniGameName> CalculateType = new List<MiniGameName>();
    public List<MiniGameName> VisualizeType = new List<MiniGameName>();
}
