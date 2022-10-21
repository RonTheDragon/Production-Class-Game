using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    [HideInInspector] public ThirdPersonMovement PlayerAimer;

    public bool AnimationTrigger;
    public bool RecoilTrigger;
    [HideInInspector]
    public string Bullet;
    [HideInInspector]
    public float ProjectileSpeed;
    public float Gravity;
    public float ExplosionRadius;
    public float RecoilStrength;
    bool _alreadyON;
    bool _RalreadyON;
    float _recoilingtime;
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

        if (!_RalreadyON && RecoilTrigger)
        {
            _RalreadyON = true;
            Recoil();
        }
        else if (_RalreadyON && !RecoilTrigger)
        {
            _RalreadyON = false;
        }

        if (_recoilingtime > 0)
        {
            _recoilingtime -= Time.deltaTime;
            Recoiling();
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
            p.Gravity = Gravity;
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

    public void Recoil()
    {
        _recoilingtime = 1;
    }

    void Recoiling()
    {
        PlayerAimer.GetComponent<CharacterController>().Move(PlayerAimer.transform.forward * -RecoilStrength  * _recoilingtime * Time.deltaTime);
    }
}