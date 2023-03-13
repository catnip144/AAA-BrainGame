using System;

public interface MiniGameInterface
{
    Action ReturnContinueGame();
    void ContinueGame();
    void ProblemEnd(); // AnswerManager.instance.SetCorrectAnswer($"{답}");
}
