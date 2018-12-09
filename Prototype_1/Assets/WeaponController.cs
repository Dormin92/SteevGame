﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

    private GameObject player;
	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            player.GetComponent<STeevPlayerController>().hittingEnemy = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            player.GetComponent<STeevPlayerController>().hittingEnemy = false;
    }

}
