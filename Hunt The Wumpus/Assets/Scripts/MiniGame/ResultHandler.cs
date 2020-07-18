﻿using System;
using System.Collections;
using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame
{
    public class ResultHandler : MonoBehaviour
    {
        private GameObject _planet;
        private Planet _planetHandler;
        private MiniGameResult _result;

        private GameMeta _meta;

        // Start is called before the first frame update
        void Start()
        {
            _planet = GameObject.Find("Planet");
            _planetHandler = _planet.GetComponent<Planet>();
            _meta = _planet.GetComponent<GameMeta>();
            _result = _planetHandler.result;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public int NumTroopsLeft()
        {
            return _result.InGameTroops.Count;
        }


        public void EarnMoney(int money)
        {
            _result.MoneyCollected += money;
        }

        public void RemoveTroop(TroopMeta troopMeta)
        {
            _result.InGameTroops.Remove(troopMeta);
        }

        public void EndMiniGame(bool didWin = true)
        {
            _planetHandler.didSomething = true;
            _result.DidWin = didWin;

            StartCoroutine(FadeOutAndSwitch());

            _planetHandler.backFromMiniGame = true;
        }

        private IEnumerator FadeOutAndSwitch()
        {
            _planet.GetComponent<MusicController>().FadeOut();
            yield return new WaitUntil(() => Math.Abs(AudioListener.volume) < 0.001);
            SceneManager.LoadScene(0);

            _meta.UpdateGameStateWithResult();
        }
    }
}