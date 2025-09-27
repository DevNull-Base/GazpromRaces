using UnityEngine;

public class RaceGameplay : MonoBehaviour
{
    [SerializeField] private TestLogic testLogic;
    [SerializeField] private ShipMover playerShip;
    [SerializeField] private ShipMover enemyShip;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostTime = 2f;

    private void Awake()
    {
        testLogic.OnTestStarted += HandleStartRace;
        testLogic.OnCorrectAnswer += HandleCorrectAnswer;
        testLogic.OnWrongAnswer += HandleWrongAnswer;
    }

    private void HandleStartRace()
    {
        playerShip.StartingRace();
        enemyShip.StartingRace();
    }

    private void OnDestroy()
    {
        testLogic.OnCorrectAnswer -= HandleCorrectAnswer;
        testLogic.OnWrongAnswer -= HandleWrongAnswer;
    }

    private void HandleCorrectAnswer()
    {
        playerShip.Boost(boostTime, boostMultiplier);
    }

    private void HandleWrongAnswer()
    {
        Debug.Log("Упс! Неправильный ответ.");
    }
}