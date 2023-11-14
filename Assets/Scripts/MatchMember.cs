using Mirror;
using UnityEngine.Events;

[System.Serializable]
public class MatchMemberData
{
    public int Id;
    public string Nickname;
    public int TeamId;
    public NetworkIdentity Member;

    public MatchMemberData(int id, string nickname, int teamId, NetworkIdentity member)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
        Member = member;
    }
}

public static class MatchMemberDataExtention
{
    public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
        writer.WriteNetworkIdentity(value.Member);
    }

    public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
    {
        return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
    }
}
public class MatchMember : NetworkBehaviour
{
    public static event UnityAction<MatchMember, int> ChangeFrags;
    public static event UnityAction<MatchMember> DestroyMember;

    public Vehicle activeVehicle { get; set; }

    #region Data

    protected MatchMemberData _data;
    public MatchMemberData MemberData => _data;

    [Command]
    protected void CmdUpdateData(MatchMemberData playerData)
    {
        _data = playerData;
    }


    #endregion

    #region DestroyMember

    [Server]
    public void SvDestroyMember()
    {
        DestroyMember?.Invoke(this);
    }
    #endregion

    #region Frags

    [SyncVar(hook = nameof(OnFragChanged))]
    private int _fragsAmmount;

    [Server]
    public void SvAddFrags()
    { 
        _fragsAmmount++;
        ChangeFrags?.Invoke(this, _fragsAmmount);
    }

    [Server]
    public void SvResetFrags()
    {
        _fragsAmmount = 0;
    }

    private void OnFragChanged(int old, int newValue)
    {
        ChangeFrags?.Invoke(this, newValue);
    }

    #endregion

    #region NickName

    [SyncVar(hook = nameof(OnNicknameChanged))]
    protected string _nickname;
    public string Nickname => _nickname;

    [Command]
    protected void CmdSetName(string name)
    {
        _nickname = name;
        gameObject.name = name;
    }

    private void OnNicknameChanged(string oldValue, string newValue)
    {
        gameObject.name = newValue;
    }

    #endregion

    #region TeamId

    [SyncVar]
    protected int _teamId;
    public int TeamId => _teamId;

    #endregion 
}
