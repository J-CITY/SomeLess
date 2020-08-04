using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour {
	private Text fpsText;
	private float deltaTime = 0f;

	private void Start() {
		fpsText = gameObject.GetComponent<Text>();
	}

	private void Update() {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString() + " Cubes: " + CubesGenerator.DEBUG_CUBES_COUNT.ToString();
	}
}
