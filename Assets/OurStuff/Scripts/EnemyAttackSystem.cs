using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    AudioManager Audio;
    [SerializeField] float TryToAttackEvery = 0.2f;
    float TryAttack;

    new void Start()
    {
        base.Start();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }

    new void Update()
    {
        base.Update();
    }
    public void Attack(Vector3 Target)
    {
        if (TryAttack <= 0)
        {
            TryAttack = TryToAttackEvery;

            int attackType = Random.Range(0, Attacks.Count);
            bool TheChance = true;
            if (Attacks[attackType].Chance != 100)
            {
                float R = Random.Range(0f, 100f);
                if (R > Attacks[attackType].Chance)
                    TheChance = false;
            }
            float dist = Vector3.Distance(transform.position, Target);
            if (dist > Attacks[attackType].MinRange && dist < Attacks[attackType].MaxRange && TheChance)
            {
                AttemptToAttack(attackType);
            }
        }
        else
        {
            TryAttack -= Time.deltaTime;
        }
    }

    

}