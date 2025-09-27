using System;
using Data;
using UnityEngine;

public class QuizLogic : MonoBehaviour
{
    [SerializeField] private TestData testData;
    private int currentQuestion = 0;

    public event Action<Question> OnQuestionChanged;
    public event Action OnTestStarted;
    public event Action OnTestFinished;
    public event Action OnCorrectAnswer;
    public event Action OnWrongAnswer;

    private void Awake()
    {
        ShowQuestion();
        OnTestStarted?.Invoke();
    }

    private void OnEnable()
    {
        currentQuestion = 0;
        ShowQuestion();
        OnTestStarted?.Invoke();
    }

    private void ShowQuestion()
    {
        if (currentQuestion >= testData.questions.Length)
        {
            OnTestFinished?.Invoke();
            ScreenManager.Instance.OpenScreen(ScreenTypes.Look);
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