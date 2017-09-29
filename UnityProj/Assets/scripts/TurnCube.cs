using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCube : MonoBehaviour {
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody thisBody;

    void Start()
    {
        thisBody = gameObject.GetComponent<Rigidbody>();
        thisBody.maxAngularVelocity = 50;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(roll());
        }
    }

    IEnumerator roll()
    {
        thisBody.AddForce(0, 100, 0);
        yield return new WaitForSeconds(0.05f);
        var speed = Random.Range(10, 25);
        thisBody.angularVelocity = Random.insideUnitSphere * speed;
    }


}
