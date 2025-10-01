using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private ShipMover movement;
    [SerializeField] private float decisionInterval = 3f;

    private IEnemyBrain _brain;
    private BoostConfig _config;
    private RaceGameplay _gameplay;
    private float _timer;

    public void Init(IEnemyBrain brain, RaceGameplay gameplay, BoostConfig config)
    {
        _brain = brain;
        _gameplay = gameplay;
        _config = config;
    }

    private void Update()
    {
        if (_gameplay == null || _brain == null) return;
        if (!_gameplay.IsRaceRunning) return;

        _timer += Time.deltaTime;
        if (_timer >= decisionInterval)
        {
            _timer = 0f;

            var context = _gameplay.GetRaceContext();
            if (_brain.ShouldBoost(_gameplay.PlayerShip.NormalizedPosition, movement.NormalizedPosition, context))
            {
                movement.Boost(_config);
            }
        }
    }
}