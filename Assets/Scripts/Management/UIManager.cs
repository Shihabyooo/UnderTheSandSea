using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Slider progressBar;
    Transform reportViewPort;

    public void Initialize()
    {
        progressBar = GameManager.canvas.transform.Find("General").Find("ProgressBar").GetComponent<Slider>();
        reportViewPort = GameManager.canvas.transform.Find("DayReport");
        progressBar.value = 0.0f;
        progressBar.minValue = 0.0f;
        progressBar.maxValue = 100.0f;

        progressBar.enabled = false;
        HideReport();
        
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
                return (GameManager.simMan.progress.ToString() + "%");
            case "Manpower":
                return GameManager.popMan.CountAll().ToString();
            case "Income":
                return "TBD";
            case "Expenses":
                return "TBD";
            case "Funds":
                return("$" + GameManager.simMan.fincances.funds.ToString());
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
}
