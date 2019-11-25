using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DameNumber : MonoBehaviour {

    public Text damegeText;

    public float lifetime = 1f;
    public float moveSpped = 1;

    public float placementJitter = .5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, lifetime);
        transform.position += new Vector3(0f, moveSpped * Time.deltaTime, 0f);
	}

    public void SetDamage(int damageAmount)
    {
        damegeText.text = damageAmount.ToString();
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0f);
    }
}
