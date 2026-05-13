using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [Serializable]
    public class LadderInstance
    {
        [SerializeField] public Vector2 size = new(1, 5);
        [SerializeField] public Vector2 bottomLeft;

        [SerializeField] public bool canJumpUp;
        [SerializeField] public bool stopSideJump;
        [SerializeField] public bool standOnLadder = true;
        [SerializeField] public bool alignToCenter = true;
        [SerializeField] public bool editorCheck;
        [SerializeField] public bool foldOut;

        public float bottom => rawPosition.y;
        public float top => rawPosition.y + size.y;
        public float right => rawPosition.x + size.x;
        public float left => rawPosition.x;
        public Vector2 rawPosition => target.position + startOffset;

        public Transform target { get; private set; }
        public Vector3 startOffset { get; private set; }

        public void InitializeToTarget(Transform targetRef)
        {
            target = targetRef;
            startOffset = bottomLeft - (Vector2)targetRef.position;
        }

        public float CenterX()
        {
            return rawPosition.x + size.x * 0.5f;
        }

        public bool ContainX(float x)
        {
            return x >= rawPosition.x && x <= right;
        }

        public bool ContainsY(float y)
        {
            return y <= top && y >= bottom;
        }

        public void SetPositionAndSize(Vector2 position, Vector2 sizeRef)
        {
            size = sizeRef;
            SetPosition(position);
        }

        public void SetPosition(Vector2 position)
        {
            bottomLeft = position - new Vector2(size.x, size.y) * 0.5f;
        }

        #region Draw

#if UNITY_EDITOR
        public void SetPositionAndDraw(Vector2 position)
        {
            SetPosition(position);
            Draw(bottomLeft);
        }

        public void Draw(Vector2 bottomLeft)
        {
            var topLeft = bottomLeft + Vector2.up * size.y;
            var topRight = topLeft + Vector2.right * size.x;
            var bottomRight = topRight - Vector2.up * size.y;

            var color = Application.isPlaying ? Color.blue : Color.yellow;

            Debug.DrawLine(bottomLeft, topLeft, color);
            Debug.DrawLine(topRight, topLeft, color);
            Debug.DrawLine(topRight, bottomRight, color);
            Debug.DrawLine(bottomLeft, bottomRight, color);
        }
#endif

        #endregion
    }
}