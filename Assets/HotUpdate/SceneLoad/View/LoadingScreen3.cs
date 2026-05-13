using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace HotUpdate.UI
{
    public class LoadingScreen3 : LoadingScreen
    {
        private const string StringTableName = "String Table";

        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private string key = "Carrying out an evening raid...";
        [SerializeField] private float loadingTime = 2.5f;

        private Coroutine fakeLoadingCoroutine;

        public override void Init()
        {
            base.Init();
            StartFakeLoading();
        }

        private void OnDisable()
        {
            StopFakeLoading();
        }

        private void StartFakeLoading()
        {
            StopFakeLoading();
            UpdateProgressUI(0f);
            fakeLoadingCoroutine = StartCoroutine(FakeLoadingRoutine());
        }

        private void StopFakeLoading()
        {
            if (fakeLoadingCoroutine == null)
            {
                return;
            }

            StopCoroutine(fakeLoadingCoroutine);
            fakeLoadingCoroutine = null;
        }

        private IEnumerator FakeLoadingRoutine()
        {
            if (loadingTime <= 0f)
            {
                UpdateProgressUI(1f);
                fakeLoadingCoroutine = null;
                yield break;
            }

            float elapsedTime = 0f;
            float firstPhaseDuration = loadingTime * 0.7f;
            float secondPhaseDuration = loadingTime - firstPhaseDuration;

            while (elapsedTime < loadingTime)
            {
                elapsedTime += Time.deltaTime;

                float progress;
                if (elapsedTime <= firstPhaseDuration)
                {
                    float phaseProgress = firstPhaseDuration <= 0f ? 1f : elapsedTime / firstPhaseDuration;
                    progress = Mathf.Lerp(0f, 0.6f, phaseProgress);
                }
                else
                {
                    float secondPhaseTime = elapsedTime - firstPhaseDuration;
                    float phaseProgress = secondPhaseDuration <= 0f ? 1f : secondPhaseTime / secondPhaseDuration;
                    progress = Mathf.Lerp(0.6f, 1f, phaseProgress);
                }

                UpdateProgressUI(Mathf.Clamp01(progress));
                yield return null;
            }

            UpdateProgressUI(1f);
            fakeLoadingCoroutine = null;
        }

        private void UpdateProgressUI(float progress)
        {
            if (progressFill != null)
            {
                progressFill.fillAmount = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{GetLocalizedLoadingText()}   {Mathf.RoundToInt(progress * 100f)}%";
            }
        }

        private string GetLocalizedLoadingText()
        {
            var stringTable = LocalizationSettings.StringDatabase.GetTable(StringTableName, LocalizationSettings.SelectedLocale);
            if (stringTable == null)
            {
                return key;
            }

            var entry = stringTable.GetEntry(key);
            return entry == null ? key : entry.GetLocalizedString();
        }
    }
}
