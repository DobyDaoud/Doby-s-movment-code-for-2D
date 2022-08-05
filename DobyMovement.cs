using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobyMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Variables")]

    [Header("Movement Variables")]
    [SerializeField] private float _movementAcceleration;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _linearDrag;

    [Header("Movement")]
    [SerializeField] public float moveSpeed;
    [SerializeField] private float moveInput;
    [SerializeField] public float velPower;
    [SerializeField] public float frictionAmount;
    [SerializeField] private float gravityScale;
    [SerializeField] public float fallGravityMultiplier;
    [SerializeField] public float acceleration;
    [SerializeField] public float decceleration;
    [SerializeField] private float lastGroundedTime;
    private float lastJumpTime;
    [SerializeField] private float MoveInput;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool jumpInputReleased;
    [SerializeField] public float fallMultiplier;
    [SerializeField] public float lowJumpMultiplier;
    public float jumpForce;


    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    public float jumpBufferTime;
    public float jumpCoyoteTime;
    public float jumpCutMultiplier;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravityScale = rb.gravityScale;
    }
    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        #region
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            //if so sets the lastGrounded to coyoteTime
            lastGroundedTime = jumpCoyoteTime;
        }
        #endregion


        #region Timer
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        #endregion

        #region Jump 
        // checks if was last grounded within coyoteTime and that jump has been pressed within bufferTime
        if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping)
        {
            jump();
        }
        #endregion

        #region Jump Gravity
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        #endregion

        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        #region Run
        //caclculate the direction we want to move in our desired velcoity
        float TargetSpeed = moveInput * moveSpeed;
        //caclculate difference between current velocity and desired velocity
        float speedDif = TargetSpeed - rb.velocity.x;
        //change acceleration rate depending on situation
        float accelRate = (Mathf.Abs(TargetSpeed) > 0.01f) ? acceleration : decceleration;
        //applies acceleration to speed difference, the riase to set a power so acceleration inceasre with higher speeds
        //finally multiplies by sign to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        //applies multiplies by sign to reapply direction
        rb.AddForce(movement * Vector2.right);
        #endregion

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        #region Friction
        //check if we're grounded and that we are trying to stop (not pressing fowards or backwards)
        if (lastGroundedTime > 0 && Mathf.Abs(MoveInput) < 0.01f)
        {
            //then we use either the friction amount (~ 0.2) or our velocity
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            //sets to movement direction 
            amount *= Mathf.Sign(rb.velocity.x);
            //applies force against movement direction 
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            #endregion
        }
    }
    private void jump()
    {
        //apply force, using impluse force mode
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastGroundedTime = 4;
        lastJumpTime = 4;
        isJumping = true;
        jumpInputReleased = false;
    }
    public void OnJump()
    {
        lastJumpTime = jumpBufferTime;
    }

    public void OnJumpUp()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            //reduces current y velocity by amount (0 - 1) 
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        jumpInputReleased = true;
        lastJumpTime = 0;
    }
}
