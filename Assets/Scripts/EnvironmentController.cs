using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    //References for drag/drop in inspector.
    [Header("References:")]
    [SerializeField] private Light sun;

    //Time settings
    [Header("Time:")]
    [SerializeField] [Range(1, 64)] public float timeMultiplier = 1f;
    [SerializeField] [Range(1, 120)] public int daysInMonth = 30;
    [SerializeField] [Range(1, 40)] public int monthsInYear = 12;
    //Slider for percentage of full day done
    [Range(0, 1)] [SerializeField] public float currentTimeOfDay = 0;
    [SerializeField] public float secondsInFullDay = 120f;

    //Time variables
    [Space(10)]
    [SerializeField] private float Minutes = 0;
    [SerializeField] private float Hours = 0;
    [SerializeField] private int Day = 1;
    [SerializeField] private int Month = 1;
    [SerializeField] private int Year = 1;

    [Space(10)]
    //Variables for calculations later.
    [SerializeField] private float sunInitialIntensity;
    [SerializeField] private float currentIntensity;

    //Temperature
    [Header("Temperature:")]
    //Temperature settings
    [SerializeField] [Range(0, 5)] public float tempMultiplier = 1;
    [SerializeField] public bool bIsTempFahrenheit = false;
    [SerializeField] public float temperature = 0;
    [SerializeField] [Range(0, 4)] public int tempPrecision = 2;

    //Temperature variables
    bool bNewGenerationTemp = true;
    bool bHasGeneratedTemp = false;
    float lastTemp = 0;
    float minGenTemp = 0;
    float maxGenTemp = 7;
    float generatedTemp = 5.0f;
    float[] gameTemp;
    float averageTemp;

    //Wind
    [Header("Wind:")]
    //Wind settings
    [SerializeField] [Range(0, 5)] public float windStrengthMultiplier = 1;
    [SerializeField] public float windStrength = 0;
    [SerializeField] [Range(0, 4)] public int windStrengthPrecision = 2;

    //Wind variable
    bool bNewGenerationWind = true;
    bool bHasGeneratedWind = false;
    float lastWind;
    float generatedWind;
    float[] gameWind;
    float averageWind;

    //Wind angle settings
    [SerializeField] public float windAngle = 0;
    [SerializeField] [Range(0, 4)] public int windAnglePrecision = 2;

    //Wind angle variables
    bool bNewGenerationWindAngle = true;
    bool bHasGeneratedWindAngle = false;
    float lastWindAngle;
    float generatedWindAngle;
    float[] gameWindAngle;
    float averageWindAngle;


    void Start()
    {
        //Setting defaults
        if(sun == null)
        {
            sun = transform.root.GetChild(0).GetComponent<Light>();
        }
        sunInitialIntensity = sun.intensity;
        gameTemp = new float[3];
        gameWind = new float[3];
        gameWindAngle = new float[3];
    }

    void Update()
    {
        //Functions to run every tick.
        SetClockwork();
        Clock();

        UpdateSun();
        Temperature();
        WindStrength();
        WindAngle();
    }

    void SetClockwork()
    {
        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        //Restart time of day when day is finished.
        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
            Calendar();
        }
    }

    void Clock()
    {
        //Time calculations
        float currentHours = 24.0f * currentTimeOfDay;
        float currentMinutes = 60 * (currentHours - Mathf.Floor(currentHours));

        Minutes = Mathf.FloorToInt(currentMinutes);
        Hours = Mathf.FloorToInt(currentHours);
    }

    void Calendar()
    {
        //Calendar function
        Day++;
        if (Day > daysInMonth)
        {
            Day = 1;
            Month++;
        }
        if (Month > monthsInYear)
        {
            Month = 1;
            Year++;
        }
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
        currentIntensity = sun.intensity;
    }


    void Temperature()
    {
        //Check whether its a new game for generation
        if (bNewGenerationTemp)
        {
            lastTemp = Random.Range(0.0f, 7.0f);
            bNewGenerationTemp = false;
        }

        //Only run this at the start of every hour.
        if (Minutes == 0)
        {
            if (!bHasGeneratedTemp) //Stops repeats of code within that minute.
            {
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
                    if (Hours <= 13 && Hours > 1)
                    {
                        generatedTemp = generatedTemp + Random.Range(1.5f, 3.0f);
                    }
                    else if (Hours > 13 && Hours < 24)
                    {
                        generatedTemp = generatedTemp - Random.Range(0.2f, 1.5f);
                    }
                    //Set temperature into array for average.
                    gameTemp[i] = generatedTemp;
                }
                //calculate avergae.
                averageTemp = (gameTemp[0] + gameTemp[1] + gameTemp[2]) / 3;

                //apply wind strength to temperature
                averageTemp = averageTemp - (windStrength / 5);

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

                bHasGeneratedTemp = true; //Stops repetition of code.
            }
        }
        else
        {
            bHasGeneratedTemp = false; //Makes sure code does not happen when minute is not 0
        }
        temperature = averageTemp; //sets temperature of world to generated temp.
    }

    void WindStrength()
    {
        //check for new game
        if (bNewGenerationWind)
        {
            lastWind = Random.Range(0, 7);
            bNewGenerationWind = false;
        }

        //wind only changes every 6 hours at 0 minutes
        if ((Hours == 0 && Minutes == 0) ||
            (Hours == 6 && Minutes == 0) ||
            (Hours == 12 && Minutes == 0) ||
            (Hours == 18 && Minutes == 0))
        {
            if (!bHasGeneratedWind)
            {
                for (int i = 0; i < 3; i++) //repeats for average
                {
                    generatedWind = Random.Range(-3.0f, 3.0f); //generate base wind

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
                if (averageWind > 2.5f)
                {
                    averageWind = averageWind - Random.Range(0.3f, 0.9f);
                }
                else if (averageWind < 0)
                {
                    averageWind = Random.Range(0.3f, 1.3f);
                }
                //sets last wind = generated wind
                lastWind = averageWind;
                //applies multiplier.
                averageWind = averageWind * windStrengthMultiplier;

                //rounds to chosen number of dp
                averageWind = (float)System.Math.Round(averageWind, windStrengthPrecision);

                bHasGeneratedWind = true; //stops repetition
            }
        }
        else
        {
            bHasGeneratedWind = false;
        }
        windStrength = averageWind; //sets actual world wind = generated wind.
    }

    void WindAngle()
    {
        //Checks for new generation
        if (bNewGenerationWindAngle)
        {
            lastWindAngle = Random.Range(0.0f, 360.0f);
            bNewGenerationWindAngle = false;
        }

        //only generate on new hour.
        if (Minutes == 0)
        {
            //stops repetition
            if (!bHasGeneratedWindAngle)
            {
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

                bHasGeneratedWindAngle = true; //stops repetitions.
            }
        }
        else
        {
            bHasGeneratedWindAngle = false;
        }

        windAngle = averageWindAngle; //sets actual wind angle to generated one.
    }
}