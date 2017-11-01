﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMotion : MonoBehaviour {

    [Tooltip("Time in seconds the elevator will take to travel from the ring to the ship or vice versa.")]
    public float TravelTime = 10f;

    [HideInInspector]
    public Transform CableTop;          // Position at the top the of the cable that the elevator should travel to (the ring)

    [HideInInspector]
    public Vector3 CableBotton;         // Position at the bottom of the cable that the elevator should travel to (the ship)

    private target currentTarget;       // Enum holding the current target (or destination) of the elevator
    private Vector3 velocity;           // Velocity of the object when starting movement
    private Vector3 targetPos;          // Position of the destination of the elevator
    private bool targetSet;             // Flag set when both targets have been set from ElevatorCables.cs
    private enum target                 // Enum of target types
    {
        Top,
        Bottom
    };

    // Use this for initialization
    void Start () {
        currentTarget = target.Bottom;
        velocity = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        // If the targets have not been created and set yet, return. Otherwise we will get null errors
        if (!targetSet)
        {
            return;
        }

        // Move towards the current target using SmoothDamp
        switch (currentTarget)
        {
            case target.Top:
                targetPos = CableTop.localPosition;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, TravelTime);
                break;
            case target.Bottom:
                targetPos = CableBotton;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, TravelTime);
                break;
            default:
                targetPos = CableTop.localPosition;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, TravelTime);
                break;
        }

        // If we are close enough to the current target, switch to the other one
        if (Vector3.Distance(transform.localPosition, targetPos) < 0.00003f)
        {
            UpdateTarget();
        }
    }

    /// <summary>
    /// Change target for the elevator to move towards
    /// </summary>
    public void UpdateTarget()
    {
        targetSet = true;

        // Switch the target position
        switch (currentTarget)
        {
            case target.Top:
                currentTarget = target.Bottom;
                break;
            case target.Bottom:
                currentTarget = target.Top;
                break;
            default:
                currentTarget = target.Bottom;
                break;
        }
    }
}
