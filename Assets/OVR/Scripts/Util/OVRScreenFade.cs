/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections; // required for Coroutines

/// <summary>
/// Fades the screen from black after a new scene is loaded.
/// </summary>
public class OVRScreenFade : MonoBehaviour
{
	
	/// <summary>
	/// The initial screen color.
	/// </summary>
	public Color fadeOutColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
	public Color fadeInColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

	private Material fadeMaterial = null;
	private bool isFading = false;
	private YieldInstruction fadeInstruction = new WaitForEndOfFrame();
	private Constants.Configuration config;
	private float fadeTime;

	/// <summary>
	/// Initialize.
	/// </summary>
	void Awake()
	{
		// create the fade material
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
	}

	void Start()
	{
		config = Constants.Configuration.Instance;
		fadeTime = config.FadeTime;
	}

	/// <summary>
	/// Starts a fade in when a new level is loaded
	/// </summary>
#if UNITY_5_4_OR_NEWER
	public void FadeCamera(bool fadeIn)
#else
	void OnLevelWasLoaded(int level)
#endif
	{
		StartCoroutine(Fade(fadeIn));
	}

	/// <summary>
	/// Cleans up the fade material
	/// </summary>
	void OnDestroy()
	{
		if (fadeMaterial != null)
		{
			Destroy(fadeMaterial);
		}
	}

	/// <summary>
	/// Fades alpha from 1.0 to 0.0
	/// </summary>
	IEnumerator Fade(bool fadeIn)
	{
		float elapsedTime = 0.0f;
		fadeMaterial.color =  fadeIn ? fadeInColor : fadeOutColor;
		Color color =  fadeIn ? fadeInColor : fadeOutColor;
		isFading = true;
		while (elapsedTime < fadeTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;
			if (fadeIn)
			{
				color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
			}
			else
			{
				color.a = Mathf.Clamp01(elapsedTime / fadeTime);
			}
			fadeMaterial.color = color;
		}
		isFading = false;
	}

	/// <summary>
	/// Renders the fade overlay when attached to a camera object
	/// </summary>
	void OnPostRender()
	{
		if (isFading)
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
