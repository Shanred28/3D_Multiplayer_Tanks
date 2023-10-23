using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerLable : MonoBehaviour
{
    [SerializeField] private TMP_Text _fragText;
    [SerializeField] private TMP_Text _nicknameText;

    [SerializeField] private Image _backgroundImage;

    [SerializeField] private Color _selfColor;

    private int _netId;
    public int NetId => _netId;

    public void Init(int netId, string nickname)
    {
        _netId = netId;
        _nicknameText.text = nickname;

        if (_netId == Player.Local.netId)
        { 
            _backgroundImage.color = _selfColor;
        }
    }

    public void UpdateFrag(int frag)
    {
        _fragText.text = frag.ToString();
    }
}
