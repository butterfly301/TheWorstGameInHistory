using System;
using InGameEditor;
using UnityEngine;
using Telepathy;

public class MainGameServer
{
    private readonly Server _server;
    private EditReceiver _receiver;

    public MainGameServer(EditReceiver receiver)
    {
        _receiver = receiver;
        _server = new Server(1024)
        {
            OnData = (i, bytes) =>
                ProcessData(JsonUtility.FromJson<MyModal>(MyConverter.Byte2String(bytes)))
        };
        _server.OnConnected = (_, _) => Debug.Log("conn");
        _server.Start(11451);
    }

    public void Update()
    {
        _server.Tick(128);
    }

    private void ProcessData(MyModal modal)
    {
        switch (modal.Type)
        {
            case MessageType.OnPointerDown:
                _receiver.SelectGameObject(modal.Name);
                break;
            case MessageType.OnPointerUp:
                _receiver.EndDrag(modal.Pos);
                break;
            case MessageType.OnDrag:
                _receiver.OnDrag(modal.Pos);
                break;
        }
    }
}