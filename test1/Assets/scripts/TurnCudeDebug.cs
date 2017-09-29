using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCube : MonoBehaviour
{
    int counter;
    int[] sides = { 0, 0, 0, 0, 0, 0 };

    void Start()
    {
        Rigidbody thisBody = gameObject.GetComponent<Rigidbody>();
        counter = 0;
        StartCoroutine(rollDice(thisBody));
        Time.timeScale = 100;
    }

    void Update()
    {
    }

    IEnumerator rollDice(Rigidbody body)
    {
        yield return new WaitForSeconds(0.05f);
        body.AddForce(0, 100, 0);
        body.maxAngularVelocity = 1000;
        var speed = Random.Range(10, 25);
        body.angularVelocity = Random.insideUnitSphere * speed;
        StartCoroutine(debugDice(body));
    }

    IEnumerator debugDice(Rigidbody body)
    {
        yield return new WaitUntil(body.IsSleeping);
        Dictionary<int, float> diceDictionary = new Dictionary<int, float>();
        var one = Vector3.Angle(-this.transform.forward, Vector3.up);
        var two = Vector3.Angle(this.transform.right, Vector3.up);
        var three = Vector3.Angle(-this.transform.up, Vector3.up);
        var four = Vector3.Angle(this.transform.up, Vector3.up);
        var five = Vector3.Angle(-this.transform.right, Vector3.up);
        var six = Vector3.Angle(this.transform.forward, Vector3.up);
        diceDictionary.Add(0, one);
        diceDictionary.Add(1, two);
        diceDictionary.Add(2, three);
        diceDictionary.Add(3, four);
        diceDictionary.Add(4, five);
        diceDictionary.Add(5, six);

        var currentSide = Mathf.Min(one, two, three, four, five, six);

        foreach (var item in diceDictionary.Keys)
        {
            if (diceDictionary[item] == currentSide)
            {
                sides[item]++;
            }
        }
        counter++;
        if (counter <= 1000)
        {
            StartCoroutine(rollDice(body));
        }
        else
        {
            Debug.Log("1: " + sides[0]);
            Debug.Log("2: " + sides[1]);
            Debug.Log("3: " + sides[2]);
            Debug.Log("4: " + sides[3]);
            Debug.Log("5: " + sides[4]);
            Debug.Log("6: " + sides[5]);
        }
    }
}
