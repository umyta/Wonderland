using UnityEngine;
using System.Collections;

// Stores animation states for serialization.
public enum PlayerState
{
    Idle,
    Walking,
    UI,
    Tool}
;

public enum ClueState
{
    None = 0,
    Clue1 = 1,
    Clue2 = 2
};

public enum OperatingSystemForController
{
    Windows,
    Mac,
    UnknownSystem}
;
