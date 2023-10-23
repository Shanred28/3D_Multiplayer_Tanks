using UnityEngine;
using UnityEngine.UI;

public class UICaptureBase : MonoBehaviour
{
    [SerializeField] private ConditionCaptureBase _conditionCaptureBase;

    [SerializeField] private Slider _localTeamSlider;
    [SerializeField] private Slider _otherTeamSlider;


    private void Update()
    {
        if (Player.Local == null) return;

        if (Player.Local.TeamId == TeamSide.TeamRed)
        {
            UpdateSlider(_localTeamSlider, _conditionCaptureBase.RedBaseCaptureLevel);
            UpdateSlider(_otherTeamSlider, _conditionCaptureBase.BlueBaseCaptureLevel);
        }

        if (Player.Local.TeamId == TeamSide.TeamBlue)
        {
            UpdateSlider(_localTeamSlider, _conditionCaptureBase.BlueBaseCaptureLevel);
            UpdateSlider(_otherTeamSlider, _conditionCaptureBase.RedBaseCaptureLevel);
        }
    }

    private void UpdateSlider(Slider slider, float value)
    {
        if (value == 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.activeSelf == false)
                slider.gameObject.SetActive(true);

            slider.value = value /100;
        }

    }
}
