using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralButton : MonoBehaviour
{
    [SerializeField] int actionID;
    Button button;

void Awake()
    {
        button = this.gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        switch (actionID)
        {
            case 1: //start new day
                GameManager.gameMan.StartWorkDay();
                break;
            case 2: //Day Report acknowledgement/finish
                GameManager.gameMan.StartNight();
                break;
            case 3: //Day report skip typing
                GameManager.uiMan.SkipReportTyping();
                break;
            default:
                break;
        }
    }

}
