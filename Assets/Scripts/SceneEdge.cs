using Unity.Mathematics;
using UnityEngine;

public class SceneEdge: SingletonMono<SceneEdge>
{
    [SerializeField]
    private Transform top;
    [SerializeField]
    private Transform bottom;
    [SerializeField]
    private Transform left;
    [SerializeField]
    private Transform right;

    private float t;
    private float b;
    private float l;
    private float r;
// public******************************************************************************
    public bool IsPositionLegal(Vector3 position)
    {
        if (position.y < b) return false;
        if (position.y > t) return false;
        if (position.x < l) return false;
        if (position.x > r) return false;
        return true;
    }

    public Vector3 ClampPositionIntoLegal(Vector3 position)
    {
        var x = math.clamp(position.x, l, r);
        var y = math.clamp(position.y, b, t);
        return new Vector3(x, y, 0);
    }
// private******************************************************************************
    private void Awake() {
        t = top.position.y;
        b = bottom.position.y;
        l = left.position.x;
        r = right.position.x;
    }
}
