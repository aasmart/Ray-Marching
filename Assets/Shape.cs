using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShapeType {
    Ellipsoid,
    Box,
    Torus
}

public enum BlendMode {
    Union,
    Intersection,
    Difference,
}

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class Shape : MonoBehaviour {
    public ShapeType type;
    public BlendMode blendMode;
    public bool blendSmooth;
    [Range(0, 1)] public float blendStrength = 0.5f;
    public Color albedo;
    
    private Vector3 _position;
    private Vector3 _dimensions;
    private Matrix4x4 _rotation;
    private MeshFilter _meshFilter;

    public Shape(ShapeType type, Color albedo, Vector3 position, Vector3 dimensions) {
        this.type = type;
        this.albedo = albedo;
        this._position = position;
        this._dimensions = dimensions;
    }

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update() {
        var trans = transform;
        _position = trans.position;
        
        var meshSize = _meshFilter.sharedMesh.bounds.size;
        _dimensions = Vector3.Scale(meshSize * 0.5f, trans.lossyScale);
        
        _rotation = Matrix4x4.Rotate(trans.rotation).inverse;
    }

    public ShapeData ToData() {
        var children = gameObject.GetComponentsInChildren<Shape>();

        return new ShapeData() {
            type = (int)type,
            albedo = albedo,
            position = _rotation * _position,
            dimensions = _dimensions,
            numChildren = children.Length - 1,
            operation = (int)blendMode,
            blendSmooth = blendSmooth ? 1 : 0,
            blendStrength = blendStrength,
            rotation = _rotation
        };
    }
}

public struct ShapeData {
    public int type;
    public int numChildren;
    public int operation;
    public int blendSmooth;
    public float blendStrength;
    public Vector4 albedo;
    public Vector3 position;
    public Vector3 dimensions;
    public Matrix4x4 rotation;

    public static int SizeOf() {
        return 27 * sizeof(float) + 4 * sizeof(int);
    }
}