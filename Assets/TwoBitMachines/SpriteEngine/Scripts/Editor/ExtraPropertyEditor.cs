using System;
using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
    public static class SpriteProperty
    {
        #region Setup

        public delegate void Display(SerializedProperty property, int frameIndex, bool interpolate);

        public static List<Texture2D> icons = new();
        public static List<string> names = new();

        public static void PropertyIcons()
        {
            if (icons == null || icons.Count > 0)
                return;
            for (var i = 0; i < propertyTypes.Length; i++)
                icons.Add((Texture2D)EditorGUIUtility.IconContent(propertyTypes[i].Method.Name + " Icon").image);
        }

        public static Display[] propertyTypes =
        {
            Transform,
            SpriteRenderer,
            BoxCollider2D,
            CircleCollider2D,
            AreaEffector2D,
            CapsuleCollider2D,
            Rigidbody2D,
            EdgeCollider2D,
            PolygonCollider2D,
            CompositeCollider2D
            // ConstantForce2D,
            // DistanceJoint2D,
            // FixedJoint2D,
            // FrictionJoint2D,
            // HingeJoint2D
        };

        public static void NameList()
        {
            if (names == null || names.Count > 0)
                return;
            for (var i = 0; i < propertyTypes.Length; i++) names.Add(propertyTypes[i].Method.Name);
        }

        public static Display DisplayFunction(string name)
        {
            for (var i = 0; i < propertyTypes.Length; i++)
                if (name == propertyTypes[i].Method.Name)
                    return propertyTypes[i];

            return null;
        }

        public static void CreateProperty(int index, SerializedProperty array)
        {
            var typeName = propertyTypes[Mathf.Clamp(index, 0, propertyTypes.Length)].Method.Name;
            var fullTypeName = "TwoBitMachines.TwoBitSprite." + typeName + "Property";
            var type = EditorTools.RetrieveType(fullTypeName);
            if (type != null)
            {
                array.arraySize++;
                array.LastElement().managedReferenceValue = Activator.CreateInstance(type);
            }
        }

        #endregion

        #region Array Methods

        public static void MatchArraySize(SerializedProperty array, int sizeToMatch)
        {
            for (var i = 0; i < array.arraySize; i++)
            {
                var element = array.Element(i).Get("data");
                if (element == null || sizeToMatch == element.arraySize) continue;
                for (var j = element.arraySize; j < sizeToMatch; j++) element.arraySize++;
            }
        }

        public static void SecureSameSize(SerializedProperty array, int frameSize)
        {
            for (var i = 0; i < array.arraySize; i++)
            {
                var element = array.Element(i).Get("data");
                if (element == null) continue;
                if (element.arraySize > frameSize) element.arraySize--;
            }
        }

        public static void DeleteNestedArrayElement(SerializedProperty array, int deleteIndex)
        {
            for (var i = 0; i < array.arraySize; i++)
            {
                var element = array.Element(i).Get("data");
                if (element == null) continue;
                for (var j = 0; j < element.arraySize; j++)
                    if (deleteIndex == j)
                    {
                        element.DeleteArrayElement(j);
                        return;
                    }
            }
        }

        public static void ReorderNestedArrays(SerializedProperty array, int from, int to)
        {
            for (var i = 0; i < array.arraySize; i++)
            {
                var element = array.Element(i).Get("data");
                if (element == null || from >= element.arraySize || to >= element.arraySize) continue;
                element.MoveArrayElement(from, to);
            }
        }

        #endregion

        #region Inspector

        public static void Execute(SerializedProperty property, SerializedProperty frameIndex, ref bool propertyOpen)
        {
            propertyOpen = false;
            for (var i = 0; i < property.arraySize; i++)
            {
                if (property.Element(i) == null)
                    continue;
                var element = property.Element(i);
                if (element.Get("property") == null)
                    continue;

                Block.Header(element).Style(Tint.Box)
                    .Space(14)
                    .DropArrow("view")
                    .Field("property", rightSpace: 5)
                    .Button("xsMinus", Tint.Delete, hide: false)
                    .Space(3)
                    .Build();

                if (Header.SignalActive("xsMinus"))
                {
                    property.DeleteArrayElementAtIndex(i);
                    return;
                }

                if (element.Bool("view") && element.Get("property").objectReferenceValue != null)
                {
                    propertyOpen = true;
                    var type = property.Element(i).managedReferenceFullTypename.Split('.');
                    var typeName = type.Length > 2 ? type[2].Replace("Property", "") : "";
                    DisplayFunction(typeName)(element, frameIndex.intValue, element.Bool("interpolate"));
                    Field(element, "Interpolate", "interpolate", canInterpolate: false);
                }
            }
        }

        private static void Field(SerializedProperty property, string title, string field,
            SerializedProperty enable = null, SerializedProperty interpolate = null, bool canInterpolate = false)
        {
            var previous = GUI.enabled;
            GUI.enabled = enable == null || enable.boolValue;
            {
                Bar.Setup(Texture2D.whiteTexture, FoldOut.boxColor, false, 22);
                Bar.SpaceLeft(25);
                Bar.SpaceRight(2);
                var fieldRect = new Rect(Bar.barStart)
                    { y = Bar.barStart.y + 2, width = Layout.longInfoWidth - 115, height = 18 };
                Labels.Label(title, fieldRect);
                EditorGUI.PropertyField(fieldRect.Adjust(fieldRect.width, 40), property.Get(field), GUIContent.none);
            }
            GUI.enabled = previous;
            if (enable != null)
                Bar.ButtonRight(enable, "xsAdd", Tint.On, Tint.White);
            if (interpolate != null && enable != null && enable.boolValue && canInterpolate)
                Bar.ButtonRight(interpolate, "I", Color.yellow, Tint.White);
        }

        public static void SetExtraProperty(List<SpritePacket> sprites, int spriteIndex, bool spriteExists,
            int frameIndex)
        {
            if (!spriteExists || Application.isPlaying || sprites[spriteIndex] == null)
                return;

            var sprite =
                sprites[spriteIndex]; // set property values so you can see the actual changes during editor time
            for (var i = 0; i < sprite.property.Count; i++)
            {
                if (sprite.property[i] == null) continue;
                if (sprite.property[i].view)
                {
                    sprite.property[i].wasSet = true;
                    sprite.property[i].SetState(frameIndex, !sprite.property[i].alreadySaved);
                    sprite.property[i].alreadySaved = true;
                }

                if (!sprite.property[i].view && sprite.property[i].wasSet)
                {
                    sprite.property[i].wasSet = sprite.property[i].alreadySaved = false;
                    sprite.property[i].ResetToOriginalState();
                }
            }
        }

        public static void SetExtraProperty(SpriteEngineExtra main, bool spriteExists, int frameIndex)
        {
            if (!spriteExists || Application.isPlaying || main.sprites[main.spriteIndex] == null)
                return;

            var sprite =
                main.sprites
                    [main.spriteIndex]; // set property values so you can see the actual changes during editor time
            for (var i = 0; i < sprite.property.Count; i++)
            {
                if (sprite.property[i] == null)
                    continue;
                if (sprite.property[i].view)
                {
                    sprite.property[i].wasSet = true;
                    sprite.property[i].SetState(frameIndex, !sprite.property[i].alreadySaved);
                    sprite.property[i].alreadySaved = true;
                }

                if (!sprite.property[i].view && sprite.property[i].wasSet)
                {
                    sprite.property[i].wasSet = sprite.property[i].alreadySaved = false;
                    sprite.property[i].ResetToOriginalState();
                }
            }
        }

        #endregion

        #region Properties

        public static void BoxCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Size X", "sizeX", property.Get("useSizeX"), property.Get("interpolateSize"),
                canInterpolate);
            Field(element, "Size Y", "sizeY", property.Get("useSizeY"), property.Get("interpolateSize"),
                canInterpolate);
            Field(element, "Edge Radius", "edgeRadius", property.Get("useEdgeRadius"),
                property.Get("interpolateEdgeRadius"), canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("useEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void CircleCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Radius", "radius", property.Get("useRadius"), property.Get("interpolateRadius"),
                canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateEdgeDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("useEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void Transform(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Position X", "pX", property.Get("usepX"), property.Get("interpolatePosition"),
                canInterpolate);
            Field(element, "Position Y", "pY", property.Get("usepY"), property.Get("interpolatePosition"),
                canInterpolate);
            Field(element, "Position Z", "pZ", property.Get("usepZ"), property.Get("interpolatePosition"),
                canInterpolate);

            Field(element, "Scale X", "sX", property.Get("usesX"), property.Get("interpolateScale"), canInterpolate);
            Field(element, "Scale Y", "sY", property.Get("usesY"), property.Get("interpolateScale"), canInterpolate);
            Field(element, "Scale Z", "sZ", property.Get("usesZ"), property.Get("interpolateScale"), canInterpolate);

            Field(element, "Rotation X", "eX", property.Get("useeX"), property.Get("interpolateRotation"),
                canInterpolate);
            Field(element, "Rotation Y", "eY", property.Get("useeY"), property.Get("interpolateRotation"),
                canInterpolate);
            Field(element, "Rotation Z", "eZ", property.Get("useeZ"), property.Get("interpolateRotation"),
                canInterpolate);
        }

        public static void SpriteRenderer(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Color", "color", property.Get("useColor"), property.Get("interpolateColor"),
                canInterpolate);
            // Field (element, "Size", "size", property.Get ("useSize"), property.Get ("interpolateSize"), canInterpolate : canInterpolate);
            Field(element, "Render Priority", "renderPriority", property.Get("useRenderPriority"));
            Field(element, "Sorting Order", "sortingOrder", property.Get("useSortingOrder"));
            Field(element, "Adaptive Mode", "adaptiveMode", property.Get("useAdaptiveMode"));
            Field(element, "Receive Shadows", "receiveShadows", property.Get("useReceiveShadows"));
            Field(element, "FlipX", "flipX", property.Get("useFlipX"));
            Field(element, "FlipY", "flipY", property.Get("useFlipY"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void Rigidbody2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Angular Drag", "angularDrag", property.Get("useAngularDrag"),
                property.Get("interpolateAngularDrag"), canInterpolate);
            Field(element, "Gravity Scale", "gravityScale", property.Get("useGravityScale"),
                property.Get("interpolateGravityScale"), canInterpolate);
            Field(element, "Linear Drag", "linearDrag", property.Get("useLinearDrag"),
                property.Get("interpolateLinearDrag"), canInterpolate);
            Field(element, "Mass", "mass", property.Get("useMass"), property.Get("interpolateMass"), canInterpolate);
            Field(element, "Use Auto Mass", "useAutoMass", property.Get("useAutoMass"));
            Field(element, "Simulated", "simulated", property.Get("useSimulated"));
            Field(element, "Use Kinematic Contacts", "kinematicContacts", property.Get("useKinematicContacts"));
        }

        public static void AreaEffector2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Angular Drag", "angularDrag", property.Get("useAngularDrag"),
                property.Get("interpolateAngularDrag"), canInterpolate);
            Field(element, "Drag", "drag", property.Get("useDrag"), property.Get("interpolateDrag"), canInterpolate);
            Field(element, "Force Angle", "forceAngle", property.Get("useForceAngle"),
                property.Get("interpolateForceAngle"), canInterpolate);
            Field(element, "Force Magnitude", "forceMagnitude", property.Get("useForceMagnitude"),
                property.Get("interpolateForceMagnitude"), canInterpolate);
            Field(element, "Force Variation", "forceVariation", property.Get("useForceVariation"),
                property.Get("interpolateForceVariation"), canInterpolate);
            Field(element, "Use Collide Mask", "useColliderMask", property.Get("useColliderMask"));
            Field(element, "Use Global Angle", "useGlobalAngle", property.Get("useGlobalAngle"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void CapsuleCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Size X", "sizeX", property.Get("useSizeX"), property.Get("interpolateSize"),
                canInterpolate);
            Field(element, "Size Y", "sizeY", property.Get("useSizeY"), property.Get("interpolateSize"),
                canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("usedByEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void EdgeCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Edge Radius", "edgeRadius", property.Get("useEdgeRadius"),
                property.Get("interpolateEdgeRadius"), canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("usedByEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void PolygonCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("usedByEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void CompositeCollider2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Offset X", "offsetX", property.Get("useOffsetX"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Offset Y", "offsetY", property.Get("useOffsetY"), property.Get("interpolateOffset"),
                canInterpolate);
            Field(element, "Density", "density", property.Get("useDensity"), property.Get("interpolateDensity"),
                canInterpolate);
            Field(element, "Is Trigger", "isTrigger", property.Get("useIsTrigger"));
            Field(element, "Used By Effector", "usedByEffector", property.Get("usedByEffector"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void ConstantForce2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Force", "force", property.Get("useForce"), property.Get("interpolateForce"),
                canInterpolate);
            Field(element, "Relative Force", "relativeForce", property.Get("useRelativeForce"),
                property.Get("interpolateRelativeForce"), canInterpolate);
            Field(element, "Torque", "torque", property.Get("useTorque"), property.Get("interpolateTorque"),
                canInterpolate);
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void DistanceJoint2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Anchor", "anchor", property.Get("useAnchor"), property.Get("interpolateAnchor"),
                canInterpolate);
            Field(element, "Distance", "distance", property.Get("useDistance"), property.Get("interpolateDistance"),
                canInterpolate);
            Field(element, "Auto Config Anchor", "configAnchor", property.Get("useConfigAnchor"));
            Field(element, "Auto config Distance", "configDistance", property.Get("useConfigDistance"));
            Field(element, "Enable Collision", "enableCollision", property.Get("useEnableCollision"));
            Field(element, "Max Distance Only", "maxDistanceOnly", property.Get("useMaxDistanceOnly"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void FixedJoint2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Anchor", "anchor", property.Get("useAnchor"), property.Get("interpolateAnchor"),
                canInterpolate);
            Field(element, "Frequency", "frequency", property.Get("useFrequency"), property.Get("interpolateFrequency"),
                canInterpolate);
            Field(element, "Damping Ratio", "dampingRatio", property.Get("useDampingRatio"),
                property.Get("interpolateDampingRatio"), canInterpolate);
            Field(element, "Auto Config Anchor", "configAnchor", property.Get("useConfigAnchor"));
            Field(element, "Enable Collision", "enableCollision", property.Get("useEnableCollision"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void FrictionJoint2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Anchor", "anchor", property.Get("useAnchor"), property.Get("interpolateAnchor"),
                canInterpolate);
            Field(element, "Max Force", "maxForce", property.Get("useMaxForce"), property.Get("interpolateMaxForce"),
                canInterpolate);
            Field(element, "Max Torque", "maxTorque", property.Get("useMaxTorque"),
                property.Get("interpolateMaxTorque"), canInterpolate);
            Field(element, "Auto Config Anchor", "configAnchor", property.Get("useConfigAnchor"));
            Field(element, "Enable Collision", "enableCollision", property.Get("useEnableCollision"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        public static void HingeJoint2D(SerializedProperty property, int frameIndex, bool canInterpolate)
        {
            var element = property.Get("data").Element(frameIndex);
            Field(element, "Anchor X", "anchorX", property.Get("useAnchorX"), property.Get("interpolateAnchor"),
                canInterpolate);
            Field(element, "Anchor Y", "anchorY", property.Get("useAnchorY"), property.Get("interpolateAnchor"),
                canInterpolate);
            Field(element, "Lower Angle", "lowerAngle", property.Get("useLowerAngle"),
                property.Get("interpolateLowerAngle"), canInterpolate);
            Field(element, "Upper Angle", "upperAngle", property.Get("useUpperAngle"),
                property.Get("interpolateUpperAngle"), canInterpolate);
            Field(element, "Max Motor Force", "maximumMotorForce", property.Get("useMaxMotorForce"),
                property.Get("interpolateMaxMotorForce"), canInterpolate);
            Field(element, "Motor Speed", "motorSpeed", property.Get("useMotorSpeed"),
                property.Get("interpolateMotorSpeed"), canInterpolate);
            Field(element, "Auto Config Anchor", "configAnchor", property.Get("useConfigAnchor"));
            Field(element, "Enable Collision", "enableCollision", property.Get("useEnableCollision"));
            Field(element, "Use Limits", "useLimits", property.Get("useLimits"));
            Field(element, "Use Motor", "useMotor", property.Get("useMotor"));
            Field(element, "Enabled", "enabled", property.Get("useEnabled"));
        }

        #endregion
    }
}