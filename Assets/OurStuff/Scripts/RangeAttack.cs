using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    public bool AnimationTrigger;
    [HideInInspector]
    public string Bullet;
    [HideInInspector]
    public float ProjectileSpeed;
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
        //GameObject TheBullet = Instantiate(Bullet, transform.position, transform.rotation);
        GameObject TheBullet = ObjectPooler.Instance.SpawnFromPool(Bullet, transform.position, transform.rotation);
        Projectile p = TheBullet.GetComponent<Projectile>();
        if (p != null)
        {
            p.Damage = Damage*Charge;
            p.Knock = Knock*Charge;
            p.Attackable = Attackable;
            p.Speed = ProjectileSpeed*Charge;
        }
    }
}