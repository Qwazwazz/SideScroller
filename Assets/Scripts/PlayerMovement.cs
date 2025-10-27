using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    enum PlayerStates {Idle, Look, Walking}

    public Animator animator;

    PlayerStates state;
    bool stateComplete;

    public float acceleration;
    public float groundSpeed;
    public float jumpSpeed;


    [Range(0f, 1f)]
    public float groundDecay;
    public bool grounded;
    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    float xInput;
    float yInput;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        HandleJump();

        if (stateComplete)
        {
            SelectState();
        }
        UpdateState();
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyFriction();
        MoveWithInput();
    }

    void SelectState()
    {
        stateComplete = false;

        if (grounded)
        {
            if (xInput == 0)
            {
                state = PlayerStates.Idle;
                StartIdle();
            }
            else
            {
                state = PlayerStates.Walking;
                StartWalking();
            }
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case PlayerStates.Idle:
                UpdateIdle();
                break;
            case PlayerStates.Look:
                UpdateLook();
                break;
            case PlayerStates.Walking:
                UpdateWalking();
                break;
        }
    }
    
    void StartIdle()
    {
        animator.Play("Idle");
    }

    void StartLook()
    {
        animator.Play("Head Tilt Up");
    }

    void StartWalking() {
        animator.Play("Walk");
    }

    void UpdateIdle()
    {
        if (!grounded || xInput != 0)
            stateComplete = true;
    }

    void UpdateLook()
    {
        if (!grounded || xInput != 0)
            stateComplete = true;
    }

    void UpdateWalking()
    {
        if (!grounded || xInput == 0)
            stateComplete = true;
    }

    void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void MoveWithInput()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.linearVelocityX + increment, -groundSpeed, groundSpeed);
            body.linearVelocity = new Vector2(newSpeed, body.linearVelocityY);

            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            body.linearVelocity = new Vector2(body.linearVelocityX, jumpSpeed);
        }
    }

    void CheckGround()
    {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction()
    {
        if (grounded && xInput == 0 && body.linearVelocityY <=0)
        {
            body.linearVelocity *= groundDecay;
        }
    }
}
