using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StandaloneUIEffects
{
    [RequireComponent(typeof(Graphic))]
    public class UIMirror : BaseMeshEffect
    {
        public enum MirrorType
        {
            Horizontal,
            Vertical,
            Quarter,
        }

        [SerializeField]
        private MirrorType m_MirrorType = MirrorType.Horizontal;

        [System.NonSerialized]
        private RectTransform m_RectTransform;

        public MirrorType mirrorType
        {
            get => m_MirrorType;
            set
            {
                if (m_MirrorType == value)
                {
                    return;
                }

                m_MirrorType = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }

                return m_RectTransform;
            }
        }

        public static UIMirror Get(GameObject target)
        {
            if (target == null)
            {
                return null;
            }

            var mirror = target.GetComponent<UIMirror>();
            return mirror != null ? mirror : target.AddComponent<UIMirror>();
        }

        public static UIMirror GetFrom(GameObject root, string childPath)
        {
            if (root == null || string.IsNullOrEmpty(childPath))
            {
                return null;
            }

            var child = root.transform.Find(childPath);
            return child == null ? null : Get(child.gameObject);
        }

        public void SetNativeSize()
        {
            if (!(graphic is Image image))
            {
                return;
            }

            var sprite = image.overrideSprite;
            if (sprite == null)
            {
                return;
            }

            float width = sprite.rect.width / image.pixelsPerUnit;
            float height = sprite.rect.height / image.pixelsPerUnit;

            rectTransform.anchorMax = rectTransform.anchorMin;

            switch (m_MirrorType)
            {
                case MirrorType.Horizontal:
                    rectTransform.sizeDelta = new Vector2(width * 2f, height);
                    break;
                case MirrorType.Vertical:
                    rectTransform.sizeDelta = new Vector2(width, height * 2f);
                    break;
                case MirrorType.Quarter:
                    rectTransform.sizeDelta = new Vector2(width * 2f, height * 2f);
                    break;
            }

            graphic.SetVerticesDirty();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            var vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);

            int count = vertices.Count;

            if (graphic is Image image)
            {
                switch (image.type)
                {
                    case Image.Type.Simple:
                        DrawSimple(vertices, count);
                        break;
                    case Image.Type.Sliced:
                        DrawSliced(vertices, count);
                        break;
                    case Image.Type.Tiled:
                    case Image.Type.Filled:
                        break;
                }
            }
            else
            {
                DrawSimple(vertices, count);
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }

        private void DrawSimple(List<UIVertex> vertices, int count)
        {
            Rect rect = graphic.GetPixelAdjustedRect();
            SimpleScale(rect, vertices, count);

            switch (m_MirrorType)
            {
                case MirrorType.Horizontal:
                    EnsureCapacity(vertices, count);
                    MirrorVerts(rect, vertices, count, true);
                    break;
                case MirrorType.Vertical:
                    EnsureCapacity(vertices, count);
                    MirrorVerts(rect, vertices, count, false);
                    break;
                case MirrorType.Quarter:
                    EnsureCapacity(vertices, count * 3);
                    MirrorVerts(rect, vertices, count, true);
                    MirrorVerts(rect, vertices, count * 2, false);
                    break;
            }
        }

        private void DrawSliced(List<UIVertex> vertices, int count)
        {
            var image = graphic as Image;
            if (image == null)
            {
                return;
            }

            if (!image.hasBorder)
            {
                DrawSimple(vertices, count);
                return;
            }

            Rect rect = graphic.GetPixelAdjustedRect();
            SlicedScale(rect, vertices, count);
            count = SliceExcludeVerts(vertices, count);

            switch (m_MirrorType)
            {
                case MirrorType.Horizontal:
                    EnsureCapacity(vertices, count);
                    MirrorVerts(rect, vertices, count, true);
                    break;
                case MirrorType.Vertical:
                    EnsureCapacity(vertices, count);
                    MirrorVerts(rect, vertices, count, false);
                    break;
                case MirrorType.Quarter:
                    EnsureCapacity(vertices, count * 3);
                    MirrorVerts(rect, vertices, count, true);
                    MirrorVerts(rect, vertices, count * 2, false);
                    break;
            }
        }

        private static void EnsureCapacity(List<UIVertex> vertices, int addCount)
        {
            int neededCapacity = vertices.Count + addCount;
            if (vertices.Capacity < neededCapacity)
            {
                vertices.Capacity = neededCapacity;
            }
        }

        private void SimpleScale(Rect rect, List<UIVertex> vertices, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var vertex = vertices[i];
                Vector3 position = vertex.position;

                if (m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
                {
                    position.x = (position.x + rect.x) * 0.5f;
                }

                if (m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
                {
                    position.y = (position.y + rect.y) * 0.5f;
                }

                vertex.position = position;
                vertices[i] = vertex;
            }
        }

        private void SlicedScale(Rect rect, List<UIVertex> vertices, int count)
        {
            Vector4 border = GetAdjustedBorders(rect);
            float halfWidth = rect.width * 0.5f;
            float halfHeight = rect.height * 0.5f;

            for (int i = 0; i < count; i++)
            {
                var vertex = vertices[i];
                Vector3 position = vertex.position;

                if (m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
                {
                    if (halfWidth < border.x && position.x >= rect.center.x)
                    {
                        position.x = rect.center.x;
                    }
                    else if (position.x >= border.x)
                    {
                        position.x = position.x + rect.x;
                    }
                }

                if (m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
                {
                    if (halfHeight < border.y && position.y >= rect.center.y)
                    {
                        position.y = rect.center.y;
                    }
                    else if (position.y >= border.y)
                    {
                        position.y = (position.y + rect.y) * 0.5f;
                    }
                }

                vertex.position = position;
                vertices[i] = vertex;
            }
        }

        private static void MirrorVerts(Rect rect, List<UIVertex> vertices, int count, bool isHorizontal)
        {
            for (int i = 0; i < count; i++)
            {
                var vertex = vertices[i];
                Vector3 position = vertex.position;

                if (isHorizontal)
                {
                    position.x = rect.center.x * 2f - position.x;
                }
                else
                {
                    position.y = rect.center.y * 2f - position.y;
                }

                vertex.position = position;
                vertices.Add(vertex);
            }
        }

        private static int SliceExcludeVerts(List<UIVertex> vertices, int count)
        {
            int realCount = count;
            int index = 0;

            while (index < realCount)
            {
                UIVertex v1 = vertices[index];
                UIVertex v2 = vertices[index + 1];
                UIVertex v3 = vertices[index + 2];

                if (v1.position == v2.position || v2.position == v3.position || v3.position == v1.position)
                {
                    vertices[index] = vertices[realCount - 3];
                    vertices[index + 1] = vertices[realCount - 2];
                    vertices[index + 2] = vertices[realCount - 1];
                    realCount -= 3;
                    continue;
                }

                index += 3;
            }

            if (realCount < count)
            {
                vertices.RemoveRange(realCount, count - realCount);
            }

            return realCount;
        }

        private Vector4 GetAdjustedBorders(Rect rect)
        {
            var image = graphic as Image;
            Vector4 border = Vector4.zero;

            if (image != null && image.overrideSprite != null)
            {
                border = image.overrideSprite.border / image.pixelsPerUnit;
            }

            for (int axis = 0; axis <= 1; axis++)
            {
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders > 0f)
                {
                    float scaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= scaleRatio;
                    border[axis + 2] *= scaleRatio;
                }
            }

            return border;
        }
    }
}
