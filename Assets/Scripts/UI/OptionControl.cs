using UnityEngine;
using DG.Tweening;

public class OptionControl : MonoBehaviour
{
    [SerializeField] private GameObject imageOption, numberOption;
    void Update()
    {
        if (AnswerManager.instance.answer_type == AnswerType.Number){
            imageOption.SetActive(false);
            numberOption.SetActive(true);
        }
        else {
            imageOption.SetActive(true);
            numberOption.SetActive(false);
        }
    }
}
