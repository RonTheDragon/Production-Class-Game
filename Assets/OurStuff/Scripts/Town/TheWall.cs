using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TheWall : MonoBehaviour
{
    public enum WallAttacks { None, Mines , Particle , Healing , HumanAlly}
    public SOwall[] TheWallAttacks = new SOwall[4];
    public float WallLength = 90;
    public float Wallheight = 10;
    public float WallThickness = 0;
    public bool WallFacingZ;
    [HideInInspector] public float DamageMultiplier = 1;
    [HideInInspector] public float HealingMultiplier = 1;
    [HideInInspector] public float CooldownMultiplier = 1;
    LayerMask Attackable;
    List<AbilityCoolDown> abilityCoolDowns = new List<AbilityCoolDown>();
    GameObject WallCooldowns;
    GameObject UsedParticle;

    // Start is called before the first frame update
    void Start()
    {
        Attackable = GameManager.instance.PlayerCanAttack;
        //SetUpAllCooldowns(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (TheWallAttacks[0]!=null)
            AttemptToWallAttack(TheWallAttacks[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (TheWallAttacks[1] != null)
                AttemptToWallAttack(TheWallAttacks[1]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (TheWallAttacks[2] != null)
                AttemptToWallAttack(TheWallAttacks[2]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (TheWallAttacks[3] != null)
                AttemptToWallAttack(TheWallAttacks[3]);
        }

        if (GameManager.instance.Player != null)
        {
                if (WallCooldowns == null)
                {
                    WallCooldowns = GameManager.instance.Player.GetComponent<ThirdPersonMovement>().WallCooldowns;
                }
   
                if (abilityCoolDowns.Count == WallCooldowns.transform.childCount)
                {

                    for (int i = 0; i < abilityCoolDowns.Count; i++)
                    {
                            if (abilityCoolDowns[i].Cooldown > 0)
                            {
                                abilityCoolDowns[i].Cooldown -= Time.deltaTime;
                                WallCooldowns.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = -(abilityCoolDowns[i].Cooldown / (abilityCoolDowns[i].MaxCooldown* CooldownMultiplier)) + 1;
                            }
                            else WallCooldowns.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = 1;
                    }
                }
            
        }
        else
        {
            for (int i = 0; i < abilityCoolDowns.Count; i++)
            {
                if (abilityCoolDowns[i].Cooldown > 0)
                {
                    abilityCoolDowns[i].Cooldown -= Time.deltaTime;               
                }
            }
        }
       
    }

    public void WallAttack(SOwall attack)
    {
        switch (attack.wallAttack)
        {
            case WallAttacks.Mines:
                Mines(attack);
                break;
            case WallAttacks.Particle:
                ParticleRain(attack);
                break;
            case WallAttacks.Healing:
                Heal(attack);
                break;
            case WallAttacks.HumanAlly:
                HumanAlly(attack);
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

        float X = (WallLength * -0.5f) + Random.Range(WallLength / Amount / 2, WallLength / Amount);
        for (int i = 0; i < Amount; i++)
        {
            X += Random.Range(WallLength / Amount / 2, WallLength/Amount);
            float Y = Random.Range(Wallheight, Wallheight+5);
            Shoot(attack, transform.position + (transform.forward* WallThickness) + transform.up * Y + transform.right*X, transform.rotation);
        }
        
    }

    public void ParticleRain(SOwall attack)
    {
        if (attack is SOwallParticle)
        {
            SOwallParticle AP = (SOwallParticle)attack;

            if (UsedParticle != null)
            {
                Destroy(UsedParticle);
                UsedParticle = null;
            }
            UsedParticle = Instantiate(AP.particleSystem.gameObject, transform.position+AP.Position, transform.rotation * Quaternion.Euler(AP.Rotation), transform);
            ParticleCollision pc = UsedParticle.GetComponent<ParticleCollision>();
            pc.w = this;
            pc.Damage = AP.Damage * DamageMultiplier;
            pc.Knock = AP.Knockback;
            pc.Stagger = AP.Stagger;
            pc.Temperature = AP.Temperature;
        }
    }

    public void Heal(SOwall attack)
    {
        if (attack is SOwallHeal)
        {
            SOwallHeal H = (SOwallHeal)attack;
            WallHealth hp = GetComponent<WallHealth>();
            hp.Hp += hp.MaxHp * 0.01f * (H.Heal*HealingMultiplier);
        }
    }

    public void HumanAlly(SOwall attack)
    {
        int countAllies = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("NeedsClean");
        foreach(GameObject p in players)
        {
            Ally a = p.GetComponent<Ally>();
            if (a != null) countAllies++; 
        }

        int n = 1;
        if (attack is SOwallSpawn)
        {
            SOwallSpawn s = (SOwallSpawn)attack;
            n = s.HowMany;
        }

        for (int i = 0; i < n; i++)
        {
          if (countAllies < 20)
          {
            SummonHumanAlly();
          }
        }
    }

    void SummonHumanAlly()
    {
        float X = Random.Range(-WallLength / 2, WallLength / 2);
        Vector3 SpawnPoint = transform.position + (transform.forward * WallThickness) + transform.up * -5 + transform.right * X;
        PlayerRespawnManager PRM = GameManager.instance.GetComponent<PlayerRespawnManager>();
        int r = Random.Range(0, PRM.PlayerBodies.Count);
        GameObject Ally = Instantiate(PRM.PlayerBodies[r].Body, SpawnPoint, PRM.PlayerRespawnLocation.rotation);
        ThirdPersonMovement TPM = Ally.GetComponent<ThirdPersonMovement>();
        TPM.enabled = false;
        Destroy(Ally.GetComponent<PlayerHealth>());
        Ally.GetComponent<CapsuleCollider>().enabled = true;
        Ally.GetComponent<NavMeshAgent>().enabled = true;
        Ally.GetComponent<CharacterAI>().enabled = true;
        Ally.tag = "NeedsClean";
        AllyHealth hp = Ally.GetComponent<AllyHealth>();
        PlayerAttackSystem PAS = Ally.GetComponent<PlayerAttackSystem>();
        AIAttackSystem AAS = Ally.GetComponent<AIAttackSystem>();
        Ally ally = Ally.GetComponent<Ally>();
        ally.anim = TPM.animator;
        ally.CharacterAnimationBody = TPM.animator.transform;
        ally.enabled = true;
        PAS.enabled = false;
        AAS.AbilityObjects = PAS.AbilityObjects;
        hp.MaxHp = PRM.PlayerBodies[r].health.x;
        AAS.DamageMultiplier = PRM.PlayerBodies[r].damageMultiplier.x * DamageMultiplier;
        TransferAttacks(AAS.OffensiveAttacks, PRM.PlayerBodies[r].role);
        hp.enabled = true;
        AAS.enabled = true;
        Ally.transform.GetChild(1).gameObject.SetActive(false);
        Ally.transform.GetChild(2).gameObject.SetActive(false);
        Ally.transform.GetChild(3).gameObject.SetActive(false);
        Ally.GetComponent<CharacterController>().enabled = false;
    }

    void TransferAttacks(List<SOability> attacklist, SOclass role )
    {
        foreach(SOability s in role.DownLeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.UpLeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.DownRightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.UpRightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.LeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.RightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.SpaceAbility)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.F_Ability)
        {
            attacklist.Add(s);
        }
    }

    public void AttemptToWallAttack(SOwall attack)
    {
        if (attack.AbilityCooldown != 0)
        {
            if (abilityCoolDowns.Count != 0)
            {
                if (CheckAbilityCooldown(attack.Name)) return;
            }  
        }
        StartCooldown(attack);
        WallAttack(attack);
    }

    void StartCooldown(SOwall attack)
    {
        foreach (AbilityCoolDown c in abilityCoolDowns)
        {
            if (c.AbilityName == attack.Name)
            {
                c.Cooldown = (c.MaxCooldown* CooldownMultiplier);
            }
        }
    }

    public void SetUpAllCooldowns(bool Reset)
    {
        if (WallCooldowns == null)
        {
            WallCooldowns = GameManager.instance.Player.GetComponent<ThirdPersonMovement>().WallCooldowns;
        }
        if (Reset)
        {
            abilityCoolDowns = new List<AbilityCoolDown>();
            foreach (Transform t in WallCooldowns.transform)
            {
                Destroy(t.gameObject);
            }
        }

        int count = 0;
        foreach (SOwall attack in TheWallAttacks)
        {
            if (attack != null)
            {
                if (Reset)
                {
                    AbilityCoolDown ac = new AbilityCoolDown(attack.Name, attack.AbilityCooldown);
                    ac.Cooldown = 0;
                    abilityCoolDowns.Add(ac);
                }
                GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, WallCooldowns.transform.rotation, WallCooldowns.transform);
                if (attack.Image != null)
                {
                    foreach (Transform c in Circle.transform)
                    {
                        c.GetComponent<Image>().sprite = attack.Image;
                    }
                }
                Circle.name = attack.Name;
            }
            else
            {
                if (Reset)
                {
                    abilityCoolDowns.Add(new AbilityCoolDown($"Empty Slot {count}", 0));
                }
                GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, WallCooldowns.transform.rotation, WallCooldowns.transform);
                Circle.name = $"Empty Slot {count}";
            }
            count++;
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
            if (attack is SOwallAttack)
            {
                SOwallAttack a = (SOwallAttack)attack;
                p.Damage = a.Damage * DamageMultiplier;
                p.Knock = a.Knockback;
                p.Stagger = a.Stagger;
                p.Temperature = a.Temperature;
            }
            p.Speed = ProjectileSpeed;
            p.Attackable = Attackable;
            p.Attacker = gameObject;
            if (p is Mine)
            {
                Mine m = (Mine)p;
                m.ExplosionRadius = ExplosionRadius;
            }
        }
    }
    bool CheckAbilityCooldown(string Name)
    {
        foreach (AbilityCoolDown a in abilityCoolDowns)
        {
            if (a.AbilityName == Name)
            {
                if (a.Cooldown > 0)
                {
                    Debug.Log($"{Name} is in Cooldown ({(int)a.Cooldown}s)");
                    return true;
                }
            }
        }
        return false;
    }
}
