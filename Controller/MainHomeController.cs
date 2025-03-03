using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MainHomeController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject BackGround;
    public TMP_Text levelButtonText;
    public RectTransform coinFrame;
    public RectTransform gemFrame;
    public RectTransform liveFrame;
    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text liveText;
    public RectTransform liveProgress;
    public ScrollRect leaderScrollview;
    public GameObject settingPanel;

    public GameObject coinPrefab;
    public GameObject gemPrefab;
    public GameObject livePrefab;

    public RectTransform[] screens; // Chứa các RectTransform của 3 màn hình
    //public DataLoader data;
    

    private int ScreenIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    float swipeThreshold = 50f; // Ngưỡng swipe để xác định khi nào swipe được tính là thành công

    //Swipe Move Component
    float timeMoveScreen = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        GM.Init();
        //DataLoader.LoadData();

    }

    // Update is called once per frame
    void Update()
    {
        if (timeMoveScreen > 0)
            return;

        if (GM.tmpCoin > 0)
        {
            GM.Coin -= GM.tmpCoin;
            coinText.text = GM.Coin + "";
            StartCoroutine(CreateAndMoveAsset(GM.tmpCoin, coinFrame));
            GM.tmpCoin = 0;
            return;
        }
        if (GM.tmpGem > 0)
        {
            GM.Gem -= GM.tmpGem;
            gemText.text = GM.Gem + "";
            StartCoroutine(CreateAndMoveAsset(GM.tmpGem, gemFrame));
            GM.tmpGem = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    //if (leaderScrollview.velocity.magnitude > 0.1f)
                    //    return;

                    // Xác định hướng swipe
                    float swipeDistance = Vector2.Distance(touchStartPos, touchEndPos);

                    if (swipeDistance > swipeThreshold)
                    {
                        // Xác định hướng swipe theo chiều ngang
                        float swipeDirection = Mathf.Sign(touchEndPos.x - touchStartPos.x);

                        // Chuyển đổi màn hình tương ứng với hướng swipe
                        ChangeScreen(swipeDirection);
                    }

                    break;
            }
        }
    }
    private void ChangeScreen(float direction)
    {
        //if (settingPanel.activeSelf)
        //    return;

        // Tìm màn hình hiện tại
        int currentScreen = 0;
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i].anchoredPosition.x == 0f)
            {
                currentScreen = i;
                break;
            }
        }

        // Xác định màn hình mới
        int newScreen = currentScreen - (int)direction;

        if (newScreen < screens.Length && newScreen >= 0)
        {
            currentScreen = Mathf.Clamp(newScreen, 0, screens.Length - 1);
            Debug.Log("Screen " + currentScreen);
            timeMoveScreen = screens[0].rect.width;
            StartCoroutine(moveSelf(direction));
        }
    }

    public IEnumerator moveSelf(float direction)
    {
        while (true)
        {
            float t = timeMoveScreen / 3.5f;
            if (t < screens[0].rect.width / 80f)
                t = timeMoveScreen;
            timeMoveScreen -= t;
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].anchoredPosition += new Vector2(t * direction, 0);
            }
            if (timeMoveScreen <= 0)
            {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CreateAndMoveAsset(int number, RectTransform frame)
    {
        for (int i = 0; i < number; i++)
        {
            int frameID = frame.GetInstanceID() == coinFrame.GetInstanceID() ? 1 : 2;
            GameObject itemUI = frameID == 1 ? Instantiate(coinPrefab) : Instantiate(gemPrefab);
            itemUI.transform.SetParent(canvas.transform, false);
            itemUI.transform.localPosition = Vector3.zero;
            itemUI.transform.localRotation = Quaternion.identity;
            
            GameObject iconTarget = frameID == 1 ? coinFrame.transform.Find("Icon").gameObject : gemFrame.transform.Find("Icon").gameObject;
            itemUI.transform.localScale = iconTarget.transform.localScale;

            LeanTween.move(itemUI, iconTarget.transform.position, 0.3f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    Destroy(itemUI);
                    if (frameID == 1)
                    {
                        GM.Coin += 1;
                        coinText.text = GM.Coin + "";
                    }
                    if (frameID == 2)
                    {
                        GM.Gem += 1;
                        gemText.text = GM.Gem + "";
                    }
                });

            yield return new WaitForSeconds(0.3f);
        }
    }


    public void PlayMainGame()
    {
        SceneManager.LoadScene("GameMatchMain");
        
    }
    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

}
