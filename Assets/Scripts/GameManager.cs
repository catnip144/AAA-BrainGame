using System.IO;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] List<string> unlockedCharacters = new List<string>();
    public List<string> UnlockedCharacters => unlockedCharacters;
    [SerializeField] List<string> lockedCharacters = new List<string>();

    public List<Color32> PlayerColors = new List<Color32>();
    
    private int currentMiniGameIndex;
    private GameObject currentMinigame;
    [SerializeField] private MiniGameSet miniGameClassification;


    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    
    public void NextMiniGame()
    {
        Debug.Log($"MiniGame Type : {currentMiniGameIndex + 1}");
        AnswerManager.instance.TotalCanvas.alpha = 1;

        if (currentMiniGameIndex == 5) {
            Debug.Log("게임 종료! 최종 결과 화면으로 이동합니다."); // Show Results
            currentMiniGameIndex = 0;
            return;
        }

        Destroy(currentMinigame);
        MiniGameName randomMiniGame = ReturnRandomMiniGame();
        Tutorial.instance.ShowTutorial(SetMiniGame, randomMiniGame);

        /*
        1. interface 구현된 prefab
        2. 튜토리얼 보여줄 것이고
        3. 튜토리얼 확인 누를 시 문제를 실행
        4. 문제 실행이 끝나면 AnswerManager SetCorrectAnswer(answer) 호출
        */
    }
    private MiniGameName ReturnRandomMiniGame()
    {
        List<MiniGameName> miniGameList = new List<MiniGameName>();

        switch (currentMiniGameIndex) {
            case 1:
                miniGameList = miniGameClassification.SenseType;
                break;
            case 0: // 테스트를 위한 임시방편, 원래는 1
                miniGameList = miniGameClassification.MemorizeType;
                break;
            case 2:
                miniGameList = miniGameClassification.AnalyzeType;
                break;
            case 3:
                miniGameList = miniGameClassification.CalculateType;
                break;
            case 4:
                miniGameList = miniGameClassification.VisualizeType;
                break;
            default:
                break;
        }
        int randomIndex = Random.Range(0, miniGameList.Count);       
        return miniGameList[randomIndex];
    }

    private void SetMiniGame(MiniGameName inputGameName)
    {
        currentMiniGameIndex += 1;
        MiniGameInfo targetGameInfo = null;

        var allMiniGames = Resources.LoadAll<MiniGameInfo>("MiniGame/");
        foreach (MiniGameInfo miniGame in allMiniGames) {
            if (miniGame.miniGameName == inputGameName) {
                targetGameInfo = miniGame;
                break;
            }
        }

        if (targetGameInfo == null) {
            Debug.LogError($"MiniGame : {inputGameName} could not be loaded.");
            return;
        }

        currentMinigame = Instantiate(targetGameInfo.MiniGamePrefab, AnswerManager.instance.miniGameSpawn);
        
        if (targetGameInfo.possibleOptions.Count > 0)
            AnswerManager.instance.SetPossibleImages(targetGameInfo.possibleOptions);
        
        MiniGameInterface miniGameInterface = currentMinigame.GetComponent<MiniGameInterface>();
        AnswerManager.instance.SetNextGameAction(miniGameInterface.ReturnContinueGame());
        AnswerManager.instance.ShowProblem();
    }
}

public enum MiniGameName {
    // SenseType
    // MemorizeType
    // AnalyzeType
    CrocGame, // CalculateType
    // VisualizeType
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
