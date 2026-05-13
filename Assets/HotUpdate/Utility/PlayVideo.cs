using HotUpdate.Video;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class PlayVideo : MonoBehaviour
    {
        public void PlayVideoFun()
        {
            VideoManager.Instance.Play();
        }
    }
}