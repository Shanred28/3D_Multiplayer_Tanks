using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private SizeMap _sizeMap;
    [SerializeField] private UITankMark _tankMarkPref;
    [SerializeField] private Image _background;

    private UITankMark[] _tankMarks;
    private Player[] _players;

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
            if (_players[i] == null) continue;
            Vector3 normalPos = _sizeMap.GetNormPos(_players[i].activeVehicle.transform.position);

            Vector3 posInMinimap = new Vector3(normalPos.x * _background.rectTransform.sizeDelta.x * 0.5f, 
                normalPos.z * _background.rectTransform.sizeDelta.y * 0.5f, 0);

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
      _players = FindObjectsOfType<Player>();
        _tankMarks = new UITankMark[_players.Length];

        for (int i = 0; i < _tankMarks.Length; i++)
        {
            _tankMarks[i] = Instantiate(_tankMarkPref);

            if (_players[i].TeamId == Player.Local.TeamId)
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
