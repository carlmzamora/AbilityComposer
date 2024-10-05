using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public abstract class ModifierFactory : ScriptableObject
{
    public string modifierName;
    public Sprite modifierBarIcon;
    public Color modifierBarIconColor;

    public bool allowOnlyOneInstance = true;
    public int maxStacks;
    public bool refreshOnReapply;
    public bool independentStackTimers;

    public abstract Modifier CreateModifier();
}

public class ModifierFactory<T> : ModifierFactory
    where T : Modifier
{
    public List<FloatAttribute> modifierStats = new();

    public override Modifier CreateModifier()
    {
        return new Modifier()
        {
            modifierName = modifierName,
            modifierBarIcon = modifierBarIcon,
            modifierBarIconColor = modifierBarIconColor,

            allowOnlyOneInstance = allowOnlyOneInstance,
            maxStacks = maxStacks,
            refreshOnReapply = refreshOnReapply,
            independentStackTimers = independentStackTimers,

            stats = modifierStats
        };
    }
}

public class Modifier
{
    public string modifierName;
    public Sprite modifierBarIcon;
    public Color modifierBarIconColor;

    public bool allowOnlyOneInstance = true;
    public int maxStacks;
    public bool refreshOnReapply;
    public bool independentStackTimers;

    public List<FloatAttribute> stats;

    public int currentStacks;
    public MonoBehaviour affected;
    private ModifiersController controller;
    protected float totalDuration;
    protected float startTime;

    public virtual void SetAffected(GameObject affected)
    {
        this.affected = affected.GetComponent<MonoBehaviour>();
        this.controller = affected.GetComponent<ModifiersController>();
    }

    public virtual void Instantiate(bool timedStacks)
    {
        AddStack(timedStacks);
    }

    public virtual void AddStack(bool timed)
    {
        currentStacks++;

        //affect UI here

        if (timed)
            affected.StartCoroutine(TimedStackCoroutine());
    }

    public virtual void RemoveStack()
    {
        currentStacks--;

        if (currentStacks <= 0)
            Expire();
    }

    public virtual void RefreshDuration()
    {
        startTime = Time.time;
    }

    public virtual void Expire()
    {
        currentStacks = 0;
        controller.RemoveModifier(this);
    }

    protected virtual IEnumerator TimedStackCoroutine()
    {
        yield return new WaitForSeconds(totalDuration + 0.1f);

        if (currentStacks == 0) yield break;

        currentStacks--;
    }

    public float GetFloatAttribute(ModifierKeys key)
    {
        return stats.Find(attribute => attribute.key == key).value;
    }
}

public class FloatAttribute
{
    public ModifierKeys key;
    public float value;

    public FloatAttribute(ModifierKeys key)
    {
        this.key = key;
    }
}

public enum ModifierKeys
{
    DAMAGE_PER_TICK,
    TICK_COUNT,
    TICK_INTERVAL,

    BUFF_DURATION
}