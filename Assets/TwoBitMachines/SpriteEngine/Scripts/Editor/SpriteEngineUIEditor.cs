using System.Collections.Generic;
using System.Linq;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TwoBitMachines.TwoBitSprite.Editors
{
    [CustomEditor(typeof(SpriteEngineUI), true)]
    [CanEditMultipleObjects]
    public class SpriteEngineEditorUI : UnityEditor.Editor
    {
        public SpriteEngineUI main;
        public List<string> spriteNames = new();

        //Play in Scene
        public Sprite originalSprite;
        public Sprite currentSprite;
        public SpritePacket tempSprite;
        public Image render;
        public int currentFrameIndex;
        public float timer;
        public bool propertyOpen;
        public string previousSprite;
        public List<Sprite> tempSprites = new();
        public List<Texture2D> tempTexture2D = new();
        public SerializedProperty frameIndex;

        public SerializedProperty frames;
        public SerializedObject parent;
        public SerializedProperty property;
        public SerializedProperty scrollIndex;
        public SerializedProperty sprite;
        public SerializedProperty spriteIndex;
        public SerializedProperty sprites;
        public bool spriteExists => sprite != null && frames != null;

        private void OnEnable()
        {
            main = target as SpriteEngineUI;
            parent = serializedObject;
            Layout.Initialize(this);
            SpriteProperty.PropertyIcons();
            SpriteProperty.NameList();
        }

        private void OnDisable()
        {
            StopPlayEditorOnly();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);

            serializedObject.Update();
            {
                SetCurrentSprite();
                PlayButtons();
                PreviewWindow.Display(this, parent, ref currentSprite, main.sprites, main.spriteIndex);

                if (sprites.arraySize == 0)
                {
                    sprites.CreateNewElement().FindPropertyRelative("name").stringValue = "New Sprite";
                    spriteIndex.intValue = sprites.arraySize - 1;
                    SetCurrentSprite();
                }

                if (spriteExists)
                {
                    var open = Block.Header(parent).Style(Tint.SoftDark)
                        .Fold("Sprites", bold: true, color: Tint.White)
                        .TabButton("tabIndex", 3, "Wrench", "Properties")
                        .TabButton("tabIndex", 2, "Signal", "Extra Options")
                        .TabButton("tabIndex", 1, "Event", "Sprite Options")
                        .TabButton("tabIndex", 0, "DropCorner", "Replace Sprites")
                        .Build();
                    if (open)
                    {
                        if (parent.Int("tabIndex") == 2)
                        {
                            Block.Box(2, Tint.SoftDark);
                            {
                                parent.Field_("Sprite Swap", "spriteSwap");
                                parent.FieldToggleAndEnable_("Initialize To First Animation", "setToFirst");
                            }
                        }

                        CreateSprite();

                        InspectSprite.Display(this, parent, frames, frameIndex, sprite);
                        SpriteProperty.Execute(property, frameIndex, ref propertyOpen);
                        DragAndDrop();
                        Property();
                        Options();
                    }
                }

                SpriteTreeEditor.TreeInspector(this, main.tree, parent.Get("tree"), spriteNames, main.tree.signals);
                TransitionEditor.Transition(this, parent, sprites, "sprites", spriteNames, main.tree.signals);
                ShowCurrentState(main.currentAnimation);
            }
            serializedObject.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
            SpriteProperty.SetExtraProperty(main.sprites, main.spriteIndex, spriteExists, frameIndex.intValue);
            SetSpriteEditingProperty();
            if (main.sort)
            {
                main.sort = false;
                main.sprites = main.sprites.OrderBy(sp => sp.name).ToList();
            }
        }

        private void SetCurrentSprite()
        {
            sprites = parent.Get("sprites");
            spriteIndex = parent.Get("spriteIndex");
            scrollIndex = parent.Get("scrollIndex");
            spriteIndex.intValue = Mathf.Clamp(spriteIndex.intValue, 0, sprites.arraySize);
            sprite = sprites.arraySize > 0 ? sprites.Element(spriteIndex.intValue) : null;
            frames = sprite != null ? sprite.Get("frame") : null;
            frameIndex = sprite != null ? sprite.Get("frameIndex") : null;
            property = sprite != null ? sprite.Get("property") : null;
            sprites.CreateNameList(spriteNames);
        }

        public void PlayButtons()
        {
            Bar.SetupTabBar(4, 22);
            var isOpen = parent.Bool("previewArea");
            var canPlay = main.sprites.Count != 0 && spriteIndex.intValue >= 0 &&
                          spriteIndex.intValue < main.sprites.Count;
            if (Bar.TabButton("Play", "RoundTopLeft", Tint.SoftDark, parent.Bool("playInInspector"),
                    "Play In Inspector") && canPlay && isOpen)
            {
                parent.SetTrue("playInInspector");
                parent.SetFalse("playInScene");
                parent.SetTrue("resetPlayFrame");
                StopPlayEditorOnly();
            }

            if (Bar.TabButton("PlayScene", "Square", Tint.SoftDark, parent.Bool("playInScene"), "Play in Scene") &&
                canPlay && isOpen)
            {
                parent.SetFalse("playInInspector");
                parent.SetTrue("playInScene");
                parent.SetTrue("resetPlayFrame");
                InitializePlayEditorOnly();
            }

            if (Bar.TabButton("Red", "Square", Tint.SoftDark, false, "Stop") && canPlay)
            {
                parent.SetFalse("playInInspector");
                parent.SetFalse("playInScene");
                parent.SetTrue("resetPlayFrame");
                StopPlayEditorOnly();
            }

            var previewArea = parent.Bool("previewArea");
            if (Bar.TabButton(previewArea ? "EyeOpen" : "EyeClosed", "RoundTopRight", Tint.SoftDark, false, "Hide",
                    Tint.Orange) && canPlay)
            {
                parent.Toggle("previewArea");
                parent.SetFalse("playInInspector");
                parent.SetFalse("playInScene");
                parent.SetTrue("resetPlayFrame");
            }

            Layout.VerticalSpacing();
        }

        public void CreateSprite()
        {
            Block.Header(parent).Style(Tint.SoftDark, "HeaderMiddle", noGap: false)
                .DropArrow("open")
                .Field(sprite.Get("name"), rightSpace: 7, yoffset: 1)
                .Toggle("sort", "Sort")
                .Toggle("add", "xsAdd", hide: false)
                .Build();

            if (parent.Bool("open"))
            {
                var delete = -1;
                Block.CatalogList(15, 16).Scroll(this, 2, spriteNames.Count, scrollIndex, (height, index) =>
                {
                    Block.Header().BoxRect(Tint.WarmGrey * index.RowTint(1f, 0.95f), selection: true, height: height)
                        .MouseDown(height, () =>
                        {
                            spriteIndex.intValue = index;
                            SetCurrentSprite();
                        })
                        .Space(17, spriteIndex.intValue != index)
                        .Image("CheckMark", Tint.SoftDark, spriteIndex.intValue == index)
                        .Label(spriteNames[index], 11, false, Color.black)
                        .Button("Delete", Tint.SoftDark, hide: false)
                        .Build();

                    if (Header.SignalActive("Delete")) delete = index;
                    Repaint();
                });
                if (delete != -1)
                {
                    sprites.DeleteArrayElement(delete);
                    spriteIndex.ClampToArray(sprites);
                    SetCurrentSprite();
                }

                Layout.VerticalSpacing(2);
            }

            if (parent.ReadBool("add") || sprites.arraySize == 0)
            {
                sprites.CreateNewElement().FindPropertyRelative("name").stringValue = "New Sprite";
                spriteIndex.intValue = sprites.LastIndex();
                scrollIndex.intValue = sprites.LastIndex();
                SetCurrentSprite();
            }
        }

        public void DragAndDrop()
        {
            if (frames.arraySize == 0)
            {
                CreateDragAndDropArea("Drop Sprites Here", Tint.White);
                TransferSpritesFromTempList();
            }

            if (parent.Int("tabIndex") == 0 && frames.arraySize > 0)
            {
                CreateDragAndDropArea("Replace Sprites", Tint.Delete);
                TransferSpritesFromTempList();
            }
        }

        private void CreateDragAndDropArea(string message, Color color)
        {
            tempSprites.Clear();
            tempTexture2D.Clear();
            var dropArea = Block.BasicRect(Layout.longInfoWidth, 88, -11, bottomSpace: 1, texture: Skin.square,
                color: Tint.SoftDark);
            {
                Fields.DropAreaGUI(dropArea, tempSprites);
                Fields.DropAreaGUI(dropArea, tempTexture2D);
                Labels.Centered(dropArea, message, color, 10, 15);

                var dropRect = dropArea.TextureCentered(Icon.Get("DropCorner"), new Vector2(22, 22), Tint.White, -10);
                if (frames.arraySize == 0 && dropRect.ContainsMouseDown())
                {
                    frames.arraySize++;
                    parent.SetFalse("resetSprites");
                    Selection.activeGameObject = main.gameObject;
                }
            }
        }

        private void TransferSpritesFromTempList()
        {
            if ((tempSprites.Count == 0 && tempTexture2D.Count == 0) || frames == null)
                return;

            frames.arraySize = 0;
            parent.SetFalse("resetSprites");
            Selection.activeGameObject = main.gameObject;

            for (var i = 0; i < tempSprites.Count; i++)
            {
                frames.arraySize++;
                frames.LastElement().Get("sprite").objectReferenceValue = tempSprites[i];
                frames.LastElement().Get("rate").floatValue = 1f / 10f;
            }

            for (var i = 0; i < tempTexture2D.Count; i++)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(tempTexture2D[i]));
                if (sprite == null)
                    continue;
                frames.arraySize++;
                frames.LastElement().Get("sprite").objectReferenceValue = sprite;
                frames.LastElement().Get("rate").floatValue = 1f / 10f;
            }
        }

        public void Options()
        {
            if (sprite == null || frames == null) return;

            if (parent.Int("tabIndex") == 1 && frames.arraySize > 0)
            {
                Block.Box(4, Tint.SoftDark);
                {
                    sprite.Field_("Loop Start Index", "loopStartIndex");
                    sprite.FieldAndEnable_("Synchronize", "syncID", "canSync");
                    Block.HelperText("Sync ID", 18);
                    sprite.FieldToggleAndEnable_("Loop Once", "loopOnce");
                    sprite.FieldToggleAndEnable_("Random Animations", "isRandom");
                }

                if (sprite.Bool("isRandom"))
                {
                    var array = sprite.Get("randomAnimations");
                    Block.BoxArray(array, Tint.SoftDark, 23, false, 1, "Animation Name, Probability (0-1)",
                        (height, index) =>
                        {
                            Block.Header(array.Element(index)).BoxRect(Tint.SoftDark, leftSpace: 5, height: height)
                                .Field("animation", 0.75f)
                                .Field("weight", 0.25f)
                                .ArrayButtons()
                                .BuildGet()
                                .ReadArrayButtons(array, index);
                        });
                }

                if (sprite.Bool("loopOnce")) Fields.EventField(sprite.Get("onLoopOnce"));
            }
        }

        public void Property()
        {
            if (frames == null || property == null) return;

            SpriteProperty.MatchArraySize(property, frames.arraySize);
            SpriteProperty.SecureSameSize(property, frames.arraySize);

            if (frames.arraySize != 0 && parent.Int("tabIndex") == 3) /// CREATE PROPERTIES
            {
                var create = -1;
                Block.CatalogList(5, 19).Scroll(this, 1, SpriteProperty.names.Count, parent.Get("propertyIndex"),
                    (height, index) =>
                    {
                        Block.Header().BoxRect(Tint.WarmGrey * index.RowTint(1f, 0.95f), selection: true,
                                height: height)
                            .MouseDown(height, () =>
                            {
                                parent.Get("tabIndex").intValue = -1;
                                create = index;
                            })
                            .Image(SpriteProperty.icons[index], 18)
                            .Label(SpriteProperty.names[index], 11, false, Color.black)
                            .Build();
                    });
                Repaint();
                if (create != -1)
                {
                    SpriteProperty.CreateProperty(create, property);
                    SpriteProperty.MatchArraySize(property, frames.arraySize);
                }
            }
        }

        private void SetSpriteEditingProperty()
        {
            if (main.playingInScene) return;

            if (propertyOpen && currentSprite != null)
            {
                if (render == null) render = main.gameObject.GetComponent<Image>();
                if (render == null) return;
                if (!main.settingRenderer) main.rendererSprite = render.sprite;
                main.settingRenderer = true;
                render.sprite = currentSprite;
                if (previousSprite != currentSprite.name) SceneView.RepaintAll();
                previousSprite = currentSprite.name;
            }
            else if (main.settingRenderer)
            {
                if (render == null) render = main.gameObject.GetComponent<Image>();
                if (main.rendererSprite != null && render != null) render.sprite = main.rendererSprite;
                main.settingRenderer = false;
            }
        }

        public static void ShowCurrentState(string animation)
        {
            if (Application.isPlaying)
            {
                FoldOut.BoxSingle(1, Tint.Blue * Tint.WarmGrey);
                {
                    Labels.Label("Current Animation:   " + animation, Color.white);
                }
                Layout.VerticalSpacing(5);
            }
        }

        #region Play In Scene

        public void InitializePlayEditorOnly()
        {
            StopPlayEditorOnly();
            render = main.gameObject.GetComponent<Image>();
            Clock.Initialize();
            timer = currentFrameIndex = 0;
            tempSprite = null;

            for (var i = 0; i < main.sprites.Count; i++)
            {
                if (main.spriteIndex != i)
                    continue;
                tempSprite = main.sprites[i];
                originalSprite = render != null ? render.sprite : originalSprite;
                tempSprite.SetProperties(0, true);
                EditorApplication.update += RunAnimation;
                break;
            }
        }

        private void RunAnimation()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildPipeline.isBuildingPlayer ||
                EditorApplication.isCompiling || render == null || tempSprite == null || tempSprite.frame.Count == 0)
            {
                StopPlayEditorOnly();
                return;
            }

            if (Clock.TimerEditor(ref timer, tempSprite.frame[currentFrameIndex].rate))
            {
                currentFrameIndex = currentFrameIndex + 1 >= tempSprite.frame.Count ? 0 : currentFrameIndex + 1;
                render.sprite = tempSprite.frame[currentFrameIndex].sprite;
                tempSprite.SetProperties(currentFrameIndex);
            }

            for (var i = 0; i < tempSprite.property.Count; i++)
                if (tempSprite.property[i].interpolate)
                {
                    var playRate = tempSprite.frame[currentFrameIndex].rate;
                    tempSprite.property[i].Interpolate(currentFrameIndex, playRate, timer);
                }
        }

        public void StopPlayEditorOnly()
        {
            if (tempSprite != null) tempSprite.ResetProperties();
            if (render != null && originalSprite != null) render.sprite = originalSprite;
            EditorApplication.update -= RunAnimation;
            originalSprite = null; //
            tempSprite = null;
        }

        #endregion
    }
}