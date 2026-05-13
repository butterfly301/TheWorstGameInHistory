using System;
using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
    public static class SpriteTreeEditor
    {
        private static List<string> sprites;
        private static List<string> signals;
        private static readonly int nodeLimit = 4;
        public static string inputName = "Name";
        public static float[] depthVal = { 0, 0.15f, 0.45f, 0.8f, 0, 0.9f, 0.9f, 0.9f };
        public static List<string> spriteDirection = new() { "flipLeft", "flipRight", "flipUp", "flipDown", "None" };

        public static void TreeInspector(Editor editor, SpriteTree main, SerializedProperty tree,
            List<string> spriteNames, List<string> newSignals)
        {
            signals = newSignals;
            sprites = spriteNames;
            Signals(editor, main, tree.Get("signals"), tree);
            TreeState(editor, tree.Get("branch"), tree.Get("spriteFlip"), tree);
        }

        private static void Signals(Editor editor, SpriteTree main, SerializedProperty signal, SerializedProperty tree)
        {
            var open = tree.Bool("signalFoldOut");
            if (Block.Header(tree).Style(Tint.SoftDark).Fold("Signals", "signalFoldOut", true, Tint.White)
                .Field("search", 0.5f, rightSpace: 5, execute: open).Button("Sort", tooltip: "Sort").Build())
            {
                var search = tree.String("search");
                Labels.FieldText("Search", 20, yOffset: 4, execute: search == "");
                var signalRef = tree.Get("signalRef");
                var defaultSignals = signalRef.Get("all");
                var userCreated = signalRef.Get("extra");
                var index = tree.Get("scrollIndex");
                var delete = -1;

                if (Block.InputAndButtonBox("Create Signal", "Add", Tint.Blue, ref inputName))
                {
                    userCreated.arraySize++;
                    userCreated.LastElement().Get("name").stringValue = inputName;
                    userCreated.LastElement().Get("use").boolValue = true;
                    index.intValue = userCreated.arraySize + defaultSignals.arraySize - 12; // max items in widow
                    AddSignal(signal, inputName);
                    inputName = "Name";
                }

                if (Header.SignalActive("Sort"))
                {
                    Sort(defaultSignals);
                    Sort(userCreated);
                }

                if (search == "")
                    Block.CatalogProperty(12, 18, true, false).Register(defaultSignals).Register(userCreated, true)
                        .Scroll(editor, 5, index,
                            element => { DisplaySignal(element, signal, userCreated, ref delete); });
                else
                    Block.CatalogProperty(12, 18, true, false).Register(defaultSignals).Register(userCreated, true)
                        .Search(editor, search,
                            element => { DisplaySignal(element, signal, userCreated, ref delete); });
                if (delete >= 0) userCreated.Delete(delete);
            }
        }

        private static void DisplaySignal(SerializedProperty element, SerializedProperty signal,
            SerializedProperty userCreated, ref int delete)
        {
            var canDelete = element.Bool("canDelete");
            var enabled = element.Bool("use");
            var name = element.String("name");
            var active = enabled ? Tint.White : Tint.WhiteOpacity100;

            Block.Header(element)
                .BoxRect(Tint.Box * active, "Item", height: 17, bottomSpace: 1)
                .Fold(name, "use", color: Color.black * (active * 1.15f))
                .Button("Delete", Tint.White * active, hide: false, execute: canDelete)
                .Build();

            if (enabled != element.Bool("use"))
            {
                if (element.Bool("use"))
                    AddSignal(signal, name);
                else
                    RemoveSignal(signal, name);
            }

            if (canDelete && Header.SignalActive("Delete"))
            {
                RemoveSignal(signal, name);
                for (var i = 0; i < userCreated.arraySize; i++)
                    if (name == userCreated.Element(i).String("name"))
                    {
                        delete = i;
                        break;
                    }
            }
        }

        private static void AddSignal(SerializedProperty signal, string name)
        {
            for (var i = 0; i < signal.arraySize; i++)
                if (signal.Element(i).stringValue == name)
                    return;

            signal.arraySize++;
            signal.LastElement().stringValue = name;
        }

        private static void RemoveSignal(SerializedProperty signal, string name)
        {
            for (var i = 0; i < signal.arraySize; i++)
                if (signal.Element(i).stringValue == name)
                {
                    signal.MoveArrayElement(i, signal.arraySize - 1);
                    signal.arraySize--;
                    return;
                }
        }

        private static void Sort(SerializedProperty array)
        {
            for (var i = 0; i < array.arraySize - 1; i++)
            for (var j = 0; j < array.arraySize - i - 1; j++)
            {
                var first = array.Element(j);
                var next = array.Element(j + 1);

                if (string.Compare(first.String("name"), next.String("name"), StringComparison.Ordinal) > 0)
                {
                    var temp = first.String("name");
                    first.Get("name").stringValue = next.String("name");
                    next.Get("name").stringValue = temp;
                }
            }
        }

        private static void TreeState(Editor editor, SerializedProperty branchArray, SerializedProperty spriteFlip,
            SerializedProperty tree)
        {
            if (Block.Header(tree).Style(Tint.SoftDark).Fold("State", "stateFoldOut", true, Tint.White).Button()
                .Build())
            {
                if (Header.SignalActive("Add"))
                {
                    branchArray.CreateNewElement();
                    branchArray.MoveLastToFirst();
                }

                if (signals.Count == 0)
                {
                    if (spriteFlip.arraySize == 1 &&
                        spriteFlip.LastElement().Get("nodes").arraySize <=
                        1) // if no signals, remove sprite flip if size of one to prevent a sprite flip!
                        spriteFlip.arraySize = 0;
                    GUILayout.Label("No signals available.");
                    return;
                }

                for (var i = 0; i < branchArray.arraySize; i++) Tree(editor, branchArray.Element(i), branchArray, i);
                Tree(editor, spriteFlip.IncIfZero().LastElement(), null, 0, 0, true);
            }
        }

        private static void Tree(Editor editor, SerializedProperty branch, SerializedProperty parentArray, int index,
            int depth = 0, bool spriteFlip = false)
        {
            Bar(editor, branch, parentArray, spriteFlip ? Tint.Blue : Tint.Orange, index, depth, depth + 1 < nodeLimit,
                !spriteFlip || (spriteFlip && depth > 0));

            var nodes = branch.Get("nodes");
            if (nodes != null && depth + 1 < nodeLimit && Header.SignalActive("Add"))
            {
                nodes.CreateNewElement();
                nodes.MoveLastToFirst();
                branch.SetTrue("foldOut");
                return;
            }

            if (parentArray != null && Header.SignalActive("Delete"))
            {
                parentArray.DeleteArrayElement(index);
                return;
            }

            if (!branch.Bool("foldOut")) return;
            if (nodes == null || nodes.arraySize == 0) // display chosen sprite
                branch.DropList(Block.Rect(20, shiftX: 110), "sprite", spriteFlip ? spriteDirection : sprites,
                    Tint.Green * 1.1f, 2);
            if (++depth >= nodeLimit || nodes == null) return;
            for (var i = 0; i < nodes.arraySize; i++) // iterate children
                Tree(editor, nodes.Element(i), nodes, i, depth, spriteFlip);
        }

        public static void Bar(Editor editor, SerializedProperty property, SerializedProperty array, Color barColor,
            int index, int depth, bool showAdd = true, bool showDelete = true)
        {
            Block.Header(property).Style(barColor * (1.15f - depthVal[depth]), shiftX: depth * 20)
                .Grip(editor, array, index, execute: showDelete)
                .Space(14, !showDelete)
                .DropArrow()
                .DropList("signal", signals)
                .HiddenButton("delete", "Delete", execute: showDelete)
                .Toggle("delete", "DeleteAsk", execute: showDelete)
                .Space(18, !showDelete)
                .Button(execute: showAdd)
                .Space(16, !showAdd)
                .Build();
        }
    }
}