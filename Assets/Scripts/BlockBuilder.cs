using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockBuilder : MonoBehaviour {
	public GameObject newBlock;

	void Combine(GameObject block) {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];//CombineInstance
		Destroy(this.gameObject.GetComponent<MeshCollider>());

		int i = 0;

		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);

		transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		//transform.GetComponent<MeshFilter>().mesh.Optimize();
		//MeshUtility.Optimize(transform.GetComponent<MeshFilter>().mesh);

		gameObject.AddComponent<MeshCollider>();
		transform.gameObject.SetActive(true);

		Destroy(block);
	}


	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 1000.0f)) {
				//generate new block
				Vector3 blockPos = hit.point + hit.normal / 2.0f;

				blockPos.x = (float)Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
				blockPos.y = (float)Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
				blockPos.z = (float)Math.Round(blockPos.z, MidpointRounding.AwayFromZero);

				GameObject block = (GameObject)Instantiate(newBlock, blockPos, Quaternion.identity);
				block.transform.parent = transform;
				Combine(block);

			}
		}

		if (Input.GetMouseButtonDown(1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 1000.0f)) {
				

				//pos of deliting cube
				Vector3 blockPos = hit.point - hit.normal / 2.0f;

				blockPos.x = (float)Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
				blockPos.y = (float)Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
				blockPos.z = (float)Math.Round(blockPos.z, MidpointRounding.AwayFromZero);

				Debug.Log("POS: ");
				Debug.Log(blockPos);
				
				RaycastHit _hit;
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.up), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("UP");
				}
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.down), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("DOWN");
				}
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.left), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("LEFT");
				}
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.right), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("RIGHT");
				}
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.forward), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("FORWARD");
				}
				if (Physics.Raycast(blockPos, transform.TransformDirection(Vector3.back), out _hit, 10.0f)) {
					deletePart(_hit);
					Debug.Log("BACK");
				}

			}
		}

	}

	void deletePart(RaycastHit hit) {
		int hitTri = hit.triangleIndex;

		//get neighbour
		int[] triangles = transform.GetComponent<MeshFilter>().mesh.triangles;
		Vector3[] vertices = transform.GetComponent<MeshFilter>().mesh.vertices;
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

	void deleteSquare(int index1, int index2)
	{
		Destroy(this.gameObject.GetComponent<MeshCollider>());
		Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
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
		transform.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
		this.gameObject.AddComponent<MeshCollider>();

	}

	int findVertex(Vector3 v)
	{
		Vector3[] vertices = transform.GetComponent<MeshFilter>().mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {
			if (vertices[i] == v)
				return i;
		}
		return -1;
	}

	int findTriangle(Vector3 v1, Vector3 v2, int notTriIndex)
	{
		int[] triangles = transform.GetComponent<MeshFilter>().mesh.triangles;
		Vector3[] vertices = transform.GetComponent<MeshFilter>().mesh.vertices;
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
}

