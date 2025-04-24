using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.AdditiveLevels;

public class FadeInOut : MonoBehaviour
{
	[Tooltip("Reference to Image component on child panel")]
	public Image fadeImage;

	[Tooltip("Color to use during scene transition")]
	public Color fadeColor = Color.black;

	[Range(1f, 100f)]
	[Tooltip("Rate of fade in / out: higher is faster")]
	public byte stepRate = 2;

	private float step;

	private void OnValidate()
	{
		if (fadeImage == null)
		{
			fadeImage = GetComponentInChildren<Image>();
		}
	}

	private void Start()
	{
		step = (float)(int)stepRate * 0.001f;
	}

	public float GetDuration()
	{
		float num = 1f / step;
		float deltaTime = Time.deltaTime;
		return num * deltaTime * 0.1f;
	}

	public IEnumerator FadeIn()
	{
		float alpha = fadeImage.color.a;
		while (alpha < 1f)
		{
			yield return null;
			alpha += step;
			fadeColor.a = alpha;
			fadeImage.color = fadeColor;
		}
	}

	public IEnumerator FadeOut()
	{
		float alpha = fadeImage.color.a;
		while (alpha > 0f)
		{
			yield return null;
			alpha -= step;
			fadeColor.a = alpha;
			fadeImage.color = fadeColor;
		}
	}
}
