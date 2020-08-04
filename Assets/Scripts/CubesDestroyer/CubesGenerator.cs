using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Config {
	public static bool IS_COLORFULL = true;
	public static bool IS_ONE_SHAPE = false;

	public static bool IS_ROTATE = true;
	public static bool IS_CAMERA_ROTATE = true;



	public static int DESTROY_CUBE = 1;
	//0 - destroy one cube
	//1 - gen many cubes
	//2 - gen particles
	//3 - particles with cubes
	public static int DESTROY_CUBES_COUNT = 40;


	public static float ROTATE_SPEED = 10f;

	public static bool HIDE_INVISABLE_OBJECT = true;


	public static float DIE_TIME = 5f;
	
}

public class CubesGenerator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public static int DEBUG_CUBES_COUNT = 0;
	

	public GameObject cubesObj;
	public GameObject cubesPrefab;
	public GameObject cubesParticles;


	private int height = 0;
	private int width = 0;
	private int depth = 0;


	GameObject[,,] cubesMap;

	Material[] materials = new Material[7];

	string shapeName = "torusMax";

	public bool buttonPressed = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		buttonPressed = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		buttonPressed = false;
	}

	public float minZoom = 10f;
	public float maxZoom = 70f;
	public void Zoom(float z) {
		Camera.main.fieldOfView += z;
		if (Camera.main.fieldOfView < minZoom) {
			Camera.main.fieldOfView = minZoom;
		}

		if (Camera.main.fieldOfView > maxZoom) {
			Camera.main.fieldOfView = maxZoom;
		}
	}


	void Start() {
		//load material
		if (!Config.IS_ONE_SHAPE) {
			materials[0] = Resources.Load("Materials/Mobile/MBlue") as Material;
			materials[1] = Resources.Load("Materials/Mobile/MCyan") as Material;
			materials[2] = Resources.Load("Materials/Mobile/MGreen") as Material;
			materials[3] = Resources.Load("Materials/Mobile/MMagenta") as Material;
			materials[4] = Resources.Load("Materials/Mobile/MOrange") as Material;
			materials[5] = Resources.Load("Materials/Mobile/MYellow") as Material;
			materials[6] = Resources.Load("Materials/Mobile/MRed") as Material;
			
		}

		var jsonObj = Resources.Load<TextAsset>("json/" + shapeName).ToString();

		JSONObject json = new JSONObject(jsonObj.ToString());

		JSONObject shapeSize = (JSONObject)json.GetField("dimension");

		width = int.Parse(shapeSize.list[0].GetField("width").str);
		height = int.Parse(shapeSize.list[0].GetField("height").str);
		depth = int.Parse(shapeSize.list[0].GetField("depth").str);

		cubesMap = new GameObject[width + 1, height + 1, depth + 1];
		//Debug.Log(height+" "+ width + " " + depth);

		JSONObject cubes = (JSONObject)json.GetField("voxels");
		for (int i = 0; i < cubes.list.Count; i++) {
			var cubeData = cubes.list[i];
			var x = float.Parse(cubeData.GetField("x").str);
			var y = float.Parse(cubeData.GetField("y").str);
			var z = float.Parse(cubeData.GetField("z").str);

			var cube = Instantiate(cubesPrefab, new Vector3(x - width / 2,
				y - height / 2, z - depth / 2),
				Quaternion.identity, cubesObj.transform);
			CubesGenerator.DEBUG_CUBES_COUNT++;

			//Debug.Log((int)x+" "+ (int)y+" "+ (int)z);
			cubesMap[(int)(x), (int)(y), (int)(z)] = cube;
			if (Config.IS_COLORFULL) {
				addMaterial(cube, (int)(y) % 7);
			}
		}

		//deactivate invisable cubes
		if (Config.HIDE_INVISABLE_OBJECT && !Config.IS_ONE_SHAPE) {
			for (int i = 0; i < width + 1; i++) {
				for (int j = 0; j < height + 1; j++) {
					for (int k = 0; k < depth + 1; k++) {
						if ((i > 0 && cubesMap[i - 1, j, k] != null) && (i < width && cubesMap[i + 1, j, k] != null) &&
							(j > 0 && cubesMap[i, j - 1, k] != null) && (j < height && cubesMap[i, j + 1, k] != null) &&
							(k > 0 && cubesMap[i, j, k - 1] != null) && (k < depth && cubesMap[i, j, k + 1] != null)) {
							cubesMap[i, j, k].SetActive(false);
						}
					}
				}
			}
		}

		if (Config.IS_ONE_SHAPE) {
			Combine();
		}
	}
	
    void Update() {
		if (Config.IS_ROTATE) {
			updateRotate();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Zoom(-2f);
			return;
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Zoom(2f);
			return;
		}


		if (Config.IS_ONE_SHAPE) {

			if (Input.GetMouseButton(0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 1000.0f, 1 << 9)) {


					//pos of deliting cube
					Vector3 blockPos = hit.point - hit.normal / 2.0f;

					blockPos.x = (float)Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
					blockPos.y = (float)Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
					blockPos.z = (float)Math.Round(blockPos.z, MidpointRounding.AwayFromZero);

					Debug.Log("POS: ");
					Debug.Log(blockPos);

					RaycastHit _hit;
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.up), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("UP");
					}
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.down), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("DOWN");
					}
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.left), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("LEFT");
					}
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.right), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("RIGHT");
					}
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.forward), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("FORWARD");
					}
					if (Physics.Raycast(blockPos, cubesObj.transform.TransformDirection(Vector3.back), out _hit, 2.0f)) {
						deletePart(_hit);
						Debug.Log("BACK");
					}

				}
			}
		} else {

			if (Input.GetMouseButton(0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 1000.0f, 1 << 9)) {


					var pos = hit.transform.localPosition;
					pos.x += width / 2;
					pos.y += height / 2;
					pos.z += depth / 2;

					if (Config.HIDE_INVISABLE_OBJECT && !Config.IS_ONE_SHAPE) {
						//enable 6 naighbour cubes
						if (pos.x > 0 && cubesMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] != null) {
							cubesMap[(int)pos.x - 1, (int)pos.y, (int)pos.z].SetActive(true);
						}
						if (pos.x < width && cubesMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] != null) {
							cubesMap[(int)pos.x + 1, (int)pos.y, (int)pos.z].SetActive(true);
						}
						if (pos.y > 0 && cubesMap[(int)pos.x, (int)pos.y - 1, (int)pos.z] != null) {
							cubesMap[(int)pos.x, (int)pos.y - 1, (int)pos.z].SetActive(true);
						}
						if (pos.y < height && cubesMap[(int)pos.x, (int)pos.y + 1, (int)pos.z] != null) {
							cubesMap[(int)pos.x, (int)pos.y + 1, (int)pos.z].SetActive(true);
						}
						if (pos.z > 0 && cubesMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] != null) {
							cubesMap[(int)pos.x, (int)pos.y, (int)pos.z - 1].SetActive(true);
						}
						if (pos.z < depth && cubesMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] != null) {
							cubesMap[(int)pos.x, (int)pos.y, (int)pos.z + 1].SetActive(true);
						}

					}

					DestroyCube(hit.transform.gameObject);
				}
			}
		}


	}

	void addMaterial(GameObject cube, int id) {
		if (Config.IS_ONE_SHAPE) {

		} else {
			cube.gameObject.GetComponent<Renderer>().material = materials[id];
		}
	}

	void updateRotate() {
		if (!Config.IS_CAMERA_ROTATE) {
			cubesObj.transform.RotateAround(Vector3.zero, Vector3.up, 
				Config.ROTATE_SPEED*Time.deltaTime);
		} else {
			Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, -Config.ROTATE_SPEED * Time.deltaTime);
		}
	}


	// create one big shape
	private void Combine() {
		MeshFilter[] meshFilters = cubesObj.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];//CombineInstance
		Destroy(cubesObj.gameObject.GetComponent<MeshCollider>());

		int i = 0;

		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}
		cubesObj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		cubesObj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

		cubesObj.transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		cubesObj.transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		//transform.GetComponent<MeshFilter>().mesh.Optimize();
		//MeshUtility.Optimize(cubesObj.transform.GetComponent<MeshFilter>().mesh);

		cubesObj.gameObject.AddComponent<MeshCollider>();
		cubesObj.transform.gameObject.SetActive(true);

		for (int ii = 0; ii < width + 1; ii++) {
			for (int j = 0; j < height + 1; j++) {
				for (int k = 0; k < depth + 1; k++) {
					//if ((i > 0 && cubesMap[i - 1, j, k] != null) && (i < width && cubesMap[i + 1, j, k] != null) &&
					//	(j > 0 && cubesMap[i, j - 1, k] != null) && (j < height && cubesMap[i, j + 1, k] != null) &&
					//	(k > 0 && cubesMap[i, j, k - 1] != null) && (k < depth && cubesMap[i, j, k + 1] != null)) {
					if (cubesMap[ii, j, k] != null) {
						cubesMap[ii, j, k].SetActive(false);
					}
					//}
				}
			}
		}
	}

	void deletePart(RaycastHit hit) {
		int hitTri = hit.triangleIndex;

		//get neighbour
		int[] triangles = cubesObj.transform.GetComponent<MeshFilter>().mesh.triangles;
		Vector3[] vertices = cubesObj.transform.GetComponent<MeshFilter>().mesh.vertices;
		Vector3 p0 = vertices[triangles[hitTri * 3 + 0]];
		Vector3 p1 = vertices[triangles[hitTri * 3 + 1]];
		Vector3 p2 = vertices[triangles[hitTri * 3 + 2]];

		float edge1 = Vector3.Distance(p0, p1);
		float edge2 = Vector3.Distance(p0, p2);
		float edge3 = Vector3.Distance(p1, p2);

		Vector3 shared1;
		Vector3 shared2;
		if (edge1 > edge2 && edge1 > edge3) {
			shared1 = p0;
			shared2 = p1;
		} else if (edge2 > edge1 && edge2 > edge3) {
			shared1 = p0;
			shared2 = p2;
		} else {
			shared1 = p1;
			shared2 = p2;
		}

		int v1 = findVertex(shared1);
		int v2 = findVertex(shared2);

		deleteSquare(hitTri, findTriangle(vertices[v1], vertices[v2], hitTri));
	}

	void deleteSquare(int index1, int index2) {
		Destroy(cubesObj.gameObject.GetComponent<MeshCollider>());
		Mesh mesh = cubesObj.transform.GetComponent<MeshFilter>().mesh;
		int[] oldTriangles = mesh.triangles;
		int[] newTriangles = new int[mesh.triangles.Length - 6];

		int i = 0;
		int j = 0;
		while (j < mesh.triangles.Length) {
			if (j != index1 * 3 && j != index2 * 3) {
				newTriangles[i++] = oldTriangles[j++];
				newTriangles[i++] = oldTriangles[j++];
				newTriangles[i++] = oldTriangles[j++];
			} else {

				j += 3;
			}
		}
		cubesObj.transform.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
		cubesObj.gameObject.AddComponent<MeshCollider>();

	}

	int findVertex(Vector3 v) {
		Vector3[] vertices = cubesObj.transform.GetComponent<MeshFilter>().mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {
			if (vertices[i] == v)
				return i;
		}
		return -1;
	}

	int findTriangle(Vector3 v1, Vector3 v2, int notTriIndex) {
		int[] triangles = cubesObj.transform.GetComponent<MeshFilter>().mesh.triangles;
		Vector3[] vertices = cubesObj.transform.GetComponent<MeshFilter>().mesh.vertices;
		int i = 0;
		int j = 0;
		int found = 0;
		while (j < triangles.Length) {
			if (j / 3 != notTriIndex) {
				if (vertices[triangles[j]] == v1 && (vertices[triangles[j + 1]] == v2 || vertices[triangles[j + 2]] == v2))
					return j / 3;
				else if (vertices[triangles[j]] == v2 && (vertices[triangles[j + 1]] == v1 || vertices[triangles[j + 2]] == v1))
					return j / 3;
				else if (vertices[triangles[j + 1]] == v2 && (vertices[triangles[j]] == v1 || vertices[triangles[j + 2]] == v1))
					return j / 3;
				else if (vertices[triangles[j + 1]] == v1 && (vertices[triangles[j]] == v2 || vertices[triangles[j + 2]] == v2))
					return j / 3;
			}

			j += 3;
		}

		return -1;
	}


	private void DestroyCube(GameObject cube) {
		cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		var rb = cube.AddComponent<Rigidbody>();
		if (rb) {
			rb.AddForceAtPosition(cube.transform.position.normalized * 5f, cube.transform.position, ForceMode.Impulse);
		}

		if (rb!=null && (Config.DESTROY_CUBE == 1 || Config.DESTROY_CUBE == 3)) {
			for (int i = 0; i < Config.DESTROY_CUBES_COUNT; ++i) {
				var cube_ = Instantiate(cubesPrefab, cube.transform.position,
				Quaternion.identity, cube.transform);
				CubesGenerator.DEBUG_CUBES_COUNT++;

				cube_.GetComponent<Renderer>().material = cube.GetComponent<Renderer>().material;
				//cube_.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

				var rb_ = cube_.AddComponent<Rigidbody>();
				rb_.AddExplosionForce(10f, cube_.transform.position, 10f);
			}
		}

		if (rb != null && (Config.DESTROY_CUBE == 2 || Config.DESTROY_CUBE == 3)) {
			var particle = Instantiate(cubesParticles, cube.transform.position,
				Quaternion.identity, cubesObj.transform);

			particle.transform.LookAt(cube.transform.position, Vector3.up);

			//particle.GetComponent<ParticleSystem>().startColor = new Color(1, 0, 1, 1f);

			StartCoroutine(Die(particle));
		}


		StartCoroutine(Die(cube));


	}

	IEnumerator Die(GameObject cube) {
		yield return new WaitForSeconds(Config.DIE_TIME);
		Destroy(cube);
		CubesGenerator.DEBUG_CUBES_COUNT--;
		CubesGenerator.DEBUG_CUBES_COUNT-=Config.DESTROY_CUBES_COUNT;
	}
}
