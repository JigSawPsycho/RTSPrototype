using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

interface ISelectable
{
    bool Selected { get; set; }

    void Select();

    void Deselect();
}
