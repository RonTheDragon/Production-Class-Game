using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    AudioManager Audio;
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

    }
    public void Attack(int attackType)
    {
        AttemptToAttack(attackType);
    }
}