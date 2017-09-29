using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour {
    private Vector3 screenPoint;
    private Vector3 offset;
    private Transform objectTransform;
    private Plane dragPlane = new Plane(Vector3.up, new Vector3(0, 3, 0));

    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0;
        dragPlane.Raycast(ray, out enter);
        transform.position = ray.GetPoint(enter);
    }
}
