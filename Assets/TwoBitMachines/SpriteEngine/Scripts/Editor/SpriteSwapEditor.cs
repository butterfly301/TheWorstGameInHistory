using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
    [CustomEditor(typeof(SpriteSwap), true)]
    public class SpriteSwapEditor : UnityEditor.Editor
    {
        public static string inputName = " Skin Name";
        public static string spriteName = " Sprite Name";
        public List<Sprite> tempSprites = new();
        public List<Texture2D> tempTexture2D = new();
        private SpriteSwap main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as SpriteSwap;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            var array = parent.Get("characterSkin");

            if (Fields.InputAndButtonBox("Create New Skin", "Add", Tint.Blue, ref inputName))
            {
                array.arraySize++;
                array.LastElement().Get("name").stringValue = inputName;
                inputName = " Skin Name";
            }

            for (var i = 0; i < array.arraySize; i++)
            {
                var characterSkin = array.Element(i);

                var open = characterSkin.Bool("foldOut");
                var deleteAsk = characterSkin.Bool("deleteAsk");

                if (
                    FoldOut.Bar(characterSkin, Tint.Orange, 0)
                    .Grip(parent, array, i, color: Tint.WarmWhite)
                    .LabelAndEdit("name", "edit", Color.white)
                    .RightButton("deleteAsk", "Delete", on: Tint.WarmWhite, off: Tint.WarmWhite, toolTip: "Delete Skin",
                        execute: open && !deleteAsk)
                    .RightButton("deleteAsk", "Close", "Return", execute: open && deleteAsk)
                    .RightButton("delete", "Yes", "Delete", execute: open && deleteAsk)
                    .RightButton("add", "Add", "Add Sprite", execute: open)
                    .FoldOut())
                {
                    if (characterSkin.ReadBool("delete"))
                    {
                        array.DeleteArrayElement(i);
                        break;
                    }

                    var skin = characterSkin.Get("skin");
                    if (characterSkin.ReadBool("add"))
                    {
                        skin.arraySize++;
                        skin.LastElement().Get("name").stringValue = spriteName;
                        skin.LastElement().Get("sprite").arraySize = 0;
                        spriteName = " Sprite Name"; //
                    }

                    for (var j = 0; j < skin.arraySize; j++) Skins(characterSkin, skin, skin.Element(j), j);
                }
            }

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
        }

        public void Skins(SerializedProperty characterSkin, SerializedProperty array, SerializedProperty skin, int i)
        {
            var open = skin.Bool("foldOut");
            var deleteAsk = skin.Bool("deleteAsk");

            if (
                FoldOut.Bar(skin, Tint.Box, 0)
                .Grip(characterSkin, array, i, color: Tint.WarmWhite)
                .LabelAndEdit("name", "edit", Color.white)
                .RightButton("deleteAsk", "Delete", on: Tint.WarmWhite, off: Tint.WarmWhite, toolTip: "Delete Sprite",
                    execute: open && !deleteAsk)
                .RightButton("deleteAsk", "Close", "Return", execute: open && deleteAsk)
                .RightButton("delete", "Yes", "Delete", execute: open && deleteAsk)
                .RightButton("replace", "DropCorner", "Replace Sprites", execute: open)
                .FoldOut())
            {
                if (skin.ReadBool("delete"))
                {
                    array.DeleteArrayElement(i);
                    return;
                }

                var sprite = skin.Get("sprite");

                if (sprite.arraySize == 0)
                {
                    CreateDragAndDropArea(sprite, "Add Sprites", Tint.WarmWhite);
                    TransferSprites(sprite, skin);
                }

                if (sprite.arraySize == 0) return;

                FoldOut.Box(sprite.arraySize, FoldOut.boxColor, offsetY: -2);
                {
                    for (var j = 0; j < sprite.arraySize; j++)
                    {
                        var element = sprite.Element(j);
                        Fields.ConstructField();
                        Fields.Grip(skin, sprite, j, color: Tint.WarmGrey);
                        Fields.ShowSprite((Sprite)element.objectReferenceValue, 16, 6);
                        element.ConstructField(S.FW - 54);

                        if (Fields.ConstructButton("xsAdd"))
                        {
                            sprite.InsertArrayElement(j);
                            break;
                        }

                        if (Fields.ConstructButton("xsMinus"))
                        {
                            sprite.DeleteArrayElement(j);
                            break;
                        }
                    }
                }
                Layout.VerticalSpacing(3);

                if (skin.Bool("replace"))
                {
                    CreateDragAndDropArea(sprite, "Replace Sprites", Tint.WarmWhite);
                    TransferSprites(sprite, skin);
                }
            }
        }

        private void CreateDragAndDropArea(SerializedProperty array, string message, Color color)
        {
            tempSprites.Clear();
            tempTexture2D.Clear();
            var dropArea = Layout.CreateRectAndDraw(Layout.longInfoWidth, 88, -11, -2, FoldOut.background, Tint.Box);
            {
                Fields.DropAreaGUI(dropArea, tempSprites);
                Fields.DropAreaGUI(dropArea, tempTexture2D);
                Labels.Centered(dropArea, message, color, 12, 15);

                var dropRect = dropArea.TextureCentered(Icon.Get("DropCorner"), new Vector2(22, 22), Tint.White, -10);
                if (array.arraySize == 0 && dropRect.ContainsMouseDown()) array.arraySize++;
            }
        }

        private void TransferSprites(SerializedProperty array, SerializedProperty skin)
        {
            if ((tempSprites.Count == 0 && tempTexture2D.Count == 0) || array == null) return;

            array.arraySize = 0;
            skin.SetFalse("replace");

            for (var i = 0; i < tempSprites.Count; i++)
            {
                array.arraySize++;
                array.LastElement().objectReferenceValue = tempSprites[i];
            }

            for (var i = 0; i < tempTexture2D.Count; i++)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(tempTexture2D[i]));
                if (sprite == null)
                    continue;
                array.arraySize++;
                array.LastElement().objectReferenceValue = sprite;
            }
        }
    }
}