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
                if (_ray.IsRayHitFront[RayIndex(i, j)])
                {
                    AssembleVertex("front", i, j);
                }
                else
                {
                    ClearTriangle("front", RayIndex(i, j), 0, 6);
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

        // for (int i = 0; i < _ray.RayCount; i++)
        // {
        //     if (_ray.IsBorderVertex[i])
        //     {
        //         _frontTrace = Instantiate(_sphere.BlueSphere, _ray.FrontHitPoint[i], Quaternion.identity);
        //         Destroy(_frontTrace, 3f);
        //     }
        // }
    }

    int RayIndex(int radiusIdx, int vertexIdx)
    {
        return _ray.RayIndex[string.Format("{0}{1}", radiusIdx, vertexIdx)];
    }

    void MoveRightVertex(string side, int triVertexIdx, int pivotIdx, int radius, int vertex)
    {
        if (IsHit(side, RayIndex(radius, vertex)))
        {
            SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius, vertex));
        }
        else
        {
            if (radius != 0)
                MoveRightVertex(side, triVertexIdx, pivotIdx, radius - 1, vertex);
            else if (IsHit(side, RayIndex(radius + 2, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 2, vertex));
            }
            else if (IsHit(side, RayIndex(radius + 3, vertex)))
            {
                SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius + 3, vertex));
            }
            else if (radius == 0 || radius == 3)
                ClearTriangle(side, pivotIdx, 0, 3);
        }
    }

    void MoveLeftVertex(string side, int triVertexIdx, int pivotIdx, int radius, int vertex)
    {
        if (IsHit(side, RayIndex(radius, vertex)))
        {
            SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius, vertex));
        }
        else
        {
            // if (radius < 3)
            // {
            //     MoveLeftVertex(side, triVertexIdx, pivotIdx, radius + 1, vertex);
            // }
            // else if (IsHit(side, RayIndex(radius - 1, vertex)))
            // {
            //     SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius - 1, vertex));
            // }
            // else if (IsHit(side, RayIndex(radius - 2, vertex)))
            // {
            //     SetTriangle(side, triVertexIdx, pivotIdx, RayIndex(radius - 2, vertex));
            // }
            // else if (radius == 0 || radius == 3)
            // {
            //     ClearTriangle(side, pivotIdx, 3, 6); // clear center
            // }

            if (radius != 0)
                MoveLeftVertex(side, triVertexIdx, pivotIdx, radius - 1, vertex);
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
                            MoveRightVertex(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), currentRadius1, currentVertex1);
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
                        MoveLeftVertex(side, triVertexIdx, RayIndex(radiusIdx, vertexIdx), currentRadius5, currentVertex5);
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
        frontFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        backFaceMeshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

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
