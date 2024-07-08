using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public static class Extensions
{
    #region ListExt

    public static T GetRandomItem<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

    public static void Shuffle<T>(this IList<T> list)
    {
        for (var i = list.Count - 1; i > 1; i--)
        {
            var j = Random.Range(0, i + 1);
            var value = list[j];
            list[j] = list[i];
            list[i] = value;
        }
    }

    public static bool IsEmpty<T>(this List<T> list) => list.Count == 0;

    public static T First<T>(this List<T> list) => list[0];

    public static T Last<T>(this List<T> list) => list[^1];

    public static void Off(this List<GameObject> list)
    {
        foreach (GameObject obj in list)
            obj.SetActive(false);
    }

    public static void On(this List<GameObject> list)
    {
        foreach (GameObject obj in list)
            obj.SetActive(true);
    }

    public static int GetEqualsCount<T>(this List<T> list, T obj)
    {
        int index = 0;

        foreach (var item in list)
            if (item.Equals(obj))
                index++;

        if (index > 0)
            return index;
        else return -1;
    }

    public static void All<T>(this List<T> list, Action<T> action)
    {
        foreach (var item in list)
            action(item);
    }

    #endregion

    #region UIExt

    // полезен если надо обновить отрисовку макета, когда на элементе один из Layout-ов
    public static void RefreshLayout(this RectTransform transform, bool hard = false)
    {
        if (hard)
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform); // Принудительно перестраиваем макет
        else
            LayoutRebuilder.MarkLayoutForRebuild(transform); // Помечаем макет для перестроения
    }

    //полезен при работе с DoTween, если надо сбросить цвет перед Анимацией
    public static void SetColorAlpha(this MaskableGraphic targetGraphic, float newAlpha)
    {
        var backColor = targetGraphic.color;
        backColor.a = newAlpha;
        targetGraphic.color = backColor;
    }


    #endregion

    #region VectorExt
    public static Vector3 WithX(this Vector3 value, float x)
    {
        value.x = x;
        return value;
    }

    public static Vector3 WithY(this Vector3 value, float y)
    {
        value.y = y;
        return value;
    }

    public static Vector3 WithZ(this Vector3 value, float z)
    {
        value.z = z;
        return value;
    }

    public static Vector3 AddX(this Vector3 value, float x)
    {
        value.x += x;
        return value;
    }

    public static Vector3 AddY(this Vector3 value, float y)
    {
        value.y += y;
        return value;
    }

    public static Vector3 AddZ(this Vector3 value, float z)
    {
        value.z += z;
        return value;
    }

    public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.x, vector.z);

    public static Vector3 X0Z(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2
        {
            x = vector3.x,
            y = vector3.z
        };
    }

    public static Vector3 ToVector3(this Vector2 vector2, float y = 0)
    {
        return new Vector3
        {
            x = vector2.x,
            y = y,
            z = vector2.y
        };
    }

    public static Vector3Int FloorToVector3Int(this Vector3 vector)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(vector.x),
            y = Mathf.FloorToInt(vector.x),
            z = Mathf.FloorToInt(vector.x)
        };
    }
    public static Vector3Int FloorToVector3Int(this Vector3 vector, float offset)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(vector.x + offset),
            y = Mathf.FloorToInt(vector.y + offset),
            z = Mathf.FloorToInt(vector.z + offset),
        };
    }


    #endregion

    #region TransformExt

    public static void DestroyChildren(this Transform transform)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            Object.Destroy(transform.GetChild(i).gameObject);
    }

    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    #endregion
    
    #region QuaternionExt

    public static Quaternion AngVelToDeriv(Quaternion Current, Vector3 AngVel) {
        var Spin = new Quaternion(AngVel.x, AngVel.y, AngVel.z, 0f);
        var Result = Spin * Current;
        return new Quaternion(0.5f * Result.x, 0.5f * Result.y, 0.5f * Result.z, 0.5f * Result.w);
    } 

    public static Vector3 DerivToAngVel(Quaternion Current, Quaternion Deriv) {
        var Result = Deriv * Quaternion.Inverse(Current);
        return new Vector3(2f * Result.x, 2f * Result.y, 2f * Result.z);
    }

    public static Quaternion IntegrateRotation(Quaternion Rotation, Vector3 AngularVelocity, float DeltaTime) {
        if (DeltaTime < Mathf.Epsilon) return Rotation;
        var Deriv = AngVelToDeriv(Rotation, AngularVelocity);
        var Pred = new Vector4(
            Rotation.x + Deriv.x * DeltaTime,
            Rotation.y + Deriv.y * DeltaTime,
            Rotation.z + Deriv.z * DeltaTime,
            Rotation.w + Deriv.w * DeltaTime
        ).normalized;
        return new Quaternion(Pred.x, Pred.y, Pred.z, Pred.w);
    }
	
    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time) {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;
		
        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;		
		
        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    #endregion
    
    #region Builder

    public static T With<T>(this T self, Action<T> set)
    {
        set.Invoke(self);
        return self;
    }

    public static T With<T>(this T self, Action<T> apply, Func<bool> when)
    {
        if (when())
        {
            apply(self);
        }

        return self;
    }

    public static T With<T>(this T self, Action<T> apply, bool when)
    {
        if (when)
        {
            apply(self);
        }

        return self;
    }

    #endregion
}
