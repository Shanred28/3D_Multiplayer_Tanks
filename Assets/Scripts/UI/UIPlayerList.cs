using System.Collections.Generic;
using UnityEngine;

public class UIPlayerList : MonoBehaviour
{
    [SerializeField] private Transform _localTeamPanel;
    [SerializeField] private Transform _otherTeamPanel;

    [SerializeField] private UIPlayerLable _playerLablePref;

    private List<UIPlayerLable> _allPlayerLable = new List<UIPlayerLable>();

    private void Start()
    {
        MatchMemberList.UpdateMemberList += OnUpdatePlayerList;
        MatchMember.ChangeFrags += OnChangeFrags;
        MatchMember.DestroyMember += OnDestroyedVehicleMember;
    }

    private void OnDestroy()
    {
        MatchMemberList.UpdateMemberList -= OnUpdatePlayerList;
        MatchMember.ChangeFrags -= OnChangeFrags;
        MatchMember.DestroyMember -= OnDestroyedVehicleMember;
    }

    private void OnUpdatePlayerList(List<MatchMemberData> playerData)
    {
        for (int i = 0; i < _localTeamPanel.childCount; i++)
        {
            Destroy(_localTeamPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < _otherTeamPanel.childCount; i++)
        {
            Destroy(_otherTeamPanel.GetChild(i).gameObject);
        }

        _allPlayerLable.Clear();

        for (int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].TeamId == Player.Local.TeamId)
            {
                AddPlayerLable(playerData[i], _playerLablePref,_localTeamPanel);
            }

            if (playerData[i].TeamId != Player.Local.TeamId)
            {
                AddPlayerLable(playerData[i], _playerLablePref, _otherTeamPanel);
            }
        }
    }

    private void AddPlayerLable(MatchMemberData playerData, UIPlayerLable playerLable, Transform parent)
    {
        UIPlayerLable uiPlayerLable = Instantiate(playerLable);

        uiPlayerLable.transform.SetParent(parent);
        uiPlayerLable.Init(playerData.Id, playerData.Nickname);

        _allPlayerLable.Add(uiPlayerLable);
    }

    private void OnChangeFrags(MatchMember matchMember, int frags)
    {
        for (int i = 0; i < _allPlayerLable.Count; i++)
        {
            if (_allPlayerLable[i].NetId == matchMember.netId)
            {
                _allPlayerLable[i].UpdateFrag(frags);
            }
        }
    }

    private void OnDestroyedVehicleMember(MatchMember matchMember)
    {
        for (int i = 0; i < _allPlayerLable.Count; i++)
        {
            if (_allPlayerLable[i].NetId == matchMember.netId)
            {
                _allPlayerLable[i].MemberDestroyed();
            }
        }
    }
}
