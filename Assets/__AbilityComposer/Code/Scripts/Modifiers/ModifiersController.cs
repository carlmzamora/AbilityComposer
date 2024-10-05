using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    private Dictionary<Type, List<Modifier>> activeModifiers = new();

    public void ApplyModifier(Modifier modifier, HealthEntity affected)
    {
        modifier.affected = affected.GetComponent<MonoBehaviour>();
        ApplyModifier(modifier);
    }

    private void ApplyModifier(Modifier modifier)
    {
        Type modifierType = modifier.GetType();

        //does this controller already have this type of modifier active?
        List<Modifier> currentModifier = ValidateModifierExistence(modifierType);
        int instanceCount = currentModifier.Count;

        //if there are no instances yet
        if (instanceCount <= 0)
        {
            modifier.Instantiate(modifier.independentStackTimers);
            currentModifier.Add(modifier);
        }
        else
        {
            //if you collect further applications into one instance only
            if (currentModifier[0].allowOnlyOneInstance)
            {
                Modifier firstInstance = currentModifier[0];
                int firstInstanceStackCount = firstInstance.currentStacks;

                //if you can collect infinite stacks
                if (firstInstance.maxStacks == 0)
                {
                    firstInstance.AddStack(firstInstance.independentStackTimers);

                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
                else if (firstInstance.maxStacks == 1) //if you can't collect stacks
                {
                    //refresh only on further applicatons
                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
                else if (firstInstance.maxStacks > 1) //if you can collect stacks up to a limit
                {
                    if (firstInstanceStackCount < firstInstance.maxStacks) //if still below limit
                    {
                        firstInstance.AddStack(firstInstance.independentStackTimers);
                    }

                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
            }
            else //if further applications create multiple instances, eg. infernal blade
            {
                //probably currentModifier.Add(modifier)
            }
        }
    }

    public void RemoveModifier(Modifier modifierToRemove)
    {
        Type type = modifierToRemove.GetType();
        activeModifiers[type].Remove(modifierToRemove); //remove modifier from list of its type

        if (activeModifiers[type].Count <= 0) //if list of its type no longer has any active instance
        {
            activeModifiers.Remove(type); //it is now completely inactive, so should be removed
            //healthUI.icons.gameObject.SetActive(false);
        }
    }

    private List<Modifier> ValidateModifierExistence(Type type)
    {
        if (!activeModifiers.ContainsKey(type))
        {
            List<Modifier> newModifierList = new();
            activeModifiers.Add(type, newModifierList);
            return newModifierList;
        }
        else
            return activeModifiers[type];
    }
}
