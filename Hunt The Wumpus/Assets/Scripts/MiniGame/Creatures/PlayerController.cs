using System;
using MiniGame.Creatures;
using UnityEngine;
using UnityEngine.AI;

namespace MiniGame
{
    public class PlayerController : MonoBehaviour
    {
        public PointerController pointer;
        public bool atPointer;

        public NavMeshAgent navMeshAgent;

        public float speed = 5;
        public float minDistFromPointer = 2;
        public float maxDistFromPointer = 6;
        public float curDisFromPointer;

        private CombatController _combatController;

        //public HealthManager Healthmgr { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            _combatController = GetComponent<CombatController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (pointer && !atPointer)
            {
                if (pointer.attackMove && !_combatController.doesAttackWhileMoving && _combatController.target != null) //must stop to attack
                {
                    StopMoving();
                }
                else
                {
                    Vector3 pointerLoc = pointer.GetPosition();
                    curDisFromPointer = Vector3.Distance(pointerLoc, transform.position);

                    if (curDisFromPointer > minDistFromPointer)
                    {
                        MoveTo(pointerLoc);
                    }
                    else
                    {
                        //once you get close enough to the pointer, you are considered "there" and stop moving to it
                        ArriveAtPointer();
                        //pointer = null;
                    }
                }
            }
        }

        private void MoveTo(Vector3 pointerLoc)
        {
            navMeshAgent.SetDestination(pointerLoc);
            _combatController.isMoving = true;
        }

        private void ArriveAtPointer()
        {
            pointer.followers--;
            PointerController next = pointer.next;
            if (next != null)
            {
                pointer = next;
            }
            else
            {
                atPointer = true;
                StopMoving();
            }
        }

        private void StopMoving()
        {
            navMeshAgent.ResetPath();
            _combatController.isMoving = false;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject otherGameObject = other.gameObject;
            PlayerController otherController = otherGameObject.GetComponent<PlayerController>();
            if (otherController == null)
            {
                return;
            }

            if (otherController.pointer == pointer && otherController.atPointer && curDisFromPointer < maxDistFromPointer)
            {
                ArriveAtPointer();
            }
        }
    }
}