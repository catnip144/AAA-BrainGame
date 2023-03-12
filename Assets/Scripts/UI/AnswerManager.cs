using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager instance { get; private set; }
    public CanvasGroup TotalCanvas;

    [SerializeField] private List<TextMeshProUGUI> numberOptions;
    [SerializeField] private List<Image> imageOptions;
    private List<OptionBundle> possibleImageList = new List<OptionBundle>();
    private Action continueGame;

    private AnswerType answerType = AnswerType.Number;
    public AnswerType answer_type => answerType;
    
    private string correctAnswer;
    private int correctAnswerIndex = -1;

    private int gameCount = 1;
    
    private bool enableEvaluateMode = false;

    public void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public void SetNextGameAction(Action action)
    {
        continueGame = action;
    }

    public void SetPossibleImages(List<OptionBundle> possibleImages)
    {
        possibleImageList = possibleImages;
    }

    // 문제 보여주기가 끝나면 SetCorrectAnswer()를 호출하여 정답과 선택지를 초기화시켜준 후
    // 플레이어를 자유롭게 풀어준다.
    public void SetCorrectAnswer(string answer)
    {
        correctAnswer = answer;

        if (int.TryParse(answer, out int num))
            answerType = AnswerType.Number;
        else
            answerType = AnswerType.Image;
        
        SetOptions();
        StartProblem();
    }

    private void StartProblem()
    {
        foreach (PlayerConfiguration player in PlayerConfigManager.instance.PlayerConfigs) {
            player.InputDetect.UnlockAnswerSelection();
        }

        // Start Timer
    }

    private void SetOptions()
    {
        if (answerType == AnswerType.Number) {
            int number_answer = int.Parse(correctAnswer);
            List<int> answerValues = new List<int>();

            // 답을 포함한 랜덤 선택지 4개 생성
            while (answerValues.Count < 3) {
                int randomNumber = Random.Range(number_answer - 4, number_answer + 5);
                if (randomNumber != number_answer)
                    answerValues.Add(randomNumber);
            }
            answerValues.Add(number_answer);

            // 랜덤 순서로 선택지를 시각화
            for (int i = 0; i < 4; i++) {
                int randomIndex = Random.Range(0, answerValues.Count);
                numberOptions[i].text = $"{answerValues[randomIndex]}";

                if (number_answer == answerValues[randomIndex])
                    correctAnswerIndex = randomIndex;
                
                answerValues.RemoveAt(randomIndex);
            }
        }
        else if (answerType == AnswerType.Image) {
            OptionBundle image_answer = ReturnOptionBundle(correctAnswer);
            List<OptionBundle> answerValues = new List<OptionBundle>();

            while (answerValues.Count < 3) {
                OptionBundle ob = possibleImageList[Random.Range(0, possibleImageList.Count)];
                if (ob.imageTag != correctAnswer)
                    answerValues.Add(ob);
            }
            answerValues.Add(image_answer);

            for (int i = 0; i < 4; i++) {
                int randomIndex = Random.Range(0, answerValues.Count);
                imageOptions[i].sprite = answerValues[randomIndex].optionImage;

                if (correctAnswer == answerValues[randomIndex].imageTag)
                    correctAnswerIndex = randomIndex;
                
                answerValues.RemoveAt(randomIndex);
            }
        }
    }

    private OptionBundle ReturnOptionBundle(string imageTag)
    {
        foreach (OptionBundle ob in possibleImageList) {
            if (imageTag == ob.imageTag)
                return ob;
        }
        return null;
    }

    public bool CheckOtherPlayers()
    {
        foreach (PlayerConfiguration config in PlayerConfigManager.instance.PlayerConfigs) {
            bool isAnswerChosen = config.InputDetect.LockAnswer;
            if (!isAnswerChosen)
                return false;
        }
        enableEvaluateMode = true;
        return true;
    }

    public void EvaluateAnswers()
    {
        // Cancel Timer
        
        if (enableEvaluateMode) // 플레이어 동시 입력으로 인한 중복 호출을 방지하고자 추가
        {
            enableEvaluateMode = false;
            
            foreach (PlayerConfiguration player in PlayerConfigManager.instance.PlayerConfigs) {
                if (player.InputDetect.ChosenAnswerIndex == correctAnswerIndex)
                    CorrectPlayer(player);
                else 
                    IncorrectPlayer(player);
                
                player.InputDetect.ResetAnswerIndex();
            }
            Invoke("ContinueGame", 3f);
        }
    }

    private void CorrectPlayer(PlayerConfiguration player)
    {
        PlayerSetupControl psc = player.PlayerSetup;
        //psc.PlayerAnim.Play(psc.SelectedCharacterName + "_Happy");

        // Instantiate(RewardPoint)
        player.PlayerScore += player.InputDetect.timePoint;
        Debug.Log($"Player {player.PlayerIndex + 1} is correct!");
    }

    private void IncorrectPlayer(PlayerConfiguration player)
    {
        //psc.PlayerAnim.Play(psc.SelectedCharacterName + "_Sad");
        Debug.Log($"Player {player.PlayerIndex + 1} got it wrong.");
    }

    private void ContinueGame()
    {
        if ((continueGame != null) && (gameCount < 5)) {
            continueGame.Invoke();
            gameCount += 1;
        }
        else {
            TotalCanvas.alpha = 0;
            // Instantiate 시간 초과! .. destory after 3 seconds
            Invoke("GameManager.instance.NextMiniGame", 3f);
            gameCount = 1;
        }
    }
}

public enum AnswerType { Number, Image }
