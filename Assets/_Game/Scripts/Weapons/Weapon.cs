using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour{
    
    [SerializeField] GameObject[] Weapons = new GameObject[0];

    private int currentWeaponIndex;
    private int nextWeaponIndex;
    private int prevWeaponIndex;

    void Start() {
        currentWeaponIndex = 0;
        nextWeaponIndex = 0;
        prevWeaponIndex = 0;
    }

    void Update() {

    }

    void ChangeWeapon() {

    }

}
