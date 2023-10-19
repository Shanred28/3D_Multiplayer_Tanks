using UnityEngine;

public class TankEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particleExhaust;
    [SerializeField] private ParticleSystem[] _particleExhaustAtMovementStart;

    [SerializeField] private Vector2 _minMaxExhaustEmission;

    private TrackTank _tank;
    private bool isTankStoped;
    private void Start()
    {
        _tank = GetComponent<TrackTank>();
    }

    private void Update()
    {
        float exhaustEmission = Mathf.Lerp(_minMaxExhaustEmission.x, _minMaxExhaustEmission.y, _tank.NormalizedLinearVelocity);

        for (int i = 0; i < _particleExhaust.Length; i++)
        {
            ParticleSystem.EmissionModule emission = _particleExhaust[i].emission;
            emission.rateOverTime = exhaustEmission;
        }

        if (_tank.LinearVelocity < 0.1f)
        {
            isTankStoped = true;
        }

        if (_tank.LinearVelocity > 0.1f)
        {
            if (isTankStoped == true)
            {
                for (int i = 0; i < _particleExhaustAtMovementStart.Length; i++)
                {
                    _particleExhaustAtMovementStart[i].Play();
                }
            }
            isTankStoped =false;
        }
    }
}
