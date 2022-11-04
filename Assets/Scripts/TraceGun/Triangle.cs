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
    GameObject _sideFaceObj;
    Mesh _frontFaceMesh;
    Mesh _backFaceMesh;
    Mesh _sideFaceMesh;
    bool _isSideTriangleSet;

    GameObject _frontTrace;

    List<Vector3> _sideVertex;
    List<int> _sideTriangle;

    Renderer _sphereRenderer;

    const int VERTEX_PER_TRI = 3;

    public void CreateTriangle()
    {
        for (int i = 0; i < _ray.IsBorderVertex.Count; i++)
        {
            _ray.IsBorderVertex[i] = false;
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < _ray.VertexCount; j++)  // if first vertex of triangle hit
            {
                if (_ray.IsRayHitFront[RayIndex(i, j)])
                {
                    AssembleVertex("front", i, j);
                }
                else
                {
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
                {
                    AssembleVertex("back", i, j);
                }
                else
                {
                    ClearTriangle("back", RayIndex(i, j), 0, 6);
                }
            }
        }

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
                    SetTriangle(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), RayIndex(radiusIdx, vertexIdx));
                    if (radiusIdx == 0)
                        _ray.IsBorderVertex[RayIndex(radiusIdx, vertexIdx)] = true;
                    break;
                case 1:
                    if (radiusIdx < 3)
                    {
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
                        }
                    }
                    break;
                case 2:
                    if (radiusIdx < 3)
                    {
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
                        }
                    }
                    break;

                case 3:
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
                    }
                    break;
            }
        }
    }

    void SetTriangle(string side, int triVertexIdx, int vertexIdx, int rayIdx)
    {
        if (side == "front")
        {
            _ray.FrontTriangle[vertexIdx * (VERTEX_PER_TRI * 2) + triVertexIdx] = rayIdx;
        }
        else if (side == "back")
        {
            _ray.BackTriangle[vertexIdx * (VERTEX_PER_TRI * 2) + triVertexIdx] = rayIdx;
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

    void ClearTriangle(string side, int rayIdx, int a, int b)
    {
        for (int triVertexIdx = a; triVertexIdx < b; triVertexIdx++)
        {
            SetTriangle(side, triVertexIdx, rayIdx, 0);
        }
    }

    public void CreateShape()
    {
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
    }

    void Start()
    {
        _ray = gameObject.GetComponent<Ray>();
        _sphere = gameObject.GetComponent<Sphere>();

        _frontFaceObj = new GameObject("FrontFace");
        _backFaceObj = new GameObject("BackFace");
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
