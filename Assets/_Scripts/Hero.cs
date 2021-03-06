﻿using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero		S;

    [Header("Set in Inspector")]
    public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;
    public float    gameRestartDelay = 2f;
    public GameObject   projectilePrefab;
    public float    projectileSpeed = 40;
    public          Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

	public Bounds bounds;

	void Awake(){
        if (S == null) {
            S = this;
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);
        }
        //fireDelegate += TempFire;
	}
	
	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		bounds.center = transform.position;
		
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}
		
		transform.rotation =Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);

        //if ( Input.GetKeyDown( KeyCode.Space) ) {
        //    TempFire();
        //}
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }
    }

    void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: "+go.name);
        //print("Triggered: " + other.gameObject.name);

        if (go == lastTriggerGo) {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy") {
            shieldLevel--;
            Destroy(go);
        } else if (go.tag == "PowerUp") {
            AbsorbPowerUp(go);
        } else {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type) {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel {
        get {
            return (_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0) {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot() {
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].type == WeaponType.none) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }
}