using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
	float r = 10.8f;

	public GameObject player;

	public GameObject o1_1;
	public GameObject o1_2;
	public GameObject o2_1;
	public GameObject o2_2;
	public GameObject o3_1;
	public GameObject o3_2;


    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(UpdateEnv());
    }

	IEnumerator UpdateEnv()
	{
		var d1 = Vector3.Distance(player.transform.position, o1_1.transform.position);

		if (d1 > r * 1.2f) {
			if (Random.Range(0f, 1f) > 0.5f) {
				o1_1.SetActive(true);
				o1_2.SetActive(false);
			} else {
				o1_1.SetActive(false);
				o1_2.SetActive(true);
			}
		}

		var d2 = Vector3.Distance(player.transform.position, o2_1.transform.position);

		if (d2 > r * 1.2f) {
			if (Random.Range(0f, 1f) > 0.5f) {
				o2_1.SetActive(true);
				o2_2.SetActive(false);
			} else {
				o2_1.SetActive(false);
				o2_2.SetActive(true);
			}
		}

		var d3 = Vector3.Distance(player.transform.position, o3_1.transform.position);

		if (d3 > r * 1.2f) {
			if (Random.Range(0f, 1f) > 0.5f) {
				o3_1.SetActive(true);
				o3_2.SetActive(false);
			} else {
				o3_1.SetActive(false);
				o3_2.SetActive(true);
			}
		}
		yield return new WaitForSeconds(5f);

		StartCoroutine(UpdateEnv());
	}
	
}
