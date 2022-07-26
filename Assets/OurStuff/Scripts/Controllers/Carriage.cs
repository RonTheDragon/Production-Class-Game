using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : CharacterAI
{
    public Vector2 SoulsContained = new Vector2(3,10);
    protected override void Behaviour()
    {
        GoToWall();
    }

    new void Awake()
    {
        TheBody = transform;
        base.Awake();
    }
    new void Start()
    {
        base.Start();
    }
    new void Update()
    {
        base.Update();
    }

    protected void GoToWall()
    {
        Vector3 MoveTo;

        float WallSmaller = GameManager.instance.WallLength * 0.8f;
        if (GameManager.instance.WallFacingZ)
        {
            float Xpos = Mathf.Clamp(TheBody.position.x, TheWall.transform.position.x - (WallSmaller / 2), TheWall.transform.position.x + (WallSmaller / 2));
            MoveTo = new Vector3(Xpos, TheBody.position.y, TheWall.transform.position.z);
        }
        else
        {
            float Zpos = Mathf.Clamp(TheBody.position.z, TheWall.transform.position.z - (WallSmaller / 2), TheWall.transform.position.z + (WallSmaller / 2));
            // Debug.Log(Zpos);
            MoveTo = new Vector3(TheWall.transform.position.x + 2, TheBody.position.y, Zpos);
        }

        NMA.SetDestination(MoveTo);
        float distance = Vector3.Distance(TheBody.position, MoveTo);
        if (distance <= NMA.stoppingDistance + 2)
        {
            int r = (int)Random.Range(SoulsContained.x,SoulsContained.y);
            for (int i = 0; i < r; i++)
            {
            GameObject soul = ObjectPooler.Instance.SpawnFromPool("Soul", transform.position + Vector3.up, transform.rotation);
            soul.GetComponent<Soul>().SoulEnergy = (int)Random.Range(SoulValue.x,SoulValue.y);
            transform.parent.gameObject.SetActive(false);
            }
        }
        
    }
}
