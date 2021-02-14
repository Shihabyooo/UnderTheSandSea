using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO consider having a method that handles the loop inside DayPassage() and NightPassage(), that takes begining and end angles, and max intensity as arguments.

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] GameObject sun;
    [SerializeField] float sunAngle = 10.0f; //(negative of) angle from the equator (assume game in northern hemisphere).
    [SerializeField] float moonAngle = -5.0f; //ditto
    [SerializeField] float dawnAngle = 80.0f;
    [SerializeField] float duskAngle = -80.0f;
    [SerializeField] float gameplayStartAngle = 40.0f;
    [SerializeField] float gameplayEndAngle = -60.0f;

    [SerializeField] float maxSunIntensity = 1.0f;
    [SerializeField] float maxMoonIntensity = 0.35f;
    [SerializeField] Color sunLightColour = new Color();
    [SerializeField] Color moonLightColour = new Color();

    [SerializeField] AnimationCurve maxSunIntensityFade; //From left to right -> Fade in (dawn to noon). Inverse -> Fade out (noon to dusk). 

    [SerializeField] float dayAnimationSpeed = 20.0f; //angles per second.
    [SerializeField] float nightAnimationSpeed = 40.0f; //angles per second.

    public bool isAnimating {get; private set;}

    void Awake()
    {
        isAnimating = false;
    }

    public void Initialize()
    {
        //sun = GameObject.Find("Sun");
        StopAllCoroutines();
        sun.transform.localEulerAngles = new Vector3(sunAngle, gameplayStartAngle, 0.0f);
        isAnimating = false;
    }

    Coroutine dayPassage = null;

    public void StartDay()
    {
        isAnimating = true;
        dayPassage = StartCoroutine(DayPassage());
    }

    IEnumerator DayPassage()
    {
        sun.transform.localEulerAngles = new Vector3(sunAngle, gameplayStartAngle, 0.0f);
        float angleRange = Mathf.Abs(dawnAngle) + Mathf.Abs(duskAngle);
        
        yield return StartCoroutine(SimulateSunMoon(angleRange, gameplayEndAngle, sunAngle, maxSunIntensity, dayAnimationSpeed));
        
        yield return new WaitForSeconds (0.5f);

        isAnimating = false;
        dayPassage = null;
        yield return null;
    }

    Coroutine nightPassage = null;

    public void StartNight()
    {
        isAnimating = true;
        nightPassage = StartCoroutine(NightPassage());
    }

    IEnumerator NightPassage()
    {   
        //Finish up sun's cycle.
        float angleRange = Mathf.Abs(dawnAngle) + Mathf.Abs(duskAngle);

        yield return StartCoroutine(SimulateSunMoon(angleRange, duskAngle, sunAngle, maxSunIntensity, nightAnimationSpeed));

        //switch sun to moon 
        sun.GetComponent<Light>().color = moonLightColour;


        //Do moon cycle
        //angleRange remains same.
        sun.transform.localEulerAngles = new Vector3(moonAngle,
                                                    dawnAngle,
                                                    0.0f);

        yield return StartCoroutine(SimulateSunMoon(angleRange, duskAngle, moonAngle, maxMoonIntensity, nightAnimationSpeed));
        
        //switch moon to sun
        sun.GetComponent<Light>().color = sunLightColour;
        sun.transform.localEulerAngles = new Vector3(sunAngle,
                                                    dawnAngle,
                                                    0.0f);

        //do first part of sun cycle before workday
        yield return StartCoroutine(SimulateSunMoon(angleRange, gameplayStartAngle, sunAngle, maxSunIntensity, nightAnimationSpeed));

        yield return new WaitForSeconds (0.5f);

        isAnimating = false;
        nightPassage = null;
        yield return null;
    }


    IEnumerator SimulateSunMoon(float angleRange, float endAngle, float inclination, float maxIntensity, float speed)
    {
        while (true)
        {
            //adjust position.
            float newAngle = sun.transform.localEulerAngles.y;
            newAngle = newAngle > 180.0f? newAngle - 360.0f : newAngle;
            newAngle = newAngle - (speed * Time.deltaTime);

            sun.transform.localEulerAngles = new Vector3(inclination,
                                                        newAngle,
                                                        0.0f);

            //adjust intensity
            float relativeTime = (newAngle - duskAngle) / angleRange;
            float intensityMod = maxSunIntensityFade.Evaluate(Mathf.Sin(Mathf.Deg2Rad * (relativeTime * 180.0f)));
            sun.GetComponent<Light>().intensity = intensityMod * maxIntensity;

            //check if we have reached our end.
            if (newAngle <= endAngle)
                break;

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
