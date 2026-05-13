using System;
using InGameEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private EditorClient _client;
    private Transform _target;
    private Camera _camera;

    private void Start()
    {
        _client = new EditorClient(this);
        _camera = Camera.main;
    }

    private void Update()
    {
        _client.Update();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = _camera.ScreenPointToRay(eventData.position);
        _target = Physics2D.Raycast(ray.origin, ray.direction).transform;
        _client.SendInput(MessageType.OnPointerDown, _target.name, _target.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _client.SendInput(MessageType.OnPointerUp, _target.name, _target.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!_target) return;
        Vector3 v = _camera.ScreenToWorldPoint(eventData.position);
        v.z = _target.position.z;
        _target.position = v;
        _client.SendInput(MessageType.OnDrag, _target.name, v);
    }
}