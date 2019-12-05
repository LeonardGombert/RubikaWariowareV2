using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScaler : MonoBehaviour
{
    public static double Scale(int valueToScale, int valueMin, int valueMax, int minScaleTo, int maxScaleTo)
    {
        double scaledValue = minScaleTo + (double)(valueToScale - valueMin) / (valueMax - valueMin) * (maxScaleTo - minScaleTo);
        return scaledValue;
    }
}
