using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class MainButtonEfect : MonoBehaviour {

	public RectTransform frame;
	public RectTransform accessory;
	public RectTransform accessory2;
	public RectTransform accessory3;
	public RectTransform Broad;

	[HideInInspector]
	public Sequence seqFrame;
	[HideInInspector]
	public Sequence seqAccessory;
	[HideInInspector]
	public Sequence seqAccessory2;
	[HideInInspector]
	public Sequence seqAccessory3;
	[HideInInspector]
	public Sequence seqBroad;



	// Use this for initialization
	void Start () {
		float beatTime = 60f / 105f;

		/*seqFrame = DOTween.Sequence ();
		seqFrame.Append (frame.DOScale (new Vector2 (0.9f, 0.9f), beatTime));
		seqFrame.Append (frame.DOScale (new Vector2 (1.0f, 1.0f), beatTime));
		seqFrame.SetLoops (-1);*/

		seqFrame = DOTween.Sequence ();
		seqFrame.Append (frame.DOLocalRotate(new Vector3 (0,0, 5), 0.5f, RotateMode.FastBeyond360).SetDelay(beatTime));
		seqFrame.Append (frame.DOLocalRotate(new Vector3 (0,0, 0), 0.5f, RotateMode.FastBeyond360).SetDelay(beatTime));
		//seqFrame.Append (frame.DOShakeRotation(1.0f, new Vector3 (0,0, 40f)).SetDelay(beatTime));
		//seqFrame.Append (frame.DOPunchRotation(new Vector3 (0,0, 15f), 1.5f).SetDelay(beatTime));
		//seqFrame.Append (frame.DOLocalRotate (new Vector3 (0,0, 0), beatTime));
		seqFrame.SetLoops (-1);


		seqBroad = DOTween.Sequence ();
		seqBroad.Append (Broad.DOLocalRotate(new Vector3 (0,0, -180), 5f));
		seqBroad.Append (Broad.DOLocalRotate(new Vector3 (0,0, -360), 5f));
		seqBroad.SetLoops (-1);

		seqAccessory = DOTween.Sequence ();
		seqAccessory.Append (accessory.DOLocalRotate(new Vector3 (0,0, 180), 2f));
		seqAccessory.Append (accessory.DOLocalRotate(new Vector3 (0,0, 360), 2f));
		seqAccessory.SetLoops (-1);

		seqAccessory2 = DOTween.Sequence ();
		seqAccessory2.Append (accessory2.DOLocalRotate(new Vector3 (0,0, -180), 1f));
		seqAccessory2.Append (accessory2.DOLocalRotate(new Vector3 (0,0, -360), 1f));
		seqAccessory2.SetLoops (-1);

		seqAccessory3 = DOTween.Sequence ();
		seqAccessory3.Append (accessory3.DOLocalRotate(new Vector3 (0,0, 180), 5f));
		seqAccessory3.Append (accessory3.DOLocalRotate(new Vector3 (0,0, 360), 5f));
		seqAccessory3.SetLoops (-1);


	}


}
