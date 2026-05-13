using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/ManageScenes")]
    public class ManageScenes : MonoBehaviour
    {
        public enum LoadState
        {
            wait,
            Begin
        }

        public static bool enteringScene;
        [SerializeField] private string nextSceneName = "";
        [SerializeField] private string menuName = "";
        [SerializeField] private LoadSceneOn loadSceneOn;
        [SerializeField] private PauseGameType pause;
        [SerializeField] private List<LoadStep> step = new();
        [SerializeField] private List<Texture2D> text = new();
        [NonSerialized] private string sceneName = "";
        [NonSerialized] private LoadState state = LoadState.wait;

        [NonSerialized] private int stepIndex;

        public void Update()
        {
            if (state == LoadState.wait)
                return; //|| loading == null) return;

            for (var i = stepIndex; i < step.Count; i++)
            {
                if (step[i].Play(loadSceneOn, text, sceneName))
                {
                    stepIndex++;
                    continue;
                }

                return;
            }
        }

#if UNITY_EDITOR
        public void OnDestroy()
        {
            for (var i = 0; i < step.Count; i++)
                if (step[i].image != null)
                    step[i].image.material.SetFloat("_CutOff", -0.1f);
        }
#endif

        public void LoadScene(string sceneName)
        {
            if (SceneNameValid(sceneName)) BeginSceneLoad(sceneName);
        }

        public void LoadNextScene()
        {
            if (SceneNameValid(nextSceneName)) BeginSceneLoad(nextSceneName);
        }

        public void LoadMenu()
        {
            if (SceneNameValid(menuName)) BeginSceneLoad(menuName);
        }

        private bool SceneNameValid(string sceneName)
        {
            for (var i = 0; i < Util.SceneCount(); i++)
                if (sceneName == Util.GetSceneName(i))
                    return true;

            return false;
        }

        private void BeginSceneLoad(string name)
        {
            if (state == LoadState.Begin) return;
            if (pause == PauseGameType.PauseGame) WorldManager.get.PauseNoInvoke();
            for (var i = 0; i < step.Count; i++) step[i].Reset();

            stepIndex = 0;
            sceneName = name;
            state = LoadState.Begin;
            enteringScene = true;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool add;
        [SerializeField] [HideInInspector] private bool active;
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool randomFoldOut;
        [SerializeField] [HideInInspector] private int signalIndex;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class LoadStep
    {
        [SerializeField] private LoadStepType type;
        [SerializeField] private GameObject gameObject;

        [SerializeField] private float time = 1f;
        [SerializeField] private float loadSpeed = 1f;
        [SerializeField] private DeactivateStep deactivate = DeactivateStep.LeaveAsIs;

        [SerializeField] private UnityEvent onStart;
        [SerializeField] private UnityEvent onComplete;
        [SerializeField] private UnityEventFloat loadingProgress;
        [SerializeField] private UnityEventString loadingProgressString;
        [NonSerialized] private float counter;
        [NonSerialized] private bool firstFrame = true;

        [NonSerialized] public Image image;
        [NonSerialized] private bool loadComplete;
        [NonSerialized] private AsyncOperation loading;
        [NonSerialized] private float loadingProgressTarget;

        public void Reset()
        {
            counter = 0;
            firstFrame = true;
            loadComplete = false;
            loadingProgressTarget = 0;
        }

        public bool Play(LoadSceneOn loadSceneOn, List<Texture2D> text, string sceneName)
        {
            if (type == LoadStepType.TransitionIn) return TransitionIn(text);
            if (type == LoadStepType.TransitionOut) return TransitionOut(text);

            return LoadScene(loadSceneOn, sceneName);
        }

        private bool TransitionIn(List<Texture2D> text)
        {
            if (firstFrame)
            {
                firstFrame = false;

                if (onStart.GetPersistentEventCount() > 0) onStart.Invoke();
                if (gameObject != null) gameObject.SetActive(true);
                if (gameObject != null) image = gameObject.GetComponentInChildren<Image>(true);
                if (image != null && text.Count > 0)
                {
                    var newTransition = Random.Range(0, text.Count);
                    if (text[newTransition] != null) image.material.SetTexture("_Transition", text[newTransition]);
                }
            }

            var value = Compute.LerpUnscaled(-0.1f, 1.1f, time, ref counter, out var complete);

            if (image != null && image.material != null) image.material.SetFloat("_CutOff", value);
            if (complete)
            {
                if (onComplete.GetPersistentEventCount() > 0) onComplete.Invoke();
                if (deactivate == DeactivateStep.Deactivate && gameObject != null) gameObject.SetActive(false);
            }

            return complete;
        }

        private bool TransitionOut(List<Texture2D> text)
        {
            if (firstFrame)
            {
                firstFrame = false;

                if (onStart.GetPersistentEventCount() > 0) onStart.Invoke();
                if (gameObject != null) gameObject.SetActive(true);
                if (gameObject != null) image = gameObject.GetComponentInChildren<Image>(true);
                if (image != null && text.Count > 0)
                {
                    var newTransition = Random.Range(0, text.Count);
                    if (text[newTransition] != null) image.material.SetTexture("_Transition", text[newTransition]);
                }
            }

            var value = Compute.LerpUnscaled(1.1f, -0.1f, time, ref counter, out var complete);
            if (image != null && image.material != null) image.material.SetFloat("_CutOff", value);
            if (complete)
            {
                if (onComplete.GetPersistentEventCount() > 0) onComplete.Invoke();
                if (deactivate == DeactivateStep.Deactivate && gameObject != null) gameObject.SetActive(false);
            }

            return complete;
        }

        private bool LoadScene(LoadSceneOn loadSceneOn, string sceneName)
        {
            if (firstFrame)
            {
                firstFrame = false;
                loading = SceneManager.LoadSceneAsync(sceneName);
                loading.allowSceneActivation = false;

                if (onStart.GetPersistentEventCount() > 0) onStart.Invoke();
                if (gameObject != null) gameObject.SetActive(true);
            }

            var progressTarget = loading.progress / 0.9f;
            loadingProgressTarget =
                Mathf.MoveTowards(loadingProgressTarget, progressTarget, Time.unscaledDeltaTime * loadSpeed);
            loadingProgress.Invoke(loadingProgressTarget);

            if (loadingProgressString.GetPersistentEventCount() > 0)
                loadingProgressString.Invoke(Mathf.Ceil(loadingProgressTarget * 100f) + "%");

            if (loadingProgressTarget < progressTarget) return false;
            if (loadSceneOn == LoadSceneOn.OnUserInput)
            {
                loadingProgress.Invoke(1f);
                loadingProgressString.Invoke("100%");
                if (!loadComplete) onComplete.Invoke();
            }

            loadComplete = true;
            if (loadSceneOn == LoadSceneOn.Automatically || Input.anyKeyDown || Input.touchCount > 0)
            {
                loading.allowSceneActivation = true;
                // WorldManager.get.Unpause();
                // Time.timeScale = 1f;
                if (loadSceneOn == LoadSceneOn.Automatically)
                    onComplete.Invoke();
                return true;
            }

            return false;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private bool foldOut;
        [SerializeField] private bool eventFoldOut;
        [SerializeField] private bool startFoldOut;
        [SerializeField] private bool completeFoldOut;
        [SerializeField] private bool stringFoldOut;
        [SerializeField] private bool loadingFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum LoadSceneOn
    {
        Automatically,
        OnUserInput
    }

    public enum LoadStepType
    {
        TransitionIn,
        TransitionOut,
        LoadScene
    }

    public enum TransitionType
    {
        TransitionIn,
        TransitionOut,
        Both
    }

    public enum DeactivateStep
    {
        Deactivate,
        LeaveAsIs
    }
}