#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwoBitMachines.Editors
{
    public static class S // field size, condensed
    {
        public static float LW => Layout.labelWidth;
        public static float CW => Layout.contentWidth;
        public static float FW => LW + CW;
        public static float H => Layout.contentWidth * 0.5f;
        public static float Q => Layout.contentWidth * 0.25f;
        public static float T => Layout.contentWidth * 0.3333f;
        public static float Qu => Layout.contentWidth * 0.20f;
        public static float B => Layout.buttonWidth;
        public static float B2 => Layout.buttonWidth * 2f;
        public static float B3 => Layout.buttonWidth * 3f;
        public static float B4 => Layout.buttonWidth * 4f;

        public static float CWB => CW * 0.5f - B * 0.5f;
    }

    public class Property
    {
        public static Rect fieldRect;
        public static float fieldRectOffset;
        public static SerializedProperty property;
        public static SerializedObject objProperty;

        public SerializedProperty Get(string field)
        {
            return property != null ? property.Get(field) : objProperty != null ? objProperty.Get(field) : null;
        }

        public static void C()
        {
            fieldRect = Layout.CreateRectField();
            fieldRectOffset = 0;
        }

        public Property D(Color color, float expandX, float expandY)
        {
            var rect = new Rect(fieldRect)
            {
                x = fieldRect.x - expandX * 0.5f, width = fieldRect.width + expandX, y = fieldRect.y - expandY * 0.5f,
                height = fieldRect.height + expandY
            };
            rect.DrawTexture(FoldOut.background, color);
            return this;
        }

        public Property F(string field, float size)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), Get(field), GUIContent.none);
            fieldRectOffset = size;
            return this;
        }

        public Property F(float size)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), property, GUIContent.none);
            fieldRectOffset = size;
            return this;
        }

        public Property F(string field, float size, float space = 0)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), Get(field), GUIContent.none);
            fieldRectOffset = size + space;
            return this;
        }

        public Property S(float space)
        {
            fieldRectOffset += space;
            return this;
        }

        public Property L(string label, float size, float space = 0)
        {
            Labels.Label(label, fieldRect.Adjust(fieldRectOffset, size));
            fieldRectOffset = size + space;
            return this;
        }

        public static bool B(string icon)
        {
            var value = fieldRect.Adjust(fieldRectOffset, Layout.buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
            fieldRectOffset = Layout.buttonWidth;
            return value;
        }

        public Property B(string icon, float size, out bool pressed)
        {
            var tempRect = new Rect(fieldRect)
                { x = fieldRect.x + fieldRectOffset, width = size, height = 18, y = fieldRect.y - 1 };
            pressed = tempRect.Button(Icon.Get(icon), Icon.Get("ButtonWhite"), Tint.White);
            fieldRect.Adjust(fieldRectOffset, size);
            fieldRectOffset = size;
            return this;
        }

        public bool B(string icon, float size)
        {
            var tempRect = new Rect(fieldRect)
                { x = fieldRect.x + fieldRectOffset, width = size, height = 18, y = fieldRect.y - 1 };
            var pressed = tempRect.Button(Icon.Get(icon), Tint.White, center: true);
            fieldRect.Adjust(fieldRectOffset, size);
            fieldRectOffset = size;
            return pressed;
        }

        public static bool BB(string icon, float size)
        {
            var tempRect = new Rect(fieldRect)
                { x = fieldRect.x + fieldRectOffset, width = size, height = 18, y = fieldRect.y - 1 };
            var value = tempRect.Button(Icon.Get(icon), Icon.Get("ButtonBlack"), Tint.White);
            fieldRect.Adjust(fieldRectOffset, size);
            fieldRectOffset = size;
            return value;
        }
    }

    public static class Fields
    {
        public static Rect fieldRect;
        public static float fieldRectOffset;
        private static readonly int toggleSpace = 1;

        public static Property prop = new();
        public static GUIStyle editorStyle => EditorGUIUtility.isProSkin ? EditorStyles.whiteLabel : EditorStyles.label;

        public static Property Start(SerializedObject property, string label = "", float shorten = 0)
        {
            Property.objProperty = property;
            Property.property = null;
            Property.C();
            if (label != "")
                prop.L(label, Layout.labelWidth, shorten);
            return prop;
        }

        public static Property Start(SerializedProperty property, string label = "", float shorten = 0)
        {
            Property.objProperty = null;
            Property.property = property;
            Property.C();
            if (label != "")
                prop.L(label, Layout.labelWidth, shorten);
            return prop;
        }

        public static SerializedProperty Get(this SerializedObject serialObj, string value)
        {
            return serialObj.FindProperty(value);
        }

        public static SerializedProperty Get(this SerializedProperty property, string value)
        {
            return property.FindPropertyRelative(value);
        }

        public static void EventField(SerializedProperty onEvent, float adjustX = 0, Color? color = null,
            bool execute = true, bool noGap = false)
        {
            if (!execute)
                return;
            var height = EditorGUI.GetPropertyHeight(onEvent);
            GUI.backgroundColor = color != null ? color.Value : Tint.WarmGrey;
            var rect = Block.BasicRect(Layout.longInfoWidth, height - 1, -11, bottomSpace: 1, noGap: noGap,
                color: GUI.backgroundColor * 0.82f, texture: Icon.Get("HeaderBasic"));
            var rectField = new Rect(rect)
                { x = rect.x + adjustX * 0.5f, y = rect.y, width = rect.width - adjustX, height = rect.height + 1 };
            EditorGUI.PropertyField(rectField, onEvent);
            GUI.backgroundColor = Tint.White;
        }

        public static void EventFoldOut(SerializedProperty onEvent, SerializedProperty toggle, string label,
            float shiftX = 0, float offsetY = 0, Color color = default, bool execute = true, bool space = true)
        {
            if (!execute)
                return;
            if (space)
                Layout.VerticalSpacing();
            if (toggle.boolValue)
            {
                var previous = GUI.backgroundColor;
                GUI.backgroundColor = color;

                var height = EditorGUI.GetPropertyHeight(onEvent);
                var rect = Layout.CreateRect(Layout.longInfoWidth, height,
                    -11); //, texture : Skin.longBorderFrame, color : Tint.Grey100);
                var rectField = new Rect(rect)
                    { x = rect.x + shiftX, y = rect.y + offsetY, width = rect.width - shiftX };

                EditorGUI.PropertyField(rectField, onEvent, new GUIContent(label));
                rect.height = 20;
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                GUI.backgroundColor = previous;
            }
            else
            {
                var rect = Layout.CreateRect(Layout.longInfoWidth, 20, -11);
                var rectField = new Rect(rect)
                    { x = rect.x + shiftX, y = rect.y + offsetY, width = rect.width - shiftX, height = 20 };
                Skin.Draw(rectField, color == default ? FoldOut.boxColor : color, FoldOut.background);
                Labels.Display(rectField.OffsetX(2), label + " ( )", 5);
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
            }
        }

        public static void EventFoldOut(SerializedProperty onEvent, string label, float adjustX = 0, float offsetY = 0,
            Color color = default, bool execute = true)
        {
            if (!execute)
                return;

            var previous = GUI.backgroundColor;
            GUI.backgroundColor = color;

            var height = EditorGUI.GetPropertyHeight(onEvent);
            var rect = Layout.CreateRect(Layout.longInfoWidth, height,
                -11); //, texture : Skin.longBorderFrame, color : Tint.Grey100);
            var rectField = new Rect(rect)
                { x = rect.x + adjustX * 0.5f, y = rect.y + offsetY, width = rect.width - adjustX };
            EditorGUI.PropertyField(rectField, onEvent, new GUIContent(label));

            GUI.backgroundColor = previous;
        }

        public static void EventFoldOutEffect(SerializedProperty onEvent, SerializedProperty effectName,
            SerializedProperty toggle, string label, Color color = default, bool execute = true)
        {
            if (!execute)
                return;
            Layout.VerticalSpacing();
            if (toggle.boolValue)
            {
                var previous = GUI.backgroundColor;
                GUI.backgroundColor = color;
                var height = EditorGUI.GetPropertyHeight(onEvent);
                var rect = Layout.CreateRect(Layout.longInfoWidth, height,
                    -11); //, texture : Skin.longBorderFrame, color : Tint.Grey100);
                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width };
                EditorGUI.PropertyField(rectField, onEvent, new GUIContent(label));
                rect.height = 20;
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                GUI.backgroundColor = previous;
            }
            else
            {
                var contentWidth = Layout.contentWidth;
                var rect = Layout.CreateRect(Layout.longInfoWidth, 20, -11);
                var inputString = new Rect(rect) { x = rect.x, y = rect.y + 1, width = rect.width, height = 17 };
                inputString.x += inputString.width - contentWidth - 8;
                inputString.width = contentWidth;

                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width, height = 20 };
                Skin.Draw(rectField, color == default ? FoldOut.boxColor : color, FoldOut.background);
                Labels.Display(rectField.OffsetX(2), label + " ( )", 5);

                rect.width -= contentWidth;
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                if (effectName != null)
                {
                    EditorGUI.PropertyField(inputString, effectName, GUIContent.none);
                    if (effectName.stringValue == "")
                        Labels.FieldText("World Effect", 3);
                }
            }
        }

        public static void EventFoldOutEffectAndRate(SerializedProperty onEvent, SerializedProperty effectName,
            SerializedProperty effectRate, SerializedProperty toggle, string label, Color color = default,
            bool execute = true)
        {
            if (!execute)
                return;
            Layout.VerticalSpacing();
            if (toggle.boolValue)
            {
                var height = EditorGUI.GetPropertyHeight(onEvent);
                var rect = Layout.CreateRect(Layout.longInfoWidth, height,
                    -11); //, texture : Skin.longBorderFrame, color : Tint.Grey100);
                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width };
                EditorGUI.PropertyField(rectField, onEvent, new GUIContent(label));
                rect.height = 20;
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
            }
            else
            {
                var contentWidth = Layout.contentWidth;
                var rect = Layout.CreateRect(Layout.longInfoWidth, 20, -11);
                var inputRect = new Rect(rect) { x = rect.x, y = rect.y + 1, width = rect.width, height = 17 };

                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width, height = 20 };
                Skin.Draw(rectField, color == default ? FoldOut.boxColor : color, FoldOut.background);
                Labels.Display(rectField.OffsetX(2), label + " ( )", 5);
                rect.width -= contentWidth;
                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                if (effectName != null)
                {
                    inputRect.x += inputRect.width - contentWidth - 8;
                    inputRect.width = contentWidth * 0.5f;
                    EditorGUI.PropertyField(inputRect, effectName, GUIContent.none);
                    inputRect.x += inputRect.width;
                    EditorGUI.PropertyField(inputRect, effectRate, GUIContent.none);
                    Labels.FieldDoubleText(effectName.stringValue == "" ? "Effect" : "", "Rate", rightSpacing: 4f);
                }
            }
        }

        public static bool EventFoldOutFloat(SerializedProperty onEvent, SerializedProperty valueFloat,
            SerializedProperty toggle, string label, Color color = default, int offsetX = 0, bool ySpace = true,
            bool execute = true)
        {
            if (!execute)
                return false;
            if (ySpace)
                Layout.VerticalSpacing();
            var pressed = false;
            if (toggle.boolValue)
            {
                var height = EditorGUI.GetPropertyHeight(onEvent);
                var rect = Layout.CreateRect(Layout.longInfoWidth, height,
                    -11); //, texture : Skin.longBorderFrame, color : Tint.Grey100);
                var inputRect = new Rect(rect) { x = rect.x, y = rect.y + 1, width = rect.width, height = 17 };
                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width };

                EditorGUI.PropertyField(rectField, onEvent, new GUIContent(label));

                rect.height = 20;
                rect.x += offsetX;
                rect.width -= offsetX;

                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                if (valueFloat != null)
                {
                    inputRect.x += inputRect.width - Layout.contentWidth * 0.8f - 1;
                    inputRect.width = Layout.contentWidth * 0.8f - S.B;
                    EditorGUI.PropertyField(inputRect, valueFloat, GUIContent.none);
                    pressed = inputRect.Adjust(inputRect.width, S.B)
                        .Button(Icon.Get("Delete"), Tint.White, center: true);
                    Labels.FieldText("Time Percent", S.B);
                }
            }
            else
            {
                var rect = Layout.CreateRect(Layout.longInfoWidth, 20, -11);
                var inputRect = new Rect(rect) { x = rect.x, y = rect.y + 1, width = rect.width, height = 17 };

                var rectField = new Rect(rect) { x = rect.x, y = rect.y, width = rect.width, height = 20 };
                Skin.Draw(rectField, color == default ? FoldOut.boxColor : color, FoldOut.background);
                Labels.Display(rectField.OffsetX(2), label + " ( )", 5);
                rect.width -= 99;
                rect.x += offsetX;
                rect.width -= offsetX;

                if (Event.current.isMouse && rect.ContainsMouseDown()) toggle.boolValue = !toggle.boolValue;
                if (valueFloat != null)
                {
                    inputRect.x += inputRect.width - Layout.contentWidth * 0.8f - 1;
                    inputRect.width = Layout.contentWidth * 0.8f - S.B;
                    EditorGUI.PropertyField(inputRect, valueFloat, GUIContent.none);
                    pressed = inputRect.Adjust(inputRect.width, S.B)
                        .Button(Icon.Get("Delete"), Tint.White, center: true);
                    Labels.FieldText("Time Percent", S.B);
                }
            }

            return pressed;
        }

        public static void FieldOnly(this SerializedObject property, string field, int width, int space = 0)
        {
            var rect = Layout.CreateRectField();
            EditorGUI.PropertyField(rect.Adjust(space, width), property.Get(field), GUIContent.none);
        }

        public static void FieldOnly(this SerializedProperty property, string field, int width, int space = 0)
        {
            var rect = Layout.CreateRectField();
            EditorGUI.PropertyField(rect.Adjust(space, width), property.Get(field), GUIContent.none);
        }

        public static void Field(this SerializedProperty property, Rect rect)
        {
            EditorGUI.PropertyField(rect, property, GUIContent.none);
        }

        public static void ConstructField(int offsetY = 0)
        {
            fieldRect = Layout.CreateRectField(offsetY: offsetY);
            fieldRectOffset = 0;
        }

        public static void ConstructDraw(Color color, float expandX, float expandY)
        {
            var rect = new Rect(fieldRect)
            {
                x = fieldRect.x - expandX * 0.5f, width = fieldRect.width + expandX, y = fieldRect.y - expandY * 0.5f,
                height = fieldRect.height + expandY
            };
            rect.DrawTexture(FoldOut.background, color);
        }

        public static void ConstructField(this SerializedObject property, string field, float size)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), property.Get(field), GUIContent.none);
            fieldRectOffset = size;
        }

        public static void ConstructField(this SerializedProperty property, float size)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), property, GUIContent.none);
            fieldRectOffset = size;
        }

        public static void Grip(SerializedObject parent, SerializedProperty array, int index, int size = 25,
            Color? color = null)
        {
            ListReorder.Grip(parent, array, fieldRect.Adjust(fieldRectOffset, size), index, color ?? Color.white);
            fieldRectOffset = size;
        }

        public static void Grip(SerializedProperty parent, SerializedProperty array, int index, int size = 25,
            Color? color = null)
        {
            ListReorder.Grip(parent, array, fieldRect.Adjust(fieldRectOffset, size), index, color ?? Color.white);
            fieldRectOffset = size;
        }

        public static void ShowSprite(Sprite sprite, int size = 10, int offsetX = 0, int offsetY = 0,
            bool execute = true)
        {
            if (!execute || sprite == null)
                return;

            var spriteRect = sprite.rect;
            var tex = sprite.texture;
            var rect = fieldRect.Adjust(offsetX,
                size); //new Rect (fieldRect) { x = fieldRect.x + fieldRectOffset, y = fieldRect.y + yOffset + 3, width = size, height = size };
            rect.height = size;
            rect.x += offsetX;
            rect.y += offsetY;
            GUI.DrawTextureWithTexCoords(rect, tex,
                new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width,
                    spriteRect.height / tex.height));
            fieldRectOffset = size + offsetX;
        }

        public static void ConstructField(this SerializedProperty property, string field, float size, float space = 0)
        {
            EditorGUI.PropertyField(fieldRect.Adjust(fieldRectOffset, size), property.Get(field), GUIContent.none);
            fieldRectOffset = size + space;
        }

        public static void ConstructList(this SerializedProperty property, string[] names, string stringField,
            float size)
        {
            property.DropList(fieldRect.Adjust(fieldRectOffset, size), names, stringField);
            fieldRectOffset = size;
        }

        public static void ConstructSpace(float space)
        {
            fieldRectOffset += space;
        }

        public static void ConstructString(string label, float size, float space = 0)
        {
            Labels.Label(label, fieldRect.Adjust(fieldRectOffset, size));
            fieldRectOffset = size + space;
        }

        public static bool ConstructButton(string icon)
        {
            var value = fieldRect.Adjust(fieldRectOffset, Layout.buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
            fieldRectOffset = Layout.buttonWidth;
            return value;
        }

        public static bool ConstructButton(string icon, float size)
        {
            var tempRect = new Rect(fieldRect)
                { x = fieldRect.x + fieldRectOffset, width = size, height = 18, y = fieldRect.y - 1 };
            var value = tempRect.Button(Icon.Get(icon), Icon.Get("ButtonWhite"), Tint.White);
            fieldRect.Adjust(fieldRectOffset, size);
            fieldRectOffset = size;
            return value;
        }

        public static bool ConstructButtonB(string icon, float size)
        {
            var tempRect = new Rect(fieldRect)
                { x = fieldRect.x + fieldRectOffset, width = size, height = 18, y = fieldRect.y - 1 };
            var value = tempRect.Button(Icon.Get(icon), Icon.Get("ButtonBlack"), Tint.White);
            fieldRect.Adjust(fieldRectOffset, size);
            fieldRectOffset = size;
            return value;
        }

        public static void Field(this SerializedObject property, string title, string field, bool execute = true,
            float shorten = 0, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten), property.Get(field),
                GUIContent.none);
        }

        public static void FieldLayer(this SerializedProperty property, string title, bool execute = true,
            float shorten = 0, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            property.intValue = EditorGUI.LayerField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten),
                property.intValue);
        }

        public static void Field(this SerializedProperty property, string title, string field, bool execute = true,
            float shorten = 0, bool bold = false)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth), bold);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten), property.Get(field),
                GUIContent.none);
        }

        public static void FieldLabel(this SerializedProperty property, string title, string field, bool execute = true,
            float shorten = 0, bool bold = false)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth), bold);
            Labels.Label(property.String(field), rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten), bold);
        }

        public static bool FieldLabelAndButton(this SerializedProperty property, string title, string label,
            string icon, bool execute = true, float shorten = 0, bool bold = false)
        {
            if (!execute)
                return false;
            var buttonWidth = Layout.buttonWidth;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth), bold);
            Labels.Label(label, rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten - buttonWidth), bold);
            return rect.Adjust(Layout.contentWidth - buttonWidth, buttonWidth - 2)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static void TitleIsField(this SerializedProperty property, string title, string field,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            EditorGUI.PropertyField(rect.Adjust(0, Layout.labelWidth), property.Get(title), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth), property.Get(field),
                GUIContent.none);
        }

        public static bool TitleIsField(this SerializedProperty property, string title, string field, string button,
            bool execute = true)
        {
            if (!execute)
                return false;
            var rect = Layout.CreateRectField();
            EditorGUI.PropertyField(rect.Adjust(0, Layout.labelWidth), property.Get(title), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - 15), property.Get(field),
                GUIContent.none);
            return rect.Adjust(Layout.contentWidth - 15, 15).Button(Icon.Get(button), Tint.White, center: true);
        }

        public static Rect FieldProperty(this SerializedProperty property, string title, float shorten = 0,
            float space = 0)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                if (property.arraySize == 0)
                {
                    var rectField = Layout.CreateRectField();
                    rectField.width = 15;
                    rectField.x = Layout.infoWidth + 1;
                    if (rectField.Button(Icon.Get("Add"), Color.white, center: true)) property.arraySize++;
                }

                for (var i = 0; i < property.arraySize; i++)
                    ArrayProperty(property, property.Element(i), i, title + " " + i);
                return new Rect();
            }

            var rect = Layout.CreateRectField();
            Labels.Label(Util.ToProperCase(title), rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - shorten), property,
                GUIContent.none, true);
            return rect;
        }

        public static void ArrayProperty(SerializedProperty array, SerializedProperty property, int index, string title,
            float space = 0, bool skipClear = false)
        {
            float buttonWidth = 11;
            var rect = Layout.CreateRectField();
            Labels.Label(Util.ToProperCase(title), rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth * 2f - 5),
                property, GUIContent.none, false);

            rect.width = 15;
            rect.x = Layout.infoWidth - 15 * 0.5f - 5;

            if (rect.Button(Icon.Get("xsAdd"), Color.white, center: true))
            {
                array.arraySize++;
                //if (!skipClear) 
                // EditorTools.ClearPropertyExceptVectors (array.LastElement ( ));
                array.MoveArrayElement(array.arraySize - 1, index + 1);
            }

            rect.x += 5f;
            if (rect.OffsetX(buttonWidth).Button(Icon.Get("xsMinus"), Color.white, center: true))
                array.DeleteArrayElement(index);
        }

        public static void ArrayPropertyFieldDouble(SerializedProperty array, int index, string title, string field1,
            string field2)
        {
            float buttonWidth = 11;
            var rect = Layout.CreateRectField();
            Labels.Label(Util.ToProperCase(title), rect.Adjust(0, Layout.labelWidth));

            var element = array.Element(index);
            var fieldWith = (Layout.contentWidth - buttonWidth * 2f - 5) * 0.5f;
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, fieldWith), element.Get(field1), GUIContent.none,
                false);
            EditorGUI.PropertyField(rect.Adjust(fieldWith, fieldWith), element.Get(field2), GUIContent.none, false);

            rect.width = 15;
            rect.x = Layout.infoWidth - 15 * 0.5f - 5;

            if (rect.Button(Icon.Get("xsAdd"), Color.white, center: true))
            {
                array.arraySize++;
                array.MoveArrayElement(array.arraySize - 1, index + 1);
            }

            rect.x += 5f;
            if (rect.OffsetX(buttonWidth).Button(Icon.Get("xsMinus"), Color.white, center: true))
                array.DeleteArrayElement(index);
        }

        public static void Array(SerializedProperty array, string single, string list, Color color,
            bool skipClear = false)
        {
            if (array.arraySize == 0)
            {
                FoldOut.BoxSingle(1, color);
                {
                    ConstructField();
                    ConstructString(single, S.LW);
                    ConstructSpace(S.CW - S.B);
                    if (ConstructButton("Add")) array.arraySize++;
                }
                Layout.VerticalSpacing(2);
            }

            if (array.arraySize != 0)
            {
                FoldOut.Box(array.arraySize, color);
                for (var i = 0; i < array.arraySize; i++)
                    ArrayProperty(array, array.Element(i), i, list, skipClear: skipClear);
                Layout.VerticalSpacing(5);
            }
        }

        public static void ArrayBox(this SerializedProperty array, string title, Color color, int offsetY = 0)
        {
            if (array.arraySize == 0) array.arraySize++;
            FoldOut.Box(array.arraySize, color, offsetY: offsetY);
            {
                for (var i = 0; i < array.arraySize; i++)
                {
                    var element = array.Element(i);
                    ArrayProperty(array, element, i, title);
                }
            }
            Layout.VerticalSpacing(5 + offsetY);
        }

        public static void FieldClampToZero(this SerializedProperty property, string title, string field)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth), property.Get(field),
                GUIContent.none);
            if (property.Float(field) < 0)
                property.Get(field).floatValue = 0;
        }

        public static void FieldIntClamp(this SerializedProperty property, string title, string field, int min = 0,
            int max = 1)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth), property.Get(field),
                GUIContent.none);
            if (property.Int(field) < min)
                property.Get(field).intValue = min;
            if (property.Int(field) > max)
                property.Get(field).intValue = max;
        }

        public static void FieldFloatClamp(this SerializedProperty property, string title, string field, float min = 0,
            float max = 1)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth), property.Get(field),
                GUIContent.none);
            if (property.Float(field) < min)
                property.Get(field).floatValue = min;
            if (property.Float(field) > max)
                property.Get(field).floatValue = max;
        }

        public static bool DropAreaGUI<T>(Rect dropArea, SerializedProperty array) where T : Object
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return false;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (object item in DragAndDrop.objectReferences)
                        {
                            array.arraySize++;
                            array.LastElement().objectReferenceValue = item as T;
                        }

                        evt.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        public static bool DropAreaGUIList<T>(Rect dropArea, SerializedProperty array, string field) where T : Object
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return false;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (object item in DragAndDrop.objectReferences)
                        {
                            array.arraySize++;
                            array.LastElement().Get(field).objectReferenceValue = item as T;
                        }

                        evt.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        public static bool DropAreaGUI<T>(Rect dropArea, SerializedProperty array, string field) where T : Object
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return false;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (object item in DragAndDrop.objectReferences)
                        {
                            array.arraySize++;
                            array.LastElement().Get(field).objectReferenceValue = item as T;
                            evt.Use();
                            return true;
                        }

                        evt.Use();
                    }

                    break;
            }

            return false;
        }

        public static void DropAreaGUI<T>(Rect dropArea, List<T> list) where T : Object
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (object item in DragAndDrop.objectReferences)
                            if (item is T)
                                list.Add(item as T);
                        if (list.Count > 0)
                            evt.Use();
                    }

                    break;
            }
        }

        public static void FieldAndEnum(this SerializedProperty property, string title, string field, string enumA,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth - 5),
                property.Get(field), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - Layout.boolWidth - 5, Layout.boolWidth + 5),
                property.Get(enumA), GUIContent.none);
        }

        public static void FieldAndDoubleEnum(this SerializedProperty property, string title, string field,
            string enumA, string enumB, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - 40), property.Get(field),
                GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - 40, 20), property.Get(enumA), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(20, 20), property.Get(enumB), GUIContent.none);
        }

        public static void FieldAndEnable(this SerializedProperty property, string title, string field, string toggle,
            bool execute = true, bool titleIsField = false)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Layout.BeginGUIEnable(property.Bool(toggle));
            if (titleIsField)
                EditorGUI.PropertyField(rect.Adjust(0, Layout.labelWidth), property.Get(title), GUIContent.none);
            if (!titleIsField)
                Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            Layout.EndGUIEnable(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle));
        }

        public static void FieldAndEnableHalf(this SerializedProperty property, string title, string field,
            string toggle, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            Layout.BeginGUIEnable(property.Bool(toggle));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            Layout.EndGUIEnable(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle));
        }

        public static void FieldAndDisable(this SerializedProperty property, string title, string title2, string field,
            string toggle, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(property.Bool(toggle) ? title2 : title, rect.Adjust(0, Layout.labelWidth));
            Layout.BeginGUIEnable(!property.Bool(toggle));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            Layout.EndGUIEnable(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle));
        }

        public static void FieldAndEnableRaw(this SerializedObject property, string title, string field, string toggle,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle), GUIContent.none);
        }

        public static void FieldAndEnable(this SerializedObject property, string title, string field, string toggle,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Layout.BeginGUIEnable(property.Bool(toggle));
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            Layout.EndGUIEnable(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle));
        }

        public static void FieldAndToggle(this SerializedObject property, string title, string field, string toggle,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle), GUIContent.none);
        }

        public static void FieldAndToggle(this SerializedProperty property, string title, string field, string toggle,
            bool titleIsField = false, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            if (titleIsField)
                EditorGUI.PropertyField(rect.Adjust(0, Layout.labelWidth), property.Get(title), GUIContent.none);
            else
                Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - Layout.boolWidth),
                property.Get(field), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - Layout.boolWidth + toggleSpace, Layout.boolWidth),
                property.Get(toggle), GUIContent.none);
        }

        public static void FieldAndEnum(this SerializedObject property, string title, string field, string toggle,
            bool execute = true)
        {
            if (!execute)
                return;
            var width = 20f;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - width), property.Get(field),
                GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth - width + toggleSpace - 1, width),
                property.Get(toggle), GUIContent.none);
        }

        public static void FieldAndDropDownList(this SerializedProperty property, string[] names, string title,
            string normalField, string listField, bool execute = true, float space = 0)
        {
            if (!execute || names.Length == 0)
                return;
            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f - 1),
                property.Get(normalField), GUIContent.none);
            property.DropList(rect.Adjust(Layout.contentWidth / 2f, Layout.contentWidth / 2f), names, listField);
        }

        public static void FieldAndDropDownList(this SerializedObject property, string[] names, string title,
            string normalField, string listField, bool execute = true, float space = 0)
        {
            if (!execute || names.Length == 0)
                return;
            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f - 1),
                property.Get(normalField), GUIContent.none);
            property.DropList(rect.Adjust(Layout.contentWidth / 2f, Layout.contentWidth / 2f), names, listField);
        }

        public static bool FieldAndButton(this SerializedProperty property, string title, string field, string button,
            string icon, int buttonWidth = 15)
        {
            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth - 2),
                property.Get(field), GUIContent.none);
            property.Get(button).boolValue = rect.Adjust(Layout.contentWidth - buttonWidth, buttonWidth - 2)
                .Button(Icon.Get(icon), Tint.White, center: true);
            return property.Bool(button);
        }

        public static bool FieldAndButtonBox(this SerializedProperty property, string title, string icon,
            Color boxColor, int space = 0, bool useWhite = false)
        {
            var rect = Layout.Box(boxColor);
            rect.x += 7;
            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth - space), title,
                useWhite ? Labels.labelStyle : editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth - space, Layout.contentWidth - 10), property,
                GUIContent.none);
            return rect.Adjust(Layout.contentWidth - 10, 15).Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static bool FieldFullAndButtonBox(this SerializedProperty property, string icon, Color boxColor,
            int space = 0, bool useWhite = false)
        {
            var rect = Layout.Box(boxColor);
            rect.x += 7;
            EditorGUI.PropertyField(rect.Adjust(space, Layout.labelWidth + Layout.contentWidth - 12), property,
                GUIContent.none);
            return rect.Adjust(Layout.labelWidth + Layout.contentWidth - 12, 15)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static bool InputAndButtonBox(string title, string icon, Color boxColor, ref string name,
            int buttonWidth = 15, int offsetY = 0)
        {
            var rect = Layout.Box(boxColor, offsetY: offsetY);
            rect.x += 7;
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            name = EditorGUI.TextField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth + 2), name);
            return rect.Adjust(Layout.contentWidth - buttonWidth + 6, buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static bool InputAndButton(string title, string icon, ref string name, int buttonWidth = 15)
        {
            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            name = EditorGUI.TextField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth + 2), name);
            return rect.Adjust(Layout.contentWidth - buttonWidth + 2, buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static bool FieldAndButton(this SerializedProperty property, string title, string field, string icon,
            int space = 0, int buttonWidth = 15, bool titleIsField = false)
        {
            var rect = Layout.CreateRectField();

            if (titleIsField)
                EditorGUI.PropertyField(rect.Adjust(space, Layout.labelWidth), property.Get(title), GUIContent.none);
            else
                Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth + 2),
                property.Get(field), GUIContent.none);
            return rect.Adjust(Layout.contentWidth - buttonWidth + 3, buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static bool FieldRawAndButton(this SerializedProperty property, string title, string icon, int space = 0,
            int buttonWidth = 15)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth + 2), property,
                GUIContent.none);
            return rect.Adjust(Layout.contentWidth - buttonWidth + 3, buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static void FieldAndDoubleButton(this SerializedProperty property, string title, string field,
            string icon1, string icon2, out bool button1, out bool button2)
        {
            var buttonWidth = 15;
            button1 = false;
            button2 = false;

            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth * 2 - 2),
                property.Get(field), GUIContent.none);

            button1 = rect.Adjust(Layout.contentWidth - buttonWidth * 2, buttonWidth)
                .Button(Icon.Get(icon1), Tint.White, center: true);
            button2 = rect.Adjust(buttonWidth, buttonWidth - 2).Button(Icon.Get(icon2), Tint.White, center: true);
        }

        public static void FieldLabelAndDoubleButton(this SerializedProperty property, string title, string label,
            string icon1, string icon2, out bool button1, out bool button2)
        {
            var buttonWidth = 15;
            button1 = false;
            button2 = false;

            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            EditorGUI.LabelField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth * 2 - 2), label,
                editorStyle);

            button1 = rect.Adjust(Layout.contentWidth - buttonWidth * 2, buttonWidth)
                .Button(Icon.Get(icon1), Tint.White, center: true);
            button2 = rect.Adjust(buttonWidth, buttonWidth - 2).Button(Icon.Get(icon2), Tint.White, center: true);
        }

        public static void FieldAndTripleButton(this SerializedProperty property, string title, string field,
            string icon1, string icon2, string icon3, out bool button1, out bool button2, out bool button3)
        {
            var buttonWidth = 15;
            button1 = false;
            button2 = false;
            button3 = false;

            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth * 3 - 2),
                property.Get(field), GUIContent.none);

            button1 = rect.Adjust(Layout.contentWidth - buttonWidth * 3, buttonWidth)
                .Button(Icon.Get(icon1), Tint.White, center: true);
            button2 = rect.Adjust(buttonWidth, buttonWidth - 2).Button(Icon.Get(icon2), Tint.White, center: true);
            button3 = rect.Adjust(buttonWidth, buttonWidth - 2).Button(Icon.Get(icon3), Tint.White, center: true);
        }

        public static bool FieldAndButton(this SerializedObject property, string title, string field, string icon,
            int space = 0, int buttonWidth = 15, string toolTip = "")
        {
            var button = false;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth - buttonWidth),
                property.Get(field), GUIContent.none);
            button = rect.Adjust(Layout.contentWidth - buttonWidth + 3, buttonWidth)
                .Button(Icon.Get(icon), Tint.White, center: true, toolTip: toolTip);
            return button;
        }

        public static void FieldDouble(this SerializedProperty property, string title, string field1, string field2,
            bool execute = true, bool titleIsField = false, bool bold = false)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            if (titleIsField)
                EditorGUI.PropertyField(rect.Adjust(0, Layout.labelWidth), property.Get(title), GUIContent.none);
            else
                Labels.Label(title, rect.Adjust(0, Layout.labelWidth), bold);

            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f), property.Get(field1),
                GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth / 2f, Layout.contentWidth / 2f),
                property.Get(field2), GUIContent.none);
        }

        public static void FieldDouble(this SerializedObject property, string title, string field1, string field2,
            bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f), property.Get(field1),
                GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth / 2f, Layout.contentWidth / 2f),
                property.Get(field2), GUIContent.none);
        }

        public static void FieldDoublePercent(this SerializedObject property, string title, string field1,
            string field2, float percent = 0f, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f * (1f + percent)),
                property.Get(field1), GUIContent.none);
            EditorGUI.PropertyField(
                rect.Adjust(Layout.contentWidth / 2f * (1f + percent), Layout.contentWidth / 2f * (1f - percent)),
                property.Get(field2), GUIContent.none);
        }

        public static void FieldDoublePercent(this SerializedProperty property, string title, string field1,
            string field2, float percent = 0f, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(0, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 2f * (1f + percent)),
                property.Get(field1), GUIContent.none);
            EditorGUI.PropertyField(
                rect.Adjust(Layout.contentWidth / 2f * (1f + percent), Layout.contentWidth / 2f * (1f - percent)),
                property.Get(field2), GUIContent.none);
        }

        public static void FieldTriple(this SerializedObject property, string title, string field1, string field2,
            string field3, int space = 0)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(space, Layout.labelWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, Layout.contentWidth / 3f), property.Get(field1),
                GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth / 3f, Layout.contentWidth / 3f),
                property.Get(field2), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(Layout.contentWidth / 3f, Layout.contentWidth / 3f),
                property.Get(field3), GUIContent.none);
        }

        public static bool FieldDoubleAndButton(this SerializedProperty property, string title, string field1,
            string field2, string icon, bool execute = true, int space = 0)
        {
            if (!execute)
                return false;
            var rect = Layout.CreateRectField();
            var buttonWidth = Layout.buttonWidth;
            var width = (Layout.contentWidth - buttonWidth) * 0.5f;

            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, width), property.Get(field1), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(width, width), property.Get(field2), GUIContent.none);

            return rect.Adjust(width + 3, buttonWidth).Button(Icon.Get(icon), Tint.White, center: true);
        }

        public static void FieldDoubleAndEnable(this SerializedProperty property, string title, string field1,
            string field2, string toggle, bool execute = true, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            var width = (Layout.contentWidth - Layout.boolWidth) * 0.5f;
            Layout.BeginGUIEnable(property.Bool(toggle));
            {
                EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
                EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, width), property.Get(field1), GUIContent.none);
                EditorGUI.PropertyField(rect.Adjust(width, width), property.Get(field2), GUIContent.none);
            }
            Layout.EndGUIEnable(rect.Adjust(width + 1, Layout.boolWidth), property.Get(toggle));
        }

        public static void FieldDoubleAndEnum(this SerializedProperty property, string title, string field1,
            string field2, string enumA, bool execute = true, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            var width = (Layout.contentWidth - Layout.boolWidth) * 0.5f;

            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, width), property.Get(field1), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(width, width), property.Get(field2), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(width + 1, Layout.boolWidth), property.Get(enumA), GUIContent.none);
        }

        public static void FieldDoubleAndEnable(this SerializedObject property, string title, string field1,
            string field2, string toggle, bool execute = true, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            var width = (Layout.contentWidth - Layout.boolWidth) / 2f;
            Layout.BeginGUIEnable(property.Bool(toggle));
            {
                EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
                EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, width), property.Get(field1), GUIContent.none);
                EditorGUI.PropertyField(rect.Adjust(width, width), property.Get(field2), GUIContent.none);
            }
            Layout.EndGUIEnable(rect.Adjust(width + 1, Layout.boolWidth), property.Get(toggle));
        }

        public static void FieldDoubleAndEnableHalf(this SerializedProperty property, string title, string field1,
            string field2, string toggle, bool execute = true, int space = 0)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            var width = (Layout.contentWidth - Layout.boolWidth) / 2f - 1;

            EditorGUI.LabelField(rect.Adjust(space, Layout.labelWidth), title, editorStyle);
            Layout.BeginGUIEnable(property.Bool(toggle));
            EditorGUI.PropertyField(rect.Adjust(Layout.labelWidth, width), property.Get(field1), GUIContent.none);
            EditorGUI.PropertyField(rect.Adjust(width, width - 1), property.Get(field2), GUIContent.none);
            Layout.EndGUIEnable(rect.Adjust(width + 1, Layout.boolWidth), property.Get(toggle));
        }

        public static void FieldObject(this SerializedProperty property, string title, string field, Type objType)
        {
            var rect = Layout.CreateRectField();
            EditorGUI.LabelField(rect.Adjust(0, Layout.labelWidth), title, editorStyle);
            EditorGUI.ObjectField(rect.Adjust(Layout.labelWidth, Layout.contentWidth), property.Get(field), objType,
                GUIContent.none);
        }

        public static void FieldToggleAndEnable(this SerializedObject property, string title, string field,
            int labelOffset = 0, int toggleOffset = 0, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Layout.BeginGUIEnable(property.Bool(field));
            Labels.Label(title, rect.Adjust(labelOffset, Layout.infoWidth - Layout.boolWidth));
            Layout.EndGUIEnable();
            EditorGUI.PropertyField(
                rect.Adjust(Layout.infoWidth - Layout.boolWidth - toggleOffset + 1, Layout.boolWidth),
                property.Get(field), GUIContent.none);
        }

        public static void FieldToggleAndEnable(this SerializedProperty property, string title, string field,
            int labelOffset = 0, int toggleOffset = 0)
        {
            var rect = Layout.CreateRectField();
            Layout.BeginGUIEnable(property.Bool(field));
            Labels.Label(title, rect.Adjust(labelOffset, Layout.infoWidth - Layout.boolWidth));
            Layout.EndGUIEnable();
            EditorGUI.PropertyField(
                rect.Adjust(Layout.infoWidth - Layout.boolWidth - toggleOffset + 1, Layout.boolWidth),
                property.Get(field), GUIContent.none);
        }

        public static void FieldToggle(this SerializedProperty property, string title, string field,
            int labelOffset = 0, int toggleOffset = 0, bool execute = true)
        {
            if (!execute)
                return;
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(labelOffset, Layout.infoWidth - Layout.boolWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.infoWidth - Layout.boolWidth - toggleOffset, Layout.boolWidth),
                property.Get(field), GUIContent.none);
        }

        public static void FieldToggle(this SerializedObject property, string title, string field, int labelOffset = 0,
            int toggleOffset = 0)
        {
            var rect = Layout.CreateRectField();
            Labels.Label(title, rect.Adjust(labelOffset, Layout.infoWidth - Layout.boolWidth));
            EditorGUI.PropertyField(rect.Adjust(Layout.infoWidth - Layout.boolWidth - toggleOffset, Layout.boolWidth),
                property.Get(field), GUIContent.none);
        }
    }
}
#endif