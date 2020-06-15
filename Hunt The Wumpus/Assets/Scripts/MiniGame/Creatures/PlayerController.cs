using System;
using System.Collections.Generic;
using MiniGame.Creatures;
using UnityEngine;
using UnityEngine.AI;

namespace MiniGame
{
    public class PlayerController : MonoBehaviour
    {
        private Queue<PointerController> pointers = new Queue<PointerController>();
        public bool atPointer;

        public NavMeshAgent navMeshAgent;

        public float speed = 5;
        public float minDistFromPointer = 15;
        public float maxDistFromPointer = 15;
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
            PointerController pointer = PeekPointer();
            if (pointer && !atPointer)
            {
                if (pointer.attackMove && !_combatController.doesAttackWhileMoving && _combatController.target != null) //must stop to attack
                {
                    StopMoving();
                }
                else
                {
                    Vector3 pointerLoc = pointer.GetPosition();
                    curDisFromPointer = (pointerLoc - transform.position).sqrMagnitude;

                    if (curDisFromPointer > minDistFromPointer * minDistFromPointer)
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

        public PointerController PeekPointer()
        {
            if (pointers.Count == 0)
            {
                return null;
            }
            return pointers.Peek();
        }
        public PointerController DequeuePointer()
        {
            if (pointers.Count == 0)
            {
                return null;
            }
            return pointers.Dequeue();
        }
        public void AddPointer(PointerController item)
        {
            pointers.Enqueue(item);
        }
        public void RemovePointers()
        {
            foreach (PointerController pointer in pointers)
            {
                pointer.followers--;
            }
            pointers.Clear();
        }

        private void MoveTo(Vector3 pointerLoc)
        {
            navMeshAgent.SetDestination(pointerLoc);
            _combatController.isMoving = true;
        }

        private void ArriveAtPointer()
        {
            PointerController pointer = DequeuePointer();
            if (pointer != null)
            {
                pointer.followers--;
            }
            PointerController next = PeekPointer();
            if (next == null)
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

            if (otherController.PeekPointer() == PeekPointer() && 
                otherController.atPointer && 
                curDisFromPointer < maxDistFromPointer)
            {
                ArriveAtPointer();
            }
        }
    }
}