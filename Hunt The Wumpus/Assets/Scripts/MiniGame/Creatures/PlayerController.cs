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

        public HealthManager Healthmgr { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Healthmgr = GetComponent<HealthManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (pointer && !atPointer)
            {
                Vector3 pointerLoc = pointer.GetPosition();
                curDisFromPointer = Vector3.Distance(pointerLoc, transform.position);

                if (curDisFromPointer > minDistFromPointer)
                {
                    // Transform myTrans;
                    // (myTrans = transform).LookAt(pointerLoc);
                    // myTrans.eulerAngles = new Vector3(0, myTrans.rotation.eulerAngles.y, 0);
                    //
                    // transform.Translate(Vector3.forward * (Time.deltaTime * speed));
                    navMeshAgent.SetDestination(pointerLoc);
                }
                else
                {
                    //once you get close enough to the pointer, you are considered "there" and stop moving to it
                    StopMoving();
                    //pointer = null;
                }
            }
        }

        private void StopMoving()
        {
            pointer.followers--;
            atPointer = true;
            navMeshAgent.ResetPath();
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
                StopMoving();
            }
        }
    }
}