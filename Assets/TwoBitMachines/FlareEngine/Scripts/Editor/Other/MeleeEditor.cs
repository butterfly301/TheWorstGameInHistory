using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(Melee))]
    public class MeleeEditor : UnityEditor.Editor
    {
        public Color barColor = Tint.Grey100;
        private Melee main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as Melee;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();
            {
                var chained = parent.Get("melee");
                var block = parent.Get("block");
                Melee(chained);
                Inputs(chained);
                Blocking(block);
                Combos(chained);
                // Charge ();
                // Recoil ();
            }
            parent.ApplyModifiedProperties();

            Layout.VerticalSpacing(10);
        }

        public void Melee(SerializedProperty chained)
        {
            if (FoldOut.Bar(chained, Tint.Blue).Label("Melee").FoldOut())
            {
                FoldOut.Box(4, Tint.BoxTwo);
                {
                    parent.Field("Melee Name", "meleeName");
                    parent.Field("Melee Collider", "collider2DRef");
                    chained.Field("Collider Enable", "enableCollider");
                    chained.Field("Hit Layer", "layer");
                }
                Layout.VerticalSpacing(5);
            }
        }

        public void Inputs(SerializedProperty chained)
        {
            if (FoldOut.Bar(chained, Tint.Blue).Label("Input").FoldOut("inputFoldOut"))
            {
                FoldOut.Box(2, Tint.BoxTwo);
                {
                    chained.FieldDouble("Button 1", "trigger", "input");
                    chained.FieldDouble("Button 2", "trigger2", "input2");
                }
                Layout.VerticalSpacing(5);

                FoldOut.Box(2, Tint.BoxTwo);
                {
                    chained.Field("Attack From Sleep", "attackFromSleep");
                    chained.Field("Cancel Others", "cancelOtherAttacks");
                }
                Layout.VerticalSpacing(5);
            }
        }

        public void Blocking(SerializedProperty block)
        {
            if (FoldOut.Bar(block, Tint.Blue).Label("Blocking").BRE("canBlock").FoldOut())
            {
                GUI.enabled = block.Bool("canBlock");
                var type = block.Enum("mustHold");
                FoldOut.Box(6, Tint.BoxTwo);
                {
                    block.Field("Block Signal", "blockSignal");
                    block.Field("Block Button", "input");
                    block.Field("Must Hold", "mustHold", type == 0);
                    block.FieldDouble("Must Hold", "mustHold", "inputTwo", type == 1);
                    block.Field("Cancel Combo", "cancelCombo");
                    block.Field("Stop Vel X", "stopVelocityX");
                    block.Field("Deflect Projectiles", "canDeflect");
                }
                Layout.VerticalSpacing(5);

                FoldOut.Box(2, Tint.BoxTwo);
                {
                    block.FieldAndEnable("Recoil", "recoilTag", "canRecoil");
                    Labels.FieldText("Tag", 18);
                    GUI.enabled = block.Bool("canRecoil");
                    block.FieldDouble("Settings", "recoilVelocity", "recoilDuration");
                    Labels.FieldDoubleText("Velocity", "Time");
                }
                Layout.VerticalSpacing(5);
                GUI.enabled = true;
            }
        }

        public void Combos(SerializedProperty chained)
        {
            if (FoldOut.Bar(chained, Tint.Blue).Label("Combos").FoldOut("comboFoldOut"))
            {
                var combo = chained.Get("combo");

                GUI.enabled = combo.arraySize > 1;
                FoldOut.Box(5, Tint.BoxTwo, 3);
                {
                    chained.Field("Early Timer", "earlyTime");
                    chained.Field("Delay Timer", "delayTime");
                }
                GUI.enabled = true;
                parent.Field("Combo Cool Down", "coolDown");
                chained.FieldToggleAndEnable("Hit To Continue", "hitToContinue");
                chained.FieldToggleAndEnable("One Hit Per Combo", "oneHitPerCombo");

                if (FoldOut.FoldOutButton(chained.Get("eventsFoldOut")))
                    Fields.EventFoldOut(parent.Get("onCoolDown"), parent.Get("foldOut"), "On Cool Down",
                        color: Tint.BoxTwo);

                for (var i = 0; i < combo.arraySize; i++)
                {
                    var element = combo.Element(i);

                    var open = FoldOut.Bar(element, FoldOut.boxColor, 25)
                        .Label("Combo " + (i + 1), FoldOut.titleColor, false)
                        .BR("delete", "Delete", execute: element.Bool("foldOut")).FoldOut();
                    ListReorder.Grip(chained, combo, Bar.barStart.CenterRectHeight(), i, Tint.WarmWhite);

                    if (open)
                    {
                        var condition = element.Enum("condition");
                        var height = condition != 0 ? 1 : 0;
                        FoldOut.Box(6 + height, FoldOut.boxColor, offsetY: -2);
                        {
                            element.Field("Signal Name ", "animationSignal");
                            element.Field("Damage", "damage");
                            element.Field("Force", "forceDirection");

                            if (element.Enum("velXType") == 0)
                                element.Field("Velocity x", "velXType");
                            else
                                element.FieldDouble("Velocity x", "velX", "velXType");

                            if (element.Enum("velYType") == 0)
                                element.Field("Velocity y", "velYType");
                            else
                                element.FieldDouble("Velocity y", "velY", "velYType");

                            element.Field("Activate", "condition");
                            element.FieldToggle("State Change Exit", "exitOnStateChange", execute: condition != 0);
                        }
                        Layout.VerticalSpacing();

                        var title = element.Bool("optionsFoldOut") ? "v  Extra Options" : ">  Extra Options";
                        if (FoldOut.Bar(element, FoldOut.boxColor).Label(title, FoldOut.titleColor, false)
                            .FoldOut("optionsFoldOut"))
                        {
                            FoldOut.Box(5, FoldOut.boxColor, offsetY: -4, extraHeight: 7);
                            {
                                element.FieldAndEnable("Go To Next ", "earlyTime", "earlyNext");
                                element.FieldAndEnable("Recoil X ", "recoilX", "canRecoilX");
                                element.FieldAndEnable("Recoil Y ", "recoilY", "canRecoilY");
                                element.FieldToggleAndEnable("Is Locked ", "isLocked");
                                element.FieldToggleAndEnable("Keep Velocity Direction ", "keepInitialDirection");
                            }
                            if (FoldOut.FoldOutButton(element.Get("eventsFoldOut")))
                            {
                                Fields.EventFoldOutEffect(element.Get("onMeleeBegin"), element.Get("meleeWE"),
                                    element.Get("eventFoldOut"), "On Combo Begin", Tint.BoxTwo);
                                Fields.EventFoldOutEffect(element.Get("onHit"), element.Get("hitWE"),
                                    element.Get("hitFoldOut"), "On Hit", Tint.BoxTwo);
                            }
                        }


                        if (element.ReadBool("delete"))
                        {
                            combo.DeleteArrayElement(i);
                            break;
                        }
                    }
                }

                if (FoldOut.CornerButton(Tint.Delete)) combo.arraySize++;
            }
        }
    }
}