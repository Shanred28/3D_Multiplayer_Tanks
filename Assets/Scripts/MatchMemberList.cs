using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class MatchMemberList : NetworkBehaviour
{
    public static MatchMemberList Instance;
    public static UnityAction<List<MatchMemberData>> UpdateMemberList;
    [SerializeField] private List<MatchMemberData> _allMemberData = new List<MatchMemberData>();
    public int  MemberDataCount => _allMemberData.Count;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _allMemberData.Clear();
    }
      
    [Server]
    public void SvAddPlayer(MatchMemberData playerData )
    {
        _allMemberData.Add(playerData);

         RpcClearPlayerDataList();

        for (int i = 0; i < _allMemberData.Count; i++)
        {
            RpcAddPlayer(_allMemberData[i]);
        }
    }

    [Server]
    public void SvRemovePlayer(MatchMemberData playerData)
    {
        for (int i = 0; i < _allMemberData.Count; i++)
        {
            if (_allMemberData[i].Id == playerData.Id)
            {
                _allMemberData.RemoveAt(i);
                break;
            }                                  
        }

        RpcRemovePlayer(playerData.Id);
    }

    [ClientRpc]
    private void RpcClearPlayerDataList()
    {
        //Check host
        if (isServer == true) return;
            _allMemberData.Clear();
    }

    [ClientRpc]
    private void RpcAddPlayer(MatchMemberData memberData)
    {
        if (isClient == true && isServer == true)
        {
            UpdateMemberList?.Invoke(_allMemberData);
            return;
        }

        _allMemberData.Add(memberData);
        UpdateMemberList?.Invoke(_allMemberData);
    }

    [ClientRpc]
    private void RpcRemovePlayer(int userId)
    {
        for (int i = 0; i < _allMemberData.Count; i++)
        {
            if (_allMemberData[i].Id == userId)
            {
                _allMemberData.RemoveAt(i);
                break;
            }
        }

        UpdateMemberList?.Invoke(_allMemberData);
    }

    public int GetIdByNicName(string nickName)
    {
        for (int i = 0; i < _allMemberData.Count; i++)
        {
            if (_allMemberData[i].Nickname == nickName)
            {
                return _allMemberData[i].Id;
            }
                    
        }
        return 0;
    }
}


