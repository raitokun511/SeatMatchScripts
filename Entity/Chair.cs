using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chair : MonoBehaviour
{
    public List<Seat> seats = new List<Seat>();
    public ChairCoor chairCoor;
    public bool LockedByCitizen
    {
        set; get;
    }
    public SeatState SeatState
    {
        get
        {
            //Debug.Log("Get Seat " + seats[0].State);
            return seats[0].State;
        }
        set
        {
            foreach (Seat seat in seats)
            {
                seat.State = value;
            }
            chairCoor.State = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LockedByCitizen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFixedPosition(Vector3 position)
    {
        Vector2Int coor = chairCoor.GetCoorFromPosition(position);
        transform.position = chairCoor.GetPositionFromCoor(coor);
    }
    public void SetPositionAuto()
    {
        SetFixedPosition(transform.position);
    }

    public void SetPosition(Vector3 position)
    {
        if (IsValidPosition(position))
        {
            transform.position = position;
        }
    }

    bool IsValidPosition(Vector3 position)
    {
        //float rightCouchOffset = chairCoor.ChairType == ChairType.Couch ? -1f: 0;
        Vector2Int coor = chairCoor.GetCoorFromPosition(position);
        Vector2Int coorLeft = chairCoor.GetCoorFromPosition(position + new Vector3(0,0, 0.4f));
        Vector2Int coorRight = chairCoor.GetCoorFromPosition(position + new Vector3(0, 0, -0.5f));
        Vector2Int coorTop = chairCoor.GetCoorFromPosition(position + new Vector3(0.5f, 0, 0));
        Vector2Int coorBottom = chairCoor.GetCoorFromPosition(position + new Vector3(-0.5f, 0, 0));

        if (coor.x == chairCoor.Coor.x && coor.y == chairCoor.Coor.y)
        {
            return true;
        }
        if (!NumberUtil.InRange(coor.x, 0, GM.CurrentBoardSize.x-1) || !NumberUtil.InRange(coor.y, 0, GM.CurrentBoardSize.y-1))
        {
            return false;
        }
        if (!IsNodeValid(coorLeft))
        {
            return false;
        }
        if (!IsNodeValid(coorRight))
        {
            return false;
        }
        if (!IsNodeValid(coorTop))
        {
            return false;
        }
        if (!IsNodeValid(coorBottom))
        {
            return false;
        }
        if (chairCoor.ChairType == ChairType.Couch)
        {
            coorRight = chairCoor.GetCoorFromPosition(position + new Vector3(0, 0, -1.5f));
            coorTop = chairCoor.GetCoorFromPosition(position + new Vector3(0.5f, 0, -0.5f));
            coorBottom = chairCoor.GetCoorFromPosition(position + new Vector3(-0.5f, 0, -0.5f));
            if (!IsNodeValid(coorRight))
            {
                return false;
            }
            if (!IsNodeValid(coorTop))
            {
                return false;
            }
            if (!IsNodeValid(coorBottom))
            {
                return false;
            }
        }
        return true;
    }
    bool IsNodeValid(Vector2Int coor)
    {
        if (Board.Instance.BoardSeatValue.ContainsKey((coor.x, coor.y))
            && Board.Instance.BoardSeatValue[(coor.x, coor.y)] != SeatState.None)
        {
            return false;
        }
        return true;
    }

    public void SetCoorForChair()
    {
        chairCoor.SetCoorForChair(transform.position);
    }

    public void AddToDictionary(Dictionary<(int, int), Seat> sourceDict)
    {
        //Remove old seat value
        List<(int,int)> listKeyRemove = new List<(int,int)> ();
        if (seats.Count > 0)
        {
            foreach (var chairseat in seats) {
                foreach (var oldseat in sourceDict)
                    if (oldseat.Value == chairseat)
                    {
                        listKeyRemove.Add(oldseat.Key);
                    }
            }
            foreach (var key in listKeyRemove)
            {
                sourceDict.Remove(key);
            }
            seats.Clear();
        }

        //default
        Seat seat = new Seat(chairCoor.ColorType, chairCoor.State, this);
        seats.Add(seat);
        sourceDict[(chairCoor.Coor.x, chairCoor.Coor.y)] = seat;
        //Debug.Log($"[ReLogSeat] {seat.ToString()} {chairCoor.Coor.x} , {chairCoor.Coor.y} = {sourceDict[(chairCoor.Coor.x, chairCoor.Coor.y)]}");

        switch (chairCoor.ChairType)
        {
            case ChairType.ArmChair:
                break;
            case ChairType.Couch:
                Seat couchseat = new Seat(chairCoor.ColorType, chairCoor.State, this);
                seats.Add(couchseat);
                //Debug.Log($"[ReLogSeat] {chairCoor.Coor.x} , {chairCoor.Coor.y - 1}");
                sourceDict[(chairCoor.Coor.x, chairCoor.Coor.y - 1)] = couchseat;
                break;
        }
    }
}
