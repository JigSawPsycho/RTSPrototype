using System;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Base class for all units used in the game, bot player and bot controlled.
/// </summary>

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    public static event Action<Unit, bool> OnSelect;

    Renderer rend;

    private bool isCulled;

    Vector3 targetPos;

    float _yOffset;

    [SerializeField] bool visibleLastFrame = false;

    [SerializeField] 
    float tAreaTime;

    [SerializeField]
    float distanceBuffer;

    [SerializeField]
    float _buildTime;
    /// <summary>
    /// Time required for this unit to be built by an unit producing building
    /// </summary>
    public float BuildTime { get { return _buildTime; } private set { _buildTime = value; } }

    public UnitController unitControl;

    int health;
    /// <summary>
    /// Current health of this unit
    /// </summary>
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

    NavMeshAgent agent;
    public NavMeshAgent GetAgent
    {
        get { return agent; }
    }

    protected void Start()
    {
        gameObject.layer = 8;
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        _yOffset = gameObject.transform.localScale.y;
        targetPos = gameObject.transform.position;
        tAreaTime = 0f;
    }

    /// <summary>
    /// Update in the Unit class controls the stopping of the unit when it reaches its destination.
    /// </summary>
    protected void Update()
    {
        targetPos = GetAgent.destination + new Vector3(0, _yOffset, 0);
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
        OnSelect?.Invoke(this, selectingSingle);
    }

}
