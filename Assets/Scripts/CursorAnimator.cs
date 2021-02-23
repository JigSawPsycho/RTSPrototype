using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class CursorAnimation
{
    [SerializeField] float animSpeed;
    [SerializeField] Texture2D[] animTextures;
}

public class CursorAnimator : MonoBehaviour
{
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] CursorAnimation unitMoveAnim;
    public enum CursorAnims
    {
        MoveUnits,

    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AnimateCursor(CursorAnimation cursorAnim)
    {

    }

    public void SetCursorAnimation(CursorAnims animation)
    {

    }
}
