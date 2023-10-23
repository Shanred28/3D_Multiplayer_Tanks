using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer _matchTimer;
    [SerializeField] private TMP_Text _timerText;

    private float timer = 0.5f;

    private void Update()
    {
        if (timer <= 0)
        {
            timer = 0.5f;
            Timer();
        }
            
        else
            timer -= Time.deltaTime;
    }

    private void Timer()
    {
        _timerText.text = StringTime.SecondToTimeString(_matchTimer.TimeLeft);
    }
}
