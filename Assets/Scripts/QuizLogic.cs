using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuizLogic : MonoBehaviour
{
    [SerializeField] private TestData testData;
    [SerializeField] private int questionsPerSession = 5;
    [SerializeField] private float delayBetweenCorrectQuestions = 2f;
    [SerializeField] private float delayBetweenUnCorrectQuestions = 5f;
    
    private List<Question> currentSessionQuestions;
    private float delay;
    private int currentQuestionIndex = 0;
    private bool waitingForNextQuestion = false;

    public event Action<Question> OnQuestionChanged;
    public event Action OnTestStarted;
    public event Action OnTestFinished;
    public event Action OnCorrectAnswer;
    public event Action OnWrongAnswer;
    public event Action OnHideQuestionPanel;
    public event Action OnShowQuestionPanel;
    public event Action<string> OnFeedback;

    private void Awake()
    {
        StartSession();
        OnTestStarted?.Invoke();
    }

    private void OnEnable()
    {
        StartSession();
        OnTestStarted?.Invoke();
    }

    private void StartSession()
    {
        currentSessionQuestions = new List<Question>();
        currentSessionQuestions = GetRandomQuestions(questionsPerSession);
        currentQuestionIndex = 0;
        NextQuestion();
    }

    private void NextQuestion()
    {
        waitingForNextQuestion = false;

        if (currentQuestionIndex < currentSessionQuestions.Count)
        {
            OnShowQuestionPanel?.Invoke();
            OnQuestionChanged?.Invoke(currentSessionQuestions[currentQuestionIndex]);
        }
        else
        {
            OnTestFinished?.Invoke();
            ScreenManager.Instance.OpenScreen(ScreenTypes.Look);
            return;
        }
    }
    
    public void Answer(int answerIndex)
    {
        if (waitingForNextQuestion) return;
        string feedbackText = "";
        var q = currentSessionQuestions[currentQuestionIndex];
        if (answerIndex == q.correctIndex)
        {
            delay = delayBetweenCorrectQuestions;
            feedbackText = "Молодец! Правильно!";
            OnCorrectAnswer?.Invoke();
        }
        else
        {
            delay = delayBetweenUnCorrectQuestions;
            feedbackText = "<color=orange>Твой ответ не верный, на самом деле:</color>\n" + q.explanation; 
            OnWrongAnswer?.Invoke();
        }
        
        OnFeedback?.Invoke(feedbackText);
        currentQuestionIndex++;
        
        waitingForNextQuestion = true;
        
        OnHideQuestionPanel?.Invoke();
        StartCoroutine(WaitAndNextQuestion());
    }
    
    private List<Question> GetRandomQuestions(int count)
    {
        var availableQuestions = testData.questions.ToList();
        var selectedQuestions = new List<Question>();
    
        count = Mathf.Min(count, availableQuestions.Count);
    
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableQuestions.Count);
            selectedQuestions.Add(availableQuestions[randomIndex]);
            availableQuestions.RemoveAt(randomIndex);
        }
    
        return selectedQuestions;
    }

    
    private IEnumerator WaitAndNextQuestion()
    {
        yield return new WaitForSeconds(delay);
        NextQuestion();
    }
}