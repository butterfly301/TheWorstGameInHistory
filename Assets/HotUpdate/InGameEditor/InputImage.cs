using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputImage : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _tarPos;

    public Vector2 viewportRange;
    [Header("视角控制")] public float scaleSpeed;
    public float scaleChangeSpeed;
    [Header("相机位移")] public float camSpeed;
    public float camChangeSpeed;
    private float _targetViewport;

    private Vector3 _lastPos;

    private void Start()
    {
        _camera = Camera.main;
        _tarPos = _camera.transform.position;
        _camera = Camera.main;
        _targetViewport = _camera.orthographicSize;
    }

    private void Update()
    {
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, _tarPos, camChangeSpeed * Time.deltaTime);
        _targetViewport = Mathf.Clamp(_targetViewport - Input.mouseScrollDelta.y * scaleSpeed, viewportRange.x,
            viewportRange.y);
        _camera.orthographicSize =
            Mathf.Lerp(_camera.orthographicSize, _targetViewport, scaleChangeSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(2))
        {
            _lastPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Debug.Log($"{_lastPos} {Input.mousePosition}");
            _tarPos -= (Input.mousePosition - _lastPos).normalized * (camSpeed * Time.deltaTime);
            _lastPos = Input.mousePosition;
        }
    }

    public void OnViewBack()
    {
        Vector3 p = _tarPos;
        p.x = p.y = 0;
        _tarPos = p;
    }
}