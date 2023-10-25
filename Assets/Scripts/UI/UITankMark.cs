using UnityEngine;
using UnityEngine.UI;

public class UITankMark : MonoBehaviour
{
    [SerializeField] private Image _image;

    [SerializeField] private Color _localTeamColor;
    [SerializeField] private Color _otherTeamColor;

    public void SetLocalColor()
    { 
        _image.color = _localTeamColor;
    }

    public void SetOtherColor()
    { 
        _image.color= _otherTeamColor;
    }
}
