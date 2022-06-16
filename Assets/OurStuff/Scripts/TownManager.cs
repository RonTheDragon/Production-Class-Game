using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class TownManager : MonoBehaviour
{
    public Camera TownCamera;
    [SerializeField] TMP_Text Souls;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSoulCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = TownCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                ItownClickable TC = raycastHit.collider.gameObject.GetComponent<ItownClickable>();
                if (TC != null)
                {
                    TC.OnClicked();
                }
            }
        }
    }

    void UpdateSoulCount()
    {
        Souls.text = $"Souls: {GameManager.instance.SoulEnergy}";
    }
}
