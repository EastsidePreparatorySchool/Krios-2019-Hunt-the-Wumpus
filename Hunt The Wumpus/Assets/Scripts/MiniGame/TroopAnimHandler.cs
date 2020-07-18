using MiniGame.Creatures;
using UnityEngine;

namespace MiniGame
{
    public class TroopAnimHandler : MonoBehaviour
    {
        public CombatController combatCtr;
        private Animator _anim;
        private static readonly int IsShooting = Animator.StringToHash("isShooting");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsIdle = Animator.StringToHash("isIdle");

        void Start()
        {
            _anim = this.GetComponent<Animator>();
        }

        void Update()
        {
            if (combatCtr.isMoving)
            {
                _anim.SetBool(IsShooting, false);
                _anim.SetBool(IsRunning, true);
                _anim.SetBool(IsIdle, false);
            }
            else if (combatCtr.isAttacking)
            {
                _anim.SetBool(IsShooting, true);
                _anim.SetBool(IsRunning, false);
                _anim.SetBool(IsIdle, false);
            }
            else
            {
                _anim.SetBool(IsShooting, false);
                _anim.SetBool(IsRunning, false);
                _anim.SetBool(IsIdle, true);
            }
        }
    }
}