using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDetect : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerSetupControl playerSetup;

    [SerializeField] private int chosenAnswerIndex;
    public int ChosenAnswerIndex => chosenAnswerIndex;

    public int timePoint { get; private set; }
    
    private bool lockAnswer = true;
    public bool LockAnswer => lockAnswer;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerSetup = GetComponent<PlayerSetupControl>();
    }

    void Update()
    {
        SelectAnswer();
    }

    private void SelectAnswer()
    {
        if (playerSetup.IsGameMode && !lockAnswer)
        {
            if (PlayerConfigManager.instance.PressKey(playerInput, InputType.SOUTHBUTTON))
            {
                chosenAnswerIndex = 0;
            }
            else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.EASTBUTTON))
            {
                chosenAnswerIndex = 1;
            }
            else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.NORTHBUTTON))
            {
                chosenAnswerIndex = 2;
            }
            else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.WESTBUTTON))
            {
                chosenAnswerIndex = 3;
            }

            if (chosenAnswerIndex != -1)
            {
                lockAnswer = true;
                playerSetup.SetPlayerStatus(lockAnswer);
                // timePoint = (int) AnswerManger.instance.lefttime
                if (AnswerManager.instance.CheckOtherPlayers())
                    AnswerManager.instance.EvaluateAnswers();
            }
        }
    }

    public void ResetAnswerIndex()
    {
        chosenAnswerIndex = -1;
    }

    public void UnlockAnswerSelection()
    {
        lockAnswer = false;
    }
}
