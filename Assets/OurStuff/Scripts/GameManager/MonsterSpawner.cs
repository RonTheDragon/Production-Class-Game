using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] List<SOlevel> Levels = new List<SOlevel>();
    List<Transform> SpawningLocations = new List<Transform>();

    float TimeTillNextWave;
    int CurrentMonsters;
    [SerializeField] float MonsterCounterCooldown = 2;
    float MonsterCounterTimer;
    [SerializeField] float TimeLeftForWave;
    [SerializeField] int TooManyToStartNextWave;
    [SerializeField] int MonstersAmount;
    [SerializeField] int CurrentWave;
    [SerializeField] int CurrentLevel;
    Vector2 TimeBetweenEnemiesSpawn;
    float TimeLeftForNextEnemy;
    SOwave Wave;
    List<MonstersLeft> MonstersLeftToSpawn = new List<MonstersLeft>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform location in transform.GetChild(0))
        {
            SpawningLocations.Add(location);
        }
        //StartWave();
    }

    void Update()
    {
        if (MonsterCounterTimer > 0)
        {
            MonsterCounterTimer -= Time.deltaTime;
        }
        else
        {
            MonsterCounterTimer = MonsterCounterCooldown;
            ScanMonster();
        }

        if (Wave != null)
        {
            DuringWave();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndLevel();
                if (Levels.Count <= CurrentLevel)
                {
                    CurrentLevel--;
                    Debug.Log("Repeat Last Level");
                }
            }
        }
    }
    void ScanMonster()
    {   
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        MonstersAmount = gos.Length;
    }

    public void StartWave()
    {
        MonstersLeftToSpawn = new List<MonstersLeft>();
        Wave = Levels[CurrentLevel].Waves[CurrentWave];       
        for (int i = 0; i < Wave.Monsters.Count; i++)
        {
            int r = Random.Range((int)Wave.Monsters[i].Amount.x, (int)Wave.Monsters[i].Amount.y+1);
            MonstersLeftToSpawn.Add(new MonstersLeft());
            MonstersLeftToSpawn[i].Amount = r;
            MonstersLeftToSpawn[i].Name = Wave.Monsters[i].Name;
        }
        if (Levels[CurrentLevel].Waves.Count <= CurrentWave + 1 || TooManyToStartNextWave < 1)
        {
            TooManyToStartNextWave = 1;
        }
        else
        {
            TooManyToStartNextWave = Random.Range((int)Wave.TooManyMonstersToStart.x, (int)Wave.TooManyMonstersToStart.y+1);
        }
        TimeLeftForWave = Random.Range((int)Wave.TimeTillNextWave.x, (int)Wave.TimeTillNextWave.y+1);
        TimeBetweenEnemiesSpawn = Wave.TimeBetweenSpawns;

        if (CurrentWave == 0)
        {
            GetComponent<PlayerRespawnManager>().SetCharacterList(Levels[CurrentLevel].PlayerCharacterAmount);
        }
    }
    void NextWave()
    {
       
        if (Levels[CurrentLevel].Waves.Count <= CurrentWave+1)
        {
            Wave = null;
            if (Levels.Count <= CurrentLevel+1)
            {               
                 Debug.Log("No More Levels");
            }
                       
                GameManager.instance.AlreadyWon = true;
                if (GameManager.instance.Player != null)
                {
                    GameManager.instance.Player.GetComponent<ThirdPersonMovement>().PressE.SetActive(true);
                }
                else
                {
                    EndLevel();
                }
            
        }
        else
        {
            CurrentWave++;
            StartWave();
        }
        
    }

    public void WaveLost()
    {
        CurrentWave = 0;
        Wave = null;
        foreach(GameObject Enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy.SetActive(false);
        }
    }

    void DuringWave()
    {
        if (MonstersLeftToSpawn.Count > 0 && TimeLeftForNextEnemy<=0)
        {
            int r = Random.Range(0, MonstersLeftToSpawn.Count);
            int l = Random.Range(0, SpawningLocations.Count);
            if (MonstersLeftToSpawn[r].Amount > 0)
            ObjectPooler.Instance.SpawnFromPool(MonstersLeftToSpawn[r].Name, SpawningLocations[l].position, SpawningLocations[l].rotation);
            MonstersLeftToSpawn[r].Amount--;
            if (MonstersLeftToSpawn[r].Amount <= 0) { MonstersLeftToSpawn.RemoveAt(r); }
            TimeLeftForNextEnemy = Random.Range(TimeBetweenEnemiesSpawn.x, TimeBetweenEnemiesSpawn.y+1);
        }
        if (MonstersLeftToSpawn.Count <= 0 && TooManyToStartNextWave>MonstersAmount && TimeLeftForWave<=0)
        {
            NextWave();
        }
        if (TimeLeftForWave > 0) { TimeLeftForWave -= Time.deltaTime; }
        if (TimeLeftForNextEnemy > 0) { TimeLeftForNextEnemy -= Time.deltaTime; }
    }
    class MonstersLeft
    {
        public string Name;
        public int Amount;
    }
    public void EndLevel()
    {
        GameObject Player = GameManager.instance.Player;
        if (Player != null)
        {
            CurrentWave = 0;
            CurrentLevel++;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Player.GetComponent<ThirdPersonMovement>().PressE.SetActive(false);
            Player.SetActive(false);

            TownManager TM = GameManager.instance.GetComponent<TownManager>();
            TM.TownCamera.gameObject.SetActive(true);
            TM.UpdateSoulCount();
            GameManager.instance.CantUseTown = false;

            WallHealth WH = GameManager.instance.Wall.GetComponent<WallHealth>();
            WH.Hp = WH.MaxHp;
        }
        //StartWave();
    }
}
