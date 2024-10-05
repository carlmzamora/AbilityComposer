using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTModifierFactory : ModifierFactory<DOTModifier>
{
    private void Awake()
    {
        modifierStats.Add(new FloatAttribute(ModifierKeys.DAMAGE_PER_TICK));
        modifierStats.Add(new FloatAttribute(ModifierKeys.TICK_COUNT));
        modifierStats.Add(new FloatAttribute(ModifierKeys.TICK_INTERVAL));
    }
}

public class DOTModifier : Modifier
{
    public float tickInterval;

    private HealthEntity healthEntity;
    private float lastTickTime;
    private float timeBetweenRefreshAndLastTick;

    private float currentDamagePerTick;
    private int currentTickCount;
    private float currentTickInterval;

    public override void Instantiate(bool timedStacks)
    {
        // Retrieve parameters from the ParameterManager
        currentDamagePerTick = GetFloatAttribute(ModifierKeys.DAMAGE_PER_TICK);
        currentTickCount = (int)GetFloatAttribute(ModifierKeys.TICK_COUNT);
        currentTickInterval = GetFloatAttribute(ModifierKeys.TICK_INTERVAL);

        // Calculate total duration based on tick count and tick interval
        totalDuration = currentTickCount * currentTickInterval;

        healthEntity = affected.GetComponent<HealthEntity>();
        affected.StartCoroutine(DOTCoroutine());

        base.Instantiate(timedStacks);
    }

    private IEnumerator DOTCoroutine()
    {
        startTime = Time.time;

        while (Time.time - startTime < totalDuration) // while duration elapsed is less than totalDuration
        {
            yield return new WaitForSeconds(currentTickInterval);

            // Apply damage per tick
            healthEntity.TakeDamage(currentDamagePerTick * currentStacks);

            lastTickTime = Time.time;
        }

        Expire();
    }

    public override void RefreshDuration()
    {
        float timeOfRefresh = Time.time; // Set current time

        if (lastTickTime > 0)
        {
            timeBetweenRefreshAndLastTick = lastTickTime - timeOfRefresh;
        }
        else // If this modifier just started and there is no first tick yet
        {
            timeBetweenRefreshAndLastTick = startTime - timeOfRefresh;
        }

        // Add the time between the tick and refresh time so that small portion is still included
        startTime = Time.time + timeBetweenRefreshAndLastTick;
    }
}