using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

//This script acts as a control center for units. Unit movement as well as selection is handled here.
//This is only for the beginning of the prototype, I will separate movement and selection of units
//into seperate scripts soon.

public class UnitController : MonoBehaviour
{
    public GameObject unitPrefab;
    public GameObject selectionBoxPrefab;

    public Vector3 spawnPos;
    bool selecting;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            BuildUnit();
        }
        if(Input.GetMouseButtonDown(1))
        {
            MoveUnits();
        }
    }

    void MoveUnits()
    {
        if (UnitSelector.selectedUnits.Count == 0)
            return;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit pos;

        if (Physics.Raycast(camRay, out pos))
        {
            for (int i = 0; i < UnitSelector.selectedUnits.Count; i++)
            {
                if (Vector3.Distance(UnitSelector.selectedUnits[i].GetPos, pos.point) > 0.5f)
                {
                    UnitSelector.selectedUnits[i].Move(pos.point);
                }
            }
        }
    }

    void BuildUnit()
    {
        Unit unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity).GetComponent<Unit>();
        unit.unitControl = this;
        UnitSelector.units.Add(unit);
        Debug.Log(unit.gameObject);
    }
}
