using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    AudioManager Audio;
    public SOclass PlayerClass;
    ThirdPersonMovement thirdPersonMovement;
    //public LayerMask OnlyFloor;

    new void Start()
    {
        base.Start();
        Attacker = gameObject;
        CC = GetComponent<CharacterController>();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetLayersForAttacks(GameManager.instance.PlayerCanAttack);
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
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
        CC.Move(Knock);
    }

    public override void Aiming()
    {
        thirdPersonMovement.aim = true;
    }

    public override void StopAiming()
    {
        thirdPersonMovement.aim = false;
    }
}