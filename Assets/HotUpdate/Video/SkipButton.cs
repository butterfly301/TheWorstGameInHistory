using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Video
{
    public class SkipButton : MonoBehaviour
    {
        private Button btn;

        public void Init()
        {
            VideoManager.Instance.SetHasSkipButton(true);
            btn = GetComponent<Button>();
            btn.onClick.AddListener(OnSkipButtonClicked);
        }

        protected virtual void OnSkipButtonClicked()
        {
            VideoManager.Instance.PauseVideo();
            VideoManager.Instance.SetHasSkipButton(false);
        }
    }
}