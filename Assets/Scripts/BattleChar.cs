﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour {

    public bool isPlayer;
    public string[] movesAvailable;

    public string charName;
    public int currentHP, maxHP, currentMP, maxMP, strength, defence, wpnPower, armrPower;
    public bool hasDied;

    public SpriteRenderer theSprite;
    public Sprite deadSprite, aliveSrpite;

    private bool shouldFade;
    private float fadeSpeed = 1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldFade)
        {
            theSprite.color = new Color(Mathf.MoveTowards(theSprite.color.r, 1f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.g, 0f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.b, 0f, fadeSpeed * Time.deltaTime), Mathf.MoveTowards(theSprite.color.a, 0f, fadeSpeed * Time.deltaTime));
            if(theSprite.color.a==0)
            {
                gameObject.SetActive(false);
            }
        }
	}

    public void EnemyFade()
    {
        shouldFade = true;
    }
}