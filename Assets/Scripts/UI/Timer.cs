using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 90;
    public bool timerIsRunning = false;
    public Text timeText;
    public bool alarm; 

    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        alarm = false;
        timeText.color = Color.white;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 30 && alarm == false)
                {
                    alarm = true;
                    timeText.color = Color.red;
                }
            }
            else if (timeRemaining == 0)
            {
                Debug.Log("Time has run out!");
                timerIsRunning = false;
            }
            DisplayTime(timeRemaining);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float millisecond = timeToDisplay % 1 * 1000;

        if(alarm == false)
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        else if (alarm == true)
            timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, millisecond);
            
    }
}