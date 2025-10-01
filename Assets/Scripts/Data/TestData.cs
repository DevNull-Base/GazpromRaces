using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Game/Test Data")]
    public class TestData : ScriptableObject
    {
        public Question[] questions;
    }

    [Serializable]
    public class Question
    {
        [TextArea] public string text;
        [Header("Варианты ответов")] 
        public string[] answers;
        [Header("Индекс правильного ответа")]
        public int correctIndex;
        [Header("Пояснение")]
        [TextArea] public string explanation;
    }
}