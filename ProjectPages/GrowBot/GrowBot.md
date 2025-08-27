## What is GrowBot

In grow bot you play as a abandoned toy

## My Experience

I had loads of fun during this project
We suffered a causality at the beginning of this project in the sense of one programmer going missing.

We ended up with two active programmers and me being one of them had to work real hard during this project,
the people I had to work with during this project had some great communication and where able to step up whenever we needed assistance.

We had to cut a lot of our initial idea as it included upgrades and a ability to swap arms with that did different things,
we also had some vague ideas on some more visual destruction but that also got cut.

All in all this was probably the best game project I was part of mostly due to 
how fun I had developing this game.

### My Takeaways
- Communication is key
- What can be accomplished in a short time with effort
- That sacrifices on what can be done has to be done (When time is limited)

### Code Snippets

#### Health System
In grow bot we only needed health for objects in the level since the only objective
was to destroy as many objects as possible. I therefore made this simple health system
where you can damage things that are the same or lower tier than the attack

```C#

using UnityEngine;

public class Damageable : MonoBehaviour
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

```

### more text

lorem ipsum some shit here is some more code

```C#

using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]protected int health = 10, maxHealth = 10;
    [SerializeField]protected Tier thisTier = Tier.Small;
    [SerializeField]protected Rigidbody thisRigidbody;
    public enum Tier{
        Small,
        Medium,
        Large
    }

}

```