using System.Collections;
using UnityEngine;

public class StructureDamage : Damageble
{
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
    
    private void Awake(){
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.playOnAwake = false;
        }

        // Automatically get the AudioMixerGroup from AudioSettingsManager if available
        if (MixerForDestroyableObjects.Instance != null && MixerForDestroyableObjects.Instance.SFXMixerGroup != null && audioSource != null)
        {
            audioSource.outputAudioMixerGroup = MixerForDestroyableObjects.Instance.SFXMixerGroup;
        }
    }

    private void Start(){
        scoreScript = FindAnyObjectByType<ExpScript>();

        #if UNITY_EDITOR
        if(!scoreScript){
            Debug.Log("Could not find Score");
        }
        #endif
    }
    
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
}