using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private Transform _mapConvas;

    [SerializeField] private SizeMap _sizeMap;
    [SerializeField] private UITankMark _tankMarkPref;
    [SerializeField] private Image _background;

    private UITankMark[] _tankMarks;
    private Vehicle[] _vehicles;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void Update()
    {
        if (_tankMarks == null) return;

        for (int i = 0; i < _tankMarks.Length; i++)
        {
            if (_vehicles[i] == null) continue;
            if (_vehicles[i] != Player.Local.activeVehicle)
            {
                bool isVisable = Player.Local.activeVehicle.VehicleViewer.IsVisable(_vehicles[i].netIdentity);

                _tankMarks[i].gameObject.SetActive(isVisable);
            }

           

            if(_tankMarks[i].gameObject.activeSelf == false) continue;

            Vector3 normalPos = _sizeMap.GetNormPos(_vehicles[i].transform.position);

            Vector3 posInMinimap = new Vector3(normalPos.x * _background.rectTransform.sizeDelta.x * 0.5f, 
                normalPos.z * _background.rectTransform.sizeDelta.y * 0.5f, 0);

            posInMinimap.x *= _mapConvas.localScale.x;
            posInMinimap.y *= _mapConvas.localScale.y;

            _tankMarks[i].transform.position = _background.transform.position + posInMinimap;
        }
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    { 
        _vehicles = FindObjectsOfType<Vehicle>();
        _tankMarks = new UITankMark[_vehicles.Length];

        for (int i = 0; i < _tankMarks.Length; i++)
        {
            _tankMarks[i] = Instantiate(_tankMarkPref);

            if (_vehicles[i].TeamId == Player.Local.TeamId)
                _tankMarks[i].SetLocalColor();
            else
                _tankMarks[i].SetOtherColor();

            _tankMarks[i].transform.SetParent(_background.transform);
        }
    }

    private void OnMatchEnd() 
    {
        for (int i = 0; i < _background.transform.childCount; i++)
        {
            Destroy(_background.transform.GetChild(i).gameObject);
        }
        _tankMarks = null;
    }

}
