using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    float gravityStart;
    bool isAlive = true;
 
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if(!isAlive) {return; }
        Run();   
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value) {

        if(!isAlive) {return; }

        Instantiate(bullet, gun.position, transform.rotation);
        
    }

    void OnMove(InputValue value) {

        if(!isAlive) {return; }
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value) {

        if(!isAlive) {return; }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return;}

        if (value.isPressed) {

            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void Run() {

        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        
        bool playerhasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerhasHorizontalSpeed);
    }

    void FlipSprite() {

        bool playerhasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerhasHorizontalSpeed) {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder() {

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { 
            
            myRigidbody.gravityScale = gravityStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;
        bool playerhasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerhasVerticalSpeed);
        
    }

    void Die() {

        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) {

            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
