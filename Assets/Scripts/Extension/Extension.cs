using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public static class VectorExtensions
{
    public static Vector3 Add(this Vector3 origin, float x, float y, float z) => new Vector3(origin.x + x, origin.y + y, origin.z + z);
    public static Vector2 Add(this Vector2 origin, float x, float y) => Add(origin, x, y, 0);
    public static Vector3 With(this Vector3 origin, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? origin.x, y ?? origin.y, z ?? origin.z);
    public static Vector2 With(this Vector2 origin, float? x = null, float? y = null) => With(origin, x, y, null);
}

public static class ColorExtensions
{
    /// <summary>
    /// 알파 설정
    /// </summary>
    public static Color SetAlpha(this Color color, float alpha) => new(color.r, color.g, color.b, alpha);

    /// <summary>
    /// 색상 더하기 (0~1 클램프)
    /// </summary>
    public static Color Add(this Color thisColor, Color otherColor) => (thisColor + otherColor).Clamp01();

    /// <summary>
    /// 색상 빼기 (0~1 클램프)
    /// </summary>
    public static Color Subtract(this Color thisColor, Color otherColor) => (thisColor - otherColor).Clamp01();

    /// <summary>
    /// 0~1 클램프
    /// </summary>
    static Color Clamp01(this Color color)
    {
        return new Color
        {
            r = Mathf.Clamp01(color.r),
            g = Mathf.Clamp01(color.g),
            b = Mathf.Clamp01(color.b),
            a = Mathf.Clamp01(color.a)
        };
    }

    /// <summary>
    /// 헥스 문자열로 변환
    /// </summary>
    public static string ToHex(this Color color)
        => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

    /// <summary>    
    /// 헥스 문자열에서 색상으로 변환
    /// <summary>
    public static Color FromHex(this string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        throw new ArgumentException("Invalid hex string", nameof(hex));
    }

    /// <summary>
    /// 두 색상 블렌드
    /// </summary>
    public static Color Blend(this Color color1, Color color2, float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        return new Color(
            color1.r * (1 - ratio) + color2.r * ratio,
            color1.g * (1 - ratio) + color2.g * ratio,
            color1.b * (1 - ratio) + color2.b * ratio,
            color1.a * (1 - ratio) + color2.a * ratio
        );
    }

    /// <summary>
    /// 색상 반전
    /// </summary>
    public static Color Invert(this Color color) => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);

    /// <summary>
    /// 그라데이션 생성
    /// </summary>
    public static Gradient GradientTo(this Color fromColor, Color toColor)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(fromColor, 0f),
                new GradientColorKey(toColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(fromColor.a, 0f),
                new GradientAlphaKey(toColor.a, 1f)
            }
        );
        return gradient;
    }
}

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
    public static string RichColor(this string text, Color color) => text.RichColor(color.ToHex());
    public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
    public static string RichBold(this string text) => $"<b>{text}</b>";
    public static string RichItalic(this string text) => $"<i>{text}</i>";
    public static string RichUnderline(this string text) => $"<u>{text}</u>";
    public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
    public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
    public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";
    public static string RichGradient(this string text, string color1, string color2) => $"<gradient={color1},{color2}>{text}</gradient>";
    public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
    public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";
}

public static class ListExtensions
{
    /// <summary>
    /// 랜덤 요소 가져오기
    /// </summary>    
    public static T GetRandomElement<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new System.ArgumentException("The list is null or empty.");

        int randomIndex = UnityRandom.Range(0, list.Count);
        return list[randomIndex];
    }

    /// <summary>
    /// 리스트 셔플
    /// </summary>
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            int k = rnd.Next(n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }

        return list;
    }

    /// <summary>
    /// 두 요소 교환
    /// </summary>    
    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    /// <summary>
    /// 필터링
    /// </summary>    
    public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)
    {
        List<T> list = new List<T>();
        foreach (T item in source)
        {
            if (predicate(item))
            {
                list.Add(item);
            }
        }
        return list;
    }
}
