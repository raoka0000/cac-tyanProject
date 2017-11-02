using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResetButtonEfect : MonoBehaviour {

	[SerializeField]
	private GameObject obj;

	[SerializeField]
	private RectTransform yesButton;
	[SerializeField]
	private RectTransform noButton;
	[SerializeField]
	private RectTransform TextBoard; 
	[SerializeField]
	private FadeUI fade;

	private bool isShow = false;
	private bool isAnimesion = false;
	public void DoEffect(){
		if(isAnimesion){
			return;
		}
		if (!isShow) {
			Show ();
		} else {
			Hidden ();
		}
	}
	Sequence seq;
	void Start () {
		seq = DOTween.Sequence ();
	}

	Vector3 vec1 = new Vector3(-0.5f,0.5f,-0.5f);
	Vector3 vec2 = new Vector3(0.5f,0.05f,0.1f);
	public void Show(){
		isAnimesion = true;
		StageController.instance.showRestButton(true);
		isShow = true;
		obj.SetActive (true);
		seq.Kill ();
		seq = DOTween.Sequence();
		seq.Append (
			DOTween.To (()=>1f, (x)=>fade.Range = x,0f,1f)
		);
		seq.Join (
			TextBoard.DOScaleY(1f,0.1f).OnStepComplete(
				()=>{
					TextBoard.DOPunchScale(vec1,0.4f,4);
				}
			)
		);
		seq.Join (
			yesButton.DOScaleY(1f,0.1f).OnStepComplete(
				()=>{
					yesButton.DOPunchScale(vec1,0.4f,4);
				}
			)
		);
		seq.Join (
			noButton.DOScaleY (1f, 0.1f).OnStepComplete (
				() => {
					noButton.DOPunchScale (vec1, 0.4f, 4);
				}
			)
		).OnKill (()=>{isAnimesion = false;});
	}

	public void Hidden(){
		isAnimesion = true;
		isShow = false;
		fade.Range = 1;
		seq.Kill ();
		seq = DOTween.Sequence();
		seq.Append (
			DOTween.To (()=>0, (x)=>fade.Range = x,1f,1f)
		);
		seq.Join (
			TextBoard.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					TextBoard.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.Join (
			yesButton.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					yesButton.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.Join (
			noButton.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					noButton.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.OnKill (()=>{
			isAnimesion = false;
			obj.SetActive (false);
			StageController.instance.showRestButton(false);
		});
	}

	public void DoRest (){
		isShow = false;
		fade.Range = 1;
		seq.Kill ();
		seq = DOTween.Sequence();
		seq.Append (
			DOTween.To (()=>0, (x)=>fade.Range = x,1f,5f)
		);
		seq.Join (
			TextBoard.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					TextBoard.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.Join (
			yesButton.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					yesButton.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.Join (
			noButton.DOScaleY(0f,0.1f).OnStepComplete(
				()=>{
					noButton.DOPunchScale(vec2,0.4f,4);
				}
			)
		);
		seq.OnKill (()=>{
			obj.SetActive (false);
			StageController.instance.showRestButton(false);
			StageController.instance.DoRest();
		});

	}
}
