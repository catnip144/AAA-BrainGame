using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class CrocGame : MonoBehaviour, MiniGameInterface
{
    public static CrocGame instance { get; private set; }

    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private float maximumSpawn;
    private int spawnCount;

    [SerializeField] private float aliveTime;
    [SerializeField] private int minNum, maxNum;
    private int totalSum;

    [SerializeField] private float pushForce;
    private Vector2 direction;
    private float distance;

    [SerializeField] Transform numberSpawnPoint, startPoint, endPoint;
    private Vector2 startPos {
        get { return startPoint.position; }
    }
    private Vector2 endPos {
        get { return endPoint.position; }
    }

    //==================================================================================
    public void ContinueGame()
    {
        InvokeRepeating("SpawnNumbers", 0.1f, 1f);
    }
    public Action ReturnContinueGame()
    {
        return ContinueGame;
    }
    public void ProblemEnd()
    {
        CancelInvoke();
        AnswerManager.instance.SetCorrectAnswer($"{totalSum}");
        spawnCount = 0;
        totalSum = 0;
    }
    //==================================================================================
    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    private void SpawnNumbers()
    {
        if (spawnCount >= maximumSpawn) {
            ProblemEnd();
            return;
        }
        int numValue = Random.Range(minNum, maxNum + 1);

        GameObject number = Instantiate(numberPrefab, numberSpawnPoint);
        CrocNumber crocNumber = number.GetComponent<CrocNumber>();

        ActivateCrocNumber(crocNumber, numValue);
        Destroy(number, aliveTime);

        spawnCount += 1;
        totalSum += numValue;
    }

    private void ActivateCrocNumber(CrocNumber croc_number, int value)
    {
        croc_number.numberText.text = value.ToString();

        croc_number.ActivateRb();
        distance = Vector2.Distance(startPos, endPos);
        direction = (endPos - startPos).normalized;
        croc_number.Push(direction * distance * pushForce);
    }
}
