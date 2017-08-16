using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Useful mesh functions
*/
public class MeshUtils : MonoBehaviour 
{
	// Finds a set of adjacent vertices for a given vertex
	// Note the success of this routine expects only the set of neighboring faces to eacn contain one vertex corresponding
	// to the vertex in question
	public static List<Vector3> findAdjacentNeighbors ( Vector3[] v, int[] t, Vector3 vertex )
	{
		List<Vector3>adjacentV = new List<Vector3>();
		List<int>facemarker = new List<int>();
		int facecount = 0;	

		// Find matching vertices
		for (int i=0; i<v.Length; i++)
			if (vertex == v[i])
			{
				int v1 = 0;
				int v2 = 0;
				bool marker = false;

				// Find vertex indices from the triangle array
				for(int k=0; k<t.Length; k=k+3)
					if(facemarker.Contains(k) == false)
					{
						v1 = 0;
						v2 = 0;
						marker = false;

						if(i == t[k])
						{
							v1 = t[k+1];
							v2 = t[k+2];
							marker = true;
						}

						if(i == t[k+1])
						{
							v1 = t[k];
							v2 = t[k+2];
							marker = true;
						}

						if(i == t[k+2])
						{
							v1 = t[k];
							v2 = t[k+1];
							marker = true;
						}

						facecount++;
						if(marker)
						{
							// Once face has been used mark it so it does not get used again
							facemarker.Add(k);

							// Add non duplicate vertices to the list
							if ( isVertexExist(adjacentV, v[v1]) == false )
							{	
								adjacentV.Add(v[v1]);
								//Debug.Log("Adjacent vertex index = " + v1);
							}

							if ( isVertexExist(adjacentV, v[v2]) == false )
							{
								adjacentV.Add(v[v2]);
								//Debug.Log("Adjacent vertex index = " + v2);
							}
							marker = false;
						}
					}
			}

		//Debug.Log("Faces Found = " + facecount);

		return adjacentV;
	}


	// Finds a set of adjacent vertices indexes for a given vertex
	// Note the success of this routine expects only the set of neighboring faces to eacn contain one vertex corresponding
	// to the vertex in question
	public static List<int> findAdjacentNeighborIndexes ( Vector3[] v, int[] t, Vector3 vertex )
	{
		List<int>adjacentIndexes = new List<int>();
		List<Vector3>adjacentV = new List<Vector3>();
		List<int>facemarker = new List<int>();
		int facecount = 0;	

		// Find matching vertices
		for (int i=0; i<v.Length; i++)
			if (vertex == v[i])
			{
				int v1 = 0;
				int v2 = 0;
				bool marker = false;

				// Find vertex indices from the triangle array
				for(int k=0; k<t.Length; k=k+3)
					if(facemarker.Contains(k) == false)
					{
						v1 = 0;
						v2 = 0;
						marker = false;

						if(i == t[k])
						{
							v1 = t[k+1];
							v2 = t[k+2];
							marker = true;
						}

						if(i == t[k+1])
						{
							v1 = t[k];
							v2 = t[k+2];
							marker = true;
						}

						if(i == t[k+2])
						{
							v1 = t[k];
							v2 = t[k+1];
							marker = true;
						}

						facecount++;
						if(marker)
						{
							// Once face has been used mark it so it does not get used again
							facemarker.Add(k);

							// Add non duplicate vertices to the list
							if ( isVertexExist(adjacentV, v[v1]) == false )
							{	
								adjacentV.Add(v[v1]);
								adjacentIndexes.Add(v1);
								//Debug.Log("Adjacent vertex index = " + v1);
							}

							if ( isVertexExist(adjacentV, v[v2]) == false )
							{
								adjacentV.Add(v[v2]);
								adjacentIndexes.Add(v2);
								//Debug.Log("Adjacent vertex index = " + v2);
							}
							marker = false;
						}
					}
			}

		//Debug.Log("Faces Found = " + facecount);

		return adjacentIndexes;
	}

