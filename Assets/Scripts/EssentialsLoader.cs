﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour {

    public GameObject UIScreen;
    public GameObject player;
    public GameObject gameMan;
    public GameObject audioMan;
    public GameObject battleMan;

    // Use this for initialization
    void Start () {
		if(UIFade.instance == null)
        {
            UIFade.instance =  Instantiate(UIScreen).GetComponent<UIFade>();
        }

        if(PlayerController.instance == null)
        {
           PlayerController clone =Instantiate(player).GetComponent<PlayerController>();
           PlayerController.instance = clone;
        }

        if (AudioManager.instance == null)
        {
            AudioManager.instance = Instantiate(audioMan).GetComponent<AudioManager>();
        }

        if (GameManager.instance == null)
        {
            Instantiate(gameMan);
        }

        if (BattleManager.instance == null)
        {
            BattleManager.instance = Instantiate(battleMan).GetComponent<BattleManager>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
