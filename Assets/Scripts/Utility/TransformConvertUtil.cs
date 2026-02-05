using UnityEngine;

public static class TransformConvertUtil
{
    public static Vector3 ToVector3(float[] pos)
    {
        if (pos == null || pos.Length < 3)
            return Vector3.zero;

        return new Vector3(pos[0], pos[1], pos[2]);
    }

    public static Quaternion ToQuaternion(float[] rotEuler)
    {
        if (rotEuler == null || rotEuler.Length < 3)
            return Quaternion.identity;

        return Quaternion.Euler(rotEuler[0], rotEuler[1], rotEuler[2]);
    }
}
