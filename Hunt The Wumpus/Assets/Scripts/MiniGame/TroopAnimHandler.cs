using MiniGame.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimHandler : MonoBehaviour
{
    public CombatController combatCtr;
    Animator anim;
    private GameObject gun;
    void Start()
    {
        anim = this.GetComponent<Animator>();
        gun = GameObject.Find("StandardGun -model");
    }
    void Update()
    {
        if (combatCtr.isMoving == true)
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
            gun.transform.position = new Vector3(0, 1, -1);
            gun.transform.rotation = Quaternion.Euler(204, 254, 234);
        }
        else if (combatCtr.isAttacking == true)
        {
            anim.SetBool("isShooting", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
            gun.transform.position = new Vector3(0.5f, 2, 0);
            gun.transform.rotation = Quaternion.Euler(184, 238, 292);
        }
        else
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
            gun.transform.position = new Vector3(0.5f, 1, -1);
            gun.transform.rotation = Quaternion.Euler(143, 280, 236);
        }
    }
}
