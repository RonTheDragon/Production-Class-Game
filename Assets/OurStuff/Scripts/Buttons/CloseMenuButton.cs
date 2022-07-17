using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseMenuButton : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(Click);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Click();
        }
    }

    public void Click()
    {
        transform.parent.parent.gameObject.SetActive(false);
        if (GameManager.instance.Player != null)
        {
            if (GameManager.instance.Player.activeSelf != false)
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
        }
        GameManager.instance.Shopping=false;
    }

}
