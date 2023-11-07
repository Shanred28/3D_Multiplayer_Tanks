using UnityEngine;

public class UITankInfo : MonoBehaviour
{
    [SerializeField] private UIHealthSlider _healthSlider;

    [SerializeField] private Vector3 _worldOffset;
    public Vector3 WorldOffset => _worldOffset;

    private Vehicle _tank;
    public Vehicle Tank => _tank;

    public void SetTank(Vehicle tank)
    {
        _tank = tank;

        _healthSlider.Init(_tank, _tank.TeamId, Player.Local.TeamId);    
    }
}
