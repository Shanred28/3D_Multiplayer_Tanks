using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private Slider _healthSlider;

    private Destructible _destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        
    }

    private void OnDestroy()
    {
       // NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if (_destructible != null)
            _destructible.HitPointChanged -= OnHitPointChange;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        _destructible = vehicle;
        _destructible.HitPointChanged += OnHitPointChange;
        _healthSlider.maxValue = _destructible.MaxHitPoint;
        _healthSlider.value = _destructible.MaxHitPoint;

        _healthText.text = _destructible.HitPoint.ToString();
    }
              
    private void OnHitPointChange(int healthitPoit)
    {
        _healthText.text = healthitPoit.ToString();
        _healthSlider.value = (float)( healthitPoit / _destructible.MaxHitPoint);
    }
}
