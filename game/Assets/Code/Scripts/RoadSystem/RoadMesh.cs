using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI.Extensions;

[ExecuteInEditMode()]
public class RoadMesh : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private int splineIndex;
    [SerializeField, Range(0,1f)] private float time;
    [SerializeField] private float width = 10f;
    [SerializeField] private int samples = 50;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Terrain[] terrainMaps;
    
    private float3 _position;
    private float3 _tangent;
    private float3 _up;

    private float3 _p1;
    private float3 _p2;
    
    private readonly List<Vector3> _vertsP1 = new List<Vector3>();
    private readonly List<Vector3> _vertsP2 = new List<Vector3>();

    [SerializeField] private bool connected;
    
    private Mesh _mesh;

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetVerts();
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if(splineContainer == null) return;
        GetVerts();
    }

    private void GetVerts()
    {
        _vertsP1.Clear();
        _vertsP2.Clear();
        
        for (int i = 0; i < samples; i++)
        {
            var time = i / (float) samples;
            
            splineContainer.Evaluate(splineIndex, time, out var pos, out var forward, out  var up);
            float3 right = Vector3.Cross(forward, up).normalized;
            
            _vertsP1.Add(pos + right * width);
            _vertsP2.Add(pos - right * width);
        }
    }

    [Button]
    private void BuildMesh()
    {
        _mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        int offset = 0;
        int length = _vertsP1.Count;
        float uvOffset = 0;
        
        for (int i = 1; i <= length; i++)
        {
            if(!connected && i == length) {
                break;
            }
            
            Vector3 p1 = _vertsP1[i-1];
            Vector3 p2 = _vertsP2[i-1];
            Vector3 p3, p4;

            if (i == length)
            {
                p3 = _vertsP1[0];
                p4 = _vertsP2[0];
            }
            else
            {
                p3 = _vertsP1[i];
                p4 = _vertsP2[i];
            }

            offset = 4 * (i - 1);
            
            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;
            
            verts.AddRange(new List<Vector3>() {p1, p2, p3, p4} );
            tris.AddRange(new List<int>() {t1, t2, t3, t4, t5, t6});

            float distance = Vector3.Distance(p1, p3) / 4f;
            float uvDistance = uvOffset + distance;
            uvs.AddRange(new List<Vector2>()
            {
                new Vector2(uvOffset, 0),
                new Vector2(uvOffset, 1),
                new Vector2(uvOffset, 0),
                new Vector2(uvOffset, 1)
            });
            uvOffset += distance;

        }
        
        _mesh.SetVertices(verts);
        _mesh.SetTriangles(tris, 0);
        _mesh.RecalculateNormals();
        _mesh.SetUVs(0, uvs);
        meshFilter.mesh = _mesh;
        meshCollider.sharedMesh = _mesh;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < samples; i++)
        {
            Handles.SphereHandleCap(0, _vertsP1[i], Quaternion.identity, 3f, EventType.Repaint);
            Handles.SphereHandleCap(0, _vertsP2[i], Quaternion.identity, 3f, EventType.Repaint);
        }
    }
    #endif
}
