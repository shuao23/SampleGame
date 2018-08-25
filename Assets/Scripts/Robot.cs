﻿using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Robot : MonoBehaviour
{
    [SerializeField]
    private MoveStats groundStats = MoveStats.Default;
    [SerializeField]
    private MoveStats airStats = MoveStats.Default;
    [SerializeField]
    private JumpStats jumpingStats = JumpStats.Default;
    [SerializeField]
    private LayerMask groundCollision = ~0x0;
    [SerializeField]
    private Collider2D feetCollider;

    private float deltaXQueue = 0;
    private float jumpQueueTime = -1;
    private int groundCount = 0;


    public MoveStats AirStats {
        get { return airStats; }
    }

    public MoveStats GroundStats {
        get { return groundStats; }
    }

    public bool IsGrounded {
        get { return groundCount > 0; }
    }

    public JumpStats JumpingStats {
        get { return jumpingStats; }
    }

    public bool JumpQueued {
        get { return jumpQueueTime >= 0; }
    }

    public Rigidbody2D Rigidbody { get; private set; }


    private bool ShouldJump {
        get { return jumpQueueTime >= jumpingStats.JumpReadyTime; }
    }


    public void Jump()
    {
        if (!JumpQueued)
        {
            jumpQueueTime = 0;
        }
    }

    /// <summary>
    /// Move on the horizontal axis. 
    /// </summary>
    /// <param name="unitDeltaX">The horizontal velocity usually from -1 to 1</param>
    public void Move(float unitDeltaX, MoveMode mode = MoveMode.Override)
    {
        unitDeltaX *= -1; //Becacuse x axis is opposite now
        switch (mode)
        {
            case MoveMode.Additive:
                deltaXQueue += unitDeltaX;
                break;
            case MoveMode.Override:
                deltaXQueue = unitDeltaX;
                break;
            default:
                throw new InvalidOperationException();
        }
    }


    #region Unity Methods
    private void Awake()
    {
        if(feetCollider == null)
        {
            Debug.LogWarning("Feet collider not assinged. Disabling");
            enabled = false;
        }

        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (JumpQueued)
        {
            jumpQueueTime += Time.fixedDeltaTime;
        }

        if (IsGrounded)
        {
            if (ShouldJump)
            {
                ApplyJump();
            }

            ApplyMove(groundStats, Time.fixedDeltaTime);
        }
        else
        {
            if (ShouldJump)
            {
                ClearJumpQueue();
            }

            ApplyMove(airStats, Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundCollision(collision) && 
            (ReferenceEquals(collision.collider, feetCollider) ||
            ReferenceEquals(collision.otherCollider, feetCollider)))
        {
            groundCount++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundCollision(collision))
        {
            groundCount--;
        }
    }
    #endregion


    private void ApplyJump()
    {
        Rigidbody.AddForce(Vector2.up * jumpingStats.JumpVelocity, ForceMode2D.Impulse);
        ClearJumpQueue();
    }

    private void ApplyMove(MoveStats stats, float deltaTime)
    {
        float currentXVelocity = Rigidbody.velocity.x;
        float targetXVelocity = deltaXQueue * stats.MaxVelocity;
        float newVelocity = currentXVelocity;
        if (currentXVelocity < targetXVelocity)
        {
            newVelocity = Mathf.Min(currentXVelocity + stats.Acceleration * deltaTime, targetXVelocity);
        }
        else if (currentXVelocity > targetXVelocity)
        {
            newVelocity = Mathf.Max(currentXVelocity - stats.Acceleration * deltaTime, targetXVelocity);
        }
        Rigidbody.velocity = new Vector2(newVelocity, Rigidbody.velocity.y);
        deltaXQueue = 0;
    }

    private void ClearJumpQueue()
    {
        jumpQueueTime = -1;
    }

    private bool IsGroundCollision(Collision2D collision)
    {
        return (groundCollision.value & (1 << collision.gameObject.layer)) != 0;
    }

    [Serializable]
    public struct MoveStats
    {
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float maxVelocity;


        public static MoveStats Default {
            get {
                return new MoveStats()
                {
                    acceleration = 25,
                    maxVelocity = 8
                };
            }
        }

        public float Acceleration {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public float MaxVelocity {
            get { return maxVelocity; }
            set { maxVelocity = value; }
        }
    }

    [Serializable]
    public struct JumpStats
    {
        [SerializeField]
        private float jumpReadyTime;
        [SerializeField]
        private float jumpVelocity;


        public static JumpStats Default {
            get {
                return new JumpStats()
                {
                    jumpReadyTime = 0.5f,
                    jumpVelocity = 800
                };
            }
        }

        public float JumpReadyTime {
            get { return jumpReadyTime; }
            set { jumpReadyTime = value; }
        }

        public float JumpVelocity {
            get { return jumpVelocity; }
            set { jumpVelocity = value; }
        }
    }

    public class MoveState
    {
        public Facing Facing { get; set; }

        public bool IsFacingRight {
            get { return Facing == Facing.Right; }
        }

        public bool IsFacingLeft {
            get { return Facing == Facing.Left; }
        }
    }

    public enum Facing
    {
        Right, Left
    }

    public enum MoveMode
    {
        Override, Additive
    }
}