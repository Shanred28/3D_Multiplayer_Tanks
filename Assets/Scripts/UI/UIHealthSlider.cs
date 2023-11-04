using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderImage;

    [SerializeField] private Color _localTeamColor;
    [SerializeField] private Color _otherTeamColor;

    private Destructible _destructible;

    public void Init(Destructible destructible, int destructableTeamId, int localPlayerTeamId )
    {
        _destructible = destructible;

        _destructible.HitPointChanged += OnHitPointChange;
        _slider.maxValue = _destructible.MaxHitPoint;
        _slider.value = _slider.maxValue;

        if (localPlayerTeamId == destructableTeamId)
        {
            SetLocalColor();
        }
        else
            SetOtherColor();
    }

    private void OnDestroy()
    {
        if (_destructible == null) return;

        _destructible.HitPointChanged -= OnHitPointChange;
    }

    private void SetLocalColor()
    {
        _sliderImage.color = _localTeamColor;
    }

    private void SetOtherColor()
    {
        _sliderImage.color = _otherTeamColor;
    }

    private void OnHitPointChange(int hitPoint) 
    {
        _slider.value = hitPoint;
    }
}
