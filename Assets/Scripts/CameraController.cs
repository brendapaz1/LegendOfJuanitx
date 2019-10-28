using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour {

    public Transform target;

    public Tilemap TheMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    private float halfHieght;
    private float halfWidth;

	// Use this for initialization
	void Start () {
         target = PlayerController.instance.transform;
        target = FindObjectOfType<PlayerController>().transform;


        halfHieght = Camera.main.orthographicSize;
        halfWidth = halfHieght * Camera.main.aspect;

        bottomLeftLimit = TheMap.localBounds.min + new Vector3(halfWidth,halfHieght,0f);
        topRightLimit = TheMap.localBounds.max + new Vector3(-halfWidth, -halfHieght, 0f);

        PlayerController.instance.SetBounds(TheMap.localBounds.min, TheMap.localBounds.max);
	}
	
	// LaUpdate is called once per frame after Update
	void LateUpdate () {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        //keep the camara inside the bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
	}
}
