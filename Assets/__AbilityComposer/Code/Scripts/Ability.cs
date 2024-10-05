using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public Sprite modifierBarIcon;
    public float targetRadius;

    //TODO: create subclasses Active and Passive?
    public float cooldown;

    public AbilitiesController abilitiesController;
    public HealthEntity owner;
    public int level = 1;

    public TargettingMode targettingMode = TargettingMode.NO_TARGET;
    protected List<HealthEntity> targets = new();
    protected bool targettingStarted = false;
    protected EntityGroup[] validTargets;
    protected Vector3 targetPoint;

    public void Activate()
    {
        if (targettingMode == TargettingMode.NO_TARGET)
        {
            Cast();
            return;
        }
        else if (targettingMode == TargettingMode.SELF_TARGET)
        {
            targets.Add(owner.GetComponent<HealthEntity>());
            Cast();
            return;
        }

        abilitiesController.StartTargettingMode(this, targettingMode, targetRadius);
    }

    public void ConfirmTargetting(Vector3 targetPoint)
    {
        this.targetPoint = targetPoint;

        if (targettingMode == TargettingMode.POINT_TARGET && targetPoint == null)
        {
            Debug.LogError("Attempting to cast point-targetted ability without target point!");
            return;
        }

        Cast();
    }

    public void ConfirmTargetting(List<HealthEntity> acquiredTargets)
    {
        targets = acquiredTargets;

        if ((targettingMode == TargettingMode.UNIT_TARGET || targettingMode == TargettingMode.RADIUS_TARGET)
            && targetPoint == null)
        {
            Debug.LogError("Attempting to cast point-targetted ability without target point!");
            return;
        }

        Cast();
    }

    public virtual void Cast()
    {
        if (targettingMode == TargettingMode.POINT_TARGET)
        {
            Debug.Log("Ola! at " + targetPoint);
        }
        else if (targettingMode == TargettingMode.UNIT_TARGET)
        {
            Debug.Log(targets[0].gameObject.name);
        }
        else if (targettingMode == TargettingMode.RADIUS_TARGET)
        {
            foreach (HealthEntity entity in targets)
            {
                Debug.Log(entity.gameObject.name);
            }
        }

        targets.Clear();
    }

    public virtual Ability CreateAbilityInstance(AbilitiesController abilitiesController)
    {
        Ability instance = Instantiate(this);
        return instance;
    }

    public ModifierFactory CreateModifierFactoryInstance(ModifierFactory factory)
    {
        return Instantiate(factory);
    }

    public List<ModifierFactory> CreateModifierFactoryInstances(List<ModifierFactory> multipleFactories)
    {
        List<ModifierFactory> instantiatedFactories = new();

        foreach(ModifierFactory factory in multipleFactories)
        {
            instantiatedFactories.Add(Instantiate(factory));
        }

        return instantiatedFactories;
    }
}

public enum TargettingMode
{
    NO_TARGET,
    SELF_TARGET,
    POINT_TARGET,
    UNIT_TARGET,
    RADIUS_TARGET
    //TO DO: VECTOR_TARGET
}

public enum EntityGroup
{
    ALLIES,
    ENEMIES
}