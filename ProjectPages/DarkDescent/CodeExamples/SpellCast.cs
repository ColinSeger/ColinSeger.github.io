using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellCast : MonoBehaviour
{
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
    public List<BaseSpells> GetCombo(){//Retrive casting combo
        return castingCombo;
    }
    int SpellsValue(){//Calculates the value of selected spells and should be uniq
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
}
