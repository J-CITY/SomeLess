using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : FauxGravityBody {
	
	public GameObject cubePref;

	public SphereCollider sphereCol;
	//public ParticleSystem trail;

	private Vector3 normalizeDirection;

	bool isDie = false;

	void Start()
	{
		normalizeDirection = (Vector3.zero - transform.position).normalized;
	}

	void OnCollisionEnter(Collision col)
	{
		if (isDie)
			return;

		isDie = true;

		GetComponent<Rigidbody>().isKinematic = true;

		for (int i = 0; i < 5; ++i) {
			var cube = Instantiate(cubePref, transform.position, transform.rotation);
			
			if (cube != null) {
				var rb_ = cube.AddComponent<Rigidbody>();
				rb_.AddExplosionForce(1f, cube.transform.position, 1f);
			}
		}


		//sphereCol.enabled = false;
		//trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

		enabled = false;

		Destroy(gameObject, 5f);
	}

	void Update()
	{
		transform.position += normalizeDirection * 4f * Time.deltaTime;
	}

}
