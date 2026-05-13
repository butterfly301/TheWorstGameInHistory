using System;
using System.Text;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [Serializable]
    public class RotateWeapon
    {
        [SerializeField] public AutoSeek autoSeek = new();
        [SerializeField] public Transform transformDirection;
        [SerializeField] public WeaponOrientation orientation;
        [SerializeField] public RotateType rotate;
        [SerializeField] public float angleOffset;
        [SerializeField] public float maxLimit;
        [SerializeField] public float minLimit;
        [SerializeField] public float fixedAngle;
        [SerializeField] public Vector2 fixedDirection;
        [SerializeField] public Vector2 joyStickDirection;
        [SerializeField] public bool diagonal;
        [SerializeField] public bool usingMouse = true;
        [SerializeField] public InputButtonSO up;
        [SerializeField] public InputButtonSO down;
        [SerializeField] public InputButtonSO left;
        [SerializeField] public InputButtonSO right;
        [SerializeField] public InputButtonSO joyStick;
        [SerializeField] public bool roundMouseDirection;
        [SerializeField] public bool setRotationSignals;
        [SerializeField] public int roundMouse = 45;
        [NonSerialized] private Vector3 mousePosition;
        [NonSerialized] private Vector3 previousCharacterPosition;
        [NonSerialized] private Vector3 previousWeaponPosition;
        [NonSerialized] private float rotateAngle;
        [NonSerialized] private Vector3 smoothDirection;
        [NonSerialized] private StringBuilder stringBuilder = new();

        [NonSerialized] private int weaponDirection;


        private bool clampAngle => minLimit != 0 || maxLimit != 0;
        private bool mouseRotation => orientation == WeaponOrientation.MouseDirection;

        public void Reset()
        {
            autoSeek.Reset();
            previousWeaponPosition = Vector3.zero; // avoid jitter in rotation
            previousCharacterPosition = Vector3.zero;
        }

        public void Execute(Firearm firearm, Character equipment, ref bool fire)
        {
            if (rotate == RotateType.NoRotation) return;
            if (rotate == RotateType.FixedDirection)
            {
                FixedDirection(firearm);
                return;
            }

            var transform = firearm.transform;
            // Get weapon direction
            // mousePosition = Util.MousePosition();
            SetMousePosition(transform);
            SetWeaponDirection(firearm, equipment);
            // Change weapon x position
            var lp = transform.localPosition;
            lp.x = weaponDirection > 0 ? firearm.localPosition.x : -firearm.localPosition.x;
            transform.localPosition = lp; // Util.FlipXSign(transform.localPosition, weaponDirection);
            // Rotate weapon
            if (rotate == RotateType.Mouse)
                RotateToMouse(transform, equipment.transform.right);
            else if (rotate == RotateType.EightDirection)
                RotateToEightDirection(transform);
            else if (rotate == RotateType.AutoSeek)
                RotateToAutoSeek(transform, equipment, ref fire);
            else if (rotate == RotateType.FixedAngle) Rotate(transform, fixedAngle * weaponDirection);

            if (setRotationSignals)
            {
                stringBuilder.Clear();
                stringBuilder.Append("rotateWeapon");
                stringBuilder.Append(rotateAngle.ToString("F0"));
                equipment.signals.Set(stringBuilder.ToString());
                equipment.signals.Set("rotateWeapon");
            }

            previousWeaponPosition = transform.position;
        }

        private void FixedDirection(Firearm firearm)
        {
            var v = new Vector2(Mathf.Abs(fixedDirection.x), fixedDirection.y);
            var angle = Vector2.Angle(Vector2.right, v) * Mathf.Sign(v.y);
            weaponDirection = fixedDirection.x > 0 ? 1 : -1;
            FixedRotate(firearm.transform, angle);
        }

        private void SetWeaponDirection(Firearm firearm, Character equipment)
        {
            if (firearm.IsRecoiling())
                equipment.signals.characterDirection =
                    weaponDirection; // if player is recoiling, retain same weapon direction or weapon may flip based on player input Override.
            if (orientation != WeaponOrientation.CharacterDirection)
            {
                var useTarget = orientation == WeaponOrientation.TransformDirection && transformDirection != null;
                var targetPosition = autoSeek.Rotate() ? autoSeek.Position(mousePosition) :
                    useTarget ? transformDirection.position : mousePosition;
                var targetDirection = (targetPosition - previousCharacterPosition).normalized;
                var direction = Compute.CrossSign(equipment.transform.up, targetDirection) >= 0 ? -1 : 1;

                equipment.signals.Set("mouseDirectionLeft",
                    direction == -1 && equipment.signals.characterDirection == 1);
                equipment.signals.Set("mouseDirectionRight",
                    direction == 1 && equipment.signals.characterDirection == -1);
                equipment.signals.characterDirection = direction;

                if (Player.mainPlayer != null && Player.mainPlayer.gameObject == equipment.gameObject)
                    Player.mainPlayer.abilities.playerDirection = direction;
            }

            previousCharacterPosition = equipment.transform.position; // avoid jitter
            weaponDirection = equipment.signals.characterDirection;
        }

        private void RotateToAutoSeek(Transform transform, Character equipment, ref bool fire)
        {
            autoSeek.Seek(equipment.transform.position, ref fire);
            var direction = autoSeek.Rotate()
                ? autoSeek.Position(mousePosition) - previousWeaponPosition
                : Vector3.right * weaponDirection;
            smoothDirection = Compute.LerpConditional(smoothDirection, direction.normalized, ref autoSeek.newTarget);
            var angle = Compute.AngleDirection(equipment.transform.right * weaponDirection, smoothDirection); //
            Rotate(transform, angle);
        }

        private void RotateToMouse(Transform transform, Vector2 characterRight)
        {
            var rightDirection = characterRight * weaponDirection;
            Vector2 mouseDirection =
                (mousePosition - previousWeaponPosition).normalized; // use previous position of weapon to avoid jitter
            if (roundMouseDirection)
                mouseDirection = RoundToNearest(mouseDirection);
            var rotate = Compute.AngleDirection(rightDirection, mouseDirection);

            if (clampAngle)
            {
                var minAngle = weaponDirection == 1 ? minLimit : -maxLimit;
                var maxAngle = weaponDirection == 1 ? maxLimit : -minLimit;

                if (rotate < minAngle ||
                    rotate > maxAngle) // clamp mouse to range,if not in range, make gun still rotate to mouse by checking which limit it's closest to
                {
                    var v1 = Compute.RotateVector(rightDirection, minAngle);
                    var v2 = Compute.RotateVector(rightDirection, maxAngle);
                    rotate = Vector3.Angle(v1, mouseDirection) < Vector3.Angle(v2, mouseDirection)
                        ? minAngle
                        : maxAngle;
                }
            }

            Rotate(transform, rotate, angleOffset);
        }

        private void RotateToEightDirection(Transform transform)
        {
            var inputX = right != null && right.Holding() ? 1 : left != null && left.Holding() ? -1 : 0;
            var inputY = up != null && up.Holding() ? 1 : down != null && down.Holding() ? -1 : 0;
            var angle = (diagonal && inputX != 0 ? 45f : 90f) * weaponDirection * inputY;
            Rotate(transform, angle);
        }

        private void Rotate(Transform transform, float angle, float angleOffset = 0)
        {
            rotateAngle = weaponDirection < 0 ? -angle : angle;
            transform.localRotation = Quaternion.Euler(0, weaponDirection < 0 ? 180f : 0f, rotateAngle + angleOffset);
        }

        private void FixedRotate(Transform transform, float angle)
        {
            transform.localRotation = Quaternion.Euler(0, weaponDirection < 0 ? 180f : 0f, angle);
        }

        private Vector2 RoundToNearest(Vector2 direction)
        {
            var angle = Vector2.SignedAngle(Vector2.right, direction);
            var roundedAngle = Mathf.RoundToInt(angle / roundMouse) * roundMouse;
            var roundedAngleRadians = roundedAngle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(roundedAngleRadians), Mathf.Sin(roundedAngleRadians));
        }

        private void SetMousePosition(Transform transform)
        {
            if (rotate != RotateType.Mouse || joyStick == null)
            {
                mousePosition = Util.MousePosition();
                return;
            }

            if (joyStick.valueV2 != Vector2.zero)
                usingMouse = false;
            else if (Input.GetMouseButtonDown(0)) usingMouse = true;
            if (usingMouse)
            {
                mousePosition = Util.MousePosition();
            }
            else
            {
                joyStickDirection = joyStick.valueV2 == Vector2.zero ? joyStickDirection : joyStick.valueV2 * 100f;
                mousePosition = transform.position + (Vector3)joyStickDirection;
            }
        }
    }

    public enum RotateType
    {
        Mouse,
        EightDirection,
        AutoSeek,
        FixedAngle,
        FixedDirection,
        NoRotation
    }

    public enum WeaponOrientation
    {
        CharacterDirection,
        MouseDirection,
        TransformDirection
    }
}