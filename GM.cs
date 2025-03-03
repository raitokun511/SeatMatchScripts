using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM
{
    public static int CurrentLevel;
    public static int UnlockedLevel;
    public static Vector2Int CurrentBoardSize;
    public static int Coin;
    public static int Gem;

    public static int tmpCoin;
    public static int tmpGem;


    public static Vector3 GroundPadding;
    public static Dictionary<ChairType, Vector3> ChairOffsets;
    public static int bridgeIndex;
    public static float lastTimeMove = 0;

    public static void Init()
    {
        GroundPadding = Constant.GROUND_NORMAL_PADDING;
        if (ChairOffsets == null || ChairOffsets.Count == 0)
        {
            ChairOffsets = new Dictionary<ChairType, Vector3>();
            ChairOffsets.Add(ChairType.ArmChair, new Vector3(-0.2f, 0, 0));
            ChairOffsets.Add(ChairType.Couch, new Vector3(-0.2f, 0, -0.5f));
        }
    }

}
