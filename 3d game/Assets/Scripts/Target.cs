
using UnityEngine;

public class Target : MonoBehaviour, ITargetInterface
{

    public float hp = 100f;
    
    public Enemy1 enemy;
    public enum collisionType { head, body, extremity }
    public collisionType damageType;

    void Start()
    {
       // enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(float amount)
    {

        enemy.HP -= amount;
        if(enemy.HP <= 0f)
        {
            enemy.Die();
        }
       
    }
   


}
