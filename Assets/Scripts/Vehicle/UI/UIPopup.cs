using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _lifeTime;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.Translate(_movementDirection * _movementSpeed * Time.deltaTime);
    }
}
