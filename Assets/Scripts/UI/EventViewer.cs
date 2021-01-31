using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventViewer : MonoBehaviour
{

    static public EventViewer viewer = null;
    RawImage eventImage;
    Text eventText;
    Button eventButton;

    ScenarioEvent shownEvent = null;

    void Awake()
    {
        if (viewer == null)
        {
            viewer = this;
        }
        else
        {
            Destroy(this.gameObject);
        }


        eventImage = this.transform.Find("EventImage").GetComponent<RawImage>();
        eventText = this.transform.Find("EventText").GetComponent<Text>();
        eventButton = this.transform.Find("EventButton").GetComponent<Button>();

        Hide();
    }

    public void Show(ScenarioEvent scenarioEvent)
    {
        shownEvent = scenarioEvent;

        eventImage.gameObject.SetActive(true);
        eventText.gameObject.SetActive(true);
        eventButton.gameObject.SetActive(true);
        this.gameObject.GetComponent<RawImage>().enabled =true;

        //get event image and description
        if (scenarioEvent.scenarioImage != null)
            eventImage.rectTransform.sizeDelta = ComputeViewporteSize(new Vector2Int(scenarioEvent.scenarioImage.width, scenarioEvent.scenarioImage.height));
        
        eventImage.texture = scenarioEvent.scenarioImage;
        

        eventText.text = scenarioEvent.scenarioText;
    }

    public void Hide()
    {
        eventImage.gameObject.SetActive(false);
        eventText.gameObject.SetActive(false);
        eventButton.gameObject.SetActive(false);
        this.gameObject.GetComponent<RawImage>().enabled =false;
        shownEvent = null;
    }
    
    public void OnButtonPress()
    {
        shownEvent.FinishEvent();
        Hide();
    }


    Vector2Int ComputeViewporteSize(Vector2Int imageSize)
    {
        Vector2Int size = new Vector2Int();
        
        //height is fixed at whatever was set initially in editor.
        size.y = (int)eventImage.rectTransform.sizeDelta.y;
        size.x = Mathf.RoundToInt((float)size.y * (float)imageSize.x / (float)imageSize.y);

        return size;
    }
}
