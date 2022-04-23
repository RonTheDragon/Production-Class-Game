using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    Animator Anim;
    AudioManager Audio;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

    }
    public void Attack()
    {
        if (CanAttack())
        {
            Acooldown = AttackCooldown;
           // Audio.PlaySound(Sound.Activation.Custom, "Attack");
            Anim.SetTrigger("Attack1");
        }
    }
}