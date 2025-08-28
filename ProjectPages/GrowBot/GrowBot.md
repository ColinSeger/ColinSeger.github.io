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

```

### Structure Damage

Here is some of the code of how "Damageable" is used on objects.

<!-- There is some minor things I cut out from the example (Awake) -->

Some things are missing from this example since they show up later.

```C#
/*This code is within a class inheriting from Damageable*/
[SerializeField] int xpToGive;
[SerializeField] private float _timeToGive;
[SerializeField] Animation damageAnimation;
[SerializeField] Animation deathAnimation;
[SerializeField] ParticleSystem damageParticles;
[SerializeField] ParticleSystem destructionParticles;
[SerializeField] public float knockBackForce = 5f;
    
private AudioSource audioSource; 
private bool isDying = false; // Flag to prevent interactions during death
private float _damageLerp;
private ExpScript scoreScript;

public override void DealDamage(float damage, Tier tier){
    base.DealDamage(damage, tier);

    // Check if the object is already in the process of dying
    if (isDying) return;
    StartCoroutine(Animated());
    if(damageAnimation){
        damageAnimation.Play();           
    }
}

public override void Death(AnimationCurve curve, Tier tier)
{
    // If the object is already dying, skip further execution
    if (isDying) return;

    isDying = true; // Set the flag to indicate death process has started
    
    scoreScript?.AddScore(xpToGive);

    StartCoroutine(DeathAnimation());
    if(audioSource){
        audioSource.Play(); 
    }
    
    // Increase the timer via the Instance of TimerScript
    if (TimerScript.Instance){
        TimerScript.Instance.IncreaseTimer(_timeToGive * curve.Evaluate((int)tier));
    }

    StartCoroutine(DeactivateAfterSound());
}

private IEnumerator DeactivateAfterSound()
{
    // Wait for the sound to finish playing if a death sound is assigned
    Collider[] colliders = GetComponents<Collider>();
    foreach (Collider collider in colliders)
    {
        if (collider.gameObject.GetComponent<Rigidbody>() == null)
        {
            collider.enabled = false;
        }
    }

    yield return new WaitForSeconds(1);

    gameObject.SetActive(false);
}

IEnumerator DeathAnimation()
{
    Vector3 savedScale = transform.localScale;
    float duration = 0.5f;
    float time = 0.0f;

    while (time < duration)
    {
        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);

        transform.localScale = Vector3.Lerp(savedScale, Vector3.zero, t);

        yield return null;
    }

    transform.localScale = Vector3.zero;
}

```

### Peak Code

Here is some of the worst code I probably have ever written but it works surprisingly well

```C#

IEnumerator Animated()
{
    Vector3 savedScale = this.transform.localScale;
    Vector3 minSize = new Vector3(0.8f, 0.8f, 0.8f);
    if (!this.gameObject.activeInHierarchy) yield return null;
    _damageLerp = 1;

    float speed = 0.3f;
    float amount = 0.05f;
    
    // Scale down the object with a smooth transition
    for (float i = 1; i > 0; i -= speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp -= amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
    }

    // Scale back up to original size with a smooth transition
    for (float i = 0; i < 1; i += speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp += amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
        //Debug.Log(_test);
    }
    // Scale down the object with a smooth transition
    for (float i = 1; i > 0; i -= speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp -= amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
    }

    // Scale back up to original size with a smooth transition
    for (float i = 0; i < 1; i += speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp += amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
        //Debug.Log(_test);
    }
    // Scale down the object with a smooth transition
    for (float i = 1; i > 0; i -= speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp -= amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
    }

    // Scale back up to original size with a smooth transition
    for (float i = 0; i < 1; i += speed)
    {
        yield return new WaitForSeconds(0.001f);
        _damageLerp += amount;
        this.transform.localScale = Vector3.Lerp(minSize, savedScale, _damageLerp);
        //Debug.Log(_test);
    }
    yield return null;
}

```