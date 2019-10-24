using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMethod
{
    /// <summary>
    /// Permet d'obtenir l'épaisseur d'un point sur un lineRenderer
    /// </summary>
    /// <param name="_pointIndex"></param>
    /// <param name="_lineRenderer"></param>
    /// <returns></returns>
    public static float GetLineWidthAtPoint(int _pointIndex, LineRenderer _lineRenderer)
    {
        float width = 0;

        Vector3 endPoint = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
        Vector3 startPoint = _lineRenderer.GetPosition(0);
        Vector3 targetPoint = _lineRenderer.GetPosition(_pointIndex);

        float distance = Vector2.Distance(startPoint, endPoint);
        float targetDistance = Vector2.Distance(startPoint, targetPoint);

        float curveTargetValue = targetDistance / distance;

        width = _lineRenderer.widthCurve.Evaluate(curveTargetValue);

        return width;
    }

    /// <summary>
    /// Permet de connaitre la presque égalité de deux vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;

        if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
        if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;

        return equal;
    }

    /// <summary>
    /// Permet de connaitre la presque égalité de deux vectors 2D
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static bool AlmostEqual2D(Vector2 v1, Vector2 v2, float precision)
    {
        bool equal = true;

        if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;

        return equal;
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// Permet de connaitre la presque égalité de deux vector sur un seul axe
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="precision"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static bool AlmostEqualOnOneAxis(Vector3 v1, Vector3 v2, float precision, Axis axis)
    {
        bool equal = true;

        switch (axis)
        {
            case Axis.X:
                if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
                break;
            case Axis.Y:
                if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
                break;
            case Axis.Z:
                if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;
                break;
            default:
                if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
                break;
        }

        return equal;
    }

    public static int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;

        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }

        return layerNumber - 1;
    }

    /// <summary>
    /// Method used to return a number clamped between a min and max value, based on where a variable interpolates between her min and max value.
    /// </summary>
    /// <param name="minValueReturn">The minimum of the returned value.</param>
    /// <param name="maxValueReturn">The maximum of the returned value.</param>
    /// <param name="minValueToCheck">The minimum of the value we're checking.</param>
    /// <param name="maxValueToCheck">The maximum of the value we're checking.</param>
    /// <param name="valueToCheck">The value we're checking.</param>
    public static float Interpolate(float minValueReturn, float maxValueReturn, float minValueToCheck, float maxValueToCheck, float valueToCheck)
    {
        return Mathf.Lerp(minValueReturn, maxValueReturn, Mathf.InverseLerp(minValueToCheck, maxValueToCheck, valueToCheck));
    }

    /// <summary>
    /// Method used to return a number clamped between a min and max value, based on where a variable interpolates between her min and max value.
    /// </summary>
    /// <param name="minValueReturn">The minimum of the returned value.</param>
    /// <param name="maxValueReturn">The maximum of the returned value.</param>
    /// <param name="minValueToCheck">The minimum of the value we're checking.</param>
    /// <param name="maxValueToCheck">The maximum of the value we're checking.</param>
    /// <param name="valueToCheck">The value we're checking.</param>
    public static Vector2 InterpolateVector2(Vector2 minValueReturn, Vector2 maxValueReturn, Vector2 minValueToCheck, Vector2 maxValueToCheck, Vector2 valueToCheck)
    {
        float x = Mathf.Lerp(minValueReturn.x, maxValueReturn.x, Mathf.InverseLerp(minValueToCheck.x, maxValueToCheck.x, valueToCheck.x));
        float y = Mathf.Lerp(minValueReturn.y, maxValueReturn.y, Mathf.InverseLerp(minValueToCheck.y, maxValueToCheck.y, valueToCheck.y));
        return new Vector2(x, y);
    }

    public static Vector3 MultiplyTwoVectors(Vector3 a, Vector3 b)
    {
        float x = a.x * b.x;
        float y = a.y * b.y;
        float z = a.z * b.z;

        return new Vector3(x, y, z);
    }

    public static Color GetColorWithAlpha(Color color, float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        Color newColor = new Color(color.r, color.g, color.b, alpha);
        return newColor;
    }
}

