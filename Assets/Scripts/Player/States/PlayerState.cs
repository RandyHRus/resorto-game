using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PlayerState : ScriptableObject, IStateMachineState
{
    public event ChangeState OnChangeState;
    public event EndState OnEndState;

    protected static Transform Player { get; private set; }
    protected static ProgressBar ProgressBar { get; private set; } //Progress bar is shared across all states

    public void InvokeChangeState(Type stateType, object[] args = null)
    {
        OnChangeState?.Invoke(stateType, args);
    }

    public void InvokeEndState()
    {
        OnEndState?.Invoke();
    }

    public virtual void Initialize()
    {
        if (ProgressBar == null)
        {
            Canvas progressBarCanvas = ResourceManager.Instance.IndicatorsCanvas;
            ProgressBar = new ProgressBar(progressBarCanvas.transform);
            ProgressBar.Show(false);
        }

        if (Player == null)
        {
            Player = ResourceManager.Instance.Player;
        }
    }

    protected void MoveProgressBarAbovePlayer()
    {
        ProgressBar.ObjectTransform.position = (new Vector2(Player.position.x, Player.position.y + 1.5f));
    }

    public abstract bool AllowMovement { get; }

    public abstract bool AllowMouseDirectionChange { get; }

    public abstract CameraMode CameraMode { get; }

    public abstract void StartState(object[] args);

    public abstract void EndState();

    public abstract void Execute();

    public virtual void LateExecute() { }

    public virtual void OnCancelButtonPressed()
    {
        InvokeEndState();
    }
}

public enum CameraMode
{
    Follow,
    Drag
}