using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager instance { get; private set; }
    public CanvasGroup TotalCanvas;
    public Transform miniGameSpawn;
    public int maxRounds;
    
    [SerializeField] private GameObject problemNotice, minigameOverText;
    [SerializeField] private TextMeshProUGUI roundNumber;

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

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            foreach (PlayerConfiguration player in PlayerConfigManager.instance.PlayerConfigs)
                Debug.Log($"Player {player.PlayerIndex + 1} : {player.PlayerScore} pt");
        }
    }

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
        EnableSolve();
    }
    private void EnableSolve()
    {
        foreach (PlayerConfiguration player in PlayerConfigManager.instance.PlayerConfigs) {
            player.InputDetect.UnlockAnswerSelection();
        }

        // 타이머 시작
    }
    private void SetOptions()
    {
        if (answerType == AnswerType.Number) {
            int number_answer = int.Parse(correctAnswer);
            List<int> answerValues = new List<int>();

            // 답을 포함한 랜덤 선택지 4개 생성
            while (answerValues.Count < 3) {
                int randomNumber = Random.Range(number_answer - 4, number_answer + 5);
                if ((randomNumber != number_answer) && !answerValues.Contains(randomNumber))
                    answerValues.Add(randomNumber);
            }
            answerValues.Add(number_answer);

            // 랜덤 순서로 선택지를 시각화
            for (int i = 0; i < 4; i++) {
                int randomIndex = Random.Range(0, answerValues.Count);
                numberOptions[i].text = $"{answerValues[randomIndex]}";

                if (number_answer == answerValues[randomIndex])
                    correctAnswerIndex = i;
                
                answerValues.RemoveAt(randomIndex);
            }
        }
        else if (answerType == AnswerType.Image) {
            OptionBundle image_answer = ReturnOptionBundle(correctAnswer);
            List<OptionBundle> answerValues = new List<OptionBundle>();

            while (answerValues.Count < 3) {
                OptionBundle ob = possibleImageList[Random.Range(0, possibleImageList.Count)];
                if ((ob.imageTag != correctAnswer) && !answerValues.Contains(ob))
                    answerValues.Add(ob);
            }
            answerValues.Add(image_answer);

            for (int i = 0; i < 4; i++) {
                int randomIndex = Random.Range(0, answerValues.Count);
                imageOptions[i].sprite = answerValues[randomIndex].optionImage;

                if (correctAnswer == answerValues[randomIndex].imageTag)
                    correctAnswerIndex = i;
                
                answerValues.RemoveAt(randomIndex);
            }
        }
        ShowOptions();
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
        // 타이머 강제종료
        
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
            Invoke("ShowProblem", 2f);
        }
    }

    private void CorrectPlayer(PlayerConfiguration player)
    {
        PlayerSetupControl psc = player.PlayerSetup;
        //psc.PlayerAnim.Play(psc.SelectedCharacterName + "_Happy");
        //psc.SetStatus

        // Instantiate(RewardPoint)
        player.PlayerScore += 10; // += player.InputDetect.timePoint;
        Debug.Log($"Player {player.PlayerIndex + 1} 정답입니다!");
    }

    private void IncorrectPlayer(PlayerConfiguration player)
    {
        //psc.PlayerAnim.Play(psc.SelectedCharacterName + "_Sad");
        //psc.SetStatus
        Debug.Log($"Player {player.PlayerIndex + 1} 는 오답입니다.");
    }

    public void ShowProblem()
    {
        if (gameCount <= maxRounds)
        {
            roundNumber.text = $"Round {gameCount}";
            problemNotice.SetActive(true);
            DOTween.Rewind("ShowProblemNumber");
            DOTween.Play("ShowProblemNumber");
        }
        HideOptions();
        Invoke("ContinueGame", 2f);
    }

    private void ContinueGame()
    {
        problemNotice.SetActive(false);
        foreach (PlayerConfiguration player in PlayerConfigManager.instance.PlayerConfigs)
            player.PlayerSetup.SetPlayerStatus(false);
        
        if ((continueGame != null) && (gameCount <= maxRounds)) {
            continueGame?.Invoke();
            gameCount += 1;
        }
        else {
            TotalCanvas.alpha = 0;
            Destroy(Instantiate(minigameOverText, problemNotice.transform.parent), 3f);
            GameManager.instance.Invoke("NextMiniGame", 3f);
            gameCount = 1;
        }
    }

    private void HideOptions()
    {
        if (numberOptions[0].transform.parent.localScale.x == 0)
            return;
        DOTween.Rewind("HideOptions");
        DOTween.Play("HideOptions");
    }
    private void ShowOptions()
    {
        if (numberOptions[0].transform.parent.localScale.x == 1)
            return;
        DOTween.Rewind("ShowOptions");
        DOTween.Play("ShowOptions");
    }
}

public enum AnswerType { Number, Image }
