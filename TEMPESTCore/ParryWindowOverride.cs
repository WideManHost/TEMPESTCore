using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryWindowOverride : MonoBehaviour
{
    private Enemy _en;

    [Header("Override Settings")]
    [Tooltip("if false this disables all override function from this script, besides listening to parries")]
    public bool enableOverride = true;
    public bool isParryable;
    [Tooltip("Disables isParryable after a parry has been detected")]
    public bool endAfterParry = true;
    [Tooltip("This overrides the grace period after a parry window ends that allows the enemy to still be parried, be careful")]
    public bool disableMercyFrames; 
    [Tooltip("Overrides what transforms hit count as a parry")]
    public bool parryableTransformsOverride;
    [Tooltip("Adds to the already set list of transforms instead of overwriting the existing list")]
    public bool addTransforms;
    public List <Transform> parryableTransforms;

    [Header("Parry Events")]
    public UltrakillEvent onParried;
    public UltrakillEvent onParryStart;
    public UltrakillEvent onParryEnd;

    [Header("Event While Parriable")]
    public bool eventWhileParryable = false;
    public UpdateType updateType;
    public bool oneTime;
    public bool resetOnParryStart;

    private bool _activated;
    [Tooltip("0 = every frame")]
    public float delay = 0;
    private float _timer = 0;
    public UltrakillEvent onParryStay;

    private bool _wasParryable;
    private bool IsMachine => _en.isMachine;
    private bool InternalState => isParryable   && eventWhileParryable && enableOverride;

    private void Awake()
    {
        _en = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (!enableOverride) return;

        //if _en.isMachine is true, Enemy wont send parriable Message
        //we are checking here directly whether or not the enemy was parried (i.e. stopped being parriable) 
        if (IsMachine && isParryable && !_en.parryable && _wasParryable) GotParried();
        _wasParryable = _en.parryable;

        HandleParryFrameOverride(isParryable);


        if (updateType == UpdateType.Update && InternalState)
            WhileParriedEventTick(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate && InternalState)
            WhileParriedEventTick(Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate && InternalState)
            WhileParriedEventTick(Time.deltaTime);
    }
    void HandleParryFrameOverride(bool state)
    {
        _en.parryable = state;
        _en.parryFramesLeft = state ? 999 : 0;
    }
    private void GotParried()
    {
        if (endAfterParry) SetParryState(false);
        Debug.Log("Debug: Parry detected");
        onParried.Invoke(); 
        if (oneTime) _activated = true;
    }
    public void SetParryState(bool state)
    {
        if (isParryable == state) return;
        isParryable = state;
        if (state) onParryStart.Invoke();
        else onParryEnd.Invoke();
        if (state && resetOnParryStart) _activated = false;
        if (delay > 0 && eventWhileParryable) _timer = state ? delay : 1;
        if (disableMercyFrames)
        {
            _en.partiallyParryable = state;
            if (!state && _en.parryables == null) _en.parryables = new List<Transform>();
        }
        if (state && parryableTransformsOverride)
        {
            ParryableTransformsOverride();
        }
    }
    void ParryableTransformsOverride()
    {
        if (_en.parryables == null) _en.parryables = new List<Transform>();
        if (!addTransforms) _en.parryables.Clear();
        foreach (Transform t in parryableTransforms)
        {
            if (t != null && !_en.parryables.Contains(t))
            {
                _en.parryables.Add(t);
            }
        }
    }
    private void WhileParriedEventTick(float delta)
    {
        if (oneTime && _activated) return;

        if (delay > 0)
        {
            _timer -= delta;
            if (_timer <= 0)
            {
                _timer = delay;
                onParryStay.Invoke();
                if (oneTime) _activated = true;
            }
        }
        else
        {
            onParryStay.Invoke();
            if (oneTime) _activated = true;
        }
    }
}