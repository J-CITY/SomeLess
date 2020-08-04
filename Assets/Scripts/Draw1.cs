using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw1 : MonoBehaviour
{
    void Start()
    {
        
    }
	
    void Update()
    {
		if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0)) {
			Debug.Log("!Q!");

			Plane plane = new Plane(Camera.main.transform.forward * -1, transform.position);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDist = 0f;
			if (plane.Raycast(ray, out rayDist)) {

				transform.position = ray.GetPoint(rayDist);
			}
		}    
    }
}
