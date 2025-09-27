using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "TestData", menuName = "Game/Test Data")]
    public class TestData : ScriptableObject
    {
        public Question[] questions;
    }

    [System.Serializable]
    public class Question
    {
        [TextArea] public string text;
        public string[] answers; // варианты ответов
        public int correctIndex; // индекс правильного ответа
    }
}