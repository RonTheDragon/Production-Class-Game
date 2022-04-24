using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    Animator Anim;
    AudioManager Audio;
    public MeleeAttack meleeAttack;
    public RangeAttack rangeAttack;
    public List<SOattack> Attacks = new List<SOattack>();
    public float TryToAttackEvery = 0.2f;
    float TryAttack;

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
    public void Attack(Vector3 Target)
    {
        if (TryAttack <= 0)
        {
            TryAttack = TryToAttackEvery;

            int N = Random.Range(0, Attacks.Count);
            bool TheChance = true;
            if (Attacks[N].Chance != 100)
            {
                float R = Random.Range(0f, 100f);
                if (R > Attacks[N].Chance) TheChance = false;
            }
            float dist = Vector3.Distance(transform.position, Target);
            if (dist > Attacks[N].MinRange && dist < Attacks[N].MaxRange && TheChance)
            {
                if (Attacks[N] is SOmeleeAttack && meleeAttack != null)
                {
                    SOmeleeAttack SOM = (SOmeleeAttack)Attacks[N];
                    meleeAttack.Damage = SOM.Damage;
                    meleeAttack.Knock = SOM.Knockback;
                    meleeAttack.AttackCooldown = SOM.DamagingCooldown;
                }
                if (Attacks[N] is SOrangedAttack && rangeAttack != null)
                {
                    SOrangedAttack SOR = (SOrangedAttack)Attacks[N];
                    rangeAttack.Damage = SOR.Damage;
                    rangeAttack.Knock = SOR.Knockback;
                    rangeAttack.Bullet = SOR.Projectile;
                    rangeAttack.ProjectileSpeed = SOR.ProjectileSpeed;
                }

                StaminaCost = Attacks[N].StaminaCost;
                if (CanAttack())
                {
                    Acooldown = Attacks[N].AttackCooldown;
                    // Audio.PlaySound(Sound.Activation.Custom, "Attack");
                    Anim.SetTrigger(Attacks[N].AnimationTrigger);
                    Attacks[N].AttackMethod.Invoke();
                }
            }
        }
        else
        {
            TryAttack -= Time.deltaTime;
        }
    }
}