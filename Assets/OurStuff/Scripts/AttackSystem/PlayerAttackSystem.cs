using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    AudioManager Audio;
    public SOclass PlayerClass;
    ThirdPersonMovement thirdPersonMovement;
    GameObject PlayerCooldownCircles;
    //public LayerMask OnlyFloor;

    new void Start()
    {
        base.Start();
        Attacker = gameObject;
        CC = GetComponent<CharacterController>();
        Audio = GetComponent<AudioManager>();
        Anim = GetComponent<ThirdPersonMovement>().animator;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetLayersForAttacks(GameManager.instance.PlayerCanAttack);
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        PlayerCooldownCircles = thirdPersonMovement.PlayerCooldowns;
    }

    new void Update()
    {
        base.Update();
        Attacking();
    }
    public void Attack(List<SOability> Attacks, int attackType)
    {
        AttemptToAttack(Attacks, attackType);
    }
    void Attacking()
    {
        if (PlayerClass != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < PlayerClass.DownLeftClickAttacks.Count; i++)
                { Attack(PlayerClass.DownLeftClickAttacks, i); }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                for (int i = 0; i < PlayerClass.DownRightClickAttacks.Count; i++)
                { Attack(PlayerClass.DownRightClickAttacks, i); }
            }
            else if (Input.GetMouseButton(0))
            {
                for (int i = 0; i < PlayerClass.LeftClickAttacks.Count; i++)
                { Attack(PlayerClass.LeftClickAttacks, i); }
            }
            else if (Input.GetMouseButton(1))
            {
                for (int i = 0; i < PlayerClass.RightClickAttacks.Count; i++)
                { Attack(PlayerClass.RightClickAttacks, i); }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                for (int i = 0; i < PlayerClass.UpLeftClickAttacks.Count; i++)
                { Attack(PlayerClass.UpLeftClickAttacks, i); }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                for (int i = 0; i < PlayerClass.UpRightClickAttacks.Count; i++)
                { Attack(PlayerClass.UpRightClickAttacks, i); }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < PlayerClass.SpaceAbility.Count; i++)
                { Attack(PlayerClass.SpaceAbility, i); }
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                for (int i = 0; i < PlayerClass.F_Ability.Count; i++)
                { Attack(PlayerClass.F_Ability, i); }
            }
        }
        else Debug.Log("Player Missing a Class for Combat");
    }

    public override void AttackMovement()
    {
        AttackMovementForce -= AttackMovementForce * Time.deltaTime;   
        Vector3 Knock = (transform.forward * AttackMovementDirection.x + transform.right * AttackMovementDirection.y).normalized * AttackMovementForce * Time.deltaTime;
        Knock.y = 0;
        if (CC.enabled)
        CC.Move(Knock);
    }

    public override void Aiming(bool aim)
    {
        thirdPersonMovement.aim = aim;
    }

    protected override void DrainCooldowns()
    {
        for (int i = 0; i < abilityCoolDowns.Count; i++)
        {
            abilityCoolDowns[i].Cooldown -= Time.deltaTime;
            PlayerCooldownCircles.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = -(abilityCoolDowns[i].Cooldown / abilityCoolDowns[i].MaxCooldown) + 1;
            if (abilityCoolDowns[i].Cooldown <= 0) { abilityCoolDowns.Remove(abilityCoolDowns[i]); Destroy(PlayerCooldownCircles.transform.GetChild(i).gameObject); break; }

        }
    }

    protected override void AddCooldown(SOability a)
    {
        abilityCoolDowns.Add(new AbilityCoolDown(a.Name, a.AbilityCooldown));
        GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, PlayerCooldownCircles.transform.rotation, PlayerCooldownCircles.transform);
        if (a.Image != null)
        {
            foreach (Transform c in Circle.transform)
            {
                c.GetComponent<Image>().sprite = a.Image;
            }
        }
        Circle.name = a.Name;
    }

    public float ShowCharge()
    {
        float n;
        n = (Charge - MinCharge - 1) / (MaxCharge);
        return n;
    }
}