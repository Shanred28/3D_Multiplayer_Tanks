using UnityEngine;

public class Bot : MatchMember
{
    [SerializeField] private  Vehicle _vehicle;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _teamId = MatchController.GetNextTeam();

        _nickname = "b_" + GetRandomName();

        _data = new MatchMemberData((int)netId, _nickname, _teamId, netIdentity);

        transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(_teamId);

        activeVehicle = _vehicle;
        activeVehicle.TeamId = _teamId;
        activeVehicle.Owner = netIdentity;
        activeVehicle.name = _nickname;      
    }

    private void Start()
    {
        if (isServer == true)
        {
            MatchMemberList.Instance.SvAddPlayer(_data);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        activeVehicle = _vehicle;
        activeVehicle.TeamId = _teamId;
        activeVehicle.Owner = netIdentity;
        activeVehicle.name = _nickname;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        MatchMemberList.Instance.SvRemovePlayer(_data);
    }

    private string GetRandomName()
    {
        string[] names ={
            "ABToMaT_kaJla",
            "YuSuP",
            "Don1**",
            "Estane",
            "Gralinda",
            "Rumpelshtinkel",
            "ЧинГачГук",
            "Обвислое",
            "Перо",
            "Egor_6e3_TpycoB",
            "Вонючий",
            "Суслик",
            "переросток",
            "пурген",
            "JleTHuu_BeTe",
            "Essner",
            "MePTBbIu_CJIoHuK",
            "jukkacanada",
            "*kpyTou*TuII",
            "TToJle3Hblu_urpok",
            "4epToBcku_kpyTa",
            "Blackfire",
            "Bags_Banny",
            "Блинчик",
            "Adoranin",
            "Black_Star",
            "CypoBblu_MaJlb4yraH",
            "3JIbIe_Tanku"
                        };

        return names[Random.Range(0, name.Length)];
    }    
}
