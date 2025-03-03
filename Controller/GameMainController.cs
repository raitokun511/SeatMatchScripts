using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainController : MonoBehaviour
{
    [SerializeField]
    LevelSetting levelSetting;
    [SerializeField]
    GameObject boardObject;
    [SerializeField]
    GameObject[] ConfettiEffect;

    public GameState gameState;
    public static float Time;

    Board board;

    public GameObject[] objectsToLoad;
    public bool isLoadingComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        GM.Init();
        int[] arr = new int[] { 1,2,3,4,5};
        Change(arr);
        Debug.Log(arr[1]);
        board = boardObject.GetComponent<Board>();
        board.SetLevelSetting(levelSetting, ConfettiEffect);
        StartCoroutine(board.LoadObjects(onComplete:() => {
            isLoadingComplete = true;
        }));

    }
    void Change(int[] source)
    {
        source[1] = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLoadingComplete)
        {
            return;
        }

    }
}
