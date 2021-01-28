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
            default:
                break;
        }
    }

}
