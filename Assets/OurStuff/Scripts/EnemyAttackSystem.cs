using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    [SerializeField] List<SOability> OffensiveAttacks = new List<SOability>();
    [SerializeField] List<SOability> DefensiveAttacks = new List<SOability>();
    [SerializeField] float BraveEnoughToOffensive = 70;
    AudioManager Audio;
    float TryAttack;
    Enemy enemy;

    new void Start()
    {
        base.Start();
        Attacker = transform.parent.gameObject;
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
            TryAttack = (GameManager.instance.EnemeiesTryToAttackEvery / (OffensiveAttacks.Count+ DefensiveAttacks.Count));
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
            if ((dist > Attacks[attackType].MinRange || Attacks[attackType].MinRange == 0) && dist < Attacks[attackType].MaxRange) // if In Range
            {
                if (TheChance) // if Random Worked
                AttemptToAttack(Attacks,attackType);
            }
            else
            {
                TryAttack = 0;
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