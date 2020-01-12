using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MonkeynAround
{
    public class Trash : MicroMonoBehaviour
    {
        [SerializeField] GameObject cross;
        [SerializeField] GameObject check;

        float timegGoneBy;

        void Update()
        {
            Timer();
        }

        private void Timer()
        {
            timegGoneBy += Time.deltaTime;
            if (timegGoneBy > 1f)
            {
                check.gameObject.SetActive(false);
                cross.gameObject.SetActive(false);
            }
        }

        void Check()
        {
            timegGoneBy = 0;
            check.gameObject.SetActive(true);
        }

        void Cross()
        {
            timegGoneBy = 0;
            cross.gameObject.SetActive(true);
        }
    }
}