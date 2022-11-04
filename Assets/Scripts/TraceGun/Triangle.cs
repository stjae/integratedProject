using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class Triangle : MonoBehaviour
{
    Ray _ray;
    Sphere _sphere;

    GameObject _frontFaceObj;
    GameObject _backFaceObj;
<<<<<<< HEAD
    GameObject _sideFaceObj;
    Mesh _frontFaceMesh;
    Mesh _backFaceMesh;
    Mesh _sideFaceMesh;
    bool _isSideTriangleSet;
=======
    Mesh _frontFaceMesh;
    Mesh _backFaceMesh;
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e

    GameObject _frontTrace;

    List<Vector3> _sideVertex;
<<<<<<< HEAD
    List<int> _sideTriangle;

    Renderer _sphereRenderer;
=======
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e

    const int VERTEX_PER_TRI = 3;

    public void CreateTriangle()
    {
        for (int i = 0; i < _ray.IsBorderVertex.Count; i++)
        {
<<<<<<< HEAD
            _ray.IsBorderVertex[i] = false;
=======
            if (i < _ray.VertexCount)
                _ray.IsBorderVertex[i] = true;
            else
                _ray.IsBorderVertex[i] = false;
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < _ray.VertexCount; j++)  // if first vertex of triangle hit
            {
<<<<<<< HEAD
                if (_ray.IsRayHitFront[RayIndex(i, j)])
=======
                if (_ray.IsRayHitFront[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                {
                    AssembleVertex("front", i, j);
                }
                else
                {
<<<<<<< HEAD
                    ClearTriangle("front", RayIndex(i, j), 0, 6);
                    _ray.IsBorderVertex[RayIndex(i, j)] = false;
                    if (i < 3)
                        _ray.IsBorderVertex[RayIndex(i + 1, j)] = true;
                    else if (i == 3)
                    {
                        _ray.IsBorderVertex[_ray.IsBorderVertex.Count - 1] = true;
                    }
                }

                if (_ray.IsRayHitBack[RayIndex(i, j)])
=======
                    ClearTriangle("front", i, j, 0, 6);
                    _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", i, j)]] = false;
                    if (i < 3)
                        _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", i + 1, j)]] = true;
                }

                if (_ray.IsRayHitBack[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                {
                    AssembleVertex("back", i, j);
                }
                else
                {
<<<<<<< HEAD
                    ClearTriangle("back", RayIndex(i, j), 0, 6);
=======
                    ClearTriangle("back", i, j, 0, 6);
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                }
            }
        }

<<<<<<< HEAD
        _ray.SetRayHitCenter();

        // for (int i = 0; i < _sphere.FrontSphere.Count; i++)
        // {
        //     Renderer sphereRenderer;
        //     if (_ray.IsBorderVertex[i])
        //     {
        //         sphereRenderer = _sphere.FrontSphere[i].GetComponent<Renderer>();
        //         sphereRenderer.material.SetColor("_Color", Color.blue);
        //     }
        //     else
        //     {
        //         sphereRenderer = _sphere.FrontSphere[i].GetComponent<Renderer>();
        //         sphereRenderer.material.SetColor("_Color", Color.red);
        //     }
        // }

        _sideVertex.Clear();
        _sideTriangle.Clear();

        int count = 0;
        for (int i = 0; i < _ray.IsBorderVertex.Count; i++)
        {
            if (_ray.IsBorderVertex[i])
            {
                count++;
            }
        }

        if (count > 1)
            _isSideTriangleSet = true;
        else
            _isSideTriangleSet = false;

        if (_isSideTriangleSet)
        {
            while (_sideVertex.Count < count * 2)
            {
                _sideVertex.Add(Vector3.zero);
            }

            List<Vector3> borderPoint = new List<Vector3>();

            for (int j = 0; j < _ray.IsBorderVertex.Count; j++)
            {
                if (_ray.IsBorderVertex[j])
                {
                    borderPoint.Add(_ray.FrontHitPoint[j]);
                    borderPoint.Add(_ray.BackHitPoint[j]);
                }
            }

            for (int i = 0; i < _sideVertex.Count; i++)
            {
                _sideVertex[i] = borderPoint[i];
            }

            while (_sideTriangle.Count < count * 6)
            {
                _sideTriangle.Add(0);
            }

            SetSideTriangle(count);
        }
    }

    void SetSideTriangle(int count)
    {
        int modCount = 0;
        if (count % 2 == 0)
            modCount = count / 2;
        else
            modCount = count / 2 + 1;

        for (int i = 0; i < modCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                switch (j)
                {
                    case 0:
                        _sideTriangle[i * 6 + j] = i * 2;
                        break;

                    case 1:
                        _sideTriangle[i * 6 + j] = 2 + 2 * i;
                        break;

                    case 2:
                        _sideTriangle[i * 6 + j] = 1 + 2 * i;
                        break;
                }
            }

            for (int j = 3; j < 6; j++)
            {
                switch (j)
                {
                    case 3:
                        _sideTriangle[i * 6 + j] = i * 2 + 1;
                        break;

                    case 4:
                        _sideTriangle[i * 6 + j] = i * 2 + 2;
                        break;

                    case 5:
                        _sideTriangle[i * 6 + j] = i * 2 + 3;
                        break;
                }
            }
        }
    }

    int RayIndex(int radiusIdx, int vertexIdx)
    {
        return _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)];
    }

    void MoveRightVertex(string side, int triVertexIdx, int pivotIdx, int pivotRadius, int pivotVertex, int radius, int vertex)
    {
        if (IsHit(side, RayIndex(radius, vertex)))
        {
            SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius, vertex));
            if (pivotRadius != 0)
                _ray.IsBorderVertex[RayIndex(radius, vertex)] = true;
        }
        else
        {
            if (radius != 0)
            {
                MoveRightVertex(side, triVertexIdx, pivotIdx, pivotRadius, pivotVertex, radius - 1, vertex);
            }
            else if (IsHit(side, RayIndex(radius + 2, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 2, vertex));
            }
            else if (IsHit(side, RayIndex(radius + 3, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 3, vertex));
            }
            else if (radius == 0 || radius == 3)
            {
                ClearTriangle(side, pivotIdx, 0, 3);
                if (pivotRadius != 0)
                {
                    _ray.IsBorderVertex[pivotIdx] = true;
                }
                _ray.IsBorderVertex[RayIndex(3, pivotVertex)] = true;
            }
        }
    }

    void MoveLeftVertex(string side, int triVertexIdx, int pivotIdx, int pivotRadius, int pivotVertex, int radius, int vertex)
    {
        if (IsHit(side, RayIndex(radius, vertex)))
        {
            SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius, vertex));
            _ray.IsBorderVertex[RayIndex(radius, vertex)] = true;
        }
        else
        {
            if (radius != 0)
            {
                MoveLeftVertex(side, triVertexIdx, pivotIdx, pivotVertex, pivotRadius, radius - 1, vertex);
            }
            else if (IsHit(side, RayIndex(radius + 1, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 1, vertex));
            }
            else if (IsHit(side, RayIndex(radius + 2, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 2, vertex));
            }
            else if (IsHit(side, RayIndex(radius + 3, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 3, vertex));
            }
            else
            {
                ClearTriangle(side, pivotIdx, 3, 6);
                if (pivotRadius != 0)
                {
                    _ray.IsBorderVertex[pivotIdx] = true;
                }
=======
        for (int i = 0; i < _ray.RayCount; i++)
        {
            if (_ray.IsBorderVertex[i])
            {
                _frontTrace = Instantiate(_sphere.BlueSphere, _ray.FrontHitPoint[i], Quaternion.identity);
                Destroy(_frontTrace, 3f);
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
            }
        }
    }

    void AssembleVertex(string side, int radiusIdx, int vertexIdx)
    {
        for (int triVertexIdx = 0; triVertexIdx < VERTEX_PER_TRI * 2; triVertexIdx++)
        {
            switch (triVertexIdx)
            {
                case 0:
<<<<<<< HEAD
                    SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), RayIndex(radiusIdx, vertexIdx));
                    if (radiusIdx == 0)
                        _ray.IsBorderVertex[RayIndex(radiusIdx, vertexIdx)] = true;
=======
                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)], "set");
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                    break;
                case 1:
                    if (radiusIdx < 3)
                    {
<<<<<<< HEAD
                        int currentIdx1 = (vertexIdx == 0) ? RayIndex(radiusIdx + 1, _ray.VertexCount - 1) : RayIndex(radiusIdx + 1, vertexIdx - 1);
                        int currentRadius1 = radiusIdx + 1;
                        int currentVertex1 = (vertexIdx == 0) ? _ray.VertexCount - 1 : vertexIdx - 1;

                        if (IsHit(side, currentIdx1))
                        {
                            SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), currentIdx1);
                        }
                        else
                        {
                            MoveRightVertex(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), radiusIdx, vertexIdx, currentRadius1, currentVertex1);
=======
                        int rayIdx = (vertexIdx == 0) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, _ray.VertexCount - 1)] : _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx - 1)];
                        if (IsHit(side, rayIdx))
                        {
                            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx, "set");
                        }
                        else
                        {
                            int multiplier = 0;
                            while (rayIdx + _ray.VertexCount * multiplier < _ray.RayCount)
                            {
                                if (IsHit(side, rayIdx + _ray.VertexCount * multiplier))
                                {
                                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx + _ray.VertexCount * multiplier, "set");
                                    _ray.IsBorderVertex[rayIdx + _ray.VertexCount * multiplier] = true;

                                    break;
                                }
                                else if (!IsHit(side, rayIdx + _ray.VertexCount * multiplier))
                                {
                                    if (IsHit(side, rayIdx + _ray.VertexCount * (multiplier - 1)))
                                        SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx + _ray.VertexCount * (multiplier - 1), "set");
                                    else
                                    {
                                        ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
                                        _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]] = true;
                                    }

                                    break;
                                }
                                else if (rayIdx + _ray.VertexCount * (multiplier + 1) >= _ray.RayCount)
                                {
                                    ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
                                    _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]] = true;
                                }
                                multiplier++;
                            }
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                        }
                    }
                    break;
                case 2:
                    if (radiusIdx < 3)
                    {
<<<<<<< HEAD
                        if (IsHit(side, RayIndex(radiusIdx + 1, vertexIdx)))
                        {
                            SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), RayIndex(radiusIdx + 1, vertexIdx));
                        }
                        else
                        {
                            ClearTriangle(side, RayIndex(radiusIdx, vertexIdx), 0, 3);
                            if (radiusIdx == 2 && vertexIdx > 0 && vertexIdx < _ray.VertexCount - 1)
                            {
                                _ray.IsBorderVertex[RayIndex(radiusIdx + 1, vertexIdx - 1)] = true;
                                if (vertexIdx < _ray.VertexCount - 2)
                                {
                                    if (!IsHit(side, RayIndex(radiusIdx, vertexIdx + 2)))
                                        _ray.IsBorderVertex[RayIndex(radiusIdx, vertexIdx + 1)] = true;
                                }
                            }
                            else if (radiusIdx == 1 && vertexIdx > 0 && vertexIdx < _ray.VertexCount - 2)
                            {
                                _ray.IsBorderVertex[RayIndex(radiusIdx + 2, vertexIdx - 1)] = true;
                                _ray.IsBorderVertex[RayIndex(radiusIdx + 1, vertexIdx - 1)] = true;
                                if (!IsHit(side, RayIndex(radiusIdx, vertexIdx + 2)))
                                    _ray.IsBorderVertex[RayIndex(radiusIdx, vertexIdx + 1)] = true;
                            }
                            else if (radiusIdx == 0 && vertexIdx > 0)
                            {
                                _ray.IsBorderVertex[RayIndex(radiusIdx, vertexIdx)] = false;
                                _ray.IsBorderVertex[RayIndex(radiusIdx + 1, vertexIdx - 1)] = true;
                            }
=======
                        if (IsHit(side, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]))
                        {
                            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * 6 + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)], "set");
                        }
                        else
                        {
                            ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                        }
                    }
                    break;

                case 3:
<<<<<<< HEAD
                    SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), RayIndex(radiusIdx, vertexIdx));
                    break;
                case 4:
                    int currentIdx4 = (radiusIdx < 3) ? RayIndex(radiusIdx + 1, vertexIdx) : _ray.FrontHit.Count - 1;

                    if (IsHit(side, currentIdx4))
                    {
                        SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), currentIdx4);
                    }
                    else
                    {
                        ClearTriangle(side, RayIndex(radiusIdx, vertexIdx), 3, 6);
                        if (radiusIdx == 2 && vertexIdx + 1 < _ray.VertexCount)
                            _ray.IsBorderVertex[RayIndex(radiusIdx + 1, vertexIdx + 1)] = true;
                        if (radiusIdx == 1 && vertexIdx + 1 < _ray.VertexCount)
                            _ray.IsBorderVertex[RayIndex(radiusIdx + 2, vertexIdx + 1)] = true;
                    }
                    break;
                case 5:
                    int currentIdx5 = (vertexIdx == _ray.VertexCount - 1) ? RayIndex(radiusIdx, 0) : RayIndex(radiusIdx, vertexIdx + 1);
                    int currentRadius5 = radiusIdx;
                    int currentVertex5 = (vertexIdx == _ray.VertexCount - 1) ? 0 : vertexIdx + 1;

                    if (IsHit(side, currentIdx5))
                    {
                        SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), currentIdx5);
                    }
                    else
                    {
                        MoveLeftVertex(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), radiusIdx, vertexIdx, currentRadius5, currentVertex5);
=======
                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)], "set");
                    break;
                case 4:
                    int rayIdx1 = (radiusIdx < 3) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)] : _ray.FrontHit.Count - 1;
                    if (IsHit(side, rayIdx1))
                    {
                        SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * VERTEX_PER_TRI * 2 + triVertexIdx, rayIdx1, "set");
                    }
                    else
                    {
                        ClearTriangle(side, radiusIdx, vertexIdx, 3, 6);
                    }
                    break;
                case 5:
                    int rayIdx2 = (vertexIdx == _ray.VertexCount - 1) ? _ray.RayIndex[string.Format("{0}{1}", radiusIdx, 0)] : _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx + 1)]; // 01
                    if (IsHit(side, rayIdx2))
                    {
                        SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx2, "set");
                    }
                    else
                    {
                        int multiplier1 = 0;
                        while (rayIdx2 + _ray.VertexCount * multiplier1 < _ray.RayCount)
                        {
                            if (IsHit(side, rayIdx2 + _ray.VertexCount * multiplier1))
                            {
                                SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, rayIdx2 + _ray.VertexCount * multiplier1, "set");
                                _ray.IsBorderVertex[rayIdx2 + _ray.VertexCount * multiplier1] = true;

                                break;
                            }
                            else if (rayIdx2 + _ray.VertexCount * (multiplier1 + 1) >= _ray.RayCount)
                            {
                                ClearTriangle(side, radiusIdx, vertexIdx, 3, 6);
                                _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)]] = true;
                            }
                            multiplier1++;
                        }
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
                    }
                    break;
            }
        }
    }

<<<<<<< HEAD
    void SetTriangle(string side, int triVertexIdx, int vertexIdx, int rayIdx)
    {
        if (side == "front")
        {
            _ray.FrontTriangle[vertexIdx * (VERTEX_PER_TRI * 2) + triVertexIdx] = rayIdx;
        }
        else if (side == "back")
        {
            _ray.BackTriangle[vertexIdx * (VERTEX_PER_TRI * 2) + triVertexIdx] = rayIdx;
=======
    void SetTriangle(string side, int vertexIdx, int rayIdx, string mode)
    {
        if (side == "front")
        {
            _ray.FrontTriangle[vertexIdx] = rayIdx;
        }
        else if (side == "back")
        {
            _ray.BackTriangle[vertexIdx] = rayIdx;
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
        }
    }

    bool IsHit(string side, int rayIdx)
    {
        if (side == "front")
        {
            return _ray.IsRayHitFront[rayIdx];
        }
        else if (side == "back")
        {
            return _ray.IsRayHitBack[rayIdx];
        }
        else
        {
            throw new ArgumentException();
        }
    }

<<<<<<< HEAD
    void ClearTriangle(string side, int rayIdx, int a, int b)
    {
        for (int triVertexIdx = a; triVertexIdx < b; triVertexIdx++)
        {
            SetTriangle(side, triVertexIdx, rayIdx, 0);
=======
    void ClearTriangle(string side, int radiusIdx, int vertexIdx, int a, int b)
    {
        for (int triVertexIdx = a; triVertexIdx < b; triVertexIdx++)
        {
            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, 0, "clear");
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
        }
    }

    public void CreateShape()
    {
<<<<<<< HEAD
        if (_isSideTriangleSet)
        {
            _frontFaceMesh.Clear();
            _frontFaceMesh.vertices = _ray.FrontHitPoint.ToArray();
            _frontFaceMesh.triangles = _ray.FrontTriangle.ToArray();

            _backFaceMesh.Clear();
            _backFaceMesh.vertices = _ray.BackHitPoint.ToArray();
            _backFaceMesh.triangles = _ray.BackTriangle.ToArray();
            _backFaceMesh.triangles = _backFaceMesh.triangles.ToArray();

            _sideFaceMesh.Clear();
            _sideFaceMesh.vertices = _sideVertex.ToArray();
            _sideFaceMesh.triangles = _sideTriangle.ToArray();

            // // Reshape mesh
            _frontFaceObj.GetComponent<MeshCollider>().sharedMesh = _frontFaceMesh;
            _backFaceObj.GetComponent<MeshCollider>().sharedMesh = _backFaceMesh;
            _sideFaceObj.GetComponent<MeshCollider>().sharedMesh = _sideFaceMesh;
        }
=======
        _frontFaceMesh.Clear();
        _frontFaceMesh.vertices = _ray.FrontHitPoint.ToArray();
        _frontFaceMesh.triangles = _ray.FrontTriangle.ToArray();

        _backFaceMesh.Clear();
        _backFaceMesh.vertices = _ray.BackHitPoint.ToArray();
        _backFaceMesh.triangles = _ray.BackTriangle.ToArray();
        _backFaceMesh.triangles = _backFaceMesh.triangles.Reverse().ToArray();
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
    }

    void Start()
    {
        _ray = gameObject.GetComponent<Ray>();
        _sphere = gameObject.GetComponent<Sphere>();

        _frontFaceObj = new GameObject("FrontFace");
        _backFaceObj = new GameObject("BackFace");
<<<<<<< HEAD
        _sideFaceObj = new GameObject("SideFace");
        _frontFaceObj.layer = Layer.TraceFace;
        _backFaceObj.layer = Layer.TraceFace;
        _sideFaceObj.layer = Layer.TraceFace;
        _frontFaceMesh = new Mesh();
        _backFaceMesh = new Mesh();
        _sideFaceMesh = new Mesh();
        _frontFaceObj.AddComponent<MeshFilter>();
        _backFaceObj.AddComponent<MeshFilter>();
        _sideFaceObj.AddComponent<MeshFilter>();
        _frontFaceObj.AddComponent<MeshCollider>();
        _backFaceObj.AddComponent<MeshCollider>();
        _sideFaceObj.AddComponent<MeshCollider>();
        _frontFaceObj.AddComponent<MeshRenderer>();
        _backFaceObj.AddComponent<MeshRenderer>();
        _sideFaceObj.AddComponent<MeshRenderer>();
        _frontFaceObj.GetComponent<MeshFilter>().mesh = _frontFaceMesh;
        _backFaceObj.GetComponent<MeshFilter>().mesh = _backFaceMesh;
        _sideFaceObj.GetComponent<MeshFilter>().mesh = _sideFaceMesh;

        MeshRenderer frontFaceMeshRenderer = _frontFaceObj.GetComponent<MeshRenderer>();
        MeshRenderer backFaceMeshRenderer = _backFaceObj.GetComponent<MeshRenderer>();
        MeshRenderer sideFaceMeshRenderer = _sideFaceObj.GetComponent<MeshRenderer>();
        frontFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        backFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        sideFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

        Renderer frontFaceRenderer = _frontFaceObj.GetComponent<Renderer>();
        Renderer backFaceRenderer = _backFaceObj.GetComponent<Renderer>();
        Renderer sideFaceRenderer = _sideFaceObj.GetComponent<Renderer>();
        Color yellow = Color.yellow;
        Color green = Color.green;
        Color magenta = Color.magenta;
        Color white = Color.white;
        frontFaceRenderer.material.SetColor("_Color", new Color(white.r, white.g, white.b, 0.0f));
        backFaceRenderer.material.SetColor("_Color", new Color(white.r, white.g, white.b, 0.0f));
        sideFaceRenderer.material.SetColor("_Color", new Color(white.r, white.g, white.b, 0.0f));

        _sideVertex = new List<Vector3>();
        _sideTriangle = new List<int>();
=======
        _frontFaceObj.layer = Layer.TraceFace;
        _backFaceObj.layer = Layer.TraceFace;
        _frontFaceMesh = new Mesh();
        _backFaceMesh = new Mesh();
        _frontFaceObj.AddComponent<MeshFilter>();
        _backFaceObj.AddComponent<MeshFilter>();
        _frontFaceObj.AddComponent<MeshRenderer>();
        _backFaceObj.AddComponent<MeshRenderer>();
        _frontFaceObj.GetComponent<MeshFilter>().mesh = _frontFaceMesh;
        _backFaceObj.GetComponent<MeshFilter>().mesh = _backFaceMesh;

        MeshRenderer frontFaceMeshRenderer = _frontFaceObj.GetComponent<MeshRenderer>();
        MeshRenderer backFaceMeshRenderer = _backFaceObj.GetComponent<MeshRenderer>();
        frontFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        backFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

        Renderer frontFaceRenderer = _frontFaceObj.GetComponent<Renderer>();
        Renderer backFaceRenderer = _backFaceObj.GetComponent<Renderer>();
        Color yellow = Color.yellow;
        Color green = Color.green;
        frontFaceRenderer.material.SetColor("_Color", new Color(yellow.r, yellow.g, yellow.b, 0.5f));
        backFaceRenderer.material.SetColor("_Color", new Color(green.r, green.g, green.b, 0.5f));

        _sideVertex = new List<Vector3>();
>>>>>>> 5c1df6e60a7dde2b9a69c46f83e296720e9b1e0e
    }

    void PrintTriangleVertex()
    {
        string str = "";
        int count = 0;

        foreach (var item in _ray.FrontTriangle)
        {
            str += item + " ";
            count++;

            if (count % 3 == 0)
                str += "| ";
        }

        Debug.Log(str);
    }
}
