using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.MonkeynAround
{
    public class ItemGenerator : MicroMonoBehaviour
    {
        [SerializeField] List<GameObject> difficulty1List = new List<GameObject>();
        [SerializeField] List<GameObject> difficulty2List = new List<GameObject>();
        [SerializeField] List<GameObject> difficulty3List = new List<GameObject>();
        int beatsPassed;
        public enum currentDifficulty { difficulty1, difficulty2, difficulty3 };
        [SerializeField] currentDifficulty currDifficulty;

        private void Start()
        {
            Macro.StartGame();
        }
        protected override void OnGameStart()
        {
            base.OnGameStart();
            Macro.DisplayActionVerb("SLAM !", 3);
            if (Macro.Difficulty == 1) currDifficulty = currentDifficulty.difficulty1;
            if (Macro.Difficulty == 2) currDifficulty = currentDifficulty.difficulty2;
            if (Macro.Difficulty == 3) currDifficulty = currentDifficulty.difficulty3;
            Macro.StartTimer(15);
        }

        protected override void OnBeat()
        {
            base.OnBeat();
            beatsPassed++;

            switch(currDifficulty)
            {
                case currentDifficulty.difficulty1:
                    if (beatsPassed >= 3)
                    {
                        Instantiate(difficulty1List[Random.Range(0, difficulty1List.Count)], this.transform.position, Quaternion.identity);
                        beatsPassed = 0;
                    }
                    break;
                case currentDifficulty.difficulty2:
                    if (beatsPassed >= 2)
                    {
                        Instantiate(difficulty2List[Random.Range(0, difficulty2List.Count)], this.transform.position, Quaternion.identity);
                        beatsPassed = 0;
                    }
                    break;
                case currentDifficulty.difficulty3:
                    if (beatsPassed >= 1)
                    {
                        Instantiate(difficulty3List[Random.Range(0, difficulty3List.Count)], this.transform.position, Quaternion.identity);
                        beatsPassed = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnTimerEnd()
        {
            base.OnTimerEnd();
            Macro.Win();
            Macro.EndGame();
        }
    }
}
