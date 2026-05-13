using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [Serializable]
    public class AbilityManager
    {
        [SerializeField] public Walk walk = new(); //* default abilities, always run
        [SerializeField] public Gravity gravity = new();

        [NonSerialized] private List<Ability> ability;
        [NonSerialized] public int airJumpCount;
        [NonSerialized] public bool airMomentumActive;
        [NonSerialized] private List<Ability> buffer = new();
        [NonSerialized] public Character character;
        [NonSerialized] public bool checkForAirJumps;
        [NonSerialized] public bool checkForMomentum;
        [NonSerialized] public bool dashAirJumpCheck;
        [NonSerialized] public float dashBoost;
        [NonSerialized] public Vector2 externalVelocity;
        [NonSerialized] public Vector2 finalVelocity;
        [NonSerialized] public bool hasJumped;
        [NonSerialized] public bool holdingDown;

        [NonSerialized] public UserInputs inputs;
        [NonSerialized] public float inputX;
        [NonSerialized] public float jumpBoost;
        [NonSerialized] public bool jumpButtonActive;

        [NonSerialized] public bool jumpButtonHold;
        [NonSerialized] public bool jumpButtonPressed;
        [NonSerialized] public bool jumpButtonReleased;
        [NonSerialized] public bool lockVelX;
        [NonSerialized] public float maxJumpVel;
        [NonSerialized] public float minJumpVel;
        [NonSerialized] public bool onSurface;
        [NonSerialized] public bool onVehicle;
        [NonSerialized] public int playerDirection;
        [NonSerialized] public bool pressedDown;

        [NonSerialized] private int priorityID;
        [NonSerialized] private int priorityIndex;
        [NonSerialized] public bool pushBackActive;

        [NonSerialized] public int setAirJump;
        [NonSerialized] public AnimationSignals signals;

        [NonSerialized] public float speed;

        [NonSerialized] public Vector2 velocity;
        [NonSerialized] public float velocityOnGround;
        [NonSerialized] public float velocityX;

        [NonSerialized] public bool wasJumping;
        [NonSerialized] public WorldCollision world;

        public float gravityEffect => gravity.gravityEffect;
        public bool ground => onSurface || world.onGround;
        public bool tryingToJump => jumpButtonPressed || jumpButtonHold;
        public bool crouching => world.boxCollider.size.y != world.box.boxSize.y;

        public void Initialize(Character characterRef, UserInputs inputRef, List<Ability> abilityRef, Player playerRef)
        {
            character = characterRef;
            world = characterRef.world;
            signals = playerRef.signals;
            ability = abilityRef;
            inputs = inputRef;

            dashBoost = 1f;
            playerDirection = 1;
            gravity.Initialize();
            walk.Initialize();

            for (var i = 0; i < abilityRef.Count; i++) abilityRef[i].Initialize(playerRef);
        }

        public void ResetAll()
        {
            dashBoost = 1f;
            playerDirection = 1;
            velocityOnGround = 0;
            velocity = Vector2.zero;
            finalVelocity = Vector2.zero;
            externalVelocity = Vector2.zero;

            jumpButtonReleased = false;
            jumpButtonPressed = false;
            airMomentumActive = false;
            dashAirJumpCheck = false;
            jumpButtonActive = false;
            checkForAirJumps = false;
            checkForMomentum = false;
            pushBackActive = false;
            jumpButtonHold = false;
            holdingDown = false;
            hasJumped = false;
            onSurface = false;
            onVehicle = false;
            wasJumping = false;
            lockVelX = false;

            walk.Reset();
            world.Reset();
            signals.SetDirection(1);

            for (var i = 0; i < ability.Count; i++) ability[i].Reset(this);
        }

        public void Execute()
        {
            buffer.Clear();
            world.hasJumped = false;
            wasJumping = hasJumped;
            onSurface = false;
            hasJumped = false;
            lockVelX = false;
            priorityIndex = 0;
            priorityID = int.MaxValue;

            GetDownAndJumpInputs();
            gravity.Execute(world.onCeiling || ground, ref velocity);
            walk.Execute(this, world, ref velocity);


            FindActiveAbilities();
            FindTopPriority();
            TurnOffAbilitiesSafely();
            ExecuteAbilities();
            LateExecuteAbilities();


            finalVelocity = velocity + externalVelocity;
            character.initialVelocity = finalVelocity;
            externalVelocity = Vector2.zero;
        }

        private void FindActiveAbilities()
        {
            for (var i = 0; i < ability.Count; i++)
                ability[i].EarlyExecute(this, ref velocity); // executes before isAbilityRequired
            for (var i = 0; i < ability.Count; i++)
                if (ability[i].IsAbilityRequired(this, ref velocity))
                    buffer.Add(ability[i]);
        }

        private void FindTopPriority()
        {
            for (var i = 0; i < buffer.Count; i++)
                if (buffer[i].ID < priorityID)
                {
                    priorityID = buffer[i].ID;
                    priorityIndex = i;
                }
        }

        private void TurnOffAbilitiesSafely()
        {
            if (buffer.Count == 0 || priorityIndex >= buffer.Count) return;

            for (var i = 0; i < buffer.Count; i++)
                if (i != priorityIndex && !buffer[priorityIndex].ContainsException(buffer[i].abilityName))
                    if (!buffer[i].TurnOffAbility(
                            this)) //                ability can't turn off safely, so it takes priority, ie crouch and wall
                    {
                        buffer[priorityIndex].TurnOffAbility(this); // turn off main ability and remove from list
                        buffer.RemoveAt(
                            priorityIndex); //             priority Index should not execute since an ability it's trying to override can't turn off safely
                        FindTopPriority();
                        TurnOffAbilitiesSafely(); //                  keep removing abilities until none of them conflict
                        return;
                    }
        }

        private void ExecuteAbilities()
        {
            if (buffer.Count > 0 && priorityIndex < buffer.Count)
            {
                buffer[priorityIndex].ExecuteAbility(this, ref velocity); //   execute ability with highest priority

                for (var i = 0; i < buffer.Count; i++)
                    if (i != priorityIndex && buffer[priorityIndex].ContainsException(buffer[i].abilityName))
                        buffer[i].ExecuteAbility(this, ref velocity, true); //  execute abilities with exceptions
            }
        }

        private void LateExecuteAbilities()
        {
            for (var i = 0; i < ability.Count; i++) ability[i].LateExecute(this, ref velocity);
        }

        public bool HigherPriority(string priorityCheck, int ID)
        {
            return priorityID < ID && priorityIndex >= 0 && priorityIndex < ability.Count &&
                   !ability[priorityIndex].ContainsException(priorityCheck);
        }

        public void PostCollisionExecute(Vector2 velocity)
        {
            for (var i = 0; i < ability.Count; i++) ability[i].PostCollisionExecute(this, velocity);
        }

        public void PostAIExecute()
        {
            for (var i = 0; i < ability.Count; i++) ability[i].PostAIExecute(this);
        }

        public void ClearYVelocity()
        {
            velocity.y = 0;
        }

        public void OnSurface(bool value = true)
        {
            onSurface = value;
        }

        public void StopRun()
        {
            walk.runSmoothInVelocity = 0; // will need to make run into its own ability in Flare 2.0
            walk.isRunning = false;
        }

        public void CheckForAirJumps(int setAirJumps = 0, bool setHasJumped = true)
        {
            checkForAirJumps = true;
            setAirJump = setAirJumps;
            hasJumped = setHasJumped;
        }

        public void UpdateVelocityGround()
        {
            velocityOnGround = velocity.x;
        }

        private void GetDownAndJumpInputs()
        {
            holdingDown = inputs.Holding("Down");
            pressedDown = inputs.Pressed("Down");
            jumpButtonHold = inputs.Holding("Jump");
            jumpButtonPressed = inputs.Pressed("Jump");
            jumpButtonReleased = inputs.Released("Jump");
            world.holdingDown = holdingDown;
            world.pressedDown = holdingDown && jumpButtonHold;
        }
    }
}