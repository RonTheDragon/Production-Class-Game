using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    [HideInInspector] public ThirdPersonMovement PlayerAimer;

    public bool AnimationTrigger;
    [HideInInspector]
    public string Bullet;
    [HideInInspector]
    public float ProjectileSpeed;
    public float ExplosionRadius;
    bool _alreadyON;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_alreadyON && AnimationTrigger)
        {
            _alreadyON = true;
            Shoot();
        }
        else if (_alreadyON && !AnimationTrigger)
        {
            _alreadyON = false;
        }
    }

    public void Shoot()
    {
        if (PlayerAimer != null)
        {
            PlayerAimer.RayCastToTarget();
        }
        //GameObject TheBullet = Instantiate(Bullet, transform.position, transform.rotation);
        GameObject TheBullet = ObjectPooler.Instance.SpawnFromPool(Bullet, transform.position, transform.rotation);
        Projectile p = TheBullet.GetComponent<Projectile>();
        if (p != null)
        {
            p.Damage = Damage * Charge;
            p.Knock = Knock * Charge;
            p.Stagger = Stagger;
            p.Attackable = Attackable;
            p.Speed = ProjectileSpeed * Charge;
            p.Attacker = Attacker;
            p.Temperature = Temperature;
            if (p is ExplosiveProjectile)
            {
                ExplosiveProjectile E;
                E = (ExplosiveProjectile)p;
                E.ExplosionRadius = ExplosionRadius;
            }
            if (p is Multishoot)
            {
                Multishoot E;
                E = (Multishoot)p;
                E.ExplosionRadius = ExplosionRadius;
            }
        }
        else 
        {
            InstantExplosion X = TheBullet.GetComponent<InstantExplosion>();
            if (X != null)
            {
                X.Damage = Damage * Charge;
                X.Knock = Knock * Charge;
                X.Stagger = Stagger;
                X.Attackable = Attackable;
                X.Attacker = Attacker;
                X.Temperature = Temperature;
                X.ExplosionRadius = ExplosionRadius;
            } 
        }
        AudioManager m = TheBullet.GetComponent<AudioManager>();
        if (m != null)
        {
            m.CustomStart();
            m.PlaySound(Sound.Activation.Custom, "spawn");
        }
    }
}