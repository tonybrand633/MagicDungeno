using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurveType 
{
    Linear,
    In,
    Out,
    InOut,
    SinIn,
    SinOut,
    Sin
}

public class BazierCache 
{
    public string curve;
    public float mod;
}

public class ParseBazier
{
    public static Dictionary<string, BazierCache> cache;

    public static float ParseCurve(float u, string curve)
    {
        if (cache == null)
        {
            cache = new Dictionary<string, BazierCache>();
        }
        float u2 = u;

        if (!cache.ContainsKey(curve))
        {
            InitDic(curve);
        }

        u2 = ParseToU(u, cache[curve]);
        return u2;
    }

    private static void InitDic(string curve)
    {
        BazierCache bc = new BazierCache();
        bc.curve = curve;
        bc.mod = float.NaN;
        cache.Add(curve, bc);
    }

    private static float ParseToU(float u, BazierCache bcc)
    {
        float u2 = u;
        u2 = ParseToU(u, bcc.curve, bcc.mod);
        return u2;
    }

    private static float ParseToU(float u, string curve, float mod)
    {
        float u2 = u;
        switch (curve)
        {
            case "In":
                if (float.IsNaN(mod)) mod = 2;
                u2 = Mathf.Pow(u, mod);
                break;

            case "Out":
                if (float.IsNaN(mod)) mod = 2;
                u2 = 1 - Mathf.Pow(1 - u, mod);
                break;

            case "InOut":
                if (float.IsNaN(mod)) mod = 2;
                if (u <= 0.5f)
                {
                    u2 = 0.5f * Mathf.Pow(u * 2, mod);
                }
                else
                {
                    u2 = 0.5f + 0.5f * (1 - Mathf.Pow(1 - (2 * (u - 0.5f)), mod));
                }
                break;

            case "Sin":
                if (float.IsNaN(mod)) mod = 0.15f;
                u2 = u + mod * Mathf.Sin(2 * Mathf.PI * u);
                break;

            case "SinIn":
                // mod is ignored for SinIn
                u2 = 1 - Mathf.Cos(u * Mathf.PI * 0.5f);
                break;

            case "SinOut":
                // mod is ignored for SinOut
                u2 = Mathf.Sin(u * Mathf.PI * 0.5f);
                break;
                //Linear is go straight
            case "Linear":
            default:
                // u2 already equals u
                break;
        }
        return u2;
    }
}

