using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassEnabler : MonoBehaviour
{
    public Shop shop;
    public SOPlayerBody Body;
    public bool Active = false;

    public void OnClick()
    {
        List<SOPlayerBody> PlayerList = GameManager.instance.GetComponent<PlayerRespawnManager>().PlayerBodies;
        GameObject X = transform.GetChild(0).gameObject;
        if (Active)
        {
            if (CheckIfThereAreOthers())
            {
                PlayerList.Remove(Body);
                Active = !Active;
                X.SetActive(true);
            }
            else Debug.Log("Must Have At Least 1 Enabled");
        }
        else
        {
            PlayerList.Add(Body);
            Active = !Active;
            X.SetActive(false);
        }
    }

    bool CheckIfThereAreOthers()
    {
        int counter = 0;
        foreach(Transform t in transform.parent)
        {
            if (t.GetComponent<ClassEnabler>().Active) counter++;
        }
        return (counter > 1);   
    }
}
