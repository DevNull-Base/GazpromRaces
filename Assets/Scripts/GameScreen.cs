using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public struct GameScreen : IEquatable<GameScreen>
{
    public GameObject UI;
    public ScreenTypes type;

    public bool Equals(GameScreen other)
    {
        return Equals(UI, other.UI)  && type == other.type;
    }

    public override bool Equals(object obj)
    {
        return obj is GameScreen other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UI, (int)type);
    }
}