using System;
using UnityEngine;

public class UIHitResultPanel : MonoBehaviour
{
    [SerializeField] private Transform _panelVehicle;
    [SerializeField] private UIHitResultPopUp hitResultPopUpPrefab;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        Player.Local.ProjectileHit -= OnProjectileHit;
    }

    private void OnMatchStart()
    {
        Player.Local.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileHitResult hitResult)
    {
        if (hitResult.type == ProjectileHitType.Enviroment) return;

        UIHitResultPopUp hitPopUp = Instantiate(hitResultPopUpPrefab);
        hitPopUp.transform.SetParent(_panelVehicle);
        hitPopUp.transform.localScale = Vector3.one;

        hitPopUp.transform.position = Camera.main.WorldToScreenPoint(hitResult.point);

        if (hitResult.type == ProjectileHitType.Penetration)
            hitPopUp.SetTypeResult("Пробитие!");

        if(hitResult.type == ProjectileHitType.Ricochet)
            hitPopUp.SetTypeResult("Рикошет!");

        if (hitResult.type == ProjectileHitType.NoPenetration)
            hitPopUp.SetTypeResult("Броня не пробита!");

        hitPopUp.SetDamageResult(hitResult.damage);
    }
}
