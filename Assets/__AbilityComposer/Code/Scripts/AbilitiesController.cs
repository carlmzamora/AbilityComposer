using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    enum AbilityState
    {
        READY,
        ON_COOLDOWN
    }

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject AOEReticle;
    private Ability abilityInstance;
    private float cooldownTimeProgress;
    private AbilityState state = AbilityState.READY;

    private bool inTargettingMode = false;
    private TargettingMode targettingMode = TargettingMode.NO_TARGET;
    private float targetRadius;
    private Ability abilityInTargetting;

    public void Start()
    {
        abilityInstance = ability.CreateAbilityInstance(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Trigger();
        }

        HandleAbilityInstanceState();
        HandleTargetting();
    }

    #region ACTIVATION HANDLING

    public void Trigger()
    {
        if (abilityInstance == null)
        {
            Debug.LogError("Attempting to cast with an unassigned ability slot!");
            return;
        }

        switch (state)
        {
            case AbilityState.READY:
                abilityInstance.Activate();
                break;
        }
    }    

    public void BeginCooldown()
    {
        state = AbilityState.ON_COOLDOWN;
        cooldownTimeProgress = abilityInstance.cooldown;
    }

    private void HandleAbilityInstanceState()
    {
        switch (state)
        {
            case AbilityState.ON_COOLDOWN:
                if (cooldownTimeProgress > 0)
                {
                    cooldownTimeProgress -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.READY;
                }
                break;
        }
    }

    #endregion

    #region TARGET HANDLING

    public void StartTargettingMode(Ability abilityInTargetting, TargettingMode targettingMode, float targetRadius)
    {
        this.abilityInTargetting = abilityInTargetting;
        inTargettingMode = true;
        this.targettingMode = targettingMode;
        this.targetRadius = targetRadius;
        AOEReticle.transform.localScale = new Vector3(targetRadius * 2, 1, targetRadius * 2);
        gizmoRadius = targetRadius;
    }

    private void HandleTargetting()
    {
        if (!inTargettingMode) return;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (targettingMode == TargettingMode.POINT_TARGET)
        {
            //each frame, wait for a mouse click on valid terrain
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    Vector3 intersectionPointAlongRay = hitInfo.point;
                    inTargettingMode = false;
                    abilityInTargetting.ConfirmTargetting(intersectionPointAlongRay);
                    BeginCooldown();
                }
            }
        }
        else if (targettingMode == TargettingMode.UNIT_TARGET)
        {
            List<HealthEntity> targets = new();

            //each frame, wait for a mouse click on a game object
            //add that game object to list of targets
            if (Physics.Raycast(mouseRay, out RaycastHit hit))// && Array.Exists(colliders, collider => collider == hit.collider)) //check multiple colliders
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(hit.collider.TryGetComponent(out HealthEntity entity))
                    {
                        targets.Add(entity);
                        inTargettingMode = false;
                        abilityInTargetting.ConfirmTargetting(targets);
                        BeginCooldown();
                    }
                    else
                    {
                        Debug.Log("No target!");
                    }
                }
            }
        }
        else if (targettingMode == TargettingMode.RADIUS_TARGET)
        {
            List<HealthEntity> targets = new();

            //each frame, wait for a mouse click on valid terrain
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                Vector3 intersectionPointAlongRay = hitInfo.point;
                gizmoCenter = hitInfo.point;
                //display reticle here
                AOEReticle.SetActive(true);
                AOEReticle.transform.position = intersectionPointAlongRay + Vector3.up * 0.001f;

                if (Input.GetMouseButtonDown(0))
                {
                    //add game objects that fall within {targetRadius} of {targetPoint} to list of targets
                    Collider[] radiusTargets = Physics.OverlapSphere(intersectionPointAlongRay, targetRadius);

                    foreach(Collider detected in radiusTargets)
                    {
                        if(detected.TryGetComponent(out HealthEntity entity))
                        {
                            targets.Add(entity);
                        }
                    }

                    inTargettingMode = false;
                    abilityInTargetting.ConfirmTargetting(targets);
                    AOEReticle.SetActive(false);
                    BeginCooldown();
                }
            }
            else
            {
                AOEReticle.SetActive(false);
            }
        }
    }

    private Vector3 gizmoCenter;
    private float gizmoRadius;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(gizmoCenter, gizmoRadius);
    }

    #endregion
}
