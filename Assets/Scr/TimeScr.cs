using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScr : MonoBehaviour
{
    public int day;
    public int month;
    public int year = 1900;
    public int speed = 1;
    float timeCounter = 0;

    public event System.Action OnNewDay;
    public event System.Action OnNewMonth;

    private void Update()
    {
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime * speed;
        }
        else
        {
            int previousMonth = month;

            day++;
            OnNewDay?.Invoke();

            switch (month)
            {
                case 1: if (day >= 31) { day = 1; month++; } break;
                case 2: if (day >= 28) { day = 1; month++; } break;
                case 3: if (day >= 31) { day = 1; month++; } break;
                case 4: if (day >= 30) { day = 1; month++; } break;
                case 5: if (day >= 31) { day = 1; month++; } break;
                case 6: if (day >= 30) { day = 1; month++; } break;
                case 7: if (day >= 31) { day = 1; month++; } break;
                case 8: if (day >= 31) { day = 1; month++; } break;
                case 9: if (day >= 30) { day = 1; month++; } break;
                case 10: if (day >= 31) { day = 1; month++; } break;
                case 11: if (day >= 30) { day = 1; month++; } break;
                case 12: if (day >= 31) { day = 1; month = 1; year++; } break;
            }

            if (month != previousMonth)
            {
                OnNewMonth?.Invoke();
            }

            timeCounter = 1;
        }
    }
}