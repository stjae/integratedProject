using UnityEngine;

public static class Layer
{
    static int _hitSphere = LayerMask.NameToLayer("HitSphere");
    static int _traceFace = LayerMask.NameToLayer("TraceFace");
    static int _player = LayerMask.NameToLayer("Player");

    public static int HitSphere { get { return _hitSphere; } }
    public static int TraceFace { get { return _traceFace; } }
    public static int Player { get { return _player; } }
}