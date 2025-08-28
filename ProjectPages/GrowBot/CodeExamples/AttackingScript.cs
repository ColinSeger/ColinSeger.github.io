using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackingScript : MonoBehaviour
{
    [Header("Variables")]
    Damageable _damageble;
    [SerializeField]bool _canAttack = false, selectedTarget = false;
    [SerializeField]float attackDelay = 1f, sizeMultiplier = 1f;
    float damage = 1f;

    [Space(5)][Header("Body")]
    [SerializeField]float[] sizeBaseDamage;
    [SerializeField]Damageable.Tier _currentTier = Damageable.Tier.Small;
    [SerializeField]GrowthController growth;
    [SerializeField]Animator attackAnimation;
    [SerializeField]Transform followArea, follower, secondaryArea;
    bool _isAttacking = false;

    [Space(5)]
    [Header("VFX")]
    [SerializeField] ParticleSpawner hitParticleSpawner;
    [SerializeField] ParticleSpawner fireParticleSpawner;
    [SerializeField] ScreenShakeController screenShakeController;
    [SerializeField] float hitShakeAmount = 0.1f;
    [SerializeField] float hitDurationAmount = 0.1f;
    [SerializeField] float hitCooldownAmount = 0.5f;

    [SerializeField]private AudioSource attackSound;

    private InputSystem_Actions inputActions;
    void Awake(){
        inputActions = new InputSystem_Actions();

        inputActions.Player.Attack.performed += Attack;
    }

    void Attack(InputAction.CallbackContext context){
        
    }

    void Update()
    {

        if(!selectedTarget)
        {
            follower.position = followArea.position;
        }
        else
        {
            follower.position = secondaryArea.position;
        }
        _currentTier = (Damageable.Tier)growth.GetCurrentPhase();
        if(Input.GetKeyDown(KeyCode.Mouse0) && !_isAttacking)
        {
            selectedTarget = false;
            attackAnimation.SetFloat("punchVal", 0);
            attackAnimation.SetTrigger("doPunchL");
            _isAttacking = true;
            StartCoroutine(AttackCooldown(attackDelay));
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1) && !_isAttacking)
        {
            selectedTarget = true;
            attackAnimation.SetFloat("punchVal", 1);
            attackAnimation.SetTrigger("doPunchR");
            _isAttacking = true;
            StartCoroutine(AttackCooldown(attackDelay));
        }
    }

    void OnTriggerStay(Collider collision)
    {
        //Debug.Log("Can attack");
        if (_canAttack)
        {
            //_currentTier = (Damageble.Tier)growth.GetCurrentPhase();
            _damageble = collision.gameObject.GetComponent<Damageable>();
            if (_damageble != null)
            {
                if (_damageble.GetTier() > _currentTier) return;

                if (screenShakeController != null)
                {
                    screenShakeController.Shake(hitCooldownAmount, hitDurationAmount, hitShakeAmount);
                }

                if (attackSound != null)
                {
                    attackSound.Play();
                }

                DamageCalculate();

                hitParticleSpawner?.CreateParticle(collision, follower.transform.position, growth.growthPhases[growth.GetCurrentPhase()].playerScale, selectedTarget, false);        
                
                if (fireParticleSpawner && growth.GetCurrentPhase() > 1)
                {
                    if (collision.CompareTag("canBurn"))
                    {
                        fireParticleSpawner.CreateParticle(collision, follower.transform.position, growth.growthPhases[growth.GetCurrentPhase()].playerScale, selectedTarget, true); //This last bool checks if its fire or not
                    }
                }
                _damageble?.DealDamage(damage, _damageble.GetTier());

                // Apply knockback force specific to StructureDamage objects
                StructureDamage structureDamage = collision.gameObject.GetComponent<StructureDamage>();
                if (structureDamage != null)
                {
                    Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    if (targetRigidbody != null)
                    {
                        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                        targetRigidbody.AddForce(knockbackDirection * structureDamage.knockBackForce, ForceMode.Impulse);
                    }
                }

                _damageble = null; // To clear the variable
                //_isAttacking = false;
            }
            // Start coroutine to reset attack state at the end of the frame
            StartCoroutine(EndOfFrame());
        }
    }

    void DamageCalculate()
    {
        float baseDamage = sizeBaseDamage[(int)_currentTier];
        baseDamage *= sizeMultiplier;
        damage = baseDamage;
    }
    public void SetSizeMultiplier(float multiplier)
    {
        sizeMultiplier = multiplier;
    }
    public void SetAttack()
    {
        sizeMultiplier = 1;
        _canAttack = true;
        _isAttacking = false;
        //StartCoroutine(AttackCooldown(attackDelay));
    }
    public void HeavyAttack()
    {
        sizeMultiplier = 2;
        _canAttack = true;
        _isAttacking = false;
        //StartCoroutine(AttackCooldown(attackDelay));
    }
    IEnumerator AttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        //attackAnimation.SetBool("Attacking", false);
        _isAttacking = false;
        _canAttack = false;
        yield return null;
    }
    IEnumerator EndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        _canAttack = false;
        //_isAttacking = false;
        yield return null;
    }
}
