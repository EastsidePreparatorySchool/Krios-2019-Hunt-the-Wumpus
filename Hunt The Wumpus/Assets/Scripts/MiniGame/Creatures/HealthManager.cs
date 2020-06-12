using System;
using CommandView;
using MiniGame.Creatures.DeathHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.Creatures
{
    public class HealthManager : MonoBehaviour
    {
        public int health;
        public int maxHealth;

        public DeathHandler deathHandler;

        public Image healthBar;
        public Canvas healthBarCanvas; //so I can hide it when the health is full

        public static Planet Planet;
        //public static int MoneyEarned;


        // Start is called before the first frame update
        void Start()
        {
            deathHandler = gameObject.GetComponentInParent<DeathHandler>();

            // GameObject p = this.transform.parent.gameObject;

            healthBarCanvas.transform.gameObject.SetActive(false);

            //RefreshTarget();
            Planet = GameObject.Find("Planet").GetComponent<Planet>();
            //MoneyEarned = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (health <= 0)
            {
                string message = name + " died.";

                if (deathHandler == null)
                {
                    print("no death handler");
                }
                else
                {
                    deathHandler.Die();
                }
                Destroy(gameObject);
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            healthBarCanvas.transform.gameObject.SetActive(true);

            //set the fill of the health bar
            healthBar.fillAmount = health / (float) maxHealth;
        }
    }
}