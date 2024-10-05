using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Abilities/Buff")]
public class BuffAbility : Ability
{
    public TargettingMode targetCollectionMode = TargettingMode.NO_TARGET;
    public List<ModifierFactory> modifiersAsBuffContents;

    public override Ability CreateAbilityInstance(AbilitiesController abilitiesController)
    {
        BuffAbility abilityInstance = base.CreateAbilityInstance(abilitiesController) as BuffAbility;
        abilityInstance.modifiersAsBuffContents = CreateModifierFactoryInstances(modifiersAsBuffContents);

        return abilityInstance;
    }

    public override void Cast()
    {


        targets.Clear();
    }
}
