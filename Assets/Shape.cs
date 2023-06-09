using System;
using UnityEngine;

public enum ShapeType {
    Ellipsoid,
    Box
}

[RequireComponent(typeof(MeshFilter))]
public class Shape : MonoBehaviour {
    public ShapeType type;
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
        
        var meshSize = _meshFilter.mesh.bounds.size;
        var scale = transform.localScale;
        var meshDimensions = new Vector3(
            meshSize.x * scale.x,
            meshSize.y * scale.y,
            meshSize.z * scale.z
        );
        dimensions = meshDimensions;
    }

    public ShapeData ToData() {
        return new ShapeData() {
            type = (int)type,
            albedo = albedo,
            position = position,
            dimensions = dimensions
        };
    }
}

public struct ShapeData {
    public int type;
    public Vector4 albedo;
    public Vector3 position;
    public Vector3 dimensions;

    public static int SizeOf() {
        return sizeof(float) * 10 + sizeof(int) * 1;
    }
}