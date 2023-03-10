using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDetect : MonoBehaviour
{
    [SerializeField] PlayerSetupControl playerSetup;
    [SerializeField] private int playerInputIndex;
    public int PlayerInputIndex => playerInputIndex;
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        DetectInput();
    }

    private void DetectInput()
    {
        if (PlayerConfigManager.instance.PressKey(playerInput, InputType.SOUTHBUTTON))
        {
            playerInputIndex = 0;
        }
        else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.EASTBUTTON))
        {
            playerInputIndex = 1;
        }
        else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.NORTHBUTTON))
        {
            playerInputIndex = 2;
        }
        else if (PlayerConfigManager.instance.PressKey(playerInput, InputType.WESTBUTTON))
        {
            playerInputIndex = 3;
        }
    }
}
