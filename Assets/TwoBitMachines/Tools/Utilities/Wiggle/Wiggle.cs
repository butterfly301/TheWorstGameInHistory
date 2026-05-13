using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines
{
    //Tween Library
    public class Wiggle : MonoBehaviour
    {
        public static List<TweenParent> activeTween = new(); //   active list
        public static Stack<TweenParent> parents = new(); //     inactive list, not in use, recycle
        public static Stack<TweenChild> children = new(); //      inactive list, not in use, recycle
        public static List<TweenParent> temporaryTween = new();
        public static Wiggle instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Update()
        {
            if (instance != this) return;

            for (var i = activeTween.Count - 1; i >= 0; i--)
            {
                if (activeTween[i].obj == null)
                {
                    Remove(activeTween[i]);
                    continue;
                }

                activeTween[i].Run();
            }

            for (var i = temporaryTween.Count - 1; i >= 0; i--)
                if (temporaryTween[i].obj != null && temporaryTween[i].Run())
                    temporaryTween.RemoveAt(i);
        }

        public static void AddTemporaryTween(TweenParent tweenParent)
        {
            if (!temporaryTween.Contains(tweenParent)) temporaryTween.Add(tweenParent);
        }

        public static TweenParent Target(GameObject obj, bool useAnchors = false)
        {
            var tweenParent = parents.Count > 0 ? parents.Pop() : new TweenParent();
            tweenParent.Reset(obj);
            tweenParent.useAnchors = useAnchors;
            activeTween.Add(tweenParent);
            return tweenParent;
        }

        public static TweenChild GetTweenChild()
        {
            return children.Count > 0 ? children.Pop() : new TweenChild();
        }

        public static void Remove(TweenParent tweenParent)
        {
            TransferChildren(tweenParent.children); // transfer children
            parents.Push(tweenParent); //              transfer parent
            activeTween.Remove(tweenParent);
        }

        public static void TransferChildren(List<TweenChild> children)
        {
            for (var i = 0; i < children.Count; i++) Wiggle.children.Push(children[i]);
            children.Clear();
        }

        private static TweenParent GetActiveTween(GameObject obj)
        {
            for (var i = activeTween.Count - 1; i >= 0; i--)
                if (activeTween[i].obj == obj)
                    return activeTween[i];
            return null;
        }

        public static void StopTween(GameObject gameobject)
        {
            var activeTween = GetActiveTween(gameobject);
            if (activeTween != null) Remove(activeTween);
        }

        public static void StopAllTweens()
        {
            for (var i = activeTween.Count - 1; i >= 0; i--) Remove(activeTween[i]);
        }

        public static void PauseTween(GameObject obj)
        {
            var activeTween = GetActiveTween(obj);
            if (activeTween != null) activeTween.active = false;
        }

        public static void UnpauseTween(GameObject obj)
        {
            var activeTween = GetActiveTween(obj);
            if (activeTween != null) activeTween.active = true;
        }

        public static void PauseAllTweens(bool value)
        {
            for (var i = activeTween.Count - 1; i >= 0; i--) activeTween[i].active = value;
        }

        public static bool IsTweenActive(GameObject obj)
        {
            for (var i = activeTween.Count - 1; i >= 0; i--)
                if (activeTween[i].obj == obj)
                    return activeTween[i].active;
            return false;
        }
    }
}