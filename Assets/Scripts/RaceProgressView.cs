using UnityEngine;
using UnityEngine.UI;

public class RaceProgressView : MonoBehaviour
{
    [SerializeField] private ShipMover playerShip;
    [SerializeField] private Slider progressSlider;

    private void OnEnable()
    {
        playerShip.OnProgressChanged += UpdateProgress;
    }

    private void OnDisable()
    {
        playerShip.OnProgressChanged -= UpdateProgress;
    }

    private void UpdateProgress(float value)
    {
        progressSlider.value = value;
    }
}