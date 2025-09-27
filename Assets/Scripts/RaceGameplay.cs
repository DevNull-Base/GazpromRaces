using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RaceGameplay : MonoBehaviour
{
    [SerializeField] private QuizLogic quizLogic;
    [SerializeField] private ShipMover playerShip;
    [SerializeField] private ShipMover enemyShip;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostTime = 2f;
    
    bool isStaring = false;

    private void OnEnable()
    {
        quizLogic.OnTestStarted += HandleStartRace;
        quizLogic.OnCorrectAnswer += HandleCorrectAnswer;
        quizLogic.OnWrongAnswer += HandleWrongAnswer;
        quizLogic.OnTestFinished += HandleTestFinished;
        
        playerShip.OnRaceEnding += PlayerWin;
        enemyShip.OnRaceEnding += PlayerLose;
    }

    private void HandleTestFinished()
    {
        playerShip.BoostToEnd(5);
        enemyShip.BoostToEnd(5);
    }

    private void HandleStartRace()
    {
        playerShip.StartingRace();
        enemyShip.StartingRace();
        
        isStaring = true;
    }

    private void PlayerLose()
    {
        if (!isStaring) return;
        isStaring = false;
        enemyShip.EndingRace();
        ScreenManager.Instance.OpenScreen(ScreenTypes.Lose);
    }

    private void PlayerWin()
    {
        if (!isStaring) return;
        isStaring = false;
        playerShip.EndingRace();
        ScreenManager.Instance.OpenScreen(ScreenTypes.Win);
    }


    private void OnDisable()
    {
        quizLogic.OnCorrectAnswer -= HandleCorrectAnswer;
        quizLogic.OnWrongAnswer -= HandleWrongAnswer;
        quizLogic.OnTestStarted -= HandleStartRace;
        
        playerShip.OnRaceEnding -= PlayerWin;
        enemyShip.OnRaceEnding -= PlayerLose;
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