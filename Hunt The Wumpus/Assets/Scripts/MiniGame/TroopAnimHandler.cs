using MiniGame.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimHandler : MonoBehaviour
{
    public CombatController combatCtr;
    Animator anim;
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }
    void Update()
    {
        if (combatCtr.isMoving == true)
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
        }
        else if (combatCtr.isAttacking == true)
        {
            anim.SetBool("isShooting", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
        }
        else
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
        }
    }
}
