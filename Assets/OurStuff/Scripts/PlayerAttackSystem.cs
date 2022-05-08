using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    AudioManager Audio;
    [SerializeField] List<SOability> DownLeftClickAttacks = new List<SOability>();
    [SerializeField] List<SOability> DownRightClickAttacks = new List<SOability>();
    [SerializeField] List<SOability> UpLeftClickAttacks = new List<SOability>();
    [SerializeField] List<SOability> UpRightClickAttacks = new List<SOability>();
    //public LayerMask OnlyFloor;

    new void Start()
    {
        base.Start();
        CC = GetComponent<CharacterController>();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetComponent<Animator>();
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
        if (Input.GetMouseButtonDown(0))
        {    
            for (int i = 0; i < DownLeftClickAttacks.Count; i++)     
            { Attack(DownLeftClickAttacks,i); }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < DownRightClickAttacks.Count; i++)
            { Attack(DownRightClickAttacks, i); }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < UpLeftClickAttacks.Count; i++)
            { Attack(UpLeftClickAttacks, i); }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            for (int i = 0; i < UpRightClickAttacks.Count; i++)
            { Attack(UpRightClickAttacks, i); }
        }
    }

    public override void AttackMovement()
    {
        AttackMovementForce -= AttackMovementForce * Time.deltaTime;   
        Vector3 Knock = (transform.forward * AttackMovementDirection.x + transform.right * AttackMovementDirection.y).normalized * AttackMovementForce * Time.deltaTime;
        Knock.y = 0;
        CC.Move(Knock);
    }
}