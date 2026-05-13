using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class BuildingDrawer : MonoBehaviour
    {
        public IEnumerator AnimateDrawing(GameObject building, float animationDuration)
        {
            var buildingRenderer = building.GetComponent<LineRenderer>();
            var windowRenderers = new List<LineRenderer>();
            foreach (Transform child in building.transform)
                if (child.name.StartsWith("Window"))
                {
                    var lr = child.GetComponent<LineRenderer>();
                    if (lr != null) windowRenderers.Add(lr);
                }

            // Initially hide all parts by disabling the renderers
            if (buildingRenderer) buildingRenderer.enabled = false;
            foreach (var lr in windowRenderers) lr.enabled = false;

            // Animate building outline
            if (buildingRenderer)
            {
                buildingRenderer.enabled = true;
                var originalPositions = new Vector3[buildingRenderer.positionCount];
                buildingRenderer.GetPositions(originalPositions);
                yield return StartCoroutine(AnimateLine(buildingRenderer, originalPositions, animationDuration * 0.4f));
            }


            // Animate windows
            if (windowRenderers.Count > 0)
            {
                var windowAnimDuration = animationDuration * 0.6f / windowRenderers.Count;
                foreach (var windowLr in windowRenderers)
                    if (windowLr.gameObject.activeSelf)
                    {
                        windowLr.enabled = true;
                        var windowPositions = new Vector3[windowLr.positionCount];
                        windowLr.GetPositions(windowPositions);
                        yield return StartCoroutine(AnimateLine(windowLr, windowPositions, windowAnimDuration));
                    }
            }
        }

        private IEnumerator AnimateLine(LineRenderer lineRenderer, Vector3[] targetPositions, float duration)
        {
            if (targetPositions.Length < 2) yield break;

            var segmentDuration = duration / (targetPositions.Length - 1);
            lineRenderer.positionCount = targetPositions.Length;

            var animatedPositions = new Vector3[targetPositions.Length];

            for (var i = 0; i < targetPositions.Length - 1; i++)
            {
                var startTime = Time.time;
                var startPoint = targetPositions[i];
                var endPoint = targetPositions[i + 1];

                // Set previous points
                for (var j = 0; j <= i; j++) animatedPositions[j] = targetPositions[j];

                while (Time.time < startTime + segmentDuration)
                {
                    var t = (Time.time - startTime) / segmentDuration;
                    animatedPositions[i + 1] = Vector3.Lerp(startPoint, endPoint, t);
                    lineRenderer.SetPositions(animatedPositions);
                    yield return null;
                }

                animatedPositions[i + 1] = endPoint;
                lineRenderer.SetPositions(animatedPositions);
            }
        }

        public IEnumerator AnimateErasing(GameObject building, float animationDuration, Action onComplete)
        {
            var buildingRenderer = building.GetComponent<LineRenderer>();
            var windowRenderers = new List<LineRenderer>();
            foreach (Transform child in building.transform)
                if (child.name.StartsWith("Window"))
                {
                    var lr = child.GetComponent<LineRenderer>();
                    if (lr != null && lr.enabled) windowRenderers.Add(lr);
                }

            // Animate windows disappearing (in reverse order)
            if (windowRenderers.Count > 0)
            {
                var windowAnimDuration = animationDuration * 0.6f / windowRenderers.Count;
                for (var i = windowRenderers.Count - 1; i >= 0; i--)
                {
                    var windowLr = windowRenderers[i];
                    if (windowLr.gameObject.activeSelf)
                    {
                        var windowPositions = new Vector3[windowLr.positionCount];
                        windowLr.GetPositions(windowPositions);
                        yield return StartCoroutine(AnimateLineReverse(windowLr, windowPositions, windowAnimDuration));
                        windowLr.enabled = false;
                    }
                }
            }

            // Animate building outline disappearing
            if (buildingRenderer && buildingRenderer.enabled)
            {
                var originalPositions = new Vector3[buildingRenderer.positionCount];
                buildingRenderer.GetPositions(originalPositions);
                yield return StartCoroutine(AnimateLineReverse(buildingRenderer, originalPositions,
                    animationDuration * 0.4f));
                buildingRenderer.enabled = false;
            }

            onComplete?.Invoke();
        }

        private IEnumerator AnimateLineReverse(LineRenderer lineRenderer, Vector3[] targetPositions, float duration)
        {
            if (targetPositions.Length < 2) yield break;

            var segmentDuration = duration / (targetPositions.Length - 1);
            var animatedPositions = new Vector3[targetPositions.Length];
            lineRenderer.GetPositions(animatedPositions);

            for (var i = targetPositions.Length - 2; i >= 0; i--)
            {
                var startTime = Time.time;
                var startPoint = targetPositions[i + 1];
                var endPoint = targetPositions[i];

                while (Time.time < startTime + segmentDuration)
                {
                    var t = (Time.time - startTime) / segmentDuration;
                    animatedPositions[i + 1] = Vector3.Lerp(startPoint, endPoint, t);
                    lineRenderer.SetPositions(animatedPositions);
                    yield return null;
                }

                animatedPositions[i + 1] = endPoint;
                // For reverse, we effectively shorten the line by one point
                var finalSegmentPositions = new Vector3[i + 1];
                for (var j = 0; j <= i; j++) finalSegmentPositions[j] = animatedPositions[j];

                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPositions(finalSegmentPositions);
            }
        }
    }
}