using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[RequireComponent(typeof(Camera))]
public class Movement : MonoBehaviour {
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _inputSensitivity;
    [SerializeField] private float _turnSmoothing;
    private Camera _camera;
    
    private float _moveX;
    private float _moveY;
    private float _mouseX;
    private float _mouseY;
    private float _rotationX;
    private float _rotationY;
    private float _vertical;

    // Start is called before the first frame update
    void Start() {
        _camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update() {
        _moveX = Input.GetAxis("Vertical");
        _moveY = Input.GetAxis("Horizontal");
        _mouseX = Input.GetAxis("Mouse X") * _inputSensitivity;
        _mouseY = Input.GetAxis("Mouse Y") * _inputSensitivity;
        _vertical = Input.GetKey(KeyCode.C) ? -1 : Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void FixedUpdate() {
        _rotationX -= _mouseY * Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        _rotationY += _mouseX * Time.deltaTime;
        _rotationY %= 360;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_rotationX, _rotationY, 0), _turnSmoothing * Time.deltaTime);

        var trans = transform;

        var forwards = trans.forward * (_moveX * _movementSpeed * Time.deltaTime);
        var horizontal = trans.right * (_moveY * _movementSpeed * Time.deltaTime);
        var vertical = new UnityEngine.Vector3(0, 1, 0) * (_vertical * _movementSpeed * Time.deltaTime);

        var velocity = forwards + horizontal + vertical;
        if (velocity.sqrMagnitude > 1f)
            velocity = velocity.normalized * (_movementSpeed * Time.deltaTime);
        
        transform.Translate(velocity, Space.World);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.forward);
    }
}
