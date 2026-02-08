## Trailer

<iframe width="560" height="315" src="https://www.youtube-nocookie.com/embed/Xiy50ClH2AI?si=Nyp1ovyr3f7qGiRp" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>

## What is Dark Descent


In Dark Descent you play as a templar stuck in limbo. You meet Cain the son of Adam and Eve 
who guides you on how to use your spells. During your time in limbo you are challanged with wave after wave
of demons. The final wave you face is a epic boss fight against Cain who betrays you with the objective of 
escaping hell using your daggerblade.

What where our goals with this project? Well it was to create a game that in some way tied into the "Daggerblade" and make sure it could "technically"
run on a phone, in our team we put quite some time on making sure our game actually could run on phone to some extent.

## My Experience

I enjoyed working on this project even though I did not enjoy the concept.

My job was to make the AI, at least at first that was my assigned task,

We had some issues with the magic system in the beginning and due to the one in charge of magic
being both hard to reach and having some issues understanding the requests made by designers 
the decision was made for me to overlook the implementation of the magic system.

When I got time to take a look at the spells system I made sure that the designers had their 
requirements set in stone and started work on a minor scalability issue I could see with the spell system

I collaborated with artists programmers and designers in how to optimize our game for phone

### My Takeaways
- Reinforced that communication is key
- Fmod is a pain if not setup properly
- Talking outside your discipline is critical


#### Systems I worked on
- Spell UI
- Spell Casting
- Enemies AI
- Enemy Spawning
- Enemy Types
- Phone Optimization

#### Base Spell

This is the code that is the base spell that you can use to create new spells to be added to
the game

<pre>
    <code class="language-csharp">
    
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
    //if(cooldown &gt Time.time) cooldown = 0;
    if((cooldown + baseCooldown) &gt theTime)return false;
    if(!manaSystem){
        manaSystem = FindFirstObjectByType&ltManaSystem&gt();
    }else{
        manaSystem.UseMana(manaCost);            
    }
    return true;
}
protected bool CheckCooldown(){//Should only be called when trying to attack
    float theTime = Time.time;
    //Debug.Log("Cooldown "+ cooldown +" vs " + theTime);
    //if(cooldown &gt Time.time) cooldown = 0;
    if(!manaSystem) manaSystem = FindFirstObjectByType&ltManaSystem&gt();
    if(manaSystem.CurrentMana <= 0) return false;
    if((cooldown + baseCooldown) &gt theTime){
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
    </code>
</pre>
<!--CODE-->
#### Spell Caster
Here is the code for how the spells where selected and then casted.

You might notice how te usage of the new unity input system is not great and 
that can be explained by that it was the first I had used it and therefore
I made some poor decisions on how to use it.

Nowadays I would not have the input be registered in the update loop and instead just tie it to
the input event

<pre>
    <code class="language-csharp">
    

public Action&lt;int&gt; spellIndex;
public Action&lt;int, float&gt; spellCooldown;
[SerializeField]List&lt;BaseSpells&gt; castingCombo = new List&lt;BaseSpells&gt;(3);
[SerializeField]List&lt;SkillsBase&gt spells = new List&lt;SkillsBase&gt;();
[SerializeField]Transform castLocation;
[SerializeField]List&lt;BaseSpells&gt; lockedSpells = new List&lt;BaseSpells&gt;(3);
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

    if (castingCombo.Count &gt 3){
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
    for (int i = castingCombo.Count; i &gt; 3; i++)
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
public List&gt;BaseSpells&gt GetCombo(){//Retrieve casting combo
    return castingCombo;
}
int SpellsValue(){//Calculates the value of selected spells and should be unique
    int result = 0;
    int added = -1;
    foreach(BaseSpells spell in castingCombo){
        result += (int)spell;
        if((int)spell &gt 0){
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
</code>
</pre>
