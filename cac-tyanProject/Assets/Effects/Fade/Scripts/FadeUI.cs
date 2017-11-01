using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(RawImage))]
[RequireComponent (typeof(Mask))]
public class FadeUI : UnityEngine.UI.Graphic, IFade
{

	Mask mask;

	[SerializeField]
	private Texture maskTexture = null;

	[SerializeField, Range (0,1)]
	private float cutoutRange = 1;

	protected override void Start() {
		mask = this.gameObject.GetComponent<Mask>();
		base.Start();
		UpdateMaskTexture(maskTexture);
	}

	public void UpdateMaskTexture(Texture texture) {
		material.SetTexture("_MaskTex",texture);
		material.SetColor("_Color",color);
	}

	void Update(){
		mask.enabled = false;
		mask.enabled = true;

	}

	public float Range {
		get {
			return cutoutRange;
		}
		set {
			cutoutRange = value;
			UpdateMaskCutout (cutoutRange);
		}
	}

	[SerializeField] Material mat = null;
	[SerializeField] RenderTexture rt = null;

	[SerializeField] Texture texture = null;



	private void UpdateMaskCutout (float range)
	{
		mat.SetFloat ("_Range", range);

		UnityEngine.Graphics.Blit (texture, rt, mat);
	}

	#if UNITY_EDITOR
	protected override void OnValidate() {
		base.OnValidate();
		UpdateMaskCutout(Range);
		UpdateMaskTexture(maskTexture);
	}
	#endif

}
