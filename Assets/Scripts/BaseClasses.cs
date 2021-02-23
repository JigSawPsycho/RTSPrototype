using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all units used in the game, bot player and bot controlled.
/// </summary>

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    int health;
    NavMeshAgent agent;
    public static event Action<Unit, bool> Selected = delegate { };

    //
    public Vector3 targetPos;
    float yOffset;
    [SerializeField] float tAreaTime;
    public float distanceBuffer;
    public UnitController unitControl;
    //

    Renderer rend;
    private bool isCulled;
    [SerializeField] bool visibleLastFrame = false;
    int Health
    {
        get { return health; }
        set { health = value; }
    }

    /// <summary>
    /// Get the unit's current world position.
    /// </summary>
    public Vector3 GetPos
    {
        get { return this.gameObject.transform.position; }
    }

    public NavMeshAgent GetAgent
    {
        get { return agent; }
    }

    protected void Start()
    {
        gameObject.layer = 8;
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        yOffset = gameObject.transform.localScale.y;
        targetPos = gameObject.transform.position;
        tAreaTime = 0f;
    }

    /// <summary>
    /// Update in the Unit class controls the stopping of the unit when it reaches its destination.
    /// </summary>
    protected void Update()
    {
        targetPos = GetAgent.destination + new Vector3(0, yOffset, 0);
        if (targetPos != gameObject.transform.position)
        {
            if (Vector3.Distance(gameObject.transform.position, targetPos) < distanceBuffer)
            {
                tAreaTime += 1 * Time.deltaTime;
                if (tAreaTime > 2f)
                {
                    GetAgent.ResetPath();
                    tAreaTime = 0f;
                }
            }
        }
        else
        {
            tAreaTime = 0f;
        }
    }

    /// <summary>
    /// Sets the destination in the NavMeshAgent component for the unit to move to.
    /// </summary>
    /// <param name="pos">The position to be set.</param>
    public void Move(Vector3 pos)
    {
        GetAgent.SetDestination(pos);
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Select(false);
        } else
        Select(true);
    }

    public void Select(bool selectingSingle)
    {
        Selected(this, selectingSingle);
    }

}

class Building
{
    public int Health
    {
        get;
        set;
    }

    public Vector3 GetPos
    {
        get;
        set;
    }
}

class UnitBuilder
{
    public Vector3 RallyPos
    {
        get;
        set;
    }

    public List<Unit> ProductionQueue
    { 
        get;
        set;
    }

    public void QueueBuildUnit(Unit unitToQueue)
    {
        ProductionQueue.Add(unitToQueue);
    }

    public void UnqueueBuildUnit(Unit unitToDequeue)
    {
        ProductionQueue.Remove(unitToDequeue);
    }
}
