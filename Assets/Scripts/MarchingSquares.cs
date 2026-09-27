using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarchingSquares
{
    public static Mesh GenerateMesh(float[,] voxels) {
        List<Vector3> v = new List<Vector3>();
        List<int> t = new List<int>();

        int w = voxels.GetLength(0);
        int h = voxels.GetLength(1);

        int[,][] i = new int[h, w][];

        for (int y = 1; y < h; y++) {
            for (int x = 1; x < w; x++) {
                int sqNum = ((voxels[y, x] > 0) ? 1 : 0) + ((voxels[y, x - 1] > 0) ? 2 : 0) + ((voxels[y - 1, x] > 0) ? 4 : 0) + ((voxels[y - 1, x - 1] > 0) ? 8 : 0);

                // Debug.Log(sqNum);

                if (sqNum == 15)
                    continue;
                
                i[y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, sqNum};

                int[] verts = new int[8];

                if (SquareData.squareVerticies[sqNum][0]) 
                    verts[0] = v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + 0 - 1));
                
                if (SquareData.squareVerticies[sqNum][1]) 
                    verts[1] = (i[y, x - 1] != null) ? i[y, x - 1][0] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + 0 - 1));
                
                if (SquareData.squareVerticies[sqNum][2]) 
                    verts[2] = (i[y - 1, x] != null) ? i[y - 1, x][0] : (x + 1 < w && i[y - 1, x + 1] != null) ? i[y - 1, x + 1][1] : v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + 1 - 1));
                
                if (SquareData.squareVerticies[sqNum][3]) 
                    verts[3] = (i[y - 1, x] != null) ? i[y - 1, x][1] : (i[y - 1, x - 1] != null) ? i[y - 1, x - 1][0] : (i[y, x - 1] != null) ? i[y, x - 1][2] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + 1 - 1));



                if (SquareData.squareVerticies[sqNum][4]) 
                    verts[4] = v.AddReturnIndex<Vector3>(new Vector2(x - lint(voxels[y, x], voxels[y, x - 1]), h -y + 0 - 1));                    
                
                if (SquareData.squareVerticies[sqNum][5]) 
                    verts[5] = (i[y, x - 1] != null) ? i[y, x - 1][6] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + lint(voxels[y, x - 1], voxels[y - 1, x - 1]) - 1));
                
                if (SquareData.squareVerticies[sqNum][6]) 
                    verts[6] = v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + lint(voxels[y, x], voxels[y - 1, x]) - 1));
                
                if (SquareData.squareVerticies[sqNum][7]) 
                    verts[7] = (i[y - 1, x] != null) ? i[y - 1, x][4] : v.AddReturnIndex<Vector3>(new Vector2(x - lint(voxels[y - 1, x], voxels[y - 1, x - 1]), h -y + 1 - 1));

                for (int o = 0; o < SquareData.squareTriangles[sqNum].Length; o++) {
                    t.Add(verts[SquareData.squareTriangles[sqNum][o]]);
                    i[y, x][SquareData.squareTriangles[sqNum][o]] = verts[SquareData.squareTriangles[sqNum][o]];
                }
            }
        }

        bool[,] gen = new bool[h, w];

        for (int y = 1; y < h; y++) {
            for (int x = 1; x < w; x++) {
                if (i[y, x] != null || gen[y, x])
                    continue;
                
                List<int> vertical = new List<int>();
                
                Vector2Int size = new Vector2Int();

                for (int xx = x + 1; xx < w; xx++) {
                    if (i[y, xx] != null || gen[y, xx]) break;
                    size.x++;
                }

                for (int yy = y + 1; yy < h; yy++) {
                    bool b = false;

                    for (int xx = x; xx <= x + size.x; xx++) {
                        if (i[yy, xx] != null || gen[yy, xx]) {
                            b = true;
                            break;
                        }
                    }

                    if (b) break;

                    size.y++;
                }

                for (int yy = 0; yy < size.y + 1; yy++) {
                    for (int xx = 0; xx < size.x + 1; xx++) {
                        gen[y + yy, x + xx] = true;
                    }
                }

                int[] verts = new int[4];

                verts[0] = (x + size.x + 1 < w && i[y + size.y, x + size.x + 1] != null && i[y + size.y, x + size.x + 1][1] != -1) ? i[y + size.y, x + size.x + 1][1] :
                    (y + size.y + 1 < h && i[y + size.y + 1, x + size.x] != null && i[y + size.y + 1, x + size.x][2] != -1) ? i[y + size.y + 1, x + size.x][2] :
                    (y + size.y + 1 < h && x + size.x + 1 < w && i[y + size.y + 1, x + size.x + 1] != null && i[y + size.y + 1, x + size.x + 1][3] != -1) ? i[y + size.y + 1, x + size.x + 1][3] :
                    v.AddReturnIndex<Vector3>(new Vector2(x + size.x, h - (y + size.y) - 1));

                if (verts[0] == v.Count - 1) {
                    i[y + size.y, x + size.x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y + size.y, x + size.x][0] = verts[0];
                }

                verts[1] = (i[y + size.y, x - 1] != null && i[y + size.y, x - 1][0] != -1) ? i[y + size.y, x - 1][0] : 
                    (y + size.y + 1 < h && i[y + size.y + 1, x] != null && i[y + size.y + 1, x][3] != -1) ? i[y + size.y + 1, x][3] : 
                    (y + size.y + 1 < h && i[y + size.y + 1, x - 1] != null && i[y + size.y + 1, x - 1][2] != -1) ? i[y + size.y + 1, x - 1][2] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x - 1, h - (y + size.y) - 1));

                if (verts[1] == v.Count - 1) {
                    i[y + size.y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y + size.y, x][1] = verts[1];
                }

                verts[2] = (i[y - 1, x + size.x] != null && i[y - 1, x + size.x][0] != -1) ? i[y - 1, x + size.x][0] : 
                    (x + size.x + 1 < w && i[y, x + size.x + 1] != null && i[y, x + size.x + 1][3] != -1) ? i[y, x + size.x + 1][3] : 
                    (x + size.x + 1 < w && i[y - 1, x + size.x + 1] != null && i[y - 1, x + size.x + 1][1] != -1) ? i[y - 1, x + size.x + 1][1] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x + size.x, h - (y - 1) - 1));

                if (verts[2] == v.Count - 1) {
                    i[y, x + size.x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y, x + size.x][2] = verts[2];
                }

                verts[3] = (i[y - 1, x - 1] != null && i[y - 1, x - 1][0] != -1) ? i[y - 1, x - 1][0] : 
                    (i[y, x - 1] != null && i[y, x - 1][2] != -1) ? i[y, x - 1][2] : 
                    (i[y - 1, x] != null && i[y - 1, x][1] != -1) ? i[y - 1, x][1] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x - 1, h - (y - 1) - 1));

                if (verts[3] == v.Count - 1) {
                    i[y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y, x][3] = verts[3];
                }

                t.AddRange(new int[] {verts[0], verts[1], verts[3], verts[0], verts[3], verts[2]});
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = v.ToArray();
        mesh.triangles = t.ToArray();

        return mesh;
    }

    public static SquareConstruct GenerateSquareContruct(float[,] voxels) {
        SquareConstruct construct = new SquareConstruct();

        List<Vector3> v = new List<Vector3>();
        List<int> t = new List<int>();

        Dictionary<int, int> e = new Dictionary<int, int>();

        int w = voxels.GetLength(0);
        int h = voxels.GetLength(1);

        int[,][] i = new int[h, w][];

        for (int y = 1; y < h; y++) {
            for (int x = 1; x < w; x++) {
                int sqNum = ((voxels[y, x] > 0) ? 1 : 0) + ((voxels[y, x - 1] > 0) ? 2 : 0) + ((voxels[y - 1, x] > 0) ? 4 : 0) + ((voxels[y - 1, x - 1] > 0) ? 8 : 0);

                // Debug.Log(sqNum);

                if (sqNum == 15)
                    continue;
                
                i[y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, sqNum};

                int[] verts = new int[8];

                if (SquareData.squareVerticies[sqNum][0]) 
                    verts[0] = v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + 0 - 1));
                
                if (SquareData.squareVerticies[sqNum][1]) 
                    verts[1] = (i[y, x - 1] != null) ? i[y, x - 1][0] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + 0 - 1));
                
                if (SquareData.squareVerticies[sqNum][2]) 
                    verts[2] = (i[y - 1, x] != null) ? i[y - 1, x][0] : (x + 1 < w && i[y - 1, x + 1] != null) ? i[y - 1, x + 1][1] : v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + 1 - 1));
                
                if (SquareData.squareVerticies[sqNum][3]) 
                    verts[3] = (i[y - 1, x] != null) ? i[y - 1, x][1] : (i[y - 1, x - 1] != null) ? i[y - 1, x - 1][0] : (i[y, x - 1] != null) ? i[y, x - 1][2] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + 1 - 1));



                if (SquareData.squareVerticies[sqNum][4]) 
                    verts[4] = v.AddReturnIndex<Vector3>(new Vector2(x - lint(voxels[y, x], voxels[y, x - 1]), h -y + 0 - 1));                    
                
                if (SquareData.squareVerticies[sqNum][5]) 
                    verts[5] = (i[y, x - 1] != null) ? i[y, x - 1][6] : v.AddReturnIndex<Vector3>(new Vector2(x - 1, h -y + lint(voxels[y, x - 1], voxels[y - 1, x - 1]) - 1));
                
                if (SquareData.squareVerticies[sqNum][6]) 
                    verts[6] = v.AddReturnIndex<Vector3>(new Vector2(x - 0, h -y + lint(voxels[y, x], voxels[y - 1, x]) - 1));
                
                if (SquareData.squareVerticies[sqNum][7]) 
                    verts[7] = (i[y - 1, x] != null) ? i[y - 1, x][4] : v.AddReturnIndex<Vector3>(new Vector2(x - lint(voxels[y - 1, x], voxels[y - 1, x - 1]), h -y + 1 - 1));

                for (int o = 0; o < SquareData.squareTriangles[sqNum].Length; o++) {
                    t.Add(verts[SquareData.squareTriangles[sqNum][o]]);
                    i[y, x][SquareData.squareTriangles[sqNum][o]] = verts[SquareData.squareTriangles[sqNum][o]];
                }

                for (int o = 0; o < SquareData.squareEdges[sqNum].Length; o += 2) {
                    e.Add(verts[SquareData.squareEdges[sqNum][o]], verts[SquareData.squareEdges[sqNum][o + 1]]);
                }

                if (y == 1 && SquareData.squareFullEdges[sqNum, 3].Length != 0) e.Add(verts[SquareData.squareFullEdges[sqNum, 3][0]], verts[SquareData.squareFullEdges[sqNum, 3][1]]);
                if (x == 1 && SquareData.squareFullEdges[sqNum, 1].Length != 0) e.Add(verts[SquareData.squareFullEdges[sqNum, 1][0]], verts[SquareData.squareFullEdges[sqNum, 1][1]]);
                if (y == (h - 1) && SquareData.squareFullEdges[sqNum, 0].Length != 0) e.Add(verts[SquareData.squareFullEdges[sqNum, 0][0]], verts[SquareData.squareFullEdges[sqNum, 0][1]]);
                if (x == (w - 1) && SquareData.squareFullEdges[sqNum, 2].Length != 0) e.Add(verts[SquareData.squareFullEdges[sqNum, 2][0]], verts[SquareData.squareFullEdges[sqNum, 2][1]]);
            }
        }

        bool[,] gen = new bool[h, w];

        for (int y = 1; y < h; y++) {
            for (int x = 1; x < w; x++) {
                if (i[y, x] != null || gen[y, x])
                    continue;
                
                List<int> vertical = new List<int>();
                
                Vector2Int size = new Vector2Int();

                for (int xx = x + 1; xx < w; xx++) {
                    if (i[y, xx] != null || gen[y, xx]) break;
                    size.x++;
                }

                for (int yy = y + 1; yy < h; yy++) {
                    bool b = false;

                    for (int xx = x; xx <= x + size.x; xx++) {
                        if (i[yy, xx] != null || gen[yy, xx]) {
                            b = true;
                            break;
                        }
                    }

                    if (b) break;

                    size.y++;
                }

                for (int yy = 0; yy < size.y + 1; yy++) {
                    for (int xx = 0; xx < size.x + 1; xx++) {
                        gen[y + yy, x + xx] = true;
                    }
                }

                int[] verts = new int[4];

                verts[0] = (x + size.x + 1 < w && i[y + size.y, x + size.x + 1] != null && i[y + size.y, x + size.x + 1][1] != -1) ? i[y + size.y, x + size.x + 1][1] :
                    (y + size.y + 1 < h && i[y + size.y + 1, x + size.x] != null && i[y + size.y + 1, x + size.x][2] != -1) ? i[y + size.y + 1, x + size.x][2] :
                    (y + size.y + 1 < h && x + size.x + 1 < w && i[y + size.y + 1, x + size.x + 1] != null && i[y + size.y + 1, x + size.x + 1][3] != -1) ? i[y + size.y + 1, x + size.x + 1][3] :
                    v.AddReturnIndex<Vector3>(new Vector2(x + size.x, h - (y + size.y) - 1));

                if (verts[0] == v.Count - 1) {
                    i[y + size.y, x + size.x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y + size.y, x + size.x][0] = verts[0];
                }

                verts[1] = (i[y + size.y, x - 1] != null && i[y + size.y, x - 1][0] != -1) ? i[y + size.y, x - 1][0] : 
                    (y + size.y + 1 < h && i[y + size.y + 1, x] != null && i[y + size.y + 1, x][3] != -1) ? i[y + size.y + 1, x][3] : 
                    (y + size.y + 1 < h && i[y + size.y + 1, x - 1] != null && i[y + size.y + 1, x - 1][2] != -1) ? i[y + size.y + 1, x - 1][2] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x - 1, h - (y + size.y) - 1));

                if (verts[1] == v.Count - 1) {
                    i[y + size.y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y + size.y, x][1] = verts[1];
                }

                verts[2] = (i[y - 1, x + size.x] != null && i[y - 1, x + size.x][0] != -1) ? i[y - 1, x + size.x][0] : 
                    (x + size.x + 1 < w && i[y, x + size.x + 1] != null && i[y, x + size.x + 1][3] != -1) ? i[y, x + size.x + 1][3] : 
                    (x + size.x + 1 < w && i[y - 1, x + size.x + 1] != null && i[y - 1, x + size.x + 1][1] != -1) ? i[y - 1, x + size.x + 1][1] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x + size.x, h - (y - 1) - 1));

                if (verts[2] == v.Count - 1) {
                    i[y, x + size.x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y, x + size.x][2] = verts[2];
                }

                verts[3] = (i[y - 1, x - 1] != null && i[y - 1, x - 1][0] != -1) ? i[y - 1, x - 1][0] : 
                    (i[y, x - 1] != null && i[y, x - 1][2] != -1) ? i[y, x - 1][2] : 
                    (i[y - 1, x] != null && i[y - 1, x][1] != -1) ? i[y - 1, x][1] : 
                    v.AddReturnIndex<Vector3>(new Vector2(x - 1, h - (y - 1) - 1));

                if (verts[3] == v.Count - 1) {
                    i[y, x] = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, v.Count, 15};
                    i[y, x][3] = verts[3];
                }

                t.AddRange(new int[] {verts[0], verts[1], verts[3], verts[0], verts[3], verts[2]});

                if (y == 1) e.Add(verts[3], verts[2]);
                if (x == 1) e.Add(verts[1], verts[3]);
                if ((y + size.y) == (h - 1)) e.Add(verts[0], verts[1]);
                if ((x + size.x) == (w - 1)) e.Add(verts[2], verts[0]);
            }
        }

        // foreach (int vi in t.ToArray()) {
        //     Debug.Log(vi);
        // }
        
        construct.mesh = new Mesh();
        construct.mesh.vertices = v.ToArray();
        construct.mesh.triangles = t.ToArray();

        List<List<Vector2>> paths = new List<List<Vector2>>();

        // foreach (int ve in e.Keys.ToArray()) {
        //     Debug.Log("1:" + v[ve]);
        //     Debug.Log(v[e[ve]]);
        // }

        // foreach (Vector2 ve in v) {
        //     Debug.Log(ve);
        // }

        while (e.Count > 0) {
            //Debug.Log("Path");

            int pathIndex = paths.Count;

            paths.Add(new List<Vector2>());

            int startVertex = e.Keys.First();            
            int nextVertex = startVertex;

            while (true) {
                //Debug.Log(nextVertex); 

                paths[pathIndex].Add(v[nextVertex]);

                if (e[nextVertex] == startVertex) {
                    e.Remove(nextVertex);
                    break;
                }

                int toRemove = nextVertex;

                nextVertex = e[nextVertex];

                e.Remove(toRemove);
            }
        }

        construct.points = new Vector2[paths.Count][];

        for (int o = 0; o < paths.Count; o++) {
            construct.points[o] = paths[o].ToArray();
        }

        // construct.points = new Vector2[1][];

        // construct.points[0] = new Vector2[e.Keys.Count * 2];

        // for (int o = 0; o < e.Keys.ToArray().Length; o++) {
        //     construct.points[0][o * 2] = v[e.Keys.ToArray()[o]];
        //     construct.points[0][(o * 2) + 1] = v[e.Values.ToArray()[o]];
        // }



        return construct;
    }

    public static float lint(float a, float b) { //Doing the a + b = 0 to test if both are zero,because then an error would be thrown
        return (a + b == 0) ? 0.5f : Mathf.Abs(a) / (Mathf.Abs(a) + Mathf.Abs(b));
    }
    
    public struct SquareConstruct {
        public Mesh mesh;
        public Vector2[][] points;
    }

    public static class SquareData {
        public readonly static bool[][] squareVerticies = {
            new[] {false, false, false, false, false, false, false, false}, //
            new[] {true, false, false, false, true, false, true, false}, //
            new[] {false, true, false, false, true, true, false, false}, //
            new[] {true, true, false, false, false, true, true, false}, //

            new[] {false, false, true, false, false, false, true, true}, //
            new[] {true, false, true, false, true, false, false, true}, //
            new[] {false, true, true, false, true, true, true, true}, //
            new[] {true, true, true, false, false, true, false, true}, //

            new[] {false, false, false, true, false, true, false, true}, //
            new[] {true, false, false, true, true, true, true, true}, //
            new[] {false, true, false, true, true, false, false, true}, //
            new[] {true, true, false, true, false, false, true, true}, 

            new[] {false, false, true, true, false, true, true, false}, //
            new[] {true, false, true, true, true, true, false, false}, //
            new[] {false, true, true, true, true, false, true, false}, //
            new[] {true, true, true, true, false, false, false, false} //
        };

        public readonly static int[][] squareTriangles = {
            new int[] {}, //
            new[] {0, 4, 6}, //
            new[] {4, 1, 5}, //
            new[] {0, 1, 5, 0, 5, 6}, //

            new[] {7, 2, 6}, //
            new[] {0, 4, 7, 0, 7, 2}, //
            new[] {4, 1, 5, 4, 5, 7, 4, 7, 6, 7, 2, 6}, //
            new[] {0, 1, 5, 0, 5, 7, 0, 7, 2}, //

            new[] {5, 3, 7}, //
            new[] {0, 4, 6, 4, 5, 6, 5, 7, 6, 5, 3, 7}, //
            new[] {4, 1, 3, 4, 3, 7}, //
            new[] {0, 1, 6, 1, 7, 6, 1, 3, 7}, //

            new[] {5, 3, 2, 5, 2, 6}, //
            new[] {0, 4, 2, 4, 5, 2, 5, 3, 2}, //
            new[] {4, 1, 3, 4, 3, 6, 3, 2, 6}, //
            new[] {0, 1, 3, 0, 3, 2} //
        };

        public readonly static int[][] squareEdges = {
            new int[] {},
            new int[] {4, 6},
            new int[] {5, 4},
            new int[] {5, 6},

            new int[] {6, 7},
            new int[] {4, 7},
            new int[] {5, 7, 6, 4},
            new int[] {5, 7},

            new int[] {7, 5}, 
            new int[] {4, 5, 7, 6},
            new int[] {7, 4},
            new int[] {7, 6},

            new int[] {6, 5},
            new int[] {4, 5},
            new int[] {6, 4},
            new int[] {}
        };

        public readonly static int[,][] squareFullEdges = {
            {new int[] {}, new int[] {}, new int[] {}, new int[] {}},
            {new int[] {0, 4}, new int[] {}, new int[] {6, 0}, new int[] {}},
            {new int[] {4, 1}, new int[] {1, 5}, new int[] {}, new int[] {}},
            {new int[] {0, 1}, new int[] {1, 5}, new int[] {6, 0}, new int[] {}},
            
            {new int[] {}, new int[] {}, new int[] {2, 6}, new int[] {7, 2}},
            {new int[] {0, 4}, new int[] {}, new int[] {2, 0}, new int[] {7, 2}},
            {new int[] {4, 1}, new int[] {1, 5}, new int[] {2, 6}, new int[] {7, 2}},
            {new int[] {0, 1}, new int[] {1, 5}, new int[] {2, 0}, new int[] {7, 2}},
            
            {new int[] {}, new int[] {5, 3}, new int[] {}, new int[] {3, 7}},
            {new int[] {0, 4}, new int[] {5, 3}, new int[] {6, 0}, new int[] {3, 7}},
            {new int[] {4, 1}, new int[] {1, 3}, new int[] {}, new int[] {3, 7}},
            {new int[] {0, 1}, new int[] {1, 3}, new int[] {6, 0}, new int[] {3, 7}},
            
            {new int[] {}, new int[] {5, 3}, new int[] {2, 6}, new int[] {3, 2}},
            {new int[] {0, 4}, new int[] {5, 3}, new int[] {2, 0}, new int[] {3, 2}},
            {new int[] {4, 1}, new int[] {1, 3}, new int[] {2, 6}, new int[] {3, 2}},
            {new int[] {0, 1}, new int[] {1, 3}, new int[] {2, 0}, new int[] {3, 2}}
        };

    }
}