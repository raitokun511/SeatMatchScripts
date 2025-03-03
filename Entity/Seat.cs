using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat// : MonoBehaviour
{
    public ColorType TypeColor;
    public bool Locked;
    public Direction Direction;
    public SeatState State;
    public Chair parentChair;

    public Citizen CurrentCitizen;

    public Seat()
    {
    }
    public Seat(ColorType typeColor, SeatState state, Chair parentChair)
    {
        this.TypeColor = typeColor;
        this.State = state;
        this.parentChair = parentChair;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool EqualCitizenColor(ColorType citizenColor)
    {
        if (citizenColor != ColorType.Blue)
        {
            if (TypeColor == citizenColor)
                return true;
            else
                return false;
        }
        if (TypeColor == ColorType.Blue || TypeColor == ColorType.Gray)
        {
            return true;
        }
        return false;
    }
}
