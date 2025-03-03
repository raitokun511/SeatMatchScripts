using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ChairCoor
{
    public Vector2Int Size;
    public Vector2Int Coor;
    public ColorType ColorType;
    public ChairType ChairType;
    public SeatState State;

    public ChairCoor Clone()
    {
        ChairCoor newCoor = new ChairCoor();
        newCoor.Size = Size;
        newCoor.Coor = Coor;
        newCoor.ColorType = ColorType;
        newCoor.State = State;
        newCoor.ChairType = ChairType;
        return newCoor;
    }
    public Vector2Int GetCoorFromPosition(Vector3 position)
    {
        position -= GM.GroundPadding;
        position -= GM.ChairOffsets[ChairType];
        if (GM.bridgeIndex > 0 && position.z > GM.bridgeIndex)
        {
            position -= new Vector3(0, 0, 0.5f);
        }
        Vector2Int coorInt = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        //Debug.Log("PositionCoor: " + coorInt);
        return coorInt;
    }
    public Vector3 GetPositionFromCoor(Vector2Int coor)
    {
        Vector3 position = new Vector3(coor.x, 0, coor.y);
        position += GM.GroundPadding;
        position += GM.ChairOffsets[ChairType];
        if (GM.bridgeIndex > 0 && position.z >= GM.bridgeIndex)
        {
            position += new Vector3(0, 0, 0.5f);
            //if (position.z )
        }
        //Debug.Log("Position: " + position);
        return position;
    }

    public void SetCoorForChair(Vector3 position)
    {
        Vector2Int coor = GetCoorFromPosition(position);
        Coor = coor;
    }

}
