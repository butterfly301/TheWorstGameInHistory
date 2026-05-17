using UnityEngine;

namespace HotUpdate.Utility
{
    public class GetMainCamera : MonoBehaviour
    {
        private Canvas canvas;

private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

private void OnEnable()
        {
            GetMainCameraAndRegisterToCanvas();
        }

public void GetMainCameraAndRegisterToCanvas()
        {
            canvas.worldCamera = Camera.main;
        }
    }
}