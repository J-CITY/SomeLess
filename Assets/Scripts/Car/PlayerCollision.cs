using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	public GameObject cubePref1;
	public GameObject cubePref2;
	public int cubeCount = 50;

	void OnCollisionEnter (Collision col)
	{
		if (col.collider.tag == "Meteor")
		{
			//Instantiate(deathEffect, transform.position, transform.rotation);

			gameObject.SetActive(false);

			for (int i = 0; i < cubeCount; ++i) {
				GameObject cube = null;
				if (Random.Range(0f, 1f) > 0.5f) {
					cube = Instantiate(cubePref1, transform.position, transform.rotation);
				} else {
					cube = Instantiate(cubePref2, transform.position, transform.rotation);
				}
				if (cube != null) {
					var rb_ = cube.AddComponent<Rigidbody>();
					rb_.AddExplosionForce(10f, cube.transform.position, 1f);
				}
			}

			Debug.Log("Die");

			//Destroy(gameObject);
		}
	}

}
