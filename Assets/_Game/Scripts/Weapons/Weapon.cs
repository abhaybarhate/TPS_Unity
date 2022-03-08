using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{

    [SerializeField] WeaponType weaponType;
    [SerializeField] TP_Controller playerController;
    [SerializeField] float weaponShootRange;
    [SerializeField] float weaponFiringRate;
    [SerializeField] float weaponDamage;
    [SerializeField] LayerMask ShootingLayerMask;
    [SerializeField] Transform debugTransform;
    [SerializeField] Animator PlayerAnimator;
    
    void Start()
    {

    }

    void Update()
    {
        GetMouseWorldPosition();
        Shoot();
        
    }

    void Shoot()
    {
        if(weaponType == WeaponType.Handgun)
        {
            HandlePistolShooting();
        }
        if(weaponType == WeaponType.Rifle)
        {
            HandleRifleShooting();
        }
        if(weaponType == WeaponType.SniperRifle)
        {
            HandleSniperRifleShooting();
        }
    }

    void GetMouseWorldPosition()
    {
        RaycastHit hitInfo;
        Vector2 screenCenter = new Vector2(Screen.width/2f, Screen.height/2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if(Physics.Raycast(ray,out hitInfo,999f,ShootingLayerMask))
        {
            debugTransform.position = hitInfo.point;
        }
    }

    void HandlePistolShooting()
    {
        playerController.SetIsUsingPistol(true);
        playerController.SetIsUsingRifle(false);
    }

    void HandleRifleShooting()
    {
        playerController.SetIsUsingRifle(true);
        playerController.SetIsUsingPistol(false);
    
    }

    void HandleSniperRifleShooting()
    {

    }

}