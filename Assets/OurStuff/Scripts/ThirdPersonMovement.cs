using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    Vector3 previousPos;
    Transform player;
    Animator anim;

    CharacterController        controller;
    PlayerAttackSystem         PAS;
    [SerializeField] Transform cam;

    [SerializeField] float speed = 6f;
    [SerializeField] float originalSpeed = 6f;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] float gravity = 9.8f;
    public GameObject PressE;


    //all the GetComponent's and speed
    private void Awake()
    {
        player = transform;
        PAS = GetComponent<PlayerAttackSystem>();
        controller = GetComponent<CharacterController>();
        anim = player.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
        CameraController();
        Gravity();
        WalkingAnimation();
    }

    private void WalkingAnimation()
    {
        if (previousPos == player.transform.position)
        {
            anim.SetInteger("Walk", 0);
        }
        else
        {
            anim.SetInteger("Walk", 1);
            previousPos = player.transform.position;
        }
    }

    private void Attack()
    {
        if (PAS.AttackCooldown > 0)
        {
            speed = 0;
        }
        else
        {
            speed = originalSpeed;
        }
    }

    private void Gravity()
    {
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }

    private void CameraController()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
