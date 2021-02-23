using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class VisibilityChange : MonoBehaviour
{
    public static event Action<Unit, bool> ChangedVisibility = delegate { };
    Unit _unit;

    private void Start()
    {
        _unit = GetComponent<Unit>();
    }

    //Called if the unit is no longer being rendered by a camera
    private void OnBecameInvisible()
    {
        ChangedVisibility(_unit, false);
    }

    //Called if the unit started being rendered by a camera
    private void OnBecameVisible()
    {
        ChangedVisibility(_unit, true);
    }
}
