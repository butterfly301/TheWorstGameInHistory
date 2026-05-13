using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera.TestMode
{
    public class TestMode : MonoBehaviour
    {
        [SerializeField] public Player player = new();
        [SerializeField] public PlaySprite anim = new();

        private void Awake()
        {
            anim.Initialize();
            player.Initialize(anim);
        }

        private void Update()
        {
            player.Update();
            anim.Update();
        }
    }

    [Serializable]
    public class Player
    {
        [SerializeField] public float moveSpeed = 25;

        [SerializeField] public Transform transform;

        private PlaySprite anim;
        [NonSerialized] private BoxCollider2D box2D;
        [NonSerialized] private Inputs button;
        [NonSerialized] private float gravity;
        [NonSerialized] private float jumpForce;

        [NonSerialized] private float jumpHeight = 7f;
        [NonSerialized] private float minJumpForce;
        [NonSerialized] private float minJumpHeight = 1f;
        [NonSerialized] private bool pointLeft;
        [NonSerialized] private bool pointPrevious;
        [NonSerialized] private SpriteRenderer render;
        [NonSerialized] private float speed;
        [NonSerialized] private float timeInAir = 0.5f;
        [NonSerialized] private Vector2 velocity;
        [NonSerialized] private WorldCollider world;
        [NonSerialized] private LayerMask worldCollision;

        public void Initialize(PlaySprite playSprite)
        {
            worldCollision = LayerMask.GetMask("World");
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeInAir, 2);
            jumpForce = Mathf.Abs(gravity) * timeInAir;
            minJumpForce = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
            render = transform.gameObject.GetComponent<SpriteRenderer>();
            box2D = transform.gameObject.GetComponent<BoxCollider2D>();
            world = new WorldCollider(box2D, transform, worldCollision);
            anim = playSprite;
            button = new Inputs();
        }

        public void Update()
        {
            button.Update();
            speed = Input.GetKey(KeyCode.P) ? moveSpeed * 2 : moveSpeed;
            Execute();
        }

        private void Execute()
        {
            var input = button.axis;
            velocity.x = input.x * speed;
            velocity.y += gravity * Time.deltaTime;
            Direction(input.x);
            if (world.ground || world.above)
                velocity.y = -0.1f;

            if (button.jumpHold && world.ground)
            {
                button.jumping = true; // in air
                velocity.y = jumpForce;
            }

            if (button.jumpReleased)
                if (velocity.y > minJumpForce)
                    velocity.y = minJumpForce;
            Direction(velocity.x);
            world.Check(velocity * Time.deltaTime);

            if (world.ground)
            {
                if (velocity.x == 0) anim.Set("Stand");
                else anim.Set("Run");
            }
            else
            {
                if (velocity.y > 0) anim.Set("Jump");
                else anim.Set("Fall");
            }

            render.flipX = pointLeft;
        }

        private void Direction(float left)
        {
            pointLeft = left == 0 ? pointLeft : !(left > 0);
            if (pointLeft != pointPrevious) pointPrevious = pointLeft;
        }
    }

    public class Inputs
    {
        [NonSerialized] public Vector2 axis;
        [NonSerialized] public bool jumpHold;
        [NonSerialized] public bool jumping;
        [NonSerialized] public bool jumpPressed;
        [NonSerialized] public bool jumpReleased;
        [NonSerialized] public bool left, right;
        [NonSerialized] public bool onWall;
        [NonSerialized] private bool pressReleased;
        [NonSerialized] public bool released;
        [NonSerialized] private float timer;
        [NonSerialized] public LayerMask wallMask;

        public void Update()
        {
            jumping = false;
            Keyboard();
            if (released && pressReleased) pressReleased = false;
        }

        public void Keyboard()
        {
            axis = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            jumpHold = Input.GetKey(KeyCode.Space);
            jumpReleased = Input.GetKeyUp(KeyCode.Space);
            jumpPressed = Input.GetKeyDown(KeyCode.Space);

            released = Input.GetKeyUp(KeyCode.J);
            left = Input.GetKey(KeyCode.A);
            right = Input.GetKey(KeyCode.D);
        }
    }

    public class WorldCollider
    {
        public const float skinWidth = 0.015f;
        [NonSerialized] public bool above, ground, left, right, movingPlatformY, movingPlatformX, nearWall, mid;
        [NonSerialized] public Vector2 bottomLeft, bottomRight;
        [NonSerialized] public BoxCollider2D collider;
        [NonSerialized] private readonly LayerMask collisionMask;
        [NonSerialized] public int horizontalRays = 3;
        [NonSerialized] public float horizontalSpacing;
        [NonSerialized] public float horizontalSpacingFlipped;
        [NonSerialized] public Transform movingPlatTransform;
        [NonSerialized] public Bounds player;
        [NonSerialized] public Vector2 topLeft, topRight;

        [NonSerialized] private readonly Transform transform;
        [NonSerialized] public int verticalRays = 2;
        [NonSerialized] public float verticalSpacing;
        [NonSerialized] public float verticalSpacingFlipped;

        public WorldCollider(BoxCollider2D collider, Transform transform, LayerMask collisionMask)
        {
            this.collider = collider;
            this.transform = transform;
            this.collisionMask = collisionMask;
            CalculateRaysSpacing();
        }

        public void Update(BoxCollider2D collider)
        {
            var bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);
            bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            topLeft = new Vector2(bounds.min.x, bounds.max.y);
            topRight = new Vector2(bounds.max.x, bounds.max.y);
            above = ground = left = right = mid = movingPlatformY = movingPlatformX = nearWall = false;
        }

        public void CalculateRaysSpacing()
        {
            var bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);
            horizontalRays = Mathf.Clamp(horizontalRays, 2, int.MaxValue);
            verticalRays = Mathf.Clamp(verticalRays, 2, int.MaxValue);
            horizontalSpacing = bounds.size.y / (horizontalRays - 1);
            verticalSpacing = bounds.size.x / (verticalRays - 1);
        }

        public void Check(Vector2 velocity)
        {
            Update(collider);
            HorizontalCollision(ref velocity);
            VerticalCollision(ref velocity);
            transform.Translate(velocity);
        }

        public void HorizontalCollision(ref Vector2 velocity)
        {
            if (velocity.x == 0)
                return;

            var directionX = Mathf.Sign(velocity.x);
            var rayLength = Mathf.Abs(velocity.x) + skinWidth;
            for (var i = 0; i < horizontalRays; i++)
            {
                var rayOrigin = directionX == -1 ? bottomLeft : bottomRight;
                rayOrigin += Vector2.up * (horizontalSpacing * i);
                var wall = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                if (wall)
                {
                    if (wall.distance == 0)
                        continue;
                    velocity.x = (wall.distance - skinWidth) * directionX;
                    rayLength = wall.distance;
                    left = directionX == -1;
                    right = directionX == 1;
                    nearWall = left || right;
                    if (i != 2) mid = true;
                }
            }
        }

        public void VerticalCollision(ref Vector2 velocity)
        {
            if (velocity.y == 0)
                return;
            var directionY = Mathf.Sign(velocity.y);
            var rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (var i = 0; i < verticalRays; i++)
            {
                var rayOrigin = directionY == -1 ? bottomLeft : topLeft;
                rayOrigin += Vector2.right * (verticalSpacing * i + velocity.x);
                var wall = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                if (wall)
                {
                    velocity.y = (wall.distance - skinWidth) * directionY;
                    rayLength = wall.distance;
                    ground = directionY == -1;
                    above = directionY == 1;
                }
            }
        }
    }

    [Serializable]
    public class PlaySprite
    {
        [SerializeField] public Sprites[] sprites;
        [SerializeField] public Transform transform;
        private Dictionary<string, Sprites> sprite = new();

        public bool pause { get; set; }
        public Sprites current { get; set; }
        public SpriteRenderer render { get; set; }

        public void Initialize()
        {
            render = transform.gameObject.GetComponent<SpriteRenderer>();
            InitializeSprites();
            SetSpriteReference();
            if (sprites.Length > 0)
                current = sprites[0];
        }

        public void Set(string spriteName)
        {
            if (current.name == spriteName)
                return;
            Reset(spriteName);
        }

        private void Reset(string name)
        {
            Sprites newSprite;
            if (sprite.TryGetValue(name, out newSprite))
            {
                current = newSprite.Reset();
                pause = false;
            }
        }

        public void Update()
        {
            if (!pause)
                current.Play();
        }

        public void InitializeSprites()
        {
            for (var i = 0; i < sprites.Length; i++)
                sprites[i].Initialize(this);
        }

        public void SetSpriteReference()
        {
            for (var i = 0; i < sprites.Length; i++)
                sprite.Add(sprites[i].name, sprites[i]);
        }
    }

    [Serializable]
    public class Sprites
    {
        [SerializeField] public string name;
        [SerializeField] public float rate;
        [SerializeField] public Sprite[] frame;

        [NonSerialized] private PlaySprite play;
        [NonSerialized] private float timer;

        public int next { get; private set; }

        public void Initialize(PlaySprite script)
        {
            play = script;
        }

        public Sprites Reset()
        {
            timer = next = 0;
            play.render.sprite = frame[next];
            return this;
        }

        public void Play()
        {
            if (Clock.Timer(ref timer, rate))
            {
                next = ++next == frame.Length ? 0 : next;
                play.render.sprite = frame[next];
            }
        }
    }
}