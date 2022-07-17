using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(Click);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Click();
        }
    }

    public void Click()
    {
        GameManager.instance.QuitGame();
    }
}
