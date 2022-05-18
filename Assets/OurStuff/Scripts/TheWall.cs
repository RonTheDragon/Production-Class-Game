using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWall : MonoBehaviour
{
    public enum WallAttacks { None, Mines}
    LayerMask Attackable;

    // Start is called before the first frame update
    void Start()
    {
        Attackable = GameManager.instance.PlayerCanAttack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WallAttack(SOwall attack)
    {
        switch (attack.wallAttack)
        {
            case WallAttacks.Mines:
                Mines(attack);
                break;

            default: break;
        }
    }

    public void Mines(SOwall attack)
    {
        int Amount = 10;
        if (attack is SOwallRanged)
        {
            SOwallRanged s = (SOwallRanged)attack;
            Amount = s.AmountOfShoots;
        }

        for (int i = 0; i < Amount; i++)
        {
            float X = Random.Range(-45, 45);
            float Y = Random.Range(5, 10);
            Shoot(attack, transform.position + transform.up * Y + transform.right*X, transform.rotation);
        }
        
    }

    public void Shoot(SOwall attack, Vector3 ShootFrom, Quaternion ShootDirection)
    {
        float ProjectileSpeed = 10;
        float ExplosionRadius = 10;
        string Bullet = "Mine";
        
        if (attack is SOwallRanged)
        {
            SOwallRanged a = (SOwallRanged)attack;
            ProjectileSpeed = a.ProjectileSpeed;
            Bullet = a.Projectile;
            if (attack is SOwallRangedExplode)
            {
                SOwallRangedExplode b = (SOwallRangedExplode)a;
                ExplosionRadius = b.ExplosionRadius;
            }
        }

        //GameObject TheBullet = Instantiate(Bullet, transform.position, transform.rotation);
        GameObject TheBullet = ObjectPooler.Instance.SpawnFromPool(Bullet, ShootFrom, ShootDirection);
        Projectile p = TheBullet.GetComponent<Projectile>();
        if (p != null)
        {
            p.Damage = attack.Damage;
            p.Knock = attack.Knockback;
            p.Stagger = attack.Stagger;
            p.Speed = ProjectileSpeed;
            p.Attackable = Attackable;
            if (p is Mine)
            {
                Mine m = (Mine)p;
                m.ExplosionRadius = ExplosionRadius;
            }
        }
    }
}
