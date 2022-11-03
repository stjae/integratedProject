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
    Mesh _frontFaceMesh;
    Mesh _backFaceMesh;

    GameObject _frontTrace;

    List<Vector3> _sideVertex;

    const int VERTEX_PER_TRI = 3;

    public void CreateTriangle()
    {
        for (int i = 0; i < _ray.IsBorderVertex.Count; i++)
        {
            if (i < _ray.VertexCount)
                _ray.IsBorderVertex[i] = true;
            else
                _ray.IsBorderVertex[i] = false;
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < _ray.VertexCount; j++)  // if first vertex of triangle hit
            {
                if (_ray.IsRayHitFront[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
                {
                    AssembleVertex("front", i, j);
                }
                else
                {
                    ClearTriangle("front", i, j, 0, 6);
                    _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", i, j)]] = false;
                    if (i < 3)
                        _ray.IsBorderVertex[_ray.RayIndex[string.Format("{0}{1}", i + 1, j)]] = true;
                }

                if (_ray.IsRayHitBack[_ray.RayIndex[string.Format("{0}{1}", i, j)]])
                {
                    AssembleVertex("back", i, j);
                }
                else
                {
                    ClearTriangle("back", i, j, 0, 6);
                }
            }
        }

        for (int i = 0; i < _ray.RayCount; i++)
        {
            if (_ray.IsBorderVertex[i])
            {
                _frontTrace = Instantiate(_sphere.BlueSphere, _ray.FrontHitPoint[i], Quaternion.identity);
                Destroy(_frontTrace, 3f);
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
                    SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)], "set");
                    break;
                case 1:
                    if (radiusIdx < 3)
                    {
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
                        }
                    }
                    break;
                case 2:
                    if (radiusIdx < 3)
                    {
                        if (IsHit(side, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)]))
                        {
                            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * 6 + triVertexIdx, _ray.RayIndex[string.Format("{0}{1}", radiusIdx + 1, vertexIdx)], "set");
                        }
                        else
                        {
                            ClearTriangle(side, radiusIdx, vertexIdx, 0, 3);
                        }
                    }
                    break;

                case 3:
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
                    }
                    break;
            }
        }
    }

    void SetTriangle(string side, int vertexIdx, int rayIdx, string mode)
    {
        if (side == "front")
        {
            _ray.FrontTriangle[vertexIdx] = rayIdx;
        }
        else if (side == "back")
        {
            _ray.BackTriangle[vertexIdx] = rayIdx;
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

    void ClearTriangle(string side, int radiusIdx, int vertexIdx, int a, int b)
    {
        for (int triVertexIdx = a; triVertexIdx < b; triVertexIdx++)
        {
            SetTriangle(side, (radiusIdx * _ray.VertexCount + vertexIdx) * (VERTEX_PER_TRI * 2) + triVertexIdx, 0, "clear");
        }
    }

    public void CreateShape()
    {
        _frontFaceMesh.Clear();
        _frontFaceMesh.vertices = _ray.FrontHitPoint.ToArray();
        _frontFaceMesh.triangles = _ray.FrontTriangle.ToArray();

        _backFaceMesh.Clear();
        _backFaceMesh.vertices = _ray.BackHitPoint.ToArray();
        _backFaceMesh.triangles = _ray.BackTriangle.ToArray();
        _backFaceMesh.triangles = _backFaceMesh.triangles.Reverse().ToArray();
    }

    void Start()
    {
        _ray = gameObject.GetComponent<Ray>();
        _sphere = gameObject.GetComponent<Sphere>();

        _frontFaceObj = new GameObject("FrontFace");
        _backFaceObj = new GameObject("BackFace");
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
        frontFaceMeshRenderer.material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        backFaceMeshRenderer.material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));

        Renderer frontFaceRenderer = _frontFaceObj.GetComponent<Renderer>();
        Renderer backFaceRenderer = _backFaceObj.GetComponent<Renderer>();
        Color yellow = Color.yellow;
        Color green = Color.green;
        frontFaceRenderer.material.SetColor("_Color", new Color(yellow.r, yellow.g, yellow.b, 0.5f));
        backFaceRenderer.material.SetColor("_Color", new Color(green.r, green.g, green.b, 0.5f));

        _sideVertex = new List<Vector3>();
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
