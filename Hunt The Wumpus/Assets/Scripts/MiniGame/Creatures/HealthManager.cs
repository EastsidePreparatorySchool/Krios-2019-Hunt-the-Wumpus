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
        public int attackDamage = 20;
        public float attackRange = 2.0f;
        public Boolean isEnemy; //Wumpling or Soldier

        // public NestDeathHandler nestDeathHandler;
        // public WumplingDeathHandler wumplingDeathHandler;
        // public SoldierDeathHandler soldierDeathHandler;
        public DeathHandler deathHandler;

        public HealthManager target; //Target object.
        private static HealthManager[] _peers; //TODO get rid of this

        public Image healthBar;
        public Canvas healthBarCanvas; //so I can hide it when the health is full

        private int particleTimeout = 1;
        private float _particleTimeoutTime;

        public static Planet Planet;
        public static int MoneyEarned;


        // Start is called before the first frame update
        void Start()
        {
            deathHandler = gameObject.GetComponentInParent<DeathHandler>();

            // GameObject p = this.transform.parent.gameObject;
            // _peers = (HealthManager[]) FindObjectsOfType(typeof(HealthManager));

            healthBarCanvas.transform.gameObject.SetActive(false);

            RefreshTarget();
            Planet = GameObject.Find("Planet").GetComponent<Planet>();
            MoneyEarned = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //if (particleTimeoutTime <= Time.deltaTime)
            //{
            //    GetComponent<ParticleSystem>().Pause();
            //}

            //If you're dead, you die.
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
                /*
                ResultHandler resultHandler = GetComponent<ResultHandler>();
                if (resultHandler != null)
                {
                    int soldiersRemaining = 0;
                    foreach (HealthManager h in _peers)
                    {
                        if (!h.isEnemy)
                        {
                            soldiersRemaining += 1;
                        }
                    }

                    // Moved this to the EndMiniGame function
                    // Planet.result.moneyCollected = MoneyEarned;

                    resultHandler.EndMiniGame(soldiersRemaining > 0, MoneyEarned);
                }
                // Debug.Log(message);

                else // if troop, remove yourself from the troops stack
                {
                    print("Removing troop from pile");
                    Planet.result.inGameTroops.Remove(gameObject.GetComponent<TroopMeta>());
                }*/

                Destroy(gameObject);
            }

            //refresh the list of alive peers
            // _peers = (HealthManager[]) FindObjectsOfType(
            //     typeof(HealthManager)); //The Soldier and Wumpling objects are supposed to be under the Character object.


            //Refresh your auto-generated target
            // RefreshTarget();
        }

        //Attacks the thing if it's not on this thing's side (Wumpling or Soldier)
        public void Attack(HealthManager thing)
        {
            if (thing.isEnemy == isEnemy)
            {
                //Debug.Log("You can't attack someone on your own side!");
            }
            else if (attackDamage > 0)
            {
                // if (CanAttack(thing.gameObject))
                // {
                GetComponent<ParticleSystem>().Play();
                _particleTimeoutTime = Time.deltaTime + particleTimeout;

                thing.TakeDamage(attackDamage);
                // Debug.Log(this.name + " is Attack()ing " + thing.name);
                // }
            }
        }

        private void TakeDamage(int damage)
        {
            health -= damage;
            healthBarCanvas.transform.gameObject.SetActive(true);

            //set the fill of the health bar
            healthBar.fillAmount = health / (float) maxHealth;
        }

        private void RefreshTarget()
        {
            target = GetNearestEnemy();
        }

        // private Boolean CanAttack(GameObject thing)
        // {
        //     if (DistanceTo(thing) <= attackRange)
        //     {
        //         return true;
        //     }
        //
        //     return false;
        // }
        //
        // public Boolean CanAttackTarget()
        // {
        //     return CanAttack(target.gameObject);
        // }

        // private float DistanceTo(GameObject thing)
        // {
        //     return Vector3.Distance(thing.transform.position, transform.position);
        // }

        public HealthManager GetNearestEnemy()
        {
            float nearestDistanceSqr = int.MaxValue;
            Vector3 myPos = transform.position;
            Vector3 lowerMyPos = new Vector3(myPos.x, 0.1f, myPos.z);
            HealthManager nearestTarget = null;

            Collider[] nearbyThings = new Collider[10];
            String typeLayerMask = isEnemy ? "Troop" : "MiniGameEnemy";
            LayerMask combinedMask = LayerMask.GetMask("MiniGameObstacle", typeLayerMask);

            int size = Physics.OverlapSphereNonAlloc(myPos, attackRange, nearbyThings,
                LayerMask.GetMask(typeLayerMask));

            if (size != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    Vector3 enemyDir = nearbyThings[i].gameObject.transform.position - lowerMyPos;
                    if (Physics.Raycast(lowerMyPos, enemyDir, out RaycastHit hit,
                        attackRange, combinedMask))
                    {
                        Debug.DrawRay(lowerMyPos, enemyDir * hit.distance, Color.cyan, 1f);
                        if (hit.collider.Equals(nearbyThings[i]))
                        {
                            float distSqr = enemyDir.sqrMagnitude;
                            if (distSqr < nearestDistanceSqr)
                            {
                                nearestDistanceSqr = distSqr;
                                nearestTarget = nearbyThings[i].gameObject.GetComponent<HealthManager>();
                            }
                        }
                    }
                }
            }

            print(typeLayerMask+" Size: " + size);
            print("Nearest Target: " + nearestTarget);

            target = nearestTarget;
            return nearestTarget;

            // foreach (HealthManager h in _peers)
            // {
            //     if (h.Equals(null))
            //     {
            //         continue;
            //     }
            //
            //     float distanceToH = DistanceTo(h.gameObject);
            //     if (distanceToH < nearestDistance && h.isEnemy != isEnemy)
            //     {
            //         nearestEnemy = h;
            //         nearestDistance = distanceToH;
            //     }
            // }

            //print("selected " + (nearestEnemy==null? "null":"enemy"));
            //print(_peers.Length);
            // return nearestEnemy == null ? null : nearestEnemy;

            //Debug.Log(this.name + " chose enemy " + nearestEnemy.name);
        }

        // private Boolean EnemiesExist()
        // {
        //     foreach (HealthManager p in _peers)
        //     {
        //         if (p.isEnemy != isEnemy)
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }
    }
}