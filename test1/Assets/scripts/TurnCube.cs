using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCube : MonoBehaviour {
    private Vector3 screenPoint;
    private Vector3 offset;

    void Update()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z + 2));
        Rigidbody thisBody = gameObject.GetComponent<Rigidbody>();

        if (Input.GetKeyDown(KeyCode.Space))
        {            
            thisBody.AddForce(0, 100, 0);
            thisBody.angularVelocity = new Vector3(Random.value * 100, Random.value * 100, Random.value * 100);
        }
    }
}
