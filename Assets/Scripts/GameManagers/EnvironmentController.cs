using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkUtils))]
public class EnvironmentController : NetworkBehaviour
{

    //References for drag/drop in inspector.
    [Header("References:")]
    [SerializeField] private Light sun;

    //Time settings
    [Header("Time:")]
    [SerializeField] [Range(1, 64)] public float timeMultiplier = 1f;
    //Slider for percentage of full day done
    [Range(0, 1)] [SerializeField] public float currentTimeOfDay = 0;
    [SerializeField] public float secondsInFullDay = 120f;

    [SerializeField] [Range(0.0f, 1.0f)] public float startingTime = 0.3f;

    //Time variables
    [Space(10)]
    [SerializeField] public int minutes = 0;
    [SerializeField] public int hours = 0;
    [SerializeField] public int days = 0;
    float lastHour = -1;

    [Space(10)]
    //Variables for calculations later.
    [SerializeField] private float sunInitialIntensity;

    //Temperature
    [Header("Temperature:")]
    //Temperature settings
    [SerializeField] [Range(0, 5)] public float tempMultiplier = 1;
    [SerializeField] public bool bIsTempFahrenheit = false;
    [SerializeField] public float temperature = 0;
    [SerializeField] [Range(0, 4)] public int tempPrecision = 2;

    //Temperature variables
    bool bNewGenerationTemp = true;
    float lastTemp = 0;
    float minGenTemp = 0;
    float maxGenTemp = 7;
    float generatedTemp = 5.0f;
    float[] gameTemp;
    float averageTemp;

    //Wind
    [Header("Wind:")]
    //Wind settings
    [SerializeField] [Range(0, 5)] public float windSpeedMultiplier = 1;
    [SerializeField] public float windSpeed = 0;
    [SerializeField] [Range(0, 4)] public int windSpeedPrecision = 2;

    //Wind variable
    bool bNewGenerationWind = true;
    float lastWind;
    float generatedWind;
    float[] gameWind;
    float averageWind;

    //Wind angle settings
    [SerializeField] public float windAngle = 0;
    [SerializeField] [Range(0, 4)] public int windAnglePrecision = 2;

    //Wind angle variables
    bool bNewGenerationWindAngle = true;
    float lastWindAngle;
    float generatedWindAngle;
    float[] gameWindAngle;
    float averageWindAngle;

    NetworkUtils networkUtils;

    void Start()
    {
        //Setting defaults
        if (sun == null)
        {
            sun = transform.root.GetChild(0).GetComponent<Light>();
        }
        sunInitialIntensity = sun.intensity;
        gameTemp = new float[3];
        gameWind = new float[3];
        gameWindAngle = new float[3];
        networkUtils = GetComponent<NetworkUtils>();
        currentTimeOfDay = startingTime;
    }

    void Update()
    {
        //Functions to run every tick.
        Clockwork();
        UpdateSun();
    }

