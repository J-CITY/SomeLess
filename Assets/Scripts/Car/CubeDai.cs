using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDai : MonoBehaviour
{
	void Start()
	{
		StartCoroutine(Die());
	}

	IEnumerator Die()
	{
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
