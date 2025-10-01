using UnityEngine;

public class CatchUpEnemyBrain : IEnemyBrain
{
    private readonly int _maxBoosts;
    private int _usedBoosts;

    public CatchUpEnemyBrain(int maxBoosts = 5)
    {
        _maxBoosts = maxBoosts;
        _usedBoosts = 0;
    }

    public bool ShouldBoost(float playerPos, float enemyPos, RaceContext context)
    {
        if (context.RaceFinished) return false;
        if (_usedBoosts >= _maxBoosts) return false;

        float distance = playerPos - enemyPos;
        bool shouldBoost = false;


        if (distance > 0.1f)
            shouldBoost = Random.value < 0.8f;

        else if (distance > 0f)
            shouldBoost = Random.value < 0.4f;
        
        if (enemyPos > 0.8f)
            shouldBoost = true;
        
        if (context.PlayerMadeMistake)
            shouldBoost = true;

        if (shouldBoost)
            _usedBoosts++;

        return shouldBoost;
    }
}