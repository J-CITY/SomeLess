using System.Collections;
using System.IO;
using UnityEngine;

public class Pait0 : MonoBehaviour {
	public GameObject Brush;
	public float BrushSize = 0.1f;
	public RenderTexture RTexture;

	void Start() {}

	void Update() {
		if (Input.GetMouseButton(0)) {
			//cast a ray to the plane
			var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(Ray, out hit)) {
				//instanciate a brush
				var go = Instantiate(Brush, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
				go.transform.localScale = Vector3.one * BrushSize;
			}

		}

		if (Input.GetKeyUp(KeyCode.Space)) {
			Save();
		}
	}

	public void Save() {
		StartCoroutine(CoSave());
	}

	private IEnumerator CoSave() {
		//wait for rendering
		yield return new WaitForEndOfFrame();
		Debug.Log(Application.dataPath + "/savedImage.png");

		//set active texture
		RenderTexture.active = RTexture;

		//convert rendering texture to texture2D
		var texture2D = new Texture2D(RTexture.width, RTexture.height);
		texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
		texture2D.Apply();

		//write data to file
		var data = texture2D.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);
	}
}
