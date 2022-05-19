using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    [SerializeField] List<SOability> OffensiveAttacks = new List<SOability>();
    [SerializeField] List<SOability> DefensiveAttacks = new List<SOability>();
    [SerializeField] float BraveEnoughToOffensive = 70;
    AudioManager Audio;
    [SerializeField] float TryToAttackEvery = 0.2f;
    float TryAttack;
    Enemy enemy;

    new void Start()
    {
        base.Start();
        enemy = transform.parent.GetComponent<Enemy>();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetComponent<Animator>();
        SetLayersForAttacks(GameManager.instance.enemiesCanAttack);
    }

    new void Update()
    {
        base.Update();
    }
    public void Attack(Vector3 Target,bool UseDefensives)
    {
        if (TryAttack <= 0)
        {
            TryAttack = TryToAttackEvery;
            List<SOability> Attacks;
            if (UseDefensives && DefensiveAttacks.Count > 0)
            {             
                    int OffensiveChance = 1;
                    if (enemy.CheckBravery(BraveEnoughToOffensive))
                    {
                        OffensiveChance+=2;
                    }
                    int R = Random.Range(0, 4);
                    if (R > OffensiveChance)
                    {
                        Attacks = DefensiveAttacks;
                    }
                    else
                    {
                        Attacks = OffensiveAttacks;
                    }               
            }
            else
            {
                Attacks = OffensiveAttacks;
            }
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
                AttemptToAttack(Attacks,attackType);
            }
        }
        else
        {
            TryAttack -= Time.deltaTime;
        }
    }

    public override void AttackMovement()
    {
        AttackMovementForce -= AttackMovementForce * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, transform.position+(transform.forward*AttackMovementDirection.x+transform.right*AttackMovementDirection.y).normalized, AttackMovementForce * Time.deltaTime);
    }
}