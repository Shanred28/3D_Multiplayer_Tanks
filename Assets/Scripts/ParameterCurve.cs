using UnityEngine;

[System.Serializable]
public class ParameterCurve
{
    [SerializeField] private AnimationCurve _curve;

    [SerializeField] private float _duration = 1;

    private float _expiredTime;

    public float MoveTowards(float deltaTime)
    {
        _expiredTime += deltaTime;

        return _curve.Evaluate(_expiredTime / _duration);
    }

    public float Reset()
    {
        _expiredTime = 0;

        return _curve.Evaluate(0);
    }

    public float GetValueBetweeen(float startValue, float endValue, float currentValue)
    { 
        if(_curve.length == 0 || startValue == endValue) return 0;

        float startTime = _curve.keys[0].time;
        float endTime = _curve.keys[_curve.length - 1].time;

        float currentTime = Mathf.Lerp(startTime, endTime,(currentValue - startValue) / (endValue-startValue));

        return _curve.Evaluate(currentTime);
    }
}
