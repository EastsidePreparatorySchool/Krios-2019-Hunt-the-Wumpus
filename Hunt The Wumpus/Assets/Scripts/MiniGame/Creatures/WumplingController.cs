using MiniGame.Creatures;
using UnityEngine;
using UnityEngine.AI;

namespace MiniGame
{
    public class WumplingController : MonoBehaviour
    {
        //private HealthManager _healthmgr;
        private CombatController _combatController;
        public NavMeshAgent navMeshAgent;

        public Vector3 nearestTarget; 

        public float searchRange = 20;
        public float speed = 3;

        // Start is called before the first frame update
        void Start()
        {
            _combatController = GetComponent<CombatController>();
        }

        // Update is called once per frame
        void Update()
        {
            //Use the HealthManager's GetNearestEnemy() method to find the nearest Soldier and attack it
            float nearestDistanceSqr = int.MaxValue;
            Vector3 myPos = transform.position;
            nearestTarget = myPos;

            Collider[] nearbyThings = new Collider[30];
            int size = Physics.OverlapSphereNonAlloc(myPos, searchRange, nearbyThings, LayerMask.GetMask("Troop"));

            if (size != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    float distSqr = (nearbyThings[i].gameObject.transform.position - myPos).sqrMagnitude;
                    if (distSqr < nearestDistanceSqr)
                    {
                        nearestDistanceSqr = distSqr;
                        nearestTarget = nearbyThings[i].transform.position;
                    }
                }
            }

            if ((myPos -nearestTarget).sqrMagnitude > _combatController.attackRange * _combatController.attackRange)
            {
                navMeshAgent.SetDestination(nearestTarget);
            }
        }
    }
}