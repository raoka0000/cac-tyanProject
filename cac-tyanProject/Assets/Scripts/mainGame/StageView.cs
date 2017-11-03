using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class StageView : SingletonMonoBehaviour<StageView> {

	private const float CAC_TYAN_MOVE_TIME = 1.0f;
	private const float QUESTION_MOVE_TIME = 0.2f;
	private const float MESSAGE_WINDOW_FADE_TIME = 0.4f;

	private List<ButtonNode> buttons = new List<ButtonNode>();

	[SerializeField]
	private RectTransform MainButton;
	[SerializeField]
	public RectTransform messageWindow;
	[SerializeField]
	public RectTransform questionButton;
	[SerializeField]
	public RectTransform resetButton;
	[SerializeField]
	public ResetButtonEfect reset;

	void Start () {
		StageModel.instance.AddCurrentQuestionsListener     = AddButton;
		StageModel.instance.LossCurrentQuestionsListener    = LossButton;
		StageModel.instance.AllLossCurrentQuestionsListener = AllLossButton;
	}

	void Update(){
		if(Input.GetMouseButtonDown(0)){ // Editor/マウス操作の場合は Input.GetMouseButton(0) にする
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			ParticleManager.instance.DoTapEffect(pos);
		}
	}


	//リスナー
	private void AddButton(ScriptNode node){
		RectTransform item = GameObject.Instantiate(questionButton) as RectTransform;
		item.SetParent(transform, false);

		ButtonNode buttonNode = item.GetComponent<ButtonNode> ();
		buttonNode.Init (StageModel.instance.stageScriptData.stage, node);
		buttons.Add (buttonNode);

	}
	//リスナー
	private void LossButton(ScriptNode node){
		for (int i = buttons.Count - 1; i >= 0; i--) {
			if(buttons [i].scriptNode == node){
				Destroy (buttons [i].gameObject);
				buttons.RemoveAt (i);
				return;
			}
		}
	}
	//リスナー
	private void AllLossButton(){
		for (int i = buttons.Count - 1; i >= 0; i--) {
				Destroy (buttons [i].gameObject);
				buttons.RemoveAt (i);
		}
	}

	public void PushMainButton(){
		StageController.instance.MainButton ();
	}

	private bool isMoveingLive2dModel = false;
	public void MoveLive2dModel (bool isGo,FunctionVoid callback = null){
		if (isMoveingLive2dModel) return;
		if (isGo) {
			StageController.instance.modelProxy.transform
				.DOMove (new Vector3 (-7.7f, 0, 0), CAC_TYAN_MOVE_TIME)
				.OnComplete (()=>{
					isMoveingLive2dModel = false;
					callback.NullGuard();
				});
		} else {
			StageController.instance.modelProxy.transform
				.DOMove (new Vector3 (0, 0, 0), CAC_TYAN_MOVE_TIME)
				.OnComplete (()=>{
					isMoveingLive2dModel = false;
					callback.NullGuard();
				});
			
		}
	}

	public bool isShowQuestions = false;
	public void ShowQuestions(){
		isShowQuestions = true;
		const float r = -350f;
		const float maxRad = Mathf.PI * 0.5f;
		MainButton.DOAnchorPos (Vector2.zero, QUESTION_MOVE_TIME);
		for(int i  = 0; i < buttons.Count; i++){
			Color col = Camera.main.backgroundColor;
			col.a = 0.8f;
			buttons [i].image.color = col;

			float theta = Mathf.Lerp (0, maxRad, ((float)i + 1.0f) / ((float)buttons.Count + 1) ) + (Mathf.PI * 1.75f) - 0.05f;
			int id = buttons.Count - 1 - i;
			buttons [id].rectTransform.localRotation = Quaternion.Euler(0, 0, theta * Mathf.Rad2Deg);
			Sequence seq = DOTween.Sequence();
			buttons [id].rectTransform.DOKill ();
			seq.SetDelay((float)i / 10f);
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 1.3f, Mathf.Sin(theta) * r * 1.3f), QUESTION_MOVE_TIME, true));
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 0.9f, Mathf.Sin(theta) * r * 0.9f), QUESTION_MOVE_TIME, true));
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r, Mathf.Sin(theta) * r), QUESTION_MOVE_TIME, true));
		}
	}
	public void HideQuestions(){
		foreach(ButtonNode buttonNode in buttons){
			buttonNode.rectTransform.DOAnchorPos (new Vector2(350, 0), QUESTION_MOVE_TIME
			).OnComplete(
				() => {
					buttonNode.Hidden();
				}
			);
		}
		/*StartQuestionButtonの処理*/
		MainButton.DOAnchorPos (new Vector2(92,0), QUESTION_MOVE_TIME).OnComplete(()=>{isShowQuestions = false;});

	}


	public void Selected(ButtonNode bNode){
		const float TIME = 0.5f;
		//押されたボタンの処理
		bNode.rectTransform.DOKill ();
		Sequence seq = DOTween.Sequence();
		seq.Append (
			//node.rectTransform.DOPunchScale (new Vector3 (0.3f, 0.3f), TIME, 4)
			bNode.rectTransform.DOScale(new Vector3(3f, 3f, 0), TIME)
		);
		seq.Join(
			DOTween.ToAlpha(
				() => bNode.text.color, 
				color => bNode.text.color = color,
				0f,                                // 最終的なalpha値
				TIME
			)
		);
		Image newbatch = bNode.newBatch.GetComponent<Image> ();
		seq.Join(
			DOTween.ToAlpha(
				() => newbatch.color, 
				color => newbatch.color = color,
				0f,                                // 最終的なalpha値
				TIME
			)
		);
		seq.Join(
			DOTween.ToAlpha(
				() => bNode.image.color, 
				color => bNode.image.color = color,
				0f,                                // 最終的なalpha値
				TIME
			)
		);
		seq.OnComplete (
			() => {
				StageController.instance.QuestionSelected (bNode.scriptNode);
			}
		);
			
		//押されていないボタンの処理
		foreach(ButtonNode buttonNode in buttons){
			if (bNode != buttonNode) {
				buttonNode.rectTransform.DOAnchorPos (new Vector2(350, 0), QUESTION_MOVE_TIME
				).OnComplete(
					() => {
						buttonNode.Hidden();
					}
				);
			}
		}
		/*StartQuestionButtonの処理*/
		MainButton.DOAnchorPos (new Vector2(92,0), QUESTION_MOVE_TIME);
	}

	public void ChengeBeseColor(Color col){
		Camera.main.DOColor(col, 1);
	}


	Sequence tmpSeq;
	public void ShowMessage(string str){
		tmpSeq.Kill ();
		tmpSeq = DOTween.Sequence();
		messageWindow.gameObject.SetActive (true);
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		text.text = str;
		tmpSeq.Append (
			DOTween.ToAlpha (
				() => text.color, 
				color => text.color = color,
				1f,                                // 最終的なalpha値
				MESSAGE_WINDOW_FADE_TIME
			)
		);
	}

	public void HideMessage(){
		if (!messageWindow.gameObject.activeSelf) return;
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		tmpSeq.Kill ();
		tmpSeq = DOTween.Sequence();
		tmpSeq.Append (
			DOTween.ToAlpha (
				() => text.color, 
				color => text.color = color,
				0f,                                // 最終的なalpha値
				MESSAGE_WINDOW_FADE_TIME
			).OnComplete (() => messageWindow.gameObject.SetActive (false))
		);
	}

	public void DoRest(){
		foreach(ButtonNode bn in buttons){
			bn.gameObject.SetActive (false);
		}
		MainButton.gameObject.SetActive (false);
		resetButton.gameObject.SetActive (false);
		GameObject camera = Camera.main.gameObject;
		CRT crt = camera.GetComponent<CRT> ();
		crt.enabled = true;
		Vector2 vec = Vector2.up;
		Vector2 vec2 = Vector2.one;
		Sequence seq = DOTween.Sequence();
		seq.timeScale = 1000.0f;
		seq.Append (
			DOVirtual.DelayedCall(0.8f,
				()=> {
					crt.Offset = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
				}
			).SetLoops(5)
		);
		/*
		seq.Join (
			DOTween.To (
				()=>crt.Offset.y,
				(num)=>crt.Offset = vec*num ,
				5f,
				5f
			).SetEase (Ease.InOutBounce)
		);*/

		seq.Append (
			DOTween.To (
				() => crt.NoiseX,
				(num) => crt.NoiseX = num,
				0.05f,
				1.0f
			).SetEase (Ease.InOutBounce)
		);
		seq.Append (
			DOTween.To (
				() => crt.NoiseX,
				(num) => crt.NoiseX = num,
				0f,
				1.5f
			).SetEase (Ease.InOutBounce)
		);
		seq.Join (
			DOTween.To (
				() => Time.timeScale,
				(num) => Time.timeScale = num,
				0.1f,
				1.5f
			).SetEase (Ease.InOutBounce)
			.OnStepComplete(()=>{Time.timeScale = 1;})
		);
		seq.Join (
			DOTween.To (
				() => seq.timeScale,
				(num) => seq.timeScale = num,
				1f,
				1.5f
			).SetEase (Ease.InOutBounce)
		);


		seq.Append (
			DOTween.To (
				() => crt.SinNoiseWidth,
				(num) => crt.SinNoiseWidth = num,
				5.55f,
				1.5f
			).SetEase (Ease.InOutBounce)
		);
		seq.Join (
			DOTween.To (
				() => crt.ScanLineTail,
				(num) => crt.ScanLineTail = num,
				0f,
				3.5f
			).SetEase (Ease.InOutBounce)
		).AppendInterval(1.0f)
			.OnKill(()=>SceneManager.LoadScene ("refection"));
		
		seq.Join (
			DOTween.To (
				() => crt.SinNoiseWidth,
				(num) => crt.SinNoiseWidth = num,
				0f,
				1.5f
			).SetEase (Ease.InOutBounce)
		);



	}


}
