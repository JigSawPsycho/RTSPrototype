using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Todo:
//I need to have a screen to point ray when you start selecting to be set to the baseMin of the box, and have each frame check if the new position is lower than the start,
//and if so, set the base min's y to that y

/// <summary>
/// The selection of units, assignment to unit lists as well as management of the graphical representation
/// of unit selection is handled by this script
/// </summary>
public class UnitSelector : MonoBehaviour
{
    Box selectionBox;
    Collider[] selections;

    [SerializeField] RectTransform selectionBoxUItransform;

    [SerializeField] bool selectionBox3D;

    [SerializeField] GameObject selectionBox3DPrefab;
    GameObject box3D;

    public static List<Unit> units;
    public static List<Unit> selectedUnits;
    [SerializeField] int selectedUnitCount;
    [SerializeField] List<Unit> onScreenUnits;

    Vector3 startMousePos;

    LayerMask terrainLayerMask;
    LayerMask unitLayerMask;

    private void Awake()
    {
        unitLayerMask = LayerMask.GetMask("Unit");

        units = new List<Unit>();
        selectedUnits = new List<Unit>();
        onScreenUnits = new List<Unit>();
        
        terrainLayerMask = LayerMask.GetMask("Terrain");
        Unit.OnSelect += Unit_Selected;
        VisibilityChange.ChangedVisibility += Unit_VisibilityChanged;
    }

    private void Update()
    {
        selectedUnitCount = selectedUnits.Count;
        if (selectionBox3D)
        {
            SelectionMode3DManager();
        } else
        {
            SelectionMode2DManager();
        }
    }

    void SelectionMode3DManager()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;

            Ray camRay = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit pos;
            Physics.Raycast(camRay, out pos, 100f, terrainLayerMask);

            if (Input.GetMouseButtonDown(0))
            {
                //Started selecting
                selectionBox = new Box();
                selectionBox.baseMin = pos.point;
                startMousePos = pos.point;
                box3D = Instantiate(selectionBox3DPrefab, startMousePos + new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
            }
            
            selectionBox.baseMax = pos.point;
            box3D.transform.localScale = selectionBox.Size / 10;
            box3D.transform.position = new Vector3(selectionBox.Center.x, 0, selectionBox.Center.z);
        }
        else
            if (Input.GetMouseButtonUp(0))
        {
            //Finished Selecting
            box3D.SetActive(false);

            selections = Physics.OverlapBox(selectionBox.Center, selectionBox.Extents, Quaternion.identity, unitLayerMask);
            selectionBox = null;

            //Clear the previous selection of units if the player is holding shift
            bool clearOldSelection = !Input.GetKey(KeyCode.LeftShift);
            if (clearOldSelection)
            {
                DeselectUnits();
            }
            for (int i = 0; i < selections.Length; i++)
            {
                Unit _unit = selections[i].GetComponent<Unit>();
                AddUnitToSelectedUnits(_unit);
            }
        }
    }

    void SelectionMode2DManager()
    {
        Vector3 mousePos = Input.mousePosition;

        if(Input.GetMouseButton(0))
        {
            //Below you'll find a rather extended way of checking what units are currently being selected.
            //We check whether each unit is within the on screen box, selecting appropriately.
            //I'd like to perhaps find a more effecient way of doing this at some point.
            //For now this works.
            if (Input.GetMouseButtonDown(0))
            {
                //Create UI representation of selection box
                selectionBoxUItransform.gameObject.SetActive(true);
                selectionBoxUItransform.position = mousePos;
                startMousePos = mousePos;
            }

            selectionBoxUItransform.sizeDelta = startMousePos - mousePos;
            selectionBoxUItransform.sizeDelta = new Vector3(-selectionBoxUItransform.sizeDelta.x, selectionBoxUItransform.sizeDelta.y);

            if (selectionBoxUItransform.sizeDelta.x < 0)
            {
                selectionBoxUItransform.sizeDelta = new Vector3(-selectionBoxUItransform.sizeDelta.x, selectionBoxUItransform.sizeDelta.y);
                selectionBoxUItransform.pivot = new Vector2(1, selectionBoxUItransform.pivot.y);
            }
            else
            {
                selectionBoxUItransform.pivot = new Vector2(0, selectionBoxUItransform.pivot.y);
            }

            if (selectionBoxUItransform.sizeDelta.y < 0)
            {
                selectionBoxUItransform.sizeDelta = new Vector3(selectionBoxUItransform.sizeDelta.x, -selectionBoxUItransform.sizeDelta.y);
                selectionBoxUItransform.pivot = new Vector2(selectionBoxUItransform.pivot.x, 0);
            }
            else
            {
                selectionBoxUItransform.pivot = new Vector2(selectionBoxUItransform.pivot.x, 1);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            float minXPos = startMousePos.x < mousePos.x ? startMousePos.x : mousePos.x;
            float minYPos = startMousePos.y < mousePos.y ? startMousePos.y : mousePos.y;
            float maxXPos = startMousePos.x > mousePos.x ? startMousePos.x : mousePos.x;
            float maxYPos = startMousePos.y > mousePos.y ? startMousePos.y : mousePos.y;

            //Deselect currently selected units if not holding the Add To Selection
            

            selectionBoxUItransform.gameObject.SetActive(false);
            List<Unit> units = new List<Unit>();
            for(int i = 0; i < onScreenUnits.Count; i++)
            {
                Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(onScreenUnits[i].transform.position);
                if (unitScreenPos.x > minXPos && unitScreenPos.x < maxXPos && unitScreenPos.y > minYPos && unitScreenPos.y < maxYPos)
                {
                    units.Add(onScreenUnits[i]);
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            _ = Physics.Raycast(ray, out hit, 250f);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (!hit.collider.GetComponent<Unit>() || units.Count > 0)
                {
                    DeselectUnits();
                }
            } else
            {
                if (selectedUnits.Contains(hit.collider.GetComponent<Unit>()))
                {
                    DeselectUnit(hit.collider.GetComponent<Unit>());
                }
            }

            for (int k = 0; k < onScreenUnits.Count; k++)
            {
                AddUnitToSelectedUnits(units[k]);
            }
        }
    }

    void DeselectUnit(Unit _unit)
    {
        if(selectedUnits.Remove(_unit))
            _unit.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
    }

    void DeselectUnits()
    {
        for(int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
        selectedUnits.Clear();
    }

    /// <summary>
    /// Adds a unit to the list of selected units and sets the material to represent that the unit has been selected.
    /// </summary>
    /// <param name="unit"></param>
    void AddUnitToSelectedUnits(Unit _unit)
    {
        if (!selectedUnits.Contains(_unit))
        {
            selectedUnits.Add(_unit);
            _unit.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }
    }

    private void Unit_Selected(Unit obj, bool selectSingle)
    {
        if (selectSingle == true)
        {
            DeselectUnits();
        }
        AddUnitToSelectedUnits(obj);
    }

    private void OnDrawGizmos()
    {
        if (selectionBox == null)
        {
            return;
        }
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(selectionBox.Center, selectionBox.Size);
    }
    
    private void Unit_VisibilityChanged(Unit _unit, bool _visible)
    {
        if (!_visible)
        {
            onScreenUnits.Remove(_unit);
        }
        if (_visible)
        {
            onScreenUnits.Add(_unit);
        }
    }
}

public class Box
{
    public Vector3 baseMin, baseMax, startMousePos;

    public Vector3 Center
    {
        get
        {
            Vector3 center = baseMin + (baseMax - baseMin) * 0.5f;
            center.y = (baseMax - baseMin).magnitude * 0.5f;
            return center;
        }
    }

    public Vector3 Size
    {
        get
        {
            return new Vector3(Mathf.Abs(baseMax.x - baseMin.x), (baseMax - baseMin).magnitude, Mathf.Abs(baseMax.z - baseMin.z));
        }
    }

    public Vector3 Extents
    {
        get
        {
            return Size * 0.5f;
        }
    }
}
