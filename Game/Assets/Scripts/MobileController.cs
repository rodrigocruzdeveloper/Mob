using System.Linq.Expressions;
using UnityEngine;

public class MobileController : MonoBehaviour
{
    [SerializeField] private Vector2[] lanes;
    [SerializeField] private int currentLane = 1;
    [SerializeField] private float minSwipeDistance = 10.0f;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    private Vector2 touchStart;
    private bool rolling;
    private float distance;

    private Animator animator;
    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();  
    }


    private void Update()
    {
        Move();
        AnimationManager();
        if (Input.GetButtonDown("Fire1"))
        {
            touchStart = Input.mousePosition;
        }
        
        if(Input.GetButtonUp("Fire1"))
        {
            Vector2 touchEnd = Input.mousePosition;
            Vector2 swipe = touchEnd - touchStart;

            // VERIFICA SE FOI UM SWIPE
            if (swipe.magnitude >= minSwipeDistance)
            {
                if (swipe.x > 0)
                {
                    SwipeRight();
                }
                else
                {
                    SwipeLeft();
                }
            }
            else
            {
                Jump();
            }
        }      
    }


    private void Move()
    {
        Vector2 targetPosition = new Vector2(lanes[currentLane].x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (rolling)
        {
            distance = Vector2.Distance(transform.position, targetPosition);

            if(distance < 0.01f)
            {
                rolling = false;
            }
        }
    }

    private void AnimationManager()
    {
        animator.SetBool("pRoll", rolling);
    }


    private void SwipeLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
        }

        transform.eulerAngles = new Vector2(0, 180);

        rolling = true;
    }

    private void SwipeRight()
    {
        if (currentLane < lanes.Length - 1)
        {
            currentLane++;
        }

        transform.eulerAngles = new Vector2(0, 0);

        rolling = true;
    }


    bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }



    private void Jump()
    {
        if(GroundCheck())
        {
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }


}
