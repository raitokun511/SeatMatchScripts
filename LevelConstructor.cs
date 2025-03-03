using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Creation")]
public class LevelConstructor : ScriptableObject
{
    public Vector2Int BoardSize;
    public int BridgeIndex;
    public int Time;
    public List<ChairCoor> ChairList;
    public List<Door> Doors;
}