	// Does the vertex v exist in the list of vertices
	static bool isVertexExist(List<Vector3>adjacentV, Vector3 v)
	{
		bool marker = false;
		foreach (Vector3 vec in adjacentV)
			if (vec == v)
			{
				marker = true;
				break;
			}

		return marker;
	}
	// Clone a mesh
	public static Mesh CloneMesh(Mesh mesh)
	{
		Mesh clone = new Mesh();
		clone.vertices = mesh.vertices;
		clone.normals = mesh.normals;
		clone.tangents = mesh.tangents;
		clone.triangles = mesh.triangles;
		clone.uv = mesh.uv;
		clone.uv2 = mesh.uv2;
		clone.uv3 = mesh.uv3;
		clone.uv4 = mesh.uv4;
		clone.bindposes = mesh.bindposes;
		clone.boneWeights = mesh.boneWeights;
		clone.bounds = mesh.bounds;
		clone.colors = mesh.colors;
		clone.name = mesh.name;
		//TODO : Are we missing anything?
		return clone;
	}

	public static void Weld(Mesh mesh, float bucketStep = 1f){
		if (mesh.vertices.Length == 0 || bucketStep <= 0f)
			return;
		Vector3[] oldVertices = mesh.vertices;
		Vector3[] newVertices = new Vector3[oldVertices.Length];
		int[] old2new = new int[oldVertices.Length];
		int newSize = 0, i, j, x, y, z;

		//find AABB

		Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);

		for (i = 0; i < oldVertices.Length; i++) {
			min.x = Mathf.Min (min.x, oldVertices[i].x);
			min.y = Mathf.Min (min.y, oldVertices[i].y);
			min.z = Mathf.Min (min.z, oldVertices[i].z);
			max.x = Mathf.Max (max.x, oldVertices[i].x);
			max.y = Mathf.Max (max.y, oldVertices[i].y);
			max.z = Mathf.Max (max.z, oldVertices[i].z);
		}

		//make cubic buckets, each with dimentions "bucketStep"

		int bucketSizeX = Mathf.FloorToInt ((max.x - min.x) / bucketStep) + 1;
		int bucketSizeY = Mathf.FloorToInt ((max.y - min.y) / bucketStep) + 1;
		int bucketSizeZ = Mathf.FloorToInt ((max.z - min.z) / bucketStep) + 1;
		//Debug.Log(string.Format("x: {0}\ty: {1}\tz: {2}", bucketStep, max.x, min.y));
		List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

		//make new vertices
		for (i=0; i<oldVertices.Length; i++){
			//Determine which bucket it belongs to
			x = Mathf.FloorToInt((oldVertices[i].x - min.x)/bucketStep);
			y = Mathf.FloorToInt((oldVertices[i].y - min.y)/bucketStep);
			z = Mathf.FloorToInt((oldVertices[i].z - min.z)/bucketStep);

			//Check if it's already added

			if (buckets [x, y, z] == null) {
				buckets [x, y, z] = new List<int> ();
			}

			for (j = 0; j < buckets [x, y, z].Count; j++) {
				if (newVertices[buckets[x,y,z][j]] == oldVertices[i]){
					old2new [i] = buckets [x, y, z] [j];
					goto skip;
				}
			}

			//add new vertex

			newVertices [newSize] = oldVertices [i];
			buckets [x, y, z].Add (newSize);
			old2new [i] = newSize;
			newSize++;

			skip:;
		}

		// Make new triangles.
		int[] oldTris = mesh.triangles;
		int[] newTris = new int[oldTris.Length];

		for (i = 0; i < oldTris.Length; i++) {
			newTris [i] = old2new [oldTris [i]];
		}

		Vector3[] finalVertices = new Vector3[newSize];

		for (i = 0; i < newSize; i++) {
			finalVertices [i] = newVertices [i];
		}

		mesh.Clear ();
		mesh.vertices = finalVertices;
		mesh.triangles = newTris;
		mesh.RecalculateNormals();
	}

}
