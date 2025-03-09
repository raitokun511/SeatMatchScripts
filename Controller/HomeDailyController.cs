using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HomeDailyController : MonoBehaviour
{
    public Transform RowList1;
    public Transform RowList2;
    public Transform RowList3;
    public Transform RowList4;
    public Transform RowList5;
    Transform todayList;
    int day = 1;
    int month = 1;
    int year = 1;
    int todayIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        DateTime currentDate = DateTime.Now;

        day = 1;// currentDate.Day;
        month = currentDate.Month +1;
        year = currentDate.Year;
        todayList = RowList1;
        todayIndex = (day - 1) % 7;

        Debug.Log("Ngay: " + day);
        Debug.Log("Thang: " + month);
        Debug.Log("Nam: " + year);
        if (month == 4 || month == 6 || month == 9 || month == 11 || month == 2)
        {
            RowList5.GetChild(2).gameObject.SetActive(false);
        }
        if (month == 2)
        {
            RowList5.GetChild(1).gameObject.SetActive(false);
        }
        if (day > 7)
        {
            foreach (Transform dayitem in RowList1)
            {
                dayitem.GetChild(4).gameObject.SetActive(true);
            }

            if (day > 14)
            {
                foreach (Transform dayitem in RowList2)
                {
                    dayitem.GetChild(4).gameObject.SetActive(true);
                }

                if (day > 21)
                {
                    foreach (Transform dayitem in RowList3)
                    {
                        dayitem.GetChild(4).gameObject.SetActive(true);
                    }

                    if (day > 28)
                    {
                        todayList = RowList5;
                        foreach (Transform dayitem in RowList4)
                        {
                            dayitem.GetChild(4).gameObject.SetActive(true);
                        }
                    }
                    else // 21 < x <= 28
                    {

                        todayList = RowList4;
                        //day - 1 - 21
                        for (int i = 0; i < day - 15; i++)
                        {
                            RowList4.GetChild(i).GetChild(4).gameObject.SetActive(true);
                        }
                    }
                }
                else // 14 < x <= 21
                {
                    todayList = RowList3;
                    //day - 1 - 14
                    for (int i = 0; i < day - 15; i++)
                    {
                        RowList3.GetChild(i).GetChild(4).gameObject.SetActive(true);
                    }
                }
            }
            else // 7 < x <= 14
            {
                todayList = RowList2;
                //day - 1 - 7
                for (int i = 0; i < day - 8; i++)
                {
                    RowList2.GetChild(i).GetChild(4).gameObject.SetActive(true);
                }
            }
        }
        else // <= 7
        {
            todayList = RowList1;
            //day - 1 - 1
            for (int i = 0; i < day - 2;i++)
            {
                RowList1.GetChild(i).GetChild(4).gameObject.SetActive(true);
            }
        }

        int isTodayReceive = PlayerPrefs.GetInt(year +"" + month+"" + day, -1);
        if (isTodayReceive > 0)
        {
            todayList.GetChild(todayIndex).GetChild(4).gameObject.SetActive(true);
        }
        else
        {
            Button getRewardBtn = todayList.GetChild(todayIndex).AddComponent<Button>();
            getRewardBtn.onClick.AddListener(OnGetReward);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGetReward()
    {
        Debug.Log("GetReward");
        if (todayList != null)
        {
            todayList.GetChild(todayIndex).GetChild(4).gameObject.SetActive(true);
        }
    }
}
