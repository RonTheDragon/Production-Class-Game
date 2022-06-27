using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThirdPersonMovement : MonoBehaviour
{
    Vector3 previousPos;
    Transform player;
    public Animator animator;

    public bool aim = true;

    CharacterController        controller;
    PlayerAttackSystem         PAS;
    PlayerHealth               Hp;
    WallHealth                 WallHp;
    public Transform cam;

    [SerializeField] float speed         = 6f;
    [SerializeField] float originalSpeed = 6f;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] float gravity = 9.8f;
    public GameObject PressE;
    [SerializeField] Image HpBar;
    [SerializeField] Image StaminaBar;
    [SerializeField] Image WallHpBar;
    [SerializeField] TMP_Text SoulAmount;
    public GameObject PlayerCooldowns;
    public GameObject WallCooldowns;

    [SerializeField] GameObject Gun;


    //all the GetComponent's and speed
    private void Awake()
    {
        player = transform;
        PAS = GetComponent<PlayerAttackSystem>();
        controller = GetComponent<CharacterController>();
        if (GameManager.instance.Wall != null)
        {
            WallHp = GameManager.instance.Wall.GetComponent<WallHealth>();
        }
        Hp = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        Attack();
        CameraController();
        Gravity();
        WalkingAnimation();
        UpdateUI();
    }

    private void WalkingAnimation()
    {
        if (previousPos == player.transform.position)
        {
            animator.SetInteger("Walk", 0);
        }
        else
        {
            animator.SetInteger("Walk", 1);
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
        else
        {
            controller.Move(Vector3.down * 0.001f * Time.deltaTime);
        }
    }

    private void CameraController()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f || aim)
        {
            float targetAngleAim = Mathf.Atan2(0f, 1) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            Vector3 moveDir;

            if (aim)
            {
                if (!Hp.isDead())
                    transform.rotation = Quaternion.Euler(0f, targetAngleAim, 0f);
                moveDir = transform.forward * vertical + transform.right * horizontal;

                Vector3 target = new Vector3();
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, GameManager.instance.enemiesCanSee, QueryTriggerInteraction.Ignore))
                {                  
                        target = hit.point;
                }
                else
                {
                    target = cam.transform.forward * 9999;
                }

                Gun.transform.LookAt(target);
            }
            else
            {
                if (!Hp.isDead())
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }

            if (!Hp.isDead())
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void UpdateUI()
    {
        if (!Hp.isDead())
        {
            HpBar.fillAmount = Hp.Hp / Hp.MaxHp;
            StaminaBar.fillAmount = PAS.Stamina / PAS.MaxStamina;
        }
        else
        {
            HpBar.fillAmount = 0;
            StaminaBar.fillAmount = 0;
        }
        WallHpBar.fillAmount = WallHp.Hp / WallHp.MaxHp;
        SoulAmount.text = $"Souls: {GameManager.instance.SoulEnergy}";
    }
}
