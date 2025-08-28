using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]protected float maxHealth = 10f;
    [SerializeField]protected Tier thisTier = Tier.Small;
    [SerializeField]AnimationCurve returnRate;
    protected float health = 10f;
    public enum Tier{
        ExtraSmall,
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

    public virtual void DealDamage(float damage, Tier tier)
    {
        if(tier >= thisTier)
        {
            health -= damage; 

            if(health <= 0)
            {
                Death(returnRate, tier);
            }
        }
    }
    public virtual void Death(AnimationCurve curve, Tier tier)
    {
        Destroy(this.gameObject);
    }
}
