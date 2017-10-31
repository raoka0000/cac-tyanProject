using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

	void Start () {
		StageModel.instance.AddCurrentQuestionsListener     = AddButton;
		StageModel.instance.LossCurrentQuestionsListener    = LossButton;
		StageModel.instance.AllLossCurrentQuestionsListener = AllLossButton;
	}


	//リスナー
	private void AddButton(ScriptNode node){
		RectTransform item = GameObject.Instantiate(questionButton) as RectTransform;
		item.SetParent(transform, false);

		ButtonNode buttonNode = item.GetComponent<ButtonNode> ();
		buttonNode.Init (StageModel.instance.stageScriptData.stage, node);
		buttonNode.isNew = false;

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

	public void MoveLive2dModel (FunctionVoid callback = null){
		StageController.instance.modelProxy.transform
			.DOMove (new Vector3 (-8, 0, 0), CAC_TYAN_MOVE_TIME)
			.OnComplete(callback.NullGuard);
	}

	public void ShowQuestions(){
		const float r = -350f;
		const float maxRad = Mathf.PI * 0.5f;
		MainButton.DOAnchorPos (Vector2.zero, QUESTION_MOVE_TIME);
		for(int i  = 0; i < buttons.Count; i++){
			float theta = Mathf.Lerp (0, maxRad, ((float)i + 1.0f) / ((float)buttons.Count + 1) ) + (Mathf.PI * 1.75f) - 0.1f;
			int id = buttons.Count - 1 - i;
			buttons [id].rectTransform.localRotation = Quaternion.Euler(0, 0, theta * Mathf.Rad2Deg);
			Sequence seq = DOTween.Sequence();
			seq.SetDelay((float)i / 10f);
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 1.3f, Mathf.Sin(theta) * r * 1.3f), QUESTION_MOVE_TIME, true));
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 0.9f, Mathf.Sin(theta) * r * 0.9f), QUESTION_MOVE_TIME, true));
			seq.Append (buttons[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r, Mathf.Sin(theta) * r), QUESTION_MOVE_TIME, true));
		}
	}

	public void Selected(ButtonNode bNode){
		const float TIME = 0.5f;
		//押されたボタンの処理
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
		seq.Join(
			DOTween.ToAlpha(
				() => bNode.image.color, 
				color => bNode.image.color = color,
				0f,                                // 最終的なalpha値
				TIME
			).OnComplete(
				() => {
					StageController.instance.QuestionSelected(bNode.scriptNode);
				}
			)
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



	public void ShowMessage(string str){
		messageWindow.gameObject.SetActive (true);
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		text.text = str;
		DOTween.ToAlpha (
			() => text.color, 
			color => text.color = color,
			1f,                                // 最終的なalpha値
			MESSAGE_WINDOW_FADE_TIME
		);
	}

	public void HideMessage(){
		if (!messageWindow.gameObject.activeSelf) return;
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		DOTween.ToAlpha (
			() => text.color, 
			color => text.color = color,
			0f,                                // 最終的なalpha値
			MESSAGE_WINDOW_FADE_TIME
		).OnComplete (() => messageWindow.gameObject.SetActive (false));
	}



}
