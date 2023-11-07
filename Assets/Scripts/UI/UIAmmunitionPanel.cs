using System.Collections.Generic;
using UnityEngine;

public class UIAmmunitionPanel : MonoBehaviour
{
    [SerializeField] private Transform _ammunitionPanel;
    [SerializeField] private UIAmmunitionElement _ammunitionElementPrefab;

    private List<UIAmmunitionElement> _allAmmunitionElements = new List<UIAmmunitionElement>();
    private List<Ammunition> _allAmmunations = new List<Ammunition>();

    private Turret _turret;
    private int _lastSelectionAmmunationIndex;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnded;
    }

    private void OnDestroy()
    {
        
        NetworkSessionManager.Match.MatchStart -= OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnded;

    }

    private void OnMatchStarted()
    {
        _turret = Player.Local.activeVehicle.Turret;
        _turret.UpdateSelectedAmmunation += OnTurretUpdateSelectedAmmunation;

        for (int i = 0; i < _ammunitionPanel.childCount; i++)
        {
            Destroy(_ammunitionPanel.GetChild(i).gameObject);
        }

        _allAmmunitionElements.Clear();
        _allAmmunations.Clear();

        for (int i = 0; i < _turret.Ammunitions.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(_ammunitionElementPrefab);
            ammunitionElement.transform.SetParent(_ammunitionPanel);
            ammunitionElement.transform.localScale = Vector3.one;
            ammunitionElement.SetAmmunation(_turret.Ammunitions[i]);

            _turret.Ammunitions[i].AmmoCountChanged += OnAmmoCountChanged;

            _allAmmunitionElements.Add(ammunitionElement);
            _allAmmunations.Add(_turret.Ammunitions[i]);

            if (i == 0)
                ammunitionElement.Select();
        }
    }

    private void OnMatchEnded()
    {
        if (_turret != null)
            _turret.UpdateSelectedAmmunation -= OnTurretUpdateSelectedAmmunation;

        for (int i = 0; i < _allAmmunations.Count; i++)
        {
            _turret.Ammunitions[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }

    private void OnAmmoCountChanged(int ammoCount)
    {
        _allAmmunitionElements[_turret.SelectedAmmunitionIndex].UpdateAmmoCount(ammoCount);
    }

    private void OnTurretUpdateSelectedAmmunation(int index)
    {
        _allAmmunitionElements[_lastSelectionAmmunationIndex].UnSelect();
        _allAmmunitionElements[index].Select();

        _lastSelectionAmmunationIndex = index;
    }
}
