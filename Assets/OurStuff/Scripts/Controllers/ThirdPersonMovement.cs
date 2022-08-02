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
    public float originalSpeed = 6f;
    public float SprintSpeed = 12f;
    [SerializeField] float SprintDrain = 15f;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] float gravity = 9.8f;
    public GameObject PressE;
    [SerializeField] Image HpBar;
    [SerializeField] Image StaminaBar;
    [SerializeField] Image ChargeBar;
    [SerializeField] Image WallHpBar;
    [SerializeField] TMP_Text SoulAmount;
    [SerializeField] GameObject ExitGameMenu;
    public GameObject PlayerCooldowns;
    public GameObject WallCooldowns;

    [SerializeField] GameObject Gun;
    bool Sprint;


    //all the GetComponent's and speed
    private void Awake()
    {
        player = transform;
        PAS = GetComponent<PlayerAttackSystem>();
        controller = GetComponent<CharacterController>();
        try
        {
            if (GameManager.instance.Wall != null)
            {
                WallHp = GameManager.instance.Wall.GetComponent<WallHealth>();
            }
        }
        catch (System.Exception) { }
        Hp = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!Hp.Frozen)
        {
            Attack();
        }
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
        else if (Sprint)
        {
            animator.SetInteger("Walk", 2);
            previousPos = player.transform.position;
            PAS.Stamina -= Time.deltaTime * SprintDrain;
        }
        else
        {
            animator.SetInteger("Walk", 1);
            previousPos = player.transform.position;
        }
        if (Hp.Frozen)
        {
            animator.SetBool("Frozen", true);
        }
        else
        {
            animator.SetBool("Frozen", false);
        }

    }

    private void Attack()
    {
        if (PAS.AttackCooldown > 0)
        {
            speed = 0;
        }
        else if (Sprint)
        {
            if (PAS.Stamina > (PAS.MaxStamina / 2))
            {
                speed = SprintSpeed;
            }
            else
            {
                speed = Mathf.Lerp(originalSpeed, SprintSpeed, PAS.Stamina / (PAS.MaxStamina / 2));
            }
        }
        else
        {
            speed = originalSpeed;
        }
    }

    private void Gravity()
    {
        if (controller.enabled)
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
    }

    private void CameraController()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Sprint = false;
        }
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f || aim)
        {
            float targetAngleAim = Mathf.Atan2(0f, 1) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            Vector3 moveDir;

            if (aim)
            {
                if (!Hp.isDead() && !Hp.Frozen)
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
                if (!Hp.isDead() && !Hp.Frozen)              
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                
            }

            if (!Hp.isDead())
            controller.Move(moveDir.normalized * GetSpeed() * Time.deltaTime);
        }
    }

    void UpdateUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGameMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        GameObject C = ChargeBar.gameObject;
        if (!Hp.isDead())
        {
            HpBar.fillAmount = Hp.Hp / Hp.MaxHp;
            StaminaBar.fillAmount = PAS.Stamina / PAS.MaxStamina;
            float n = PAS.ShowCharge();
            if (n > 0)
            {
                if (!C.activeSelf)
                C.SetActive(true);
                ChargeBar.fillAmount = n;
            }
            else if (C.activeSelf) C.SetActive(false);

        }
        else
        {
            HpBar.fillAmount = 0;
            StaminaBar.fillAmount = 0;
            C.SetActive(false);
        }
        WallHpBar.fillAmount = WallHp.Hp / WallHp.MaxHp;
        SoulAmount.text = $"Souls: {GameManager.instance.SoulEnergy}";
    }
    float GetSpeed()
    {
        float Speed = speed;
        if (Hp.Temperature < 0) { Speed *= 0.01f * (Hp.Temperature + 100); }
        if (Hp.Frozen) { Speed = 0; }
        return Speed;
    }
}
