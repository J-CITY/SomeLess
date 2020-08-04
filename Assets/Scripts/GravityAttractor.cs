using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
	public static List<GravityAttractor> Attractors;

	public Rigidbody rb;
	public float G = 6674f;

	void FixedUpdate()
	{
		foreach (GravityAttractor attractor in Attractors) {
			if (attractor != this)
				Attract(attractor);
		}
	}

	void OnEnable()
	{
		if (Attractors == null)
			Attractors = new List<GravityAttractor>();

		Attractors.Add(this);
	}

	void OnDisable()
	{
		Attractors.Remove(this);
	}

	void Attract(GravityAttractor ga)
	{

		Rigidbody rbToAttract = ga.rb;

		Vector3 direction = rb.position - rbToAttract.position;
		float distance = direction.magnitude;

		if (distance == 0f)
			return;

		float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
		Vector3 force = direction.normalized * forceMagnitude;

		rbToAttract.AddForce(force);
	}
}
