using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestView : MonoBehaviour
{
    [SerializeField] private TestLogic logic;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;

    private void Awake()
    {
        logic.OnQuestionChanged += RenderQuestion;
    }

    private void OnDestroy()
    {
        logic.OnQuestionChanged -= RenderQuestion;
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
}