using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject playerVisual;
    public GameObject helper;

    Animator anim;
    SpriteRenderer sprite;

    
    public float moveSpeed = 1f;
    public float gravitySpeed = 0.2f;

    Vector2 moveVector;
    float playerOffset = 0.445f; //Offset from player GO center to the feet
    int ignorePlayerLayer = ~(1 << 8);

    float currentJumpVelocity = 0f;
    public float jumpVelocityStart = 0.4f;
    bool jumping = false;
    float jumpingTimer = 0f;
    float jumpDuration = 0.5f;

    //Put into Player State manager
    bool isGrounded = true;

    bool isMoving = false;
    bool isIdle = true;
    
  
    //Unity Run Animations
   
    //Falling Velocity, when player falls, he goes to full gravitySpeed
    //Whe using jump, the jump velocity is firt higher than gravity, than as it degrades, gravityValue is 0
    //and then is increasing to full gravitySpeed. That is the difference between falling and jumping
     
    //Create player state machine, states like grounded, jumping, moving left etc
    //Animator machine, controlled by state machine

    void OnEnable()
    {
        ControlsEvents.AxisEvent += GetMoveInput;
        ControlsEvents.JumpEvent += Jump;
    }

    void OnDisable()
    {
        ControlsEvents.AxisEvent -= GetMoveInput;
        ControlsEvents.JumpEvent -= Jump;
    }

    void Start()
    {
        anim = playerVisual.GetComponent<Animator>();
        sprite = playerVisual.GetComponent<SpriteRenderer>();
    }



    void Update()
    {
        Movement();
        Jumping();


        /*
        if (Mathf.Abs(moveVector.x) == 0f){
            if(!jumping && !isIdle)
            {
                isIdle = true;
                playerVisual.GetComponent<Animator>().SetTrigger("Idle");
            }
           
        }

        if (moveVector.x > 0f)
        {
            sprite.flipX = false;
            anim.SetBool("RunRight", true);
            anim.SetBool("RunLeft", false);
        }

            if (moveVector.x < 0f)
        {
            sprite.flipX = true;
            anim.SetBool("RunLeft", true);
            anim.SetBool("RunRight", false);
        }*/


        /*
        if (Mathf.Abs(moveVector.x) > 0f)
        {
            if (!isMoving)
            {
                isMoving = true;
                isIdle = false;
                anim.SetFloat("Move", moveVector.x);
            }

        }
        else
        {
            if (!isIdle)
            {
                isIdle = true;
                isMoving = false;
                playerVisual.GetComponent<Animator>().SetTrigger("Idle");
            }
        }*/
    }

    void Movement()
    {
        float distanceToGround = 0f;        
        float gravityValue = 0f;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y - playerOffset);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 10f, ignorePlayerLayer);        
        if(hit.collider != null)
        {
            distanceToGround = pos.y - hit.point.y;
            helper.transform.position = hit.point;
        }

        //If distance to ground is more than zero, then player is falling
        //But make sure that the falling value is never bigger than actual distance to ground
        //or player falls through ground        
        if (distanceToGround > 0f)
        {
            isGrounded = false;
            float deltaGravity = gravitySpeed * Time.deltaTime; //How much will the player move this frame with Translate

            if (distanceToGround < deltaGravity)
            {                
                //Move falling player only as much as he is above the ground, not more or he goes through
                //Player is 1 unit abover ground, falling speed is 3. This frame he has to fall only 1 unit
                gravityValue = distanceToGround;
                currentJumpVelocity = 0f;
                jumping = false;
            }
            else
            {
                //player is so high that he can fall at full speed
                gravityValue = deltaGravity;
            }
        }
        else
        {
            if(!isGrounded)
            {
                isGrounded = true;
                playerVisual.GetComponent<Animator>().SetTrigger("Idle");
            }
           
        }

        float finalGravity = -gravityValue + (currentJumpVelocity * Time.deltaTime);
        float finalHorizontal = moveVector.x * moveSpeed * Time.deltaTime;

        //print(distanceToGround + " " + gravityValue + " " + finalGravity + " " + currentJumpVelocity);

        Vector2 finalMovement = new Vector2(finalHorizontal, finalGravity);

        this.transform.Translate(finalMovement);
    }

    void GetMoveInput(Vector2 axisValues)
    {
        if(isGrounded)
        {
            moveVector = axisValues;
        }
        else
        {
            moveVector = axisValues;
           // moveVector *= 0.5f;
        }       
        
    }

    void Jump()
    {
        currentJumpVelocity = jumpVelocityStart;
        jumpingTimer = jumpDuration;
        jumping = true;

        playerVisual.GetComponent<Animator>().SetTrigger("Jump");
    }

    void Jumping()
    {
        if(jumping)
        {
            jumpingTimer -= Time.deltaTime;
            float index = jumpingTimer / jumpDuration;
            currentJumpVelocity = jumpVelocityStart * index;

           /* if(currentJumpVelocity < gravitySpeed)
            {
                currentJumpVelocity = 0f;
            }*/

            if (jumpingTimer <= 0f)
            {
                currentJumpVelocity = 0f;
                jumping = false;
            }
        }      
    }
}
