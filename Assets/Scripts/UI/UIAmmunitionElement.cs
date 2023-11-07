using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmunitionElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoCountText;
    [SerializeField] private Image _projectileIcon;
    [SerializeField] private GameObject _selectionBorder;

    public void SetAmmunation(Ammunition ammunition)
    {
        _projectileIcon.sprite = ammunition.ProjectileProp.IconProjectile;

        UpdateAmmoCount(ammunition.AmmoCount);
    }

    public void UpdateAmmoCount(int count)
    {
        _ammoCountText.text = count.ToString();
    }

    public void Select()
    { 
        _selectionBorder.SetActive(true);
    }

    public void UnSelect()
    { 
        _selectionBorder.SetActive(false);
    }
}
