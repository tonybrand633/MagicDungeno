using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtils 
{
    
}

public class EasingCurveCache 
{
    public List<string> curves = new List<string>();
    public List<float> mods = new List<float>();
}

public class MyEasing 
{
	static public string Linear = ",Linear|";
	static public string In = ",In|";
	static public string Out = ",Out|";
	static public string InOut = ",InOut|";
	static public string Sin = ",Sin|";
	static public string SinIn = ",SinIn|";
	static public string SinOut = ",SinOut|";

	public static Dictionary<string, EasingCurveCache> myCaches;

	public static float Ease(float u, params string[]curveParams) 
	{
		//curveParams = ",Linear|"
		//Set up Cache for curves
		if (myCaches == null) 
		{
			myCaches = new Dictionary<string, EasingCurveCache>();
		}

		float u2 = u;
		foreach (string curve in curveParams) 
		{
			//Debug.Log(curve);
			if (!myCaches.ContainsKey(curve)) 
			{
				EaseParse(curve);
			}

			u2 = EaseP(u2, myCaches[curve]);
		}

		return u2;
	}

	private static void EaseParse(string curve) 
	{
		//List for float N string
		EasingCurveCache ECC = new EasingCurveCache();

		//curve = ",Linear|"
		string[] curves = curve.Split(',');
		foreach (string c in curves) 
		{
			//Curves Count:2  "" "Linear|"
			//Debug.Log(c+"Count"+curves.Length);
			if (c == "") 
			{
				continue;
			}

			string[] curveA = c.Split('|');
			//Debug.Log(curveA.Length);
			ECC.curves.Add(curveA[0]);

			if (curveA.Length == 1 || curveA[1] == "")
			{
				//Debug.Log("Add Mods NaN");
				ECC.mods.Add(float.NaN);
			}
			else 
			{
				float parseRes;
				//float.TryParse 将字符串转换为float，out res
				if (float.TryParse(curveA[1], out parseRes))
				{
					ECC.mods.Add(parseRes);
				}
				else 
				{
					ECC.mods.Add(float.NaN);
				}
			}
		}
		myCaches.Add(curve,ECC);
	}

	private static float EaseP(float u,EasingCurveCache ecc) 
	{
		float u2 = u;
        for (int i = 0; i < ecc.curves.Count; i++)
        {
			u2 = EaseP(u2, ecc.curves[i], ecc.mods[i]);
        }
		return u2;
	}

	private static float EaseP(float u ,string curve,float mod) 
	{
		float u2 = u;
		//Debug.Log(u2);
		switch (curve) 
		{
			case "In":
				if (float.IsNaN(mod)) 
				{
					mod = 2;
				}
				u2 = Mathf.Pow(u, mod);
				break;

			case "Out":
				if (float.IsNaN(mod))
				{
					mod = 2;
				}
				u2 = 1 - Mathf.Pow(1 - u, mod);
				break;

			case "InOut":
				if (float.IsNaN(mod))
				{
					mod = 2;
				}
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
				if (float.IsNaN(mod))
				{
					mod = 0.15f;
				}
				u2 = u + mod * Mathf.Sin(2 * Mathf.PI * u);
				break;
			case "SinIn":
				//ignore mod
				u2 = 1 - Mathf.Cos(u * Mathf.PI * 0.5f);
				break;
			case "SinOut":
				//ignore mod
				u2 = Mathf.Sin(u * Mathf.PI * 0.5f);
				break;
			case "Linear":
			default:
				break;
		}
		return u2;
	}

}
