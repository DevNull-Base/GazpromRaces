using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtension;

public class RaceGameplay : Singleton<RaceGameplay>
{
    [SerializeField] private QuizLogic quizLogic;
    [SerializeField] private ShipMover playerShip;
    [SerializeField] private ShipMover enemyShip;
    [SerializeField] private EnemyController enemyController;
    
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostTime = 2f;
    
    [SerializeField] private BoostConfig boostConfig;
    
    private bool isRunning = false;
    private bool playerMadeMistake = false;

    public bool IsRaceRunning => isRunning;
    public ShipMover PlayerShip => playerShip;

    private void OnEnable()
    {
        quizLogic.OnTestStarted += HandleStartRace;
        quizLogic.OnCorrectAnswer += HandleCorrectAnswer;
        quizLogic.OnWrongAnswer += HandleWrongAnswer;
        quizLogic.OnTestFinished += HandleTestFinished;
        
        playerShip.OnRaceEnding += PlayerWin;
        enemyShip.OnRaceEnding += PlayerLose;
    }
    
    private void OnDisable()
    {
        quizLogic.OnCorrectAnswer -= HandleCorrectAnswer;
        quizLogic.OnWrongAnswer -= HandleWrongAnswer;
        quizLogic.OnTestStarted -= HandleStartRace;
        
        playerShip.OnRaceEnding -= PlayerWin;
        enemyShip.OnRaceEnding -= PlayerLose;
    }

    private void HandleTestFinished()
    {
        playerShip.BoostToEnd(5);
        enemyShip.BoostToEnd(5);
    }

    private void HandleStartRace()
    {
        isRunning = true;
        playerMadeMistake = false;
        
        playerShip.StartingRace();
        enemyShip.StartingRace();
        
        var brain = new CatchUpEnemyBrain(4);
        enemyController.Init(brain, this, boostConfig);
    }

    private void PlayerLose()
    {
        if (!isRunning) return;
        isRunning = false;
        
        enemyShip.EndingRace();
        ScreenManager.Instance.OpenScreen(ScreenTypes.Lose);
    }

    private void PlayerWin()
    {
        if (!isRunning) return;
        isRunning = false;
        
        playerShip.EndingRace();
        ScreenManager.Instance.OpenScreen(ScreenTypes.Win);
    }
    

    private void HandleCorrectAnswer()
    {
        playerShip.Boost(boostConfig);
    }

    private void HandleWrongAnswer()
    {
        playerMadeMistake = true;
    }
    
    public RaceContext GetRaceContext()
    {
        return new RaceContext
        {
            PlayerMadeMistake = playerMadeMistake,
            RaceFinished = !isRunning,
            RaceProgress = Mathf.Max(playerShip.NormalizedPosition, enemyShip.NormalizedPosition)
        };
    }
}