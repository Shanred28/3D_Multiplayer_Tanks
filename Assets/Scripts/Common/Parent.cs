using UnityEngine;

public class Parent : MonoBehaviour
{
    [SerializeField] private Transform _parent;

    private void Awake()
    {
        transform.SetParent(_parent);
    }
}
