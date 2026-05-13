using System;
using System.Text;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private UnityEventString time = new();
        [NonSerialized] private readonly StringBuilder timeStringBuilder = new(); // Create the StringBuilder
        [NonSerialized] public float gameTime;
        [NonSerialized] public bool pause;

        public void LateUpdate()
        {
            if (pause) return;
            gameTime += Time.deltaTime;

            var minutes = Mathf.FloorToInt(gameTime / 60);
            var seconds = Mathf.FloorToInt(gameTime % 60);
            timeStringBuilder.Clear();
            timeStringBuilder.AppendFormat("{0:00}:{1:00}", minutes, seconds);
            time.Invoke(timeStringBuilder.ToString());
        }

        public void Pause(bool value)
        {
            pause = value;
        }

        public float GameTime()
        {
            return gameTime;
        }
    }
}