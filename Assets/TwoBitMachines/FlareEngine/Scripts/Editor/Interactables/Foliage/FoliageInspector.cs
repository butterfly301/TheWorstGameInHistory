using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class FoliageInspector
    {
        public static void Properties(SerializedProperty foliage, SerializedObject parent)
        {
            if (FoldOut.Bar(parent, Tint.Blue).Label("Foliage", Color.white).FoldOut())
            {
                FoldOut.Box(3, FoldOut.boxColor);
                foliage.Slider("Jiggle", "frequency", 0.1f, 5f);
                foliage.Slider("Damping", "damping", 0.01f, 5f);
                foliage.Slider("Uniformity", "uniformity");
                Layout.VerticalSpacing(5);

                FoldOut.Box(2, FoldOut.boxColor);
                foliage.Slider("Wind Strength", "windStrength");
                foliage.Slider("Wind Frequency", "windFrequency", 0.1f, 5f);
                Layout.VerticalSpacing(5);
            }
        }

        public static void TextureBoxes(SerializedObject parent, SerializedProperty textures,
            SerializedProperty textSize, SerializedProperty ppu, UnityEditor.Editor editor)
        {
            if (textures.arraySize == 0)
                return;

            var textRect = new Rect();
            Layout.VerticalSpacing();
            var textureIndex = parent.Get("index");
            var brushIndex = parent.Get("brushes");

            var
                space = 10; //                                                                                                                            space between textures;
            var
                maxSize = 50; //                                                                                                                          max size of texture in inspector
            float xPosition = Layout.longInfoWidth;
            var min = Mathf.Min(Mathf.FloorToInt(maxSize / textSize.vector2IntValue.x),
                Mathf.FloorToInt(maxSize / textSize.vector2IntValue.y));
            var size = min <= 1
                ? new Vector2(maxSize, maxSize)
                : new Vector2(textSize.vector2IntValue.x * min, textSize.vector2IntValue.y * min);
            var colY = Mathf.Clamp(Mathf.FloorToInt((Layout.longInfoWidth - 20f) / (size.x + 10f)), 1f,
                10000f); //                           20 is for side padding
            var height = Mathf.CeilToInt(textures.arraySize / colY);
            Layout.GetLastRectDraw(Layout.longInfoWidth, height * (size.y + 10) + 10, -11, -2,
                Icon.Get("BackgroundLight128x128"), FoldOut.boxColor);

            for (var i = 0; i < textures.arraySize; i++)
            {
                if (xPosition + size.x >= Layout.longInfoWidth)
                {
                    Layout.VerticalSpacing(space);
                    textRect = Layout.CreateRect(size.x, size.y, xPosition = space + 10);
                }

                textRect.x = xPosition;
                xPosition += size.x + space;
                var selectRect = textRect.ExpandRect(2);
                var element = textures.Element(i);
                var texture = element.Get("texture").objectReferenceValue;

                if (textureIndex.intValue == i && brushIndex.intValue != 1)
                    Skin.Draw(selectRect, Tint.WarmWhite, Skin.round);
                if (brushIndex.intValue == 1 && element.Bool("isRandom"))
                    Skin.Draw(selectRect, Tint.Purple, Skin.round);
                if (textRect.ContainsMouseDown(false))
                {
                    textureIndex.intValue = i;
                    if (brushIndex.intValue == 1)
                        element.Toggle("isRandom");
                }

                if (texture == null)
                {
                    element.Get("texture").objectReferenceValue =
                        EditorGUI.ObjectField(textRect, texture, typeof(Texture2D), false);
                }
                else
                {
                    Skin.Draw(textRect, Tint.LightGrey, Skin.round);
                    GUI.DrawTexture(textRect, (Texture2D)texture);
                }
            }

            Layout.VerticalSpacing(7);
        }

        public static void TextureProperties(SerializedProperty index, SerializedProperty textures,
            SerializedProperty foliage, ref bool refresh)
        {
            for (var i = 0; i < textures.arraySize; i++)
            {
                if (index.intValue != i)
                    continue;

                FoldOut.Box(4, FoldOut.boxColor);
                {
                    var element = textures.Element(i);

                    var depth = element.Get("z").floatValue;
                    var orientation = element.Get("orientation").boolValue;
                    var texture = element.Get("texture").objectReferenceValue;

                    if (element.FieldAndButton("Texture2D", "texture", "Delete"))
                    {
                        textures.DeleteArrayElement(i);
                        DeleteFoliageInstances(foliage, i, textures.arraySize);
                        return;
                    }

                    element.Field("Orientation", "orientation");
                    element.Field("Depth", "z");
                    element.Slider("Interaction", "interact");

                    if (orientation != element.Get("orientation").boolValue || (depth != element.Get("z").floatValue) |
                        (texture != element.Get("texture").objectReferenceValue)) refresh = true;
                }
                Layout.VerticalSpacing(5);
                return;
            }
        }

        private static void DeleteFoliageInstances(SerializedProperty foliage, int index, int textureArraySize)
        {
            for (var i = foliage.arraySize - 1; i >= 0; i--)
                if (foliage.Element(i).Get("textureIndex").intValue == index)
                    foliage.DeleteArrayElement(
                        i); //                                                                delete all foliage instances mapped to this texture

            for (var i = 0; i < foliage.arraySize; i++)
            {
                var textureIndex = foliage.Element(i).Get("textureIndex");
                if (textureIndex.intValue > index)
                    textureIndex.intValue =
                        Mathf.Clamp(textureIndex.intValue - 1, 0,
                            textureArraySize - 1); //      reset index for all foliage instances
            }
        }

        public static void PaintBar(SerializedObject parent, SerializedProperty brushIndex)
        {
            Bar.SetupTabBar(3);
            Bar.TabButton(brushIndex, 0, "PaintBrush", "LeftBar", Tint.Orange, Tint.BoxTwo, "Paint Brush");
            Bar.TabButton(brushIndex, 1, "RandomPaintBrush", "MiddleBar", Tint.Orange, Tint.BoxTwo,
                "Random Paint Brush");
            Bar.TabButton(brushIndex, 2, "Eraser", "RightBar", Tint.Orange, Tint.BoxTwo, "Eraser");

            if (brushIndex.intValue == 1)
            {
                FoldOut.BoxSingle(1, Tint.Orange);
                parent.Slider("Density", "randomDensity", 1f, 5f);
                parent.Round("randomDensity", 1f);
                Layout.VerticalSpacing(2);
            }

            if (brushIndex.intValue == 2)
            {
                FoldOut.BoxSingle(1, Tint.Orange);
                parent.Slider("Radius", "deleteRadius", 0.1f, 10f);
                Layout.VerticalSpacing(2);
            }
        }
    }
}