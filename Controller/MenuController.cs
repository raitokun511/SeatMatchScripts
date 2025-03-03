using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI LevelText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMainController.Time > 0)
        {
            GameMainController.Time -= Time.deltaTime;
            int minute = (int) GameMainController.Time / 60;
            int second = (int)GameMainController.Time % 60;
            LevelText.text = (minute > 9 ? minute.ToString() : "0" + minute.ToString())
                           + ":" + (second > 9 ? second.ToString() : "0" + second.ToString());
        }
    }
}
