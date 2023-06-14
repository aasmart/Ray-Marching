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
    
    [Range(0, 1)]
    public float blendStrength = 0.5f;
    public Color albedo;
    private Vector3 position;
    private Vector3 dimensions;

    private MeshFilter _meshFilter;

    public Shape(ShapeType type, Color albedo, Vector3 position, Vector3 dimensions) {
        this.type = type;
        this.albedo = albedo;
        this.position = position;
        this.dimensions = dimensions;
    }

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update() {
        position = transform.position;
        
        var meshSize = _meshFilter.sharedMesh.bounds.size;
        dimensions = Vector3.Scale(meshSize * 0.5f, transform.lossyScale);
    }

    public ShapeData ToData() {
        var children = gameObject.GetComponentsInChildren<Shape>();

        return new ShapeData() {
            type = (int)type,
            albedo = albedo,
            position = position,
            dimensions = dimensions,
            numChildren = children.Length - 1,
            operation = (int)blendMode,
            blendSmooth = blendSmooth ? 1 : 0,
            blendStrength = blendStrength
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

    public static int SizeOf() {
        return 11 * sizeof(float) + 4 * sizeof(int);
    }
}