using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalPauseManager : MonoBehaviour
{
    public static Action<bool> OnPauseStateChanged;
    bool pauseState = false;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnPauseButtonPressed += FireOffPauseEvent;
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnPauseButtonPressed -= FireOffPauseEvent;
    }

    private void OnApplicationFocus(bool focus)
    {
        pauseState = !focus;
        if (!focus) OnPauseStateChanged?.Invoke(pauseState);
    }

    private void FireOffPauseEvent()
    {
        pauseState = !pauseState;
        OnPauseStateChanged?.Invoke(pauseState);
    }
}
