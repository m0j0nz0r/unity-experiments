using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/* *    
 *      7-----6   
 *     /|    /|
 *    / |   / |
 *   3--+--2  |
 *   |  |  |  |
 *   |  |  |  |
 *   |  4--+--5
 *   | /   | /
 *   |/    |/
 *   0-----1 
 * 
 * */

public class MarchingCube : MonoBehaviour {
	public int 	iso;
	public int chunkSize;
	private Vector3[][] gridLines;
	private static int[,] offsets = {
		{0,0,0 },//0
		{1,0,0 },//1
		{1,1,0 },//2
		{0,1,0 },//3
		{0,0,1 },//4
		{1,0,1 },//5
		{1,1,1 },//6
		{0,1,1 } //7
	};
	private static int[,] tet = new int[,]{
		{0,1,2,6},//0
		{0,2,3,6},//1
		{0,3,7,6},//2
		{0,7,4,6},//3
		{0,4,5,6},//4
		{0,5,1,6} //5
	};

	public struct Gridcell{
		public Vector3[] p;
		public int[] val;
	}

	static Vector3 InterpolateVertex(float iso, Vector3 p1, Vector3 p2, float valp1, float valp2){
        return Vector3.Lerp(p1, p2, ((valp1 - valp2) / (valp1 + valp2)) + .5f); 
    }

	static Mesh PoligoniseTri(Gridcell g, float iso, int[,] tet, int tetIndex){
		int triIndex = 0;
		Mesh mesh = new Mesh ();
		Vector3 pv0 = g.p [tet[tetIndex, 0]];
		Vector3 pv1 = g.p [tet[tetIndex, 1]];
		Vector3 pv2 = g.p [tet[tetIndex, 2]];
		Vector3 pv3 = g.p [tet[tetIndex, 3]];
		int vv0 = g.val [tet[tetIndex, 0]];
		int vv1 = g.val [tet[tetIndex, 1]];
		int vv2 = g.val [tet[tetIndex, 2]];
		int vv3 = g.val [tet[tetIndex, 3]];
		if (vv0 < iso)
			triIndex |= 1;
		if (vv1 < iso)
			triIndex |= 2;
		if (vv2 < iso)
			triIndex |= 4;
		if (vv3 < iso)
			triIndex |= 8;
        bool invert = triIndex < 8;
		switch (triIndex) {
		    case 0x00:
            case 0x0F:
			    return null;
            case 0x0E:
            case 0x01:
                mesh.vertices = new Vector3[] {
                    InterpolateVertex (iso, pv0, pv1, vv0, vv1),
                    InterpolateVertex (iso, pv0, pv2, vv0, vv2),
                    InterpolateVertex (iso, pv0, pv3, vv0, vv3)
                };

                mesh.triangles = invert ? new int[] { 2, 1, 0 } : new int[]{ 0, 1, 2};

                mesh.uv = new Vector2[] {
                    new Vector2(0,0),
                    new Vector2(0,1),
                    new Vector2(1,0)
                };
                break;
		    case 0x0D:
		    case 0x02:
                mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv1, pv2, vv1, vv2),
				    InterpolateVertex (iso, pv1, pv0, vv1, vv0),
				    InterpolateVertex (iso, pv1, pv3, vv1, vv3)
			    };
			    mesh.triangles = invert ? new int[] { 2, 1, 0 } : new int[]{ 0, 1, 2 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0),
			    };
			    break;
		    case 0x0C:
		    case 0x03:
                mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv3, pv1, vv3, vv1),
				    InterpolateVertex (iso, pv0, pv3, vv0, vv3),
				    InterpolateVertex (iso, pv0, pv2, vv0, vv2),
				    InterpolateVertex (iso, pv2, pv1, vv2, vv1)
			    };
			    mesh.triangles = invert ? new int[] { 0, 1, 3, 2, 3, 1 } : new int[]{ 1, 3, 2, 3, 1, 0 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0),
				    new Vector2 (1, 1)
			    };
                break;
		    case 0x0B:
		    case 0x04:
			    mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv2, pv0, vv2, vv0),
				    InterpolateVertex (iso, pv2, pv3, vv2, vv3),
				    InterpolateVertex (iso, pv2, pv1, vv2, vv1)
			    };
			    mesh.triangles = invert ? new int[] { 0, 1, 2 } : new int[]{ 2, 1, 0 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0),
			    };
			    break;
		    case 0xA:
		    case 0x5:
                mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv0, pv3, vv0, vv3),
				    InterpolateVertex (iso, pv2, pv3, vv2, vv3),
				    InterpolateVertex (iso, pv1, pv2, vv1, vv2),
				    InterpolateVertex (iso, pv0, pv1, vv0, vv1)
			    };
			    mesh.triangles = invert ? new int[] { 0, 1, 3, 3, 1, 2 } : new int[] { 2, 1, 3, 3, 1, 0 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0),
				    new Vector2 (1, 1)
			    };
			    break;
		    case 0x9:
		    case 0x6:
                mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv0, pv1, vv0, vv1),
				    InterpolateVertex (iso, pv0, pv2, vv0, vv2),
				    InterpolateVertex (iso, pv2, pv3, vv2, vv3),
				    InterpolateVertex (iso, pv3, pv1, vv3, vv1)
			    };
			    mesh.triangles = invert ? new int[] { 0, 1, 2, 2, 3, 0 } : new int[] { 0, 3, 2, 2, 1, 0 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0),
				    new Vector2 (1, 1)
			    };
			    break;
		    case 0x8:
		    case 0x7:
			    mesh.vertices = new Vector3[] {
				    InterpolateVertex (iso, pv1, pv3, vv1, vv3),
				    InterpolateVertex (iso, pv2, pv3, vv2, vv3),
				    InterpolateVertex (iso, pv0, pv3, vv0, vv3)
			    };
			    mesh.triangles = invert ? new int[] { 2, 1, 0 } : new int[]{ 0, 1, 2 };
			    mesh.uv = new Vector2[] {
				    new Vector2 (0, 0),
				    new Vector2 (0, 1),
				    new Vector2 (1, 0)
			    };
			    break;
        }
		return mesh;
	}

	void DrawGrid(){
        Color c;
		foreach (Vector3[] row in gridLines) {
            /**
             * x green
             * y blue
             * z red
             * */
            if (row[0].x != row[1].x && row[0].y == row[1].y && row[0].z == row[1].z) c = Color.green;
            else if (row[0].x == row[1].x && row[0].y != row[1].y && row[0].z == row[1].z) c = Color.blue;
            else if (row[0].x == row[1].x && row[0].y == row[1].y && row[0].z != row[1].z) c = Color.red;
            else c = Color.yellow;
			Debug.DrawLine (row[0], row[1], c);
            
		}
	}
	void initDrawGrid(Gridcell[,,] inputGrid){
		int nx = inputGrid.GetLength(0);
		int ny = inputGrid.GetLength(1);
		int nz = inputGrid.GetLength(2);
		List<Vector3[]> grid = new List<Vector3[]> ();
		for (int z = 0; z < nz; z++){
			for (int y = 0; y < ny; y++) {
				grid.Add (new Vector3[]{ 
					inputGrid[0,y,z].p[0],
					inputGrid[nx-1,y,z].p[1]
				});
            }
            for (int x = 0; x < nx; x++) {
				grid.Add (new Vector3[]{
					inputGrid[x,0,z].p[0],
					inputGrid[x,ny-1,z].p[3]
				});
			}

            grid.Add(new Vector3[] {
				inputGrid[0, ny-1, z].p[3],
				inputGrid[nx-1, ny-1, z].p[2]
            });
            grid.Add(new Vector3[] {
				inputGrid[nx-1, 0, z].p[1],
				inputGrid[nx-1, ny-1, z].p[2]
			});
        }
        for (int x = 0; x < nx; x++) {
			for (int y = 0; y < ny; y++) {
				grid.Add (new Vector3[]{
					inputGrid[x,y,0].p[0],
					inputGrid[x,y,nz-1].p[4]
				});
			}
            grid.Add(new Vector3[]{
				inputGrid[x,0,nz-1].p[4],
				inputGrid[x,ny-1,nz-1].p[7]
			});
            grid.Add(new Vector3[]{
				inputGrid[x,ny-1,0].p[3],
				inputGrid[x,ny-1,nz-1].p[7]
			});
        }
        for (int y = 0; y < ny; y++)
        {
            grid.Add(new Vector3[] {
				inputGrid[0, y, nz-1].p[4],
				inputGrid[nx-1, y, nz-1].p[5]
			});
            grid.Add(new Vector3[] {
				inputGrid[nx-1, y, 0].p[1],
				inputGrid[nx-1, y, nz-1].p[5]
			});
        }
        grid.Add(new Vector3[] {
			inputGrid[0, ny-1, nz-1].p[7],
			inputGrid[nx-1, ny-1, nz-1].p[6]
		});
        grid.Add(new Vector3[] {
			inputGrid[nx-1, 0, nz-1].p[5],
			inputGrid[nx-1, ny-1, nz-1].p[6]
		});
        grid.Add(new Vector3[] {
			inputGrid[nx-1, ny-1, 0].p[2],
			inputGrid[nx-1, ny-1, nz-1].p[6]
		});
        gridLines = grid.ToArray ();
	}
	int[,,] LoadData() {
        int nx = 200;
        int ny = 160;
        int nz = 160;
        TextAsset file = Resources.Load("mri") as TextAsset;
        Stream stream = new MemoryStream(file.bytes);
        int[,,] byteArray = new int[nx, ny, nz];
        int thisByte;
        for (int z = 0; z < nz; z++)
        {
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    thisByte = stream.ReadByte();
                    if (thisByte != -1)
                    {
                        byteArray[x, y, z] = thisByte;
                    }
                }
            }
        }
		return byteArray;
    }

    public static Gridcell[,,] makeGrid(int[,,] c) {
        int thisByte;
        int nx = c.GetLength(0);
        int ny = c.GetLength(1);
        int nz = c.GetLength(2);
        Gridcell[,,] result = new Gridcell[nx, ny, nz];
        Gridcell thisCell;
        for (int z = 0; z < nz; z++)
        {
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    thisByte = c[x,y,z];
                    if (thisByte != -1)
                    {
                        thisCell = new Gridcell();
                        thisCell.val = new int[8];
                        thisCell.p = new Vector3[8];
                        result[x, y, z] = thisCell;
                        for (int i = 0; i < 8; i++)
                        {
                            thisCell.p[i] = new Vector3(x + offsets[i, 0], y + offsets[i, 1], z + offsets[i, 2]);
                        }
                    }
                }
            }
        }
        for (int z = 0; z < nz - 1; z++)
        {
            for (int y = 0; y < ny - 1; y++)
            {
                for (int x = 0; x < nx - 1; x++)
                {
                    thisCell = result[x, y, z];
                    for (int i = 0; i < 8; i++)
                    {
                        thisCell.val[i] = c[x + offsets[i, 0], y + offsets[i, 1], z + offsets[i, 2]];
                    }
                }
            }
        }
        return result;

    }
    int getAverage(int[] a) {
        int avg = 0;
        foreach (int n in a) avg += n;
        return Mathf.RoundToInt(avg / a.Length);
    }

	int[,,] ReduceData(int [,,] g){
		int x, y, z, nx = g.GetLength(0), ny = g.GetLength(1), nz = g.GetLength(2), acum, i, currentX, currentY, currentZ;
		int[,,] targetGrid = new int[Mathf.CeilToInt (nx / 2) + 1, Mathf.CeilToInt (ny / 2) + 1, Mathf.CeilToInt (nz / 2) + 1];
		for (x = 1; x <= nx; x += 2) {
			for (y = 1; y <= ny; y += 2) {
				for (z = 1; z <= nz; z += 2) {
					acum = 0;
					for (i = 0; i < 8; i++) {
						currentX = x - offsets [i, 0];
						currentY = y - offsets [i, 1];
						currentZ = z - offsets [i, 2];
						if (currentX < nx && currentY < ny && currentZ < nz) {
							acum += g [currentX, currentY, currentZ];
						}
					}
					targetGrid [Mathf.FloorToInt (x / 2), Mathf.FloorToInt (y / 2), Mathf.FloorToInt (z / 2)] = Mathf.RoundToInt (acum / 8);
				}
			}
		}
		return targetGrid;
	}
    void Start(){
		int [,,] scalarField = new int [,,] {
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,1,1,1,0,0,0,1,1,0,0,0,0,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,0,0,0,0,1,1,0,0},
				{ 0,0,0,0,0,1,0,1,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,1,0,0,1,0},
				{ 0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,1,1,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,1,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,0,1,1,0,1,0,0,0,1,0,0,1,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,1,1,1,0,0,1,1,0,0,0,0,1,1,1,0,0,0,1,1,0,1,0,0,1,1,1,0,0,0,1,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
		};
		if (chunkSize <= 0)
			chunkSize = 16;
		scalarField = ReduceData(LoadData());
		//subdivide scalarField in chunkSize^3 chunks
		chunks = getChunks(scalarField, chunkSize);
		//create 1 child object per chunk with a mesh renderer
		//makeChildren(chunks);
		//make and assign meshes to each chunk.
		//grid = makeGrid (scalarField);
		//initDrawGrid ();
		//updateMesh();
	}
	private int[,,][,,] chunks;
	private int currentChunkX = 0;
	private int currentChunkY = 0;
	private int currentChunkZ = 0;

	private void makeChildren (int [,,][,,] chunks) {
		int x,y,z;
		int xMax = chunks.GetLength (0);
		int yMax = chunks.GetLength (1);
		int zMax = chunks.GetLength (2);
		GameObject child;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		Mesh mesh;
		for (x = 0; x < xMax; x++) {
			for (y = 0; y < yMax; y++) {
				for (z = 0; z < zMax; z++) { //160
					child = new GameObject ();
					child.transform.parent = transform;
					child.transform.position = transform.position + new Vector3 (x, y, z) * chunkSize;
					meshRenderer = child.AddComponent<MeshRenderer> ();
					meshRenderer.materials = GetComponent<MeshRenderer> ().materials;
					meshFilter = child.AddComponent<MeshFilter> ();
					mesh = makeMesh (makeGrid (chunks[x,y,z]), iso, transform.localToWorldMatrix);
					MeshUtils.Weld (mesh);
					meshFilter.mesh = mesh;
				}
			}
		}
	}
	private bool doneLoading = false;
	void Update(){
		if (!doneLoading) {
			int xMax = chunks.GetLength (0);
			int yMax = chunks.GetLength (1);
			int zMax = chunks.GetLength (2);
			GameObject child;
			MeshRenderer meshRenderer;
			MeshFilter meshFilter;
			Mesh mesh;
			child = new GameObject ();
			child.transform.parent = transform;
			child.transform.position = transform.position + new Vector3 (currentChunkX, currentChunkY, currentChunkZ) * chunkSize;
			meshRenderer = child.AddComponent<MeshRenderer> ();
			meshRenderer.materials = GetComponent<MeshRenderer> ().materials;
			meshFilter = child.AddComponent<MeshFilter> ();
			mesh = makeMesh (makeGrid (chunks [currentChunkX, currentChunkY, currentChunkZ]), iso, transform.localToWorldMatrix);
			MeshUtils.Weld (mesh);
			meshFilter.mesh = mesh;
			if (currentChunkZ < zMax - 1) {
				currentChunkZ++;
			} else {
				currentChunkZ = 0;
				if (currentChunkY < yMax - 1) {
					currentChunkY++;
				} else {
					currentChunkY = 0;
					if (currentChunkX < xMax - 1) {
						currentChunkX++;
					} else {
						doneLoading = true;
						Debug.Log ("Done");
					}
				}
			}
		}
	}

	private int [,,][,,] getChunks(int [,,] array, int chunkSize) {
		int arrayX = array.GetLength (0);
		int arrayY = array.GetLength (1);
		int arrayZ = array.GetLength (2);
		int chunkMaxX = Mathf.CeilToInt ((float)arrayX / chunkSize);
		int chunkMaxY = Mathf.CeilToInt ((float)arrayY / chunkSize);
		int chunkMaxZ = Mathf.CeilToInt ((float)arrayZ / chunkSize);
		int[,,][,,] returnValue = new int[chunkMaxX, chunkMaxY, chunkMaxZ][,,];
		int x, y, z, chunkX, chunkY, chunkZ, arrX, arrY, arrZ;
		bool isOutOfBounds;
		for (chunkX = 0; chunkX < chunkMaxX; chunkX++) {
			for (chunkY = 0; chunkY < chunkMaxY; chunkY++) {
				for (chunkZ = 0; chunkZ < chunkMaxZ; chunkZ++) {
					returnValue [chunkX, chunkY, chunkZ] = new int[chunkSize, chunkSize, chunkSize];
					for (x = 0; x < chunkSize; x++) {
						for (y = 0; y < chunkSize; y++) {
							for (z = 0; z < chunkSize; z++) {
								arrX = chunkX * chunkSize + x;
								arrY = chunkY * chunkSize + y;
								arrZ = chunkZ * chunkSize + z;
								isOutOfBounds = arrX >= arrayX || arrY >= arrayY || arrZ >= arrayZ;
								returnValue [chunkX, chunkY, chunkZ] [x, y, z] = isOutOfBounds ? 0 : array [arrX, arrY, arrZ];
							}
						}
					}
				}
			}
		}
		return returnValue;
	}
	static public Mesh CombineMeshArray(Mesh[] MeshArray, Matrix4x4 worldTransform) {
        CombineInstance[] combine = new CombineInstance[MeshArray.Length];
        Mesh result = new Mesh();
        for (int i = 0; i < MeshArray.Length; i++) {
            if (MeshArray[i] != null) {
                combine[i].mesh = MeshArray[i];
				combine[i].transform = worldTransform;
            }
        }

		result.CombineMeshes(combine, true);

        return result;
	}
	public static Mesh makeMesh(Gridcell[,,] grid, int iso, Matrix4x4 worldTransform) {
		int nx = grid.GetLength(0);
		int ny = grid.GetLength(1);
		int nz = grid.GetLength(2);
		List<Mesh> MeshList;
		Gridcell cell;
		MeshList = new List<Mesh>();
		Mesh currentMesh;
		for (int z = 0; z < nz; z++) {
			for (int y = 0; y < ny; y++) {
				for (int x = 0; x < nx; x++) {
					cell = grid [x, y, z];
					for (int i = 0; i < tet.GetLength(0); i++) {
						currentMesh = PoligoniseTri(cell, iso, tet, i);
						if (currentMesh != null) {
							MeshList.Add(currentMesh);
						}
					}
				}
			}
		}
		return CombineMeshArray (MeshList.ToArray (), worldTransform);
	}

	void updateMesh(int [,,] scalarField) {
		Gridcell[,,] grid = makeGrid (scalarField);
		MeshFilter m = GetComponent<MeshFilter>();
		try {
			//if the mesh is too big to be combined/welded, reduce it and try again.
			m.mesh = makeMesh(grid, iso, this.transform.localToWorldMatrix);
			MeshUtils.Weld (m.mesh);
		}
		catch(System.Exception e) {
			updateMesh (ReduceData (scalarField));
		}
	}
}