    void Clockwork()
    {
        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;
        Clock();

        //Restart time of day when day is finished.
        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
            days++;
        }
    }

    void Clock()
    {
        //Time calculations
        float currentHours = 24.0f * currentTimeOfDay;
        float currentMinutes = 60 * (currentHours - Mathf.Floor(currentHours));

        //If new hour
        if (lastHour != hours)
        {
            Temperature();
            WindSpeed();
            WindAngle();
            lastHour = hours;

            //Update environment
            PlayerConnectionObject host = networkUtils.GetHostPlayerConnectionObject();
            if(host != null)
            {
                host.CmdUpdateEnvironment();
            }
        }

        minutes = Mathf.FloorToInt(currentMinutes);
        hours = Mathf.FloorToInt(currentHours);
    }

    void UpdateSun()
    {
        //Set sun rotation
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        //Light intensity
        float intensityMultiplier = 1;
        //Night
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        //Sunrise
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        //Sunset
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }
        //Apply intensity
        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }


    //Function to generate temperature for that hour
    void Temperature()
    {
        //Check whether its a new game for generation
        if (bNewGenerationTemp)
        {
            lastTemp = Random.Range(0.0f, 7.0f);
            bNewGenerationTemp = false;
        }

        for (int i = 0; i < 3; i++) //Repeats three times to be used in average later.
        {
            maxGenTemp = 12.0f * tempMultiplier;
            minGenTemp = 2.0f * tempMultiplier;

            //Random generation between min and max generated.
            generatedTemp = Random.Range(minGenTemp, maxGenTemp);

            //Makes sure temperature gap is not too large/unreasonable.
            if ((generatedTemp - lastTemp) > 4)
            {
                generatedTemp = lastTemp + Random.Range(2.0f, 3.5f);
            }
            else if ((lastTemp - generatedTemp) > 4)
            {
                generatedTemp = lastTemp - Random.Range(0.0f, 2.5f);
            }

            //Gradual increase towards midday and decrease away from midday.
            if (hours <= 13 && hours > 1)
            {
                generatedTemp = generatedTemp + Random.Range(1.5f, 3.0f);
            }
            else if (hours > 13 && hours < 24)
            {
                generatedTemp = generatedTemp - Random.Range(0.2f, 1.5f);
            }
            //Set temperature into array for average.
            gameTemp[i] = generatedTemp;
        }
        //calculate avergae.
        averageTemp = (gameTemp[0] + gameTemp[1] + gameTemp[2]) / 3;

        //apply wind strength to temperature
        averageTemp = averageTemp - (windSpeed / 5);

        //Makes sure temperature is not below absolute zero.
        if (averageTemp < -273.0f)
        {
            averageTemp = -273.0f;
        }
        //sets last temp = new temp
        lastTemp = averageTemp;

        //converts to fahrenheit if needed.
        if (bIsTempFahrenheit)
        {
            averageTemp = (averageTemp * (9 / 5)) + 32;
        }

        //Rounds to chosen number of dp
        averageTemp = (float)System.Math.Round(averageTemp, tempPrecision);

        temperature = averageTemp; //sets temperature of world to generated temp.
    }

    //Function to generate wind strength for that hour
    void WindSpeed()
    {
        //check for new game
        if (bNewGenerationWind)
        {
            lastWind = Random.Range(0.0f, 7.0f);
            bNewGenerationWind = false;
        }

        for (int i = 0; i < 3; i++) //repeats for average
        {
            generatedWind = Random.Range(0.0f, 7.0f); //generate base wind

            //makes sure wind difference is not too large
            if ((generatedWind - lastWind) > 3)
            {
                generatedWind = lastWind + Random.Range(0.0f, 3.0f);
            }
            else if ((lastWind - generatedWind) < 3)
            {
                generatedWind = lastWind - Random.Range(0.0f, 3.0f);
            }

            //sets into array for average
            gameWind[i] = generatedWind;
        }

        //calculates average
        averageWind = (gameWind[0] + gameWind[1] + gameWind[2]) / 3;

        //makes sure wind is not too extreme and wind is not less than 0
        if (averageWind > 10.0f)
        {
            averageWind = averageWind - Random.Range(0.3f, 3.9f);
        }
        else if (averageWind < 0)
        {
            averageWind = Random.Range(0.3f, 1.3f);
        }
        //sets last wind = generated wind
        lastWind = averageWind;
        //applies multiplier.
        averageWind = averageWind * windSpeedMultiplier;

        //rounds to chosen number of dp
        averageWind = (float)System.Math.Round(averageWind, windSpeedPrecision);

        windSpeed = averageWind; //sets actual world wind = generated wind.
    }

    //Function to generate wind angle for that hour
    void WindAngle()
    {
        //Checks for new generation
        if (bNewGenerationWindAngle)
        {
            lastWindAngle = Random.Range(0.0f, 360.0f);
            bNewGenerationWindAngle = false;
        }

        for (int i = 0; i < 3; i++)
        {  //repeats for average
            //base wind angle
            generatedWindAngle = Random.Range(0.0f, 360.0f);

            //makes sure difference is not too great.
            if ((generatedWindAngle - lastWindAngle) > 15)
            {
                generatedWindAngle = lastWindAngle + Random.Range(0.0f, 10.0f);
            }
            else if ((lastWindAngle - generatedWindAngle) < 15)
            {
                generatedWindAngle = lastWindAngle - Random.Range(0.0f, 10.0f);
            }
            //puts into array for average calculation
            gameWindAngle[i] = generatedWindAngle;
        }

        //calculates average.
        averageWindAngle = (gameWindAngle[0] + gameWindAngle[1] + gameWindAngle[2]) / 3;

        //used for bridging gap between 0 and 360 as you cannot go above 360 in angle and cannot go below 0.
        if (averageWindAngle < 0)
        {
            averageWindAngle = 340 - averageWindAngle;
        }
        else if (averageWindAngle > 360)
        {
            averageWindAngle = averageWindAngle - 340;
        }
        //sets last angle = generated angle
        lastWindAngle = averageWindAngle;

        //rounds to chosen number of dp
        averageWindAngle = (float)System.Math.Round(averageTemp, windAnglePrecision);

        windAngle = averageWindAngle; //sets actual wind angle to generated one.
    }
}