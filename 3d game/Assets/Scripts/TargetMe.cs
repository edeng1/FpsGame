using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMe : MonoBehaviour, ITargetInterface
{
    PlayerMovement playerM;
    Manager manage;
    public enum collisionType { head, body, extremity }
    public collisionType damageType;

    void Start()
    {
        playerM = GetComponent<PlayerMovement>();

        manage = FindObjectOfType<Manager>();
    }

    public void TakeDamage(float amount)
    {

        playerM.currentHP -= amount;
        if (manage != null)
        {
            manage.TakingDmg();
        }
            if (playerM.currentHP <= 0f)
            {
                playerM.Die();
                //SoundManager.StopSound();
                if (manage != null)
                {
                    manage.GameLost();
                }


            }
        
        /*
        enemy.hp -= amount;
        if (enemy.hp <= 0f)
        {
            enemy.Die();
        }
        */
    }
    void Die()
    {
        print("Died");
    }

}
