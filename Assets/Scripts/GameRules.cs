using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ToggleMouse(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleMouse(!Cursor.visible);
        }
    }

    private void ToggleMouse(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
    }
}
