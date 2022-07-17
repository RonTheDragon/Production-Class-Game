using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class TownManager : MonoBehaviour
{
    public Camera TownCamera;
    [SerializeField] TMP_Text Souls;
    [SerializeField] TMP_Text HelpfulText;
    [SerializeField] GameObject ExitGameMenu;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSoulCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.Shopping)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGameMenu.SetActive(true);
                GameManager.instance.Shopping = true;
            }

            Ray ray = TownCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                ItownClickable TC = raycastHit.collider.gameObject.GetComponent<ItownClickable>();
                if (TC != null)
                {
                    UpdateHelpfulText(TC.OnHover());
                    if (Input.GetMouseButtonDown(0))
                    {
                        TC.OnClicked();
                    }
                }
                else
                {
                    UpdateHelpfulText(string.Empty);
                }
            }          
        }
        else UpdateHelpfulText(string.Empty);
    }

    public void UpdateSoulCount()
    {
        Souls.text = $"Souls: {GameManager.instance.SoulEnergy}";
    }

    public void UpdateHelpfulText(string t)
    {
        if (HelpfulText.text != t)
        {
            HelpfulText.text = t;
        }
    }
}
