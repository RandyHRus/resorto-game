using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    bool AllowMovement { get; }

    void StartState(object[] args);

    bool TryEndState();

    void Execute();
}
