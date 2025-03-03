using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberUtil
{
    public static bool InRange(int number, int min, int max)
    {
        return number >= min && number <= max;
    }
}
