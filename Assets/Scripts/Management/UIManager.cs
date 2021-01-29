using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Slider progressBar;

    public void Initialize()
    {
        progressBar = GameManager.canvas.transform.Find("General").Find("ProgressBar").GetComponent<Slider>();
        progressBar.value = 0.0f;
        progressBar.minValue = 0.0f;
        progressBar.maxValue = 100.0f;

        progressBar.enabled = false;
    }

    public void UpdateProgress(uint newProgress)
    {
        newProgress = (uint)Mathf.Clamp(newProgress, 0, 100);
        progressBar.enabled = true;
        progressBar.value = newProgress;
        progressBar.enabled = false;
    }
}
