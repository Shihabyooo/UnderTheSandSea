using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventViewer : MonoBehaviour
{

    static public EventViewer viewer = null;
    RawImage eventImage;
    TextMeshProUGUI eventDescription;
    TextMeshProUGUI eventEffects;
    TextMeshProUGUI eventEffectsTitle;
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
        eventButton = this.transform.Find("EventButton").GetComponent<Button>();
        
        Transform scrollViewContent = this.transform.Find("Scroll View").Find("Viewport").Find("Content");
        eventDescription = scrollViewContent.Find("EventText").GetComponent<TextMeshProUGUI>();
        eventEffects = scrollViewContent.Find("EventEffects").GetComponent<TextMeshProUGUI>();
        eventEffectsTitle = scrollViewContent.Find("EventEffectsTitle").GetComponent<TextMeshProUGUI>();
        

        Hide();
    }

    public void Show(ScenarioEvent scenarioEvent)
    {
        shownEvent = scenarioEvent;

        // eventImage.gameObject.SetActive(true);
        // eventText.gameObject.SetActive(true);
        // eventButton.gameObject.SetActive(true);

        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }

        this.gameObject.GetComponent<RawImage>().enabled =true;

        //get event image and description
        if (scenarioEvent.scenarioImage != null)
            eventImage.rectTransform.sizeDelta = ComputeViewporteSize(new Vector2Int(scenarioEvent.scenarioImage.width, scenarioEvent.scenarioImage.height));
        
        eventImage.texture = scenarioEvent.scenarioImage;
        

        //eventDescription.text = scenarioEvent.scenarioText;
        SetEventDescription();
    }

    void SetEventDescription()
    {
        eventDescription.text = shownEvent.scenarioText;

        if (shownEvent.scenarioEffectText == null)
        {
            eventEffectsTitle.gameObject.SetActive(false);
            eventEffects.text = "";
        }
        else
        {
            eventEffectsTitle.gameObject.SetActive(true);
            eventEffects.text = shownEvent.scenarioEffectText;
        }

    }

    public void Hide()
    {
        // eventImage.gameObject.SetActive(false);
        // eventText.gameObject.SetActive(false);
        // eventButton.gameObject.SetActive(false);
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        this.gameObject.GetComponent<RawImage>().enabled = false;
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
