using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MarchingCube : MonoBehaviour {
	public int[,,] scalarField;
	public Gridcell[,,] grid;
	public int iso;

	private Vector3[][] gridLines;
	private int[,] offsets = {
		{0,0,0 },//0
		{1,0,0 },//1
		{1,1,0 },//2
		{0,1,0 },//3
		{0,0,1 },//4
		{1,0,1 },//5
		{1,1,1 },//6
		{0,1,1 } //7
	};
	private int[,] tet = new int[,]{
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

	static bool Vector3LessThan(Vector3 left, Vector3 right){
		if (left.x < right.x) {
			return true;
		} else if (left.x > right.x) {
			return false;
		}
		if (left.y < right.y) {
			return true;
		} else if (left.y > right.y) {
			return false;
		}
		if (left.z < right.z) {
			return true;
		} else if (left.z > right.z) {
			return false;
		}
		return false;
	}

	Vector3 InterpolateVertex(float iso, Vector3 p1, Vector3 p2, float valp1, float valp2){
        return Vector3.Lerp(p1, p2, Mathf.Abs((valp1 - valp2) / (2 * (valp1 + valp2)))); 
    }

	Mesh PoligoniseTri(Gridcell g, float iso, int[,] tet, int tetIndex){
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
                //needs more testing.
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
                //needs more testing.
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
                //needs more testing.
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
        //initDrawGrid();
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
	void initDrawGrid(){
        int nx = this.grid.GetLength(0);
        int ny = this.grid.GetLength(1);
        int nz = this.grid.GetLength(2);
		List<Vector3[]> grid = new List<Vector3[]> ();
		for (int z = 0; z < nz; z++){
			for (int y = 0; y < ny; y++) {
				grid.Add (new Vector3[]{ 
                    this.grid[0,y,z].p[0],
                    this.grid[nx-1,y,z].p[1]
				});
            }
            for (int x = 0; x < nx; x++) {
				grid.Add (new Vector3[]{
                    this.grid[x,0,z].p[0],
                    this.grid[x,ny-1,z].p[3]
				});
			}

            grid.Add(new Vector3[] {
                    this.grid[0, ny-1, z].p[3],
                    this.grid[nx-1, ny-1, z].p[2]
                });
            grid.Add(new Vector3[] {
                    this.grid[nx-1, 0, z].p[1],
                    this.grid[nx-1, ny-1, z].p[2]
                });
        }
        for (int x = 0; x < nx; x++) {
			for (int y = 0; y < ny; y++) {
				grid.Add (new Vector3[]{
                    this.grid[x,y,0].p[0],
                    this.grid[x,y,nz-1].p[4]
				});
			}
            grid.Add(new Vector3[]{
                    this.grid[x,0,nz-1].p[4],
                    this.grid[x,ny-1,nz-1].p[7]
                });
            grid.Add(new Vector3[]{
                    this.grid[x,ny-1,0].p[3],
                    this.grid[x,ny-1,nz-1].p[7]
                });
        }
        for (int y = 0; y < ny; y++)
        {
            grid.Add(new Vector3[] {
                    this.grid[0, y, nz-1].p[4],
                    this.grid[nx-1, y, nz-1].p[5]
                });
            grid.Add(new Vector3[] {
                    this.grid[nx-1, y, 0].p[1],
                    this.grid[nx-1, y, nz-1].p[5]
                });
        }
        grid.Add(new Vector3[] {
                    this.grid[0, ny-1, nz-1].p[7],
                    this.grid[nx-1, ny-1, nz-1].p[6]
                });
        grid.Add(new Vector3[] {
                    this.grid[nx-1, 0, nz-1].p[5],
                    this.grid[nx-1, ny-1, nz-1].p[6]
                });
        grid.Add(new Vector3[] {
                    this.grid[nx-1, ny-1, 0].p[2],
                    this.grid[nx-1, ny-1, nz-1].p[6]
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

    Gridcell[,,] makeGrid(int[,,] c) {
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
    void Update(){
		DrawGrid ();
    }
    void testGrid() {
		test1();
    }
    void Start(){
		scalarField = new int [,,] {
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,1,0,0,0,0,1,1,1,0,0,0,1,1,1,0,0,0,0,1,1,0,0,0,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,1,0,1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1,0,1,1,0,0,1,0,0,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,1,1,0,0,0,0,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,0,0,1,0,1,1,0,0,0,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			},
		};
        scalarField = LoadData();
		while (scalarField.Length > 12000)//8125
        {
			Debug.Log (scalarField.Length);
			scalarField = ReduceData(scalarField);
        }
		grid = makeGrid (scalarField);
		initDrawGrid ();
        test1();
	}
	public Mesh CombineMeshArray(Mesh[] MeshArray){
        CombineInstance[] combine = new CombineInstance[MeshArray.Length];
        Mesh result = new Mesh();
		Matrix4x4 worldTransform = this.transform.localToWorldMatrix;
        for (int i = 0; i < MeshArray.Length; i++)
        {
            if (MeshArray[i] != null)
            {
                combine[i].mesh = MeshArray[i];
				combine[i].transform = worldTransform;
            }
        }

		result.CombineMeshes(combine, true);

        return result;
	}
    /**    7-----6   
     *    /|    /|
     *   / |   / |
     *  3--+--2  |
     *  |  |  |  |
     *  |  |  |  |
     *  |  4--+--5
     *  | /   | /
     *  |/    |/
     *  0-----1 
     * 
     * */

    public void test1(){
		updateMesh();
	}
	void updateMesh(){
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
						if (currentMesh != null)
						{
							MeshList.Add(currentMesh);
						}
					}
				}
			}
		}
		MeshFilter m = GetComponent<MeshFilter>();
		m.mesh = CombineMeshArray(MeshList.ToArray());
		MeshUtils.Weld (m.mesh);
	}
}
