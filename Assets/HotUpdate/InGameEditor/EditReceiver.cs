using System;
using UnityEngine;

public class EditReceiver : MonoBehaviour
{
    private MainGameServer _gameServer;
    private Transform _target;
    private Camera _camera;

    private void Start()
    {
        _gameServer = new MainGameServer(this);
        _camera = Camera.main;
    }

    private void Update()
    {
        _gameServer.Update();
    }

    public void SelectGameObject(string objName)
    {
        _target = GameObject.Find(objName).transform;
    }

    public void EndDrag(Vector3 pos)
    {
        if(!_target) return;
    }

    public void OnDrag(Vector3 pos)
    {
        if(!_target) return;
        _target.position = pos;
    }
}