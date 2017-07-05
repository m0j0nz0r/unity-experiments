using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    MeshSmoothTest
 
	Laplacian Smooth Filter, HC-Smooth Filter
 
	MarkGX, Jan 2011
*/
public class SmoothFilter : MonoBehaviour 
{
	/*
		Standard Laplacian Smooth Filter
	*/
	public static Vector3[] laplacianFilter(Vector3[] sv, int[] t)
	{
		Vector3[] wv = new Vector3[sv.Length];
		List<Vector3> adjacentVertices = new List<Vector3>();
		Vector3 dv;

		for (int vi=0; vi< sv.Length; vi++)
		{
			// Find the sv neighboring vertices
			adjacentVertices = MeshUtils.findAdjacentNeighbors (sv, t, sv[vi]);

			if (adjacentVertices.Count != 0)
			{
				dv = Vector3.zero;
				//Debug.Log("Vertex Index Length = "+vertexIndexes.Length);
				// Add the vertices and divide by the number of vertices
				for (int j=0; j<adjacentVertices.Count; j++)
				{
					dv = dv + adjacentVertices [j];
				}
				wv [vi] = dv / adjacentVertices.Count;
			}
		}

		return wv;
	}

	/*
		HC (Humphrey’s Classes) Smooth Algorithm - Reduces Shrinkage of Laplacian Smoother
 
		Where sv - original points
				pv - previous points,
				alpha [0..1] influences previous points pv, e.g. 0
				beta  [0..1] e.g. > 0.5
	*/
	public static Vector3[] hcFilter(Vector3[] sv, Vector3[] pv, int[] t, float alpha, float beta)
	{
		Vector3[] wv;
		Vector3[] bv = new Vector3[sv.Length];



		// Perform Laplacian Smooth
		wv = laplacianFilter(sv, t);

		// Compute Differences
		for(int i=0; i<wv.Length; i++)
		{
			bv [i] = wv [i] - (alpha * sv [i] + (1 - alpha) * sv [i]);
		}

		List<int> adjacentIndexes = new List<int>();

		Vector3 dv;
		for(int j=0; j<bv.Length; j++)
		{
			adjacentIndexes.Clear();

			// Find the bv neighboring vertices
			adjacentIndexes = MeshUtils.findAdjacentNeighborIndexes (sv, t, sv[j]);

			dv = Vector3.zero;
			for (int k=0; k<adjacentIndexes.Count; k++)
			{
				dv = dv + bv [adjacentIndexes [k]];

			}

			wv [j] = wv [j] - beta * bv [j] + ((1 - beta) / adjacentIndexes.Count) * dv;
		}

		return wv;
	}
}