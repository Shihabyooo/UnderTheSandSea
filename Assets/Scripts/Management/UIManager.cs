using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Slider progressBar;
    Transform reportViewPort;
    RosterSheet rosterSheet;
    MainMenu mainMenu;

    Dashboard activeDashboard = null;
    GameObject constructionMenu;
    GameObject generalUI;

    //TODO move DayReport to its own script.

    void Start()
    {
        progressBar = GameManager.canvas.transform.Find("General").Find("ProgressBar").GetComponent<Slider>();
        reportViewPort = GameManager.canvas.transform.Find("DayReport");
        rosterSheet = GameManager.canvas.transform.Find("RosterSheet").GetComponent<RosterSheet>();   
        mainMenu = GameManager.canvas.transform.Find("MainMenu").GetComponent<MainMenu>();
        constructionMenu = GameManager.canvas.transform.Find("ConstructionMenu").gameObject;
        generalUI = GameManager.canvas.transform.Find("General").gameObject;
    }

    public void Initialize()
    {
        progressBar.value = 0.0f;
        progressBar.minValue = 0.0f;
        progressBar.maxValue = 100.0f;

        progressBar.enabled = false;
        HideReport();
        ToggleControls(true);
        //rosterSheet.Close();
    }

    public void UpdateProgress(uint newProgress)
    {
        newProgress = (uint)Mathf.Clamp(newProgress, 0, 100);
        progressBar.enabled = true;
        progressBar.value = newProgress;
        progressBar.enabled = false;
    }

    Coroutine dayReportTyping = null;

    public void ShowDayReport()
    {
        reportViewPort.gameObject.SetActive(true);
        skipTyping = false;
        reportViewPort.gameObject.GetComponent<Button>().enabled = true;

        dayReportTyping = StartCoroutine(TypeDayReport(reportViewPort.Find("TypedStrings")));
    }

    bool skipTyping = false;
    [SerializeField] float typeEffectLatency = 0.15f;
    [SerializeField] float timeBetweenElements = 0.35f;
    [SerializeField] float waitBeforeReportFinish = 0.5f;
    IEnumerator TypeDayReport(Transform elementsContainer)
    {
        for (int i = 0; i < elementsContainer.childCount; i++)
        {
            Transform element = elementsContainer.GetChild(i);
            Text elementText = element.GetComponent<Text>();
            elementText.enabled = true;

            string tempString = "";
            string targetString = GetReportDetail(element);


            foreach (char letter in targetString)
            {
                tempString += letter;
                elementText.text = tempString;
                if (!skipTyping)
                    yield return new WaitForSeconds(typeEffectLatency);
            }
            if (!skipTyping)
                yield return new WaitForSeconds(timeBetweenElements);
        }
        

        yield return new WaitForSeconds(waitBeforeReportFinish);
        OnReportTypingFinish();
        yield return null;
    }

    string GetReportDetail(Transform textField)
    {
        string detail = "";

        switch (textField.gameObject.name)
        {
            case "Heading":
                return textField.GetComponent<Text>().text;
            case "Date":
                System.DateTime date = GameManager.simMan.currentDate;
                return (date.Day.ToString() + " - " + AbbreviatedMonth(date.Month) + " - " + date.Year.ToString());
            case "Progress":
                return (Mathf.RoundToInt(GameManager.simMan.progress).ToString() + "%");
            case "Manpower":
                return GameManager.popMan.CountAll().ToString();
            case "Income":
                return ("$" + GameManager.simMan.finances.dayRevenue.ToString());
            case "Expenses":
                return ("$" + GameManager.simMan.finances.dayExpenses.ToString());
            case "Funds":
                return("$" + GameManager.simMan.finances.funds.ToString());
        }

        return detail;
    }

    string AbbreviatedMonth(int month)
    {
        switch (month)
        {
            case 1:
                return "Jan";
            case 2:
                return "Feb";
            case 3:
                return "Mar";
            case 4:
                return "Apr";
            case 5:
                return "May";
            case 6:
                return "Jun";
            case 7:
                return "Jul";
            case 8:
                return "Aug";
            case 9:
                return "Sep";
            case 10:
                return "Oct";
            case 11:
                return "Nov";
            case 12:
                return "Dec";
            default:
                return "NUL";
        }
    }

    void OnReportTypingFinish()
    {
        dayReportTyping = null;
        reportViewPort.Find("SignButton").gameObject.SetActive(true);
    }

    public void SkipReportTyping()
    {
        skipTyping = true;
        reportViewPort.gameObject.GetComponent<Button>().enabled = false;
    }

    public void HideReport()
    {
        foreach(Transform element in reportViewPort.Find("TypedStrings"))
        {
            element.gameObject.GetComponent<Text>().enabled = false;
        }
        reportViewPort.Find("SignButton").gameObject.SetActive(false);
        reportViewPort.gameObject.SetActive(false);
    }

    public void ShowRosterSheet()
    {
        rosterSheet.enabled = true;
        rosterSheet.Show();
    }

    public void ShowMainMenu()
    {
        mainMenu.Show();
    }

    public void SwitchDashboard(Dashboard newDashboard)
    {
        if (activeDashboard != null && activeDashboard.gameObject.activeSelf && newDashboard != activeDashboard)
        {
            activeDashboard.Close();
        }
        activeDashboard = newDashboard;
    }

    public void ToggleControls(bool newState)
    {
        generalUI.SetActive(newState);
        constructionMenu.SetActive(newState);
    }

}