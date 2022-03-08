using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    private int currentWeaponIndex = 0;

    void Start()
    {
        SetActiveWeapon();
    }

    void Update()
    {
        int previousWeaponIndex = currentWeaponIndex;
        ProcessWeaponKeyInput();
        ProcessWeaponScrollInput();
        if(previousWeaponIndex != currentWeaponIndex)
        {
            SetActiveWeapon();
        }
    }

    void SetActiveWeapon()
    {
        int weaponIndex = 0;
        foreach(Transform weapon in transform) 
        {
            if(weaponIndex == currentWeaponIndex) 
            {
                weapon.gameObject.SetActive(true);
            }
            else if(weaponIndex != currentWeaponIndex) 
            {
                weapon.gameObject.SetActive(false);
            }
            weaponIndex++;
        }
    }

    void ProcessWeaponKeyInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            currentWeaponIndex = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            currentWeaponIndex = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            currentWeaponIndex = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            currentWeaponIndex = 3;
        }
    }

    void ProcessWeaponScrollInput()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(currentWeaponIndex >= transform.childCount - 1)
            {
                currentWeaponIndex = 0;
            }
            else
            {
                currentWeaponIndex++;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(currentWeaponIndex <= transform.childCount - transform.childCount)
            {
                currentWeaponIndex = transform.childCount - 1;
            }
            else 
            {
                currentWeaponIndex--;
            }
        }
    }

}
