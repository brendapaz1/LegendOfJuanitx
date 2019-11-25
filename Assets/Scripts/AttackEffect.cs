using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour {

    public float effectLenght;
    public int SoundEffect;

	// Use this for initialization
	void Start () {
        AudioManager.instance.PlaySFX(SoundEffect);
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, effectLenght);
	}
}
