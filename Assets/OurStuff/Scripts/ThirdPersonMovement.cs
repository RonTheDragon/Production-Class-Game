using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    CharacterController controller;
    PlayerAttackSystem PAS;
    [SerializeField] Transform cam;

    [SerializeField] float speed = 6f;
    float originalSpeed;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] float gravity = 9.8f;

    [SerializeField] List<int> DownLeftClickAttacks;
    [SerializeField] List<int> DownRightClickAttacks;
    [SerializeField] List<int> UpLeftClickAttacks;
    [SerializeField] List<int> UpRightClickAttacks;
    

    //all the GetComponent's and speed
    private void Awake()
    {
        PAS = GetComponent<PlayerAttackSystem>();
        controller = GetComponent<CharacterController>();
        originalSpeed = speed;
    }

    void Update()
    {
        Attack();
        CameraController();
        Gravity();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach(int i in DownLeftClickAttacks)
            { PAS.Attack(i); }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            foreach (int i in DownRightClickAttacks)
            { PAS.Attack(i); }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            foreach (int i in UpLeftClickAttacks)
            { PAS.Attack(i); }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            foreach (int i in UpRightClickAttacks)
            { PAS.Attack(i); }
        }

        if (PAS.Acooldown > 0)
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
