
using UnityEngine;

public class Target : MonoBehaviour
{

    public float hp = 100f;
    //public Enemy1 enemy1;
    //public Enemy enemy;
    
    public enum collisionType { head, body, extremity }
    public collisionType damageType;
    public void TakeDamage(float amount)
    {

        hp -= amount;
        if(hp<=0f)
        {
            Die();
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
