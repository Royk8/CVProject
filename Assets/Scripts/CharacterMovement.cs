using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private State state, lastState;
    private Rigidbody rb;
    private float jumpStart, superJumpStart;
    private float timeStuckFalling = 2f, startedFalling;
    private Vector3 standardGravity, augmentedGravity;
    [SerializeField] private Animator anim;
    [SerializeField] private float speed, x, y;
    [SerializeField] private float jumpForce, superJumpForce;
    [SerializeField] private float jumpCooldown, superJumpPreparation;
    [SerializeField] private Transform feetRaycaster, headRaycaster, floorRaycaster, backRaycaster;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private int superJumps;
    [SerializeField] private float gravityMultiplier;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpStart = 0;
        standardGravity = Physics.gravity;
        augmentedGravity = Physics.gravity * gravityMultiplier;
    }

    /**
     * Maquina de estados que controla el personaje
     * */
    void Update()
    {
        switch (state)
        {
            case State.walking:
                Walk();
                break;
            case State.jumping:
                Jump();
                break;
            case State.superJumping:
                SuperJump();
                break;
            case State.elevating:
                Elevating();
                break;
            case State.goingBack:
                GoingBack();
                break;
            case State.falling:
                Falling();
                break;
            case State.frontJumping:
                FrontJump();
                break;
            default:
                break;
        }
        GravityControl();
        Debug.DrawRay(floorRaycaster.position, new Vector3(x, y, 0) * 10f, Color.green);
    }

    private void CharacterMove(Vector3 movement)
    {
        rb.velocity = movement;
    }

    private void CharacterJump(Vector3 jumpDirection)
    {
        rb.velocity = jumpDirection;
    }

    /**
     * Character Walks en checks for jumps.
     * */
    public void Walk()
    {        
        Vector3 movement = new Vector3(speed, rb.velocity.y, 0);
        CharacterMove(movement);
        anim.SetInteger("State", 1);
        CheckObstacles();
    }

    public void Jump()
    {
        CharacterJump(new Vector3(2, jumpForce, 0));
        jumpStart = Time.time;
        ChangeState(State.elevating);
        anim.SetInteger("State", 2);
    }

    public void SuperJump()
    {
        if (Time.time > superJumpStart + superJumpPreparation)
        {
            CharacterJump(new Vector3(2, superJumpForce, 0)); ;
            superJumps--;
            jumpStart = Time.time;
            ChangeState(State.elevating);       
        }
        anim.SetInteger("State", 3);
    }

    public void FrontJump()
    {
        CharacterJump(new Vector3(speed, jumpForce, 0));
        jumpStart = Time.time;
        ChangeState(State.elevating);
        anim.SetInteger("State", 2);
    }

    private void Elevating()
    {
        if(rb.velocity.y < 0)
        {
            ChangeState(State.falling);
            Physics.gravity = augmentedGravity;
        }
        anim.SetInteger("State", 4);
    }

    private void Falling()
    {
        if(state != lastState)
        {
            startedFalling = Time.deltaTime;
        }
        if(CheckGround())
        {
            if(Physics.Raycast(feetRaycaster.position, Vector3.right, 1f, floorMask))
            {
                ChangeState(State.goingBack);
            }
            else
            {
                ChangeState(State.walking);
            }            
            Physics.gravity = standardGravity;
            return;
        }
        Vector3 movement;
        if (Physics.Raycast(feetRaycaster.position, Vector3.right, 1f, floorMask) && Time.time > startedFalling + timeStuckFalling)
        {
            movement = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            movement = new Vector3(speed, rb.velocity.y, 0);
        }
        Physics.gravity = Physics.gravity * 1.1f;
        
        CharacterMove(movement);
        anim.SetInteger("State", 5);
    }

    private void GoingBack()
    {
        if(!Physics.Raycast(feetRaycaster.position, Vector3.right, 1.5f, floorMask))
        {
            ChangeState(State.walking);
        }
        if(Physics.Raycast(backRaycaster.position, Vector3.left, 0.5f, floorMask))
        {
            ChangeState(State.walking);
        }
        Vector3 movement = new Vector3(-speed, rb.velocity.y, 0);
        CharacterMove(movement);
    }
    private void CheckObstacles()
    {
        if (!CheckGround())
        {
            //ExtDebug.DrawBoxCastBox(floorRaycaster.position, Vector3.one * 3f, Quaternion.identity, Vector3.down, 5f, Color.blue);
            ChangeState(State.falling);
            return;
        }

        Debug.DrawRay(feetRaycaster.position, new Vector3(0, -1, 0) * 3f, Color.cyan);
        Debug.DrawRay(feetRaycaster.position, new Vector3(1, -0.3f, 0) * 10f, Color.red);

        if (Time.time > jumpStart + jumpCooldown)
        {
            if(superJumps > 0)
            {
                Debug.DrawRay(headRaycaster.position, Vector3.right * 2.5f);
                if(Physics.Raycast(headRaycaster.position, Vector3.right, 2.5f, floorMask))
                {
                    ChangeState(State.superJumping);
                    superJumpStart = Time.time;
                }
            }
            Debug.DrawRay(feetRaycaster.position, Vector3.right * 2f);
            if (Physics.Raycast(feetRaycaster.position, Vector3.right, 2f, floorMask))
            {
                ChangeState(State.jumping);
            }
            else if (CheckHoleInRoad())
            {
                Debug.Log("A Whole Hole!!");
                ChangeState(State.frontJumping);
            }
        }
    }

    private bool CheckGround()
    {
        return Physics.BoxCast(floorRaycaster.position, Vector3.one * 0.1f, Vector3.down, Quaternion.identity, 0.1f, floorMask);
    }

    private bool CheckHoleInRoad()
    {
        return !Physics.Raycast(feetRaycaster.position, Vector3.down, 3f, floorMask) &&
                Physics.Raycast(feetRaycaster.position, new Vector3(1, -0.3f, 0), 9f, floorMask);
    }

    private void GravityControl()
    {
        if(jumpCooldown + jumpStart > Time.time)
        {
            Physics.gravity = standardGravity;
        }
        else
        {
            Physics.gravity = augmentedGravity;
        }        
    }

    private void ChangeState(State newState)
    {
        lastState = state;
        state = newState;
    }

    public void Accelerate()
    {
        speed += 2;
    }

    public void RecoverSuperJump()
    {
        superJumps++;
    }

}


enum State
{
    walking,
    jumping,
    superJumping,
    frontJumping,
    elevating,
    goingBack,
    falling
}