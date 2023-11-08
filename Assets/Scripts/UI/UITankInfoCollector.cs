using System.Collections.Generic;
using UnityEngine;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform _tankInfoPanel;
    [SerializeField] private UITankInfo _tankInfoPref;

    private UITankInfo[] _tanksInfo;
    private List<Vehicle> _playersWithoutLocal;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void Update()
    {
        if (_tanksInfo == null) return;

        for (int i = 0; i < _tanksInfo.Length; i++)
        {

            if (_tanksInfo[i] == null ) continue;

            bool isVisible = Player.Local.activeVehicle.VehicleViewer.IsVisable(_tanksInfo[i].Tank.netIdentity);

            _tanksInfo[i].gameObject.SetActive(isVisible);

            if(_tanksInfo[i].gameObject.activeSelf == false) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(_tanksInfo[i].Tank.transform.position + _tanksInfo[i].WorldOffset);

            if (screenPos.z > 0)
            {
                _tanksInfo[i].transform.position = screenPos;
            }
        }
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        _playersWithoutLocal = new List<Vehicle>(vehicles.Length - 1);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] == Player.Local.activeVehicle) continue;

            _playersWithoutLocal.Add(vehicles[i]);
        }

        _tanksInfo = new UITankInfo[_playersWithoutLocal.Count];

        for (int i = 0; i < _playersWithoutLocal.Count; i++)
        {
            _tanksInfo[i] = Instantiate(_tankInfoPref);

            _tanksInfo[i].SetTank(_playersWithoutLocal[i]);
            _tanksInfo[i].transform.SetParent(_tankInfoPanel);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < _tankInfoPanel.transform.childCount; i++)
        {
            Destroy(_tankInfoPanel.transform.GetChild(i).gameObject);
        }

        _tanksInfo = null;
    }
}
