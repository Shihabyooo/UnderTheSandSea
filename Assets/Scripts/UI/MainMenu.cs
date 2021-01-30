using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGame;

    void Awake()
    {
        newGame = this.transform.Find("StartNewGame").GetComponent<Button>();

        //Hide();
    }

    public void Show()
    {
        newGame.gameObject.SetActive(this);
        this.GetComponent<RawImage>().enabled = true;
    }

    public void Hide()
    {
        newGame.gameObject.SetActive(false);
        this.GetComponent<RawImage>().enabled = false;
    }

    public void OnButtonClick(int buttonID)
    {
        switch(buttonID)
        {
            case 1: //start new game
                Hide();
                GameManager.gameMan.StartNewGame();
                break;
        }
    }
}
