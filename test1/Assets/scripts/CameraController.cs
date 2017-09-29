using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject die;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = transform.position - die.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = die.transform.position + offset;
	}
}
