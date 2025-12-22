using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

public class ZoneWall : MonoBehaviour
{
    public static ZoneWall main;

    [SerializeField]
    private float startDelay;
    [SerializeField]
    private float stableDelay;
    [SerializeField]
    private int shrinkSteps;
    [SerializeField]
    private float shrinkTime;
    [SerializeField]
    private int startRadius;
    [SerializeField]
    private int endRadius;

    private ZoneState state = ZoneState.Start;
    private float shrinkStarted;
    private float stableStarted;
    private float gameStarted;
    private float shrinkRadius;
    private float currentStepStartRadius;
    private float currentStepEndRadius;
    private int currentStep = 0;

    private CapsuleCollider zoneCollider;

    void Awake() {
        if (main != null) {
            Debug.LogError("Zone wall already exists!");
            Destroy(gameObject);
        }

        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameStarted = Time.time;
        shrinkRadius = ((float)(startRadius - endRadius)) / ((float)shrinkSteps);
        transform.localScale = new Vector3(startRadius, transform.localScale.y, startRadius);
        zoneCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == ZoneState.Start)
        {
            if (Time.time - gameStarted > startDelay)
            {
                state = ZoneState.Shrinking;
                shrinkStarted = Time.time;
            }
        }
        else if (state == ZoneState.Shrinking)
        {
            currentStepStartRadius = startRadius - shrinkRadius * currentStep;
            currentStepEndRadius = startRadius - shrinkRadius * (currentStep + 1);
            float currentRadius = Mathf.Lerp(currentStepStartRadius, currentStepEndRadius, (Time.time - shrinkStarted) / shrinkTime);
            transform.localScale = new Vector3(currentRadius, transform.localScale.y, currentRadius);

            if (Time.time - shrinkStarted >= shrinkTime)
            {
                currentStep++;
                // Debug.Log(currentStep);

                if (currentStep >= shrinkSteps)
                {
                    state = ZoneState.End;
                    // Debug.Log("End");
                }
                else
                {
                    state = ZoneState.Stable;
                    stableStarted = Time.time;
                }
            }
        }
        else if (state == ZoneState.Stable)
        {
            if (Time.time - stableStarted >= stableDelay)
            {
                state = ZoneState.Shrinking;
                shrinkStarted = Time.time;
            }
        }
        else
        {
            // End state, nothing to do
        }
    }

    public bool CheckInside(Transform target) {
        if (zoneCollider.bounds.Contains(target.position)) {
            return true;
        }

        return false;
    }
}

public enum ZoneState
{
    Start,
    Shrinking,
    Stable,
    End
}