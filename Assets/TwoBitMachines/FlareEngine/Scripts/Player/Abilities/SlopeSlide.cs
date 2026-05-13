#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [AddComponentMenu("")]
    public class SlopeSlide : Ability
    {
        public enum SlideType
        {
            Button,
            Automatic
        }

        [SerializeField] public string button;
        [SerializeField] public SlideType type;
        [SerializeField] public float scale = 1f;
        [SerializeField] public float exitTime = 1f;
        [SerializeField] public float rangeStart = 5f;
        [SerializeField] public float rangeEnd = 88f;
        [SerializeField] public float accelerate = 1f;

        [SerializeField] public float jumpBoostRate = 1f;
        [SerializeField] public float jumpBoostScale = 1.5f;
        [SerializeField] public bool jumpBoost;

        [SerializeField] public bool canAccelerate;
        [SerializeField] public bool autoSlide;
        [SerializeField] public bool useSlideTag;
        [SerializeField] public string slideTag;

        [SerializeField] public LayerMask damageLayer;
        [SerializeField] public bool dealDamage;
        [SerializeField] public float damageAmount = 5f;
        [SerializeField] public float damageForce = 1f;

        [SerializeField] public string enterWE;
        [SerializeField] public string exitWE;
        [SerializeField] public string slideWE;
        [SerializeField] public float onSlideRate;
        [SerializeField] public UnityEventEffect onEnter;
        [SerializeField] public UnityEventEffect onExit;
        [SerializeField] public UnityEventEffect onSlide;
        [NonSerialized] private Vector2 accel;
        [NonSerialized] private float counter;
        [NonSerialized] private float direction;

        [NonSerialized] private Health health;
        [NonSerialized] private bool isSliding;
        [NonSerialized] private float jumpBoostCount;
        [NonSerialized] private float slideRateCounter;
        [NonSerialized] private float slowDown = 1f;

        private bool automatic => type == SlideType.Automatic;

        private void Awake()
        {
            health = gameObject.GetComponent<Health>();
            autoSlide = false;
        }

        public override void Reset(AbilityManager player)
        {
            if (isSliding && dealDamage && health != null)
                health.CanTakeDamage(true);
            accel = Vector2.zero;
            isSliding = false;
            slowDown = 1f;
            jumpBoostCount = 0;
            direction = 0;
            counter = 0;
        }

        public override bool TurnOffAbility(AbilityManager player)
        {
            Reset(player);
            return true;
        }

        public override bool IsAbilityRequired(AbilityManager player, ref Vector2 velocity)
        {
            if (pause)
                return false;

            if (isSliding && player.world.onWall)
            {
                OnExit(player);
                Reset(player);
                return false;
            }

            if (isSliding && direction != 0 && velocity.x != 0 && !Compute.SameSign(direction, velocity.x))
            {
                OnExit(player);
                Reset(player); // player changed direction while sliding, exit out
                return false;
            }

            if (isSliding && (!player.world.climbingSlopeDown || ValidAngle(player)) &&
                (exitTime <= 0 || counter >= exitTime))
            {
                OnExit(player);
                Reset(player); // no ease time or ease time complete
                return false;
            }

            if (isSliding) return true;
            if (player.inputs.Holding(button) && player.world.climbingSlopeDown && ValidAngle(player) &&
                SlideOnLayer(player))
            {
                OnEnter(player);
                return isSliding = true;
            }

            if (automatic && ValidAngle(player) &&
                Compute.SameSign(player.playerDirection, player.world.groundNormal.x) && SlideOnLayer(player))
            {
                OnEnter(player);
                return isSliding = true;
            }

            return false;
        }

        public override void ExecuteAbility(AbilityManager player, ref Vector2 velocity,
            bool isRunningAsException = false)
        {
            player.signals.Set("slopeSlide");

            if (ValidAngle(player))
            {
                counter = 0;
                if (canAccelerate)
                {
                    accel.x += Time.deltaTime * accelerate * 10f;
                    direction = player.playerDirection * accel.x +
                                scale; // add initial boost so it has some velocity to start
                    velocity.x = direction;
                    jumpBoostCount += Time.deltaTime * jumpBoostRate + scale;
                }
                else
                {
                    direction = player.playerDirection * player.speed;
                    velocity.x = direction * scale;
                    jumpBoostCount += Time.deltaTime * jumpBoostRate;
                }

                if (dealDamage && direction != 0) Attack(player, Mathf.Sign(direction), direction * Time.deltaTime);
            }
            else if (exitTime > 0)
            {
                var speed = !player.world.onGround ? 0.5f : 1f;
                counter += Time.deltaTime * speed;
                var percent = Mathf.Clamp01(1f - counter / exitTime);

                if (player.inputX != 0 && !Compute.SameSign(player.inputX, direction))
                {
                    var slowDownRate = Mathf.Clamp(scale, 1f, 100f);
                    slowDown = Mathf.Clamp01(slowDown -
                                             Time.deltaTime * slowDownRate); // this will only apply if player is in air
                }

                if (canAccelerate)
                    velocity.x = direction * percent * slowDown;
                else
                    velocity.x = direction * scale * percent * slowDown;
                jumpBoostCount -= Time.deltaTime * jumpBoostRate;
                player.signals.Set("slopeSlideAuto");
            }

            if (player.world.onGround && onSlideRate > 0 && Clock.Timer(ref slideRateCounter, onSlideRate))
            {
                var impact = ImpactPacket.impact.Set(slideWE, player.world.transform, player.world.boxCollider,
                    player.world.transform.position, null, -player.world.box.right * player.playerDirection,
                    player.playerDirection, 0);
                onSlide.Invoke(impact);
            }

            if (velocity.x != 0) player.UpdateVelocityGround();
            if (jumpBoost) player.jumpBoost = Mathf.Clamp(jumpBoostCount, 1f, jumpBoostScale);
        }

        public void Attack(AbilityManager player, float signX, float velX)
        {
            health?.CanTakeDamage(false);
            var normal = player.world.groundNormal;
            var direction = normal.Rotate(90f * signX);
            var corner = player.world.box.BottomCorner(signX);
            var enemy = Physics2D.Raycast(corner, direction, velX * 1.75f, damageLayer);
            // Debug.DrawRay (corner, direction * velX * 1.25f, Color.red);
            if (enemy) Health.IncrementHealth(transform, enemy.transform, -damageAmount, direction * damageForce);
        }

        public bool ValidAngle(AbilityManager player)
        {
            var angle = Vector2.Angle(player.world.box.up, player.world.groundNormal);
            return angle >= rangeStart && angle <= rangeEnd;
        }

        public bool SlideOnLayer(AbilityManager player)
        {
            return !useSlideTag || (player.world.verticalTransform != null &&
                                    player.world.verticalTransform.gameObject.CompareTag(slideTag));
        }

        private void OnEnter(AbilityManager player)
        {
            onEnter.Invoke(ImpactPacket.impact.Set(enterWE, transform, player.world.boxCollider, player.world.position,
                null, -player.world.box.right * player.playerDirection, player.playerDirection, 0));
        }

        private void OnExit(AbilityManager player)
        {
            onExit.Invoke(ImpactPacket.impact.Set(exitWE, transform, player.world.boxCollider, player.world.position,
                null, -player.world.box.right * player.playerDirection, player.playerDirection, 0));
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool eventsFoldOut;
        [SerializeField] [HideInInspector] private bool enterFoldOut;
        [SerializeField] [HideInInspector] private bool exitFoldOut;
        [SerializeField] [HideInInspector] private bool slideFoldOut;
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Slope Slide", barColor, labelColor))
            {
                var type = parent.Enum("type");
                var height = parent.Bool("dealDamage") ? 2 : 0;
                FoldOut.Box(8 + height, FoldOut.boxColorLight, 5, -2);
                {
                    parent.DropDownFieldAndList(inputList, "Button", "type", "button", type == 0);
                    parent.Field("Button", "type", type == 1);
                    parent.FieldDouble("Angle Range", "rangeStart", "rangeEnd");
                    Labels.FieldDoubleText("Min", "Max");
                    parent.Field("Speed Boost", "scale");
                    parent.Field("Exit Time", "exitTime");

                    parent.FieldDoubleAndEnable("Jump Boost", "jumpBoostRate", "jumpBoostScale", "jumpBoost");
                    Labels.FieldDoubleText("Rate", "Boost", rightSpacing: 18);

                    parent.FieldAndEnable("Accelerate ", "accelerate", "canAccelerate");
                    parent.FieldAndEnable("Use Slide Tag", "slideTag", "useSlideTag");
                    parent.FieldAndEnable("Deal Damage", "damageLayer", "dealDamage");

                    var dealDamage = parent.Bool("dealDamage");
                    parent.Field("Damage Amount", "damageAmount", dealDamage);
                    parent.Field("Damage Force", "damageForce", dealDamage);
                }
                if (FoldOut.FoldOutButton(parent.Get("eventsFoldOut")))
                {
                    Fields.EventFoldOutEffect(parent.Get("onEnter"), parent.Get("enterWE"), parent.Get("enterFoldOut"),
                        "On Start", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("onExit"), parent.Get("exitWE"), parent.Get("exitFoldOut"),
                        "On Exit", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffectAndRate(parent.Get("onSlide"), parent.Get("slideWE"),
                        parent.Get("onSlideRate"), parent.Get("slideFoldOut"), "On Sliding", FoldOut.boxColor);
                }
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}