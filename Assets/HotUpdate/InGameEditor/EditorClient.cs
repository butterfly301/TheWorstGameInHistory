using Telepathy;
using UnityEngine;

namespace InGameEditor
{
    public class EditorClient
    {
        private readonly Client _client;
        private EditInput _input;

        public EditorClient(EditInput input)
        {
            _input = input;
            _client = new Client(1024);
            _client.Connect("127.0.0.1", 11451);
        }

        public void Update()
        {
            _client.Tick(128);
        }
        
        public void SendInput(MessageType type, string name, Vector3 pos)
        {
            MyModal modal = new MyModal()
            {
                Type = type,
                Name = name,
                Pos = pos
            };
            _client.Send(MyConverter.String2Byte(JsonUtility.ToJson(modal)));
        }
    }
}