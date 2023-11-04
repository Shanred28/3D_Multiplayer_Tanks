using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileMovement : MonoBehaviour
{
    private Projectile _projectile;
    private Vector3 _step;

    private void Awake()
    {
        _projectile = GetComponent<Projectile>();
        _step = new Vector3();
    }

    public void Move()
    {
        transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * _projectile.Properties.Mass)).normalized;

        _step = transform.forward * _projectile.Properties.Velocity * Time.deltaTime;

        transform.position += _step;
    }
}
