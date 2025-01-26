using UnityEngine;

public static class RotationUtil
{
    public static void LookAtDirection(this Transform transform, Vector3 direction)
    {
        var lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lookAngle -= 90.0f;
        if (lookAngle > 360.0f) lookAngle -= 360.0f;
        var lookQuaternion = Quaternion.Euler(new(0, 0, lookAngle));
        transform.rotation = lookQuaternion;
    }
}