using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    Vector2 cursorPos;
    public Camera cursor_Camera;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        cursorPos = cursor_Camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }
}
