using TMPro;
using UnityEngine;

public class UIHitResultPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text _typeText;
    [SerializeField] private TMP_Text _damageText;

    public void SetTypeResult(string textResult)
    { 
        _typeText.text = textResult;
    }

    public void SetDamageResult(float dmg) 
    {
        if (dmg <= 0) return;

        _damageText.text = "-" + dmg.ToString("F0");
    }
}
