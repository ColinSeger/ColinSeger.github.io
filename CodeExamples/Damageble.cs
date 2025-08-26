using UnityEngine;

public class Damageble : MonoBehaviour
{
    [SerializeField]protected int health = 10, maxHealth = 10;
    [SerializeField]protected Tier thisTier = Tier.Small;
    [SerializeField]protected Rigidbody thisRigidbody;
    public enum Tier{
        Small,
        Medium,
        Large
    }

    void Awake()
    {
        health = maxHealth;
    }
    public Tier GetTier()
    {
        return thisTier;
    }

    public void DealDamage(int damage, Tier tier)
    {
        if(tier >= thisTier)
        {
            health -= damage; 

            if(health <= 0)
            {
                Death();
            }           
        }
    }
    public virtual void Death()
    {
        Destroy(this.gameObject);
    }
}
