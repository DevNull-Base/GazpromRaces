using System;
using Data;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    [SerializeField] private TestData testData;
    private int currentQuestion = 0;

    public event Action<Question> OnQuestionChanged;
    public event Action OnTestStarted;
    public event Action OnTestFinished;
    public event Action OnCorrectAnswer;
    public event Action OnWrongAnswer;

    private void Start()
    {
        ShowQuestion();
        OnTestStarted?.Invoke();
    }

    private void ShowQuestion()
    {
        if (currentQuestion >= testData.questions.Length)
        {
            OnTestFinished?.Invoke();
            return;
        }

        OnQuestionChanged?.Invoke(testData.questions[currentQuestion]);
    }

    public void Answer(int answerIndex)
    {
        var q = testData.questions[currentQuestion];
        if (answerIndex == q.correctIndex)
            OnCorrectAnswer?.Invoke();
        else
            OnWrongAnswer?.Invoke();

        currentQuestion++;
        ShowQuestion();
    }
}