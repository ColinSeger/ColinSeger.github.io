
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
public enum AttackAreaType : byte{
    Normal,
    Aoe,
    Distant
}
public class SpellsBase : MonoBehaviour
{
    [SerializeField]AttackAreaType attackArea;

    [Header("Audio")]
    [SerializeField]protected EventReference attackSound;

    [Header("Base Settings")]
    [SerializeField]protected Transform castFrom;
    [SerializeField]protected Health.DamageType damageType;
    [SerializeField]protected float damage;
    [SerializeField]protected float baseCooldown = 2;
    [SerializeField]protected float manaCost = 1;
    protected float cooldown = 0;
    protected bool first = true;
    protected ManaSystem manaSystem;
    public virtual bool Attack(){//Overide this function to call your spell
        float theTime = Time.time;
        //if(cooldown > Time.time) cooldown = 0;
        if((cooldown + baseCooldown) > theTime)return false;
        if(!manaSystem){
            manaSystem = FindFirstObjectByType<ManaSystem>();
        }else{
            manaSystem.UseMana(manaCost);            
        }
        return true;
    }
    protected bool CheckCooldown(){//Should only be called when trying to attack
        float theTime = Time.time;
        //Debug.Log("Cooldown "+ cooldown +" vs " + theTime);
        //if(cooldown > Time.time) cooldown = 0;
        if(!manaSystem) manaSystem = FindFirstObjectByType<ManaSystem>();
        if(manaSystem.CurrentMana <= 0) return false;
        if((cooldown + baseCooldown) > theTime){
            return false;
        }
        cooldown = theTime;
        return true;
    }
    public Transform GetCastLocation(){//Get location
        return castFrom;
    }
    public void SetCastLocation(Transform newLocation){//Set location
        castFrom = newLocation;
    }
    public Health.DamageType GetDamageType(){//Get damage type
        return damageType;
    }
    public void SetDamageType(Health.DamageType newDamageType){//Set damage type
        damageType = newDamageType;
    }
    public float GetDamage(){//Get damage
        return damage;
    }
    public void SetDamage(float newDamage){//Set damage
        damage = newDamage;
    }
    public float GetCooldown(){
        return baseCooldown;
    }
    public AttackAreaType GetAttackArea(){
        return attackArea;
    }
    ///
    // Returns mouse world position or defaults to Vector.Zero
    ///
    protected Vector3 GetMouseWorldPosition(){
    #if UNITY_ANDROID
        return FakeCursorController.GetFakeCursorWorldPosition();
    #else
        if (!Camera.main){
            Debug.LogError("Main Camera is missing in the scene!");
            return Vector3.zero;
        }

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        Debug.LogWarning("Colin fucked up the from screen raycast");
        return Vector3.zero;
    #endif
    }
    public void ResetThis(){
        cooldown = -baseCooldown;
        first = true;
    }
}
