## What is Dark Descent


In Dark Descent you play as a templar stuck in limbo. With magic


I enjoyed working on this project even though I did not enjoy the concept.

My job was to make the AI, at least at first that was my assigned task,

We had some issues with the magic system in the beginning and due to the one in charge of magic
being both hard to reach and having some issues understanding the requests made by designers 
the decision was made for me to overlook the implementation of the magic system.

When I got time to take a look at the spells system I made sure that the designers had their 
requirements set in stone and started work on a minor scalability issue I could see with the spell system

### My Takeaways
- Reinforced that communication is key
- Fmod is a pain if not setup properly
- Talking outside your discipline is critical


#### Base Spell

This is the code that is the base spell that you can use to create new spells to be added to
the game

```C#

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
protected Vector3 GetMouseWorldPosition(){// Returns mouse world position or defaults to Vector.Zero
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
    Debug.LogWarning("Colin messed up the from screen raycast");
    return Vector3.zero;
#endif
}
public void ResetThis(){
    cooldown = -baseCooldown;
    first = true;
}

```

#### Spell Caster
Here is the code for how the spells where selected and then casted.

You might notice how te usage of the new unity input system is not great and 
that can be explained by that it was the first I had used it and therefore
I made some poor desicions on how to use it.

Nowadays I would not have the input be registered in the update loop and instead just tie it to
the input event

```C#

public Action<int> spellIndex;
public Action<int, float> spellCooldown;
[SerializeField]List<BaseSpells> castingCombo = new List<BaseSpells>(3);
[SerializeField]List<SkillsBase> spells = new List<SkillsBase>();
[SerializeField]Transform castLocation;
[SerializeField]List<BaseSpells> lockedSpells = new List<BaseSpells>(3);
[SerializeField]Animator animator;
#region Controls
//private InputSystem_Actions inputActions;
InputAction spell1;
InputAction spell2;
InputAction spell3;
InputAction attack;
InputAction combine;
#endregion

void Awake(){
    spell1 = InputSystem.actions.FindAction("AddSpell1");
    spell2 = InputSystem.actions.FindAction("AddSpell2");
    spell3 = InputSystem.actions.FindAction("AddSpell3");
    attack = InputSystem.actions.FindAction("Attack");
    combine = InputSystem.actions.FindAction("Combine");

    foreach (var item in spells){
        item?.ResetSpell();
    }
}
void Update()
{
    if (Time.timeScale == 0f) return;

    if (castingCombo.Count > 3){
        castingCombo.Remove(BaseSpells.Empty);
    }
    #if UNITY_ANDROID
    if(attack.WasReleasedThisFrame()){
        CastSpell();
    }
    #else
    if(attack.WasPressedThisFrame()){
        CastSpell();
    }
    #endif
    if(spell1.WasPressedThisFrame())AddSpell(BaseSpells.Holy);
    if(spell2.WasPressedThisFrame())AddSpell(BaseSpells.Flame);
    if(spell3.WasPressedThisFrame())AddSpell(BaseSpells.Order);
    if(combine.WasPressedThisFrame())castingCombo.Clear();
    
}
void CastSpell(){
    for (int i = castingCombo.Count; i < 3; i++)
    {
        castingCombo.Add(BaseSpells.Empty);
    }
    foreach (SkillsBase skill in spells){
        if(!skill.Compare(castingCombo))continue;

        skill.SetLocation(castLocation);
        if(skill.Attack()){
            switch (skill.GetSpellAttackArea())
            {
                case AttackAreaType.Normal:
                    animator.SetTrigger("attack");
                break;
                case AttackAreaType.Aoe:
                    animator.SetTrigger("aoe");
                break;
                case AttackAreaType.Distant:
                    animator.SetTrigger("dist");
                break;
                default:
                    Debug.Log("Missing");
                break;
            }
            spellCooldown.Invoke(SpellsValue(), skill.GetCooldown());
        }
    }
}
void AddSpell(BaseSpells spell){//Add spell to casting combo
    if(lockedSpells.Contains(spell)) return;
    if(!castingCombo.Contains(spell)){
        castingCombo.Add(spell);
    }else{
        castingCombo.Remove(spell);
    }
    spellIndex?.Invoke(SpellsValue());
}
public List<BaseSpells> GetCombo(){//Retrieve casting combo
    return castingCombo;
}
int SpellsValue(){//Calculates the value of selected spells and should be unique
    int result = 0;
    int added = -1;
    foreach(BaseSpells spell in castingCombo){
        result += (int)spell;
        if((int)spell > 0){
            added+=1;
        }
    }
    return result + added;
}
public void AddLockedSpell(BaseSpells spell){
    if(lockedSpells.Contains(spell)) return;
    lockedSpells.Add(spell);
}
public void RemoveLockedSpell(BaseSpells spell){
    if(!lockedSpells.Contains(spell)) return;
    lockedSpells.Remove(spell);
}
```