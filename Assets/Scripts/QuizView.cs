using System.Linq;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizView : MonoBehaviour
{
    [SerializeField] private QuizLogic logic;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Image questionPanel;
    [SerializeField] private Image feedbackPanel;
    [SerializeField] private TMP_Text feedbackText;
    
    private void OnEnable()
    {
        logic.OnQuestionChanged += RenderQuestion;
        logic.OnHideQuestionPanel += HidePanels;
        logic.OnShowQuestionPanel += ShowPanels;
        logic.OnFeedback += ShowFeedback;
    }

    private void OnDisable()
    {
        logic.OnQuestionChanged -= RenderQuestion;
        logic.OnHideQuestionPanel -= HidePanels;
        logic.OnShowQuestionPanel -= ShowPanels;
        logic.OnFeedback -= ShowFeedback;
    }

    private void RenderQuestion(Question q)
    {
        questionText.text = q.text;
        
        int[] order = Enumerable.Range(0, q.answers.Length).OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int answerIndex = order[i];
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = q.answers[answerIndex];

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => logic.Answer(answerIndex));
        }
    }
    
    private void HidePanels()
    {
        HidePanel(questionPanel);
        ShowPanel(feedbackPanel);
    }

    private void ShowPanels()
    {
        ShowPanel(questionPanel);
        HidePanel(feedbackPanel);
    }
    
    private void HidePanel(Image panel)
    {
        if (panel != null)
        {
            panel.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
            panel.transform.DOScale(0.8f, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    private void ShowPanel(Image panel)
    {
        if (panel != null)
        {
            panel.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
            panel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }
    
    private void ShowFeedback(string feedback)
    {
        feedbackText.text = feedback;
    }
}