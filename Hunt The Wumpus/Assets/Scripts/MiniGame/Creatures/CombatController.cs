using UnityEngine;

namespace MiniGame.Creatures
{
    public class CombatController : MonoBehaviour
    {
        public HealthManager healthmgr;

        private float _timeDiffCounter;
        public float attackInterval = 3.0f;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (_timeDiffCounter < attackInterval)
            {
                _timeDiffCounter += Time.deltaTime;
            }
            else // can make an attack anytime
            {
                HealthManager target = healthmgr.GetNearestEnemy();
                if (target != null)
                {
                    healthmgr.Attack(target);
                    _timeDiffCounter = 0f;
                }
            }

            // if (_timeDiffCounter <= Time.time && healthmgr.target != null) 
            //     // Expensive comparison is NEEDED because ".Equals()" is not a function of null, which is what healthmgr.target is in danger of being
            // {
            //     if (healthmgr.CanAttackTarget())
            //     {
            //         healthmgr.AttackTarget();
            //         _timeDiffCounter += attackInterval;
            //     }
            //     else
            //     {
            //         // Debug.Log(this.name + " had no targets in range.");
            //     }
            // }
        }
    }
}