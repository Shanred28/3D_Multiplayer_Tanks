using TMPro;
using UnityEngine;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;

    private Destructible _destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
       // NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if (_destructible != null)
            _destructible.HitPointChange -= OnHitPointChange;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        _destructible = vehicle;
        _destructible.HitPointChange += OnHitPointChange;

        _healthText.text = _destructible.HitPoint.ToString();
    }
              
    private void OnHitPointChange(int healthitPoit)
    {
        _healthText.text = healthitPoit.ToString();
    }
}
