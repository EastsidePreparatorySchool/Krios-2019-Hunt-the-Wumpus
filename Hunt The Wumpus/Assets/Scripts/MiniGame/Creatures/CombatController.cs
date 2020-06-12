using System;
using UnityEngine;

namespace MiniGame.Creatures
{
    public class CombatController : MonoBehaviour
    {
        public HealthManager healthmgr;

        private float _timeDiffCounter;
        public float attackInterval = 3.0f;
        public int attackDamage = 20;
        public float attackRange = 2.0f;
        
        private int particleTimeout = 1;
        private float _particleTimeoutTime;
        
        public bool isEnemy; //Wumpling or Soldier
        public CombatController target; //Target object.

        public bool doesAttack;
        public bool isMoving;
        public bool doesAttackWhileMoving;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            target = GetNearestEnemy();
            if (CanAttackNow())
            {
                if (target != null)
                {
                    Attack(target);
                    _timeDiffCounter = 0f;
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
            if (target.isEnemy == isEnemy)
            {
                //Debug.Log("You can't attack someone on your own side!");
            }
            else if (attackDamage > 0)
            {
                
                GetComponent<ParticleSystem>().Play();
                _particleTimeoutTime = Time.deltaTime + particleTimeout;

                target.healthmgr.TakeDamage(attackDamage);
            }
        }
        
        private void RefreshTarget()
        {
            target = GetNearestEnemy();
        }
        
        public CombatController GetNearestEnemy()
        {
            float nearestDistanceSqr = int.MaxValue;
            Vector3 myPos = transform.position;
            Vector3 lowerMyPos = new Vector3(myPos.x, 0.1f, myPos.z);
            CombatController nearestTarget = null;

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
                                nearestTarget = nearbyThings[i].gameObject.GetComponent<CombatController>();
                            }
                        }
                    }
                }
            }

            print(typeLayerMask+" Size: " + size);
            print("Nearest Target: " + nearestTarget);

            target = nearestTarget;
            return nearestTarget;
        }
    }
}