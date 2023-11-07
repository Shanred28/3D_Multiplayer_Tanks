using UnityEngine;
using UnityEngine.UI;

public class UIIndicatorModuleRepair : MonoBehaviour
{
    [SerializeField] private Slider _sliderLeftTrackModule;
    [SerializeField] private GameObject _imageDestroyerLeftTrack;
    [SerializeField] private Slider _sliderRightTrackModule;
    [SerializeField] private GameObject _imageDestroyerRightTrack;

    [SerializeField] private TrackModule _trackModule;


    private void Start()
    {
        _trackModule.DesotroyerModule += OnModulekDestroyer;
        _imageDestroyerLeftTrack.SetActive(false);
        _imageDestroyerRightTrack.SetActive(false);
    }

    private void Update()
    {
        if (_sliderLeftTrackModule.maxValue > 0)
        {
            _sliderLeftTrackModule.value -= Time.deltaTime;
        }
        else
            _imageDestroyerLeftTrack.SetActive(true);

        if (_sliderRightTrackModule.maxValue > 0)
        {
            _sliderRightTrackModule.value -= Time.deltaTime;
        }
        else
            _imageDestroyerRightTrack.SetActive(true);
    }

    private void OnDestroy()
    {
        _trackModule.DesotroyerModule -= OnModulekDestroyer;
    }

    private void OnModulekDestroyer(VehicleModule vehicleModule, TypeModule typeModule)
    {
        if (typeModule == TypeModule.LeftTrack)
        {
            _sliderLeftTrackModule.maxValue = vehicleModule.RemainingRecoveryTime;
            _sliderLeftTrackModule.value = vehicleModule.RemainingRecoveryTime;
            _imageDestroyerLeftTrack.SetActive(false);
        }

        if (typeModule == TypeModule.RightTrack)
        {
            _sliderRightTrackModule.maxValue = vehicleModule.RemainingRecoveryTime;
            _sliderRightTrackModule.value = vehicleModule.RemainingRecoveryTime;
            _imageDestroyerRightTrack.SetActive(false);
        }
        
    }
}
