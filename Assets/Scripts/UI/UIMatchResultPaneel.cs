using TMPro;
using UnityEngine;

public class UIMatchResultPaneel : MonoBehaviour
{
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultText;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        _resultPanel.SetActive(false);
    }

    private void OnMatchEnd()
    {
        _resultPanel.SetActive(true);

        int winTeamId = NetworkSessionManager.Match.WinTeamId;

        if (winTeamId == -1)
        {
            _resultText.text = "Ничья";
            return;
        }

        if (winTeamId == Player.Local.TeamId)
        {
            _resultText.text = "Победа!";
        }
        else
        {
            _resultText.text = "Поражение!";
        }
    }
}
