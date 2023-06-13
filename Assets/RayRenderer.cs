using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Experimental.Rendering;
using LightType = UnityEngine.LightType;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RayRenderer : MonoBehaviour {
    public ComputeShader _shader;
    private Camera _camera;
    private RenderTexture _rTexture;
    [SerializeField] private GameObject _shapes;
    [SerializeField] private Texture _texture;
    [SerializeField] private Light _lightSource;
    
    private void Init() {
        _camera = Camera.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Init();
        InitRenderTexture();

        SetLightParam();
        
        var shapeDataBuffer = SceneShapeDataBuffer();
        _shader.SetBuffer(0, "shapes", shapeDataBuffer);
        _shader.SetInt("numShapes", shapeDataBuffer.count);

        _shader.SetTexture(0, "skyboxTexture", _texture);
        _shader.SetTexture(0, "Result", _rTexture);
        _shader.SetFloat("width", _camera.pixelWidth);
        _shader.SetFloat("height", _camera.pixelHeight);
        _shader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        _shader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        
        var threadsX = Mathf.CeilToInt(_camera.pixelWidth / 8.0f);
        var threadsY = Mathf.CeilToInt(_camera.pixelHeight / 8.0f);
        
        _shader.Dispatch(0, threadsX, threadsY, 1);
        
        Graphics.Blit(_rTexture, destination);
        
        shapeDataBuffer.Dispose();
    }

    private void SetLightParam() {
        var isDirectional = _lightSource.type == LightType.Directional;
        var lightDir = isDirectional ? _lightSource.transform.forward : _lightSource.transform.position;
        _shader.SetVector("light", new Vector4(lightDir.x, lightDir.y, lightDir.z, _lightSource.intensity));
        _shader.SetBool("isLightDirectional", isDirectional);
    }

    private ComputeBuffer SceneShapeDataBuffer() {
        /*
         * A hacky solution to sort the shapes in the hierarchy based on an elements sibling index,
         * if it has a parent, and the number of previous children in the hierarchy.
         */
        
        // Number of previous children in the hierarchy
        var childOffsetAmount = 0;
        var shapes = _shapes.GetComponentsInChildren<Shape>()
            .OrderBy(shape => {
                var trans = shape.transform;
                var hasShapeParent = trans.parent.gameObject != _shapes;
                
                var siblingIndex = trans.GetSiblingIndex();
                var parentIndex = hasShapeParent ? trans.parent.GetSiblingIndex() + 1 : 0;
                var parentChildCount = hasShapeParent ? trans.parent.childCount : 0;
                
                /* The value is offset by the parent's child count since the child offset amount
                 has already been increased by the parent's number of children*/
                var absoluteIndex = siblingIndex + parentIndex + childOffsetAmount - parentChildCount;

                childOffsetAmount += trans.childCount;
                return absoluteIndex;
            }).ToArray();

        var shapeData = new ShapeData[shapes.Length];
        for (var i = 0; i < shapeData.Length; i++) {
            shapeData[i] = shapes[i].ToData();
        }

        var buffer = new ComputeBuffer(shapeData.Length, ShapeData.SizeOf());
        buffer.SetData(shapeData);
        
        // Debug.Log(buffer.GetData());

        return buffer;
    }

    private void InitRenderTexture() {
        if (_rTexture != null)
            return;
        
        _rTexture = new RenderTexture(
            Screen.width, 
            Screen.height, 
            0,
            RenderTextureFormat.ARGBFloat, 
            RenderTextureReadWrite.Linear
        ) {
            enableRandomWrite = true
        };
        _rTexture.Create();
    }
}
