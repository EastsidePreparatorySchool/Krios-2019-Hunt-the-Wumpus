using CommandView;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace MiniGame.Creatures
{
    public class CombatController : MonoBehaviour
    {
        public TroopMeta troopMeta;

        public HealthManager healthmgr;

        private float _timeDiffCounter;
        public float attackInterval = 3.0f;
        public int attackDamage;
        public float attackRange = 2.0f;

        private int particleTimeout = 1;
        private float _particleTimeoutTime;

        public bool isEnemy; //Wumpling or Soldier
        public CombatController target; //Target object.

        public bool doesAttack;
        public bool isMoving;
        public bool isAttacking;
        public bool doesAttackWhileMoving;

        // Start is called before the first frame update
        void Start()
        {
            if (troopMeta != null)
                attackDamage = troopMeta.damage;
        }

        // Update is called once per frame
        void Update()
        {
            RefreshTarget();
            if (CanAttackNow())
            {
                if (target != null)
                {
                    Attack(target);
                    _timeDiffCounter = 0f;
                }
                else
                {
                    isAttacking = false;
                }
            }
        }

        public bool CanAttackNow()
        {
            if (!doesAttack) return false;
            if (_timeDiffCounter < attackInterval)
            {
                _timeDiffCounter += Time.deltaTime;
                return false;
            }

            if (isMoving && !doesAttackWhileMoving)
            {
                return false;
            }

            return true;
        }

        //Attacks the thing if it's not on this thing's side (Wumpling or Soldier)
        public void Attack(CombatController target)
        {
            transform.LookAt(target.gameObject.transform);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            //print(transform.eulerAngles.x + " " + transform.eulerAngles.y + " " + transform.eulerAngles.z);

            /*if (target.isEnemy == isEnemy)
            {
                //Debug.Log("You can't attack someone on your own side!");
            }*/
            if (attackDamage > 0)
            {
                isAttacking = true;

                GetComponent<ParticleSystem>().Play();
                _particleTimeoutTime = Time.deltaTime + particleTimeout;

                target.healthmgr.TakeDamage(attackDamage);
            }
        }

        private void RefreshTarget()
        {
            List<CombatController> enemies = GetNearestEnemies();
            if (enemies.Count == 0)
            {
                target = null;
                return;
            }

            // IOrderedEnumerable<CombatController> combatControllers = enemies.OrderBy(enemy =>
            //     (enemy.gameObject.transform.position - transform.position).sqrMagnitude);
            // foreach (CombatController combatController in combatControllers)
            // {
            //     if (combatController.doesAttack)
            //     {
            //         target = combatController;
            //         break;
            //     }
            //
            //     target = combatController;
            // }

            enemies.Sort((a, b) =>
            {
                if (a.doesAttack != b.doesAttack)
                {
                    //if a attacks and b doesn't, return -1
                    //if b attacks and a doesn't, return 1
                    return (a.doesAttack ? -1 : 1);
                }
            
                //if they both attack or neither attack, closer one goes first
                float aDist = (a.gameObject.transform.position - transform.position).sqrMagnitude;
                float bDist = (b.gameObject.transform.position - transform.position).sqrMagnitude;
                return ((aDist - bDist) < 0 ? -1 : 1);
            });
            
            target = enemies[0];
        }

        public List<CombatController> GetNearestEnemies()
        {
            //float nearestDistanceSqr = int.MaxValue;
            Vector3 myPos = transform.position;
            Vector3 adjustedMyPos = new Vector3(myPos.x, myPos.y + 0.25f, myPos.z);
            //CombatController nearestTarget = null;

            Collider[] nearbyThings = new Collider[10];
            List<CombatController> output = new List<CombatController>();
            String typeLayerMask = isEnemy ? "Troop" : "MiniGameEnemy";
            LayerMask combinedMask = LayerMask.GetMask("MiniGameObstacle", typeLayerMask);

            int size = Physics.OverlapSphereNonAlloc(myPos, attackRange, nearbyThings,
                LayerMask.GetMask(typeLayerMask));

            if (size != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    Vector3 enemyDir = nearbyThings[i].gameObject.transform.position - adjustedMyPos;
                    if (Physics.Raycast(adjustedMyPos, enemyDir, out RaycastHit hit,
                        attackRange, combinedMask))
                    {
                        Debug.DrawRay(adjustedMyPos, enemyDir * hit.distance, isEnemy ? Color.red : Color.cyan,
                            1f);
                        if (hit.collider.Equals(nearbyThings[i]))
                        {
                            output.Add(nearbyThings[i].gameObject.GetComponent<CombatController>());
                            /*float distSqr = enemyDir.sqrMagnitude;
                            if (distSqr < nearestDistanceSqr)
                            {
                                nearestDistanceSqr = distSqr;
                                nearestTarget = nearbyThings[i].gameObject.GetComponent<CombatController>();
                            }*/
                        }

                        // else
                        // {
                        //     print(isEnemy ? "Wumpling" : "Troop" + " : " + hit.collider + " != " + nearbyThings[i]);
                        // }
                    }
                }
            }

            //print(typeLayerMask+" Size: " + size);
            //print("Nearest Target: " + nearestTarget);

            //target = nearestTarget;
            //return nearestTarget;
            return output;
        }
    }
}