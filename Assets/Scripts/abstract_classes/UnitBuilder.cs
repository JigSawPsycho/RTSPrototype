using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBuilder : MonoBehaviour, ISelectable
{
    Vector3 rallyPos;
    public Vector3 RallyPos
    {
        get { return rallyPos; }
        protected set { rallyPos = value; }
    }

    [SerializeField]
    GameObject _unitPrefab;

    List<Unit> productionQueue;
    public List<Unit> ProductionQueue
    { 
        get { return productionQueue; }
        private set { productionQueue = value; }
    }

    public bool Selected { get { return true; } set { } }

    float buildProgress;

    private void Start()
    {
        RallyPos = GetComponent<Collider>().ClosestPointOnBounds(transform.position + transform.right * 2f) + transform.right / 2;
        ProductionQueue = new List<Unit>();
    }

    private void Update()
    {
        if(ProductionQueue.Count != 0)
        {
            float unitBuildTime = ProductionQueue[0].BuildTime;

            if (buildProgress >= unitBuildTime)
            {
                BuildUnit();
            } else
            {
                buildProgress += 1 * Time.deltaTime;
            }
        }

        if (!Selected)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            QueueBuildUnit(_unitPrefab.GetComponent<Unit>());
        }
    }

    public void QueueBuildUnit(Unit unitToQueue)
    {
        ProductionQueue.Add(unitToQueue);
    }

    public void UnqueueBuildUnit(Unit unitToDequeue)
    {
        ProductionQueue.Remove(unitToDequeue);
    }

    void BuildUnit()
    {
        Unit unit = Instantiate(_unitPrefab, RallyPos, Quaternion.identity).GetComponent<Unit>();
        unit.unitControl = FindObjectOfType<UnitController>();
        UnitSelector.units.Add(unit);
        buildProgress = 0;
        UnqueueBuildUnit(ProductionQueue[0]);
    }
}
