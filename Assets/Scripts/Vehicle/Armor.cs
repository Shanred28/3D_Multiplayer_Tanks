using UnityEngine;

public enum ArmorType
{ 
    Vehicle,
    Module
}

public class Armor : MonoBehaviour
{
    [SerializeField] private ArmorType _armorType;
    [SerializeField] private Destructible _destructible;
    [SerializeField] private int _thickness;

    public ArmorType Type => _armorType;
    public Destructible Destructible => _destructible;
    public int Thickness => _thickness;

    public void SetDestrictible(Destructible destructible)
    {
        _destructible = destructible;
    }
}
