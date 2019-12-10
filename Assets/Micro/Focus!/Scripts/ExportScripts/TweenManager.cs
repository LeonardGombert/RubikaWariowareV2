using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Focus
{
    public class TweenManager : MonoBehaviour
    {
        public static float LinearTween(float time, float beginning, float change, float duration)
        {
            return time * change / duration + beginning;
        }

        public static float EaseInQuad(float time, float beginning, float change, float duration)
        {
            return change * (time /= duration) * time + beginning;
        }

        public static float EaseOutQuad(float time, float beginning, float change, float duration)
        {
            return -change * (time /= duration) * (time - 2) + beginning;
        }

        public static float EaseInOutQuad(float time, float beginning, float change, float duration)
        {
            if ((time /= duration / 2) < 1)
                return change / 2 * time * time + beginning;
            return -change / 2 * ((--time) * (time - 2) - 1) + beginning;
        }

        public static float EaseInOutQuint(float time, float beginning, float change, float duration)
        {
            if ((time /= duration / 2) < 1)
                return change / 2 * Mathf.Pow(time, 5) + beginning;
            return change / 2 * (Mathf.Pow(time - 2, 5) + 2) + beginning;
        }
        public static float EaseInOutSine(float time, float beginning, float change, float duration)
        {
            return -change / 2 * (Mathf.Cos(Mathf.PI * time / duration) - 1) + beginning;
        }
    }
}
