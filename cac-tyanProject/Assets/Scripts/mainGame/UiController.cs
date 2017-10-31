using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiController : SingletonMonoBehaviour<UiController> {

	private const float CAC_TYAN_MOVE_TIME = 1.0f;
	private const float QUESTION_MOVE_TIME = 0.2f;
	private const float MESSAGE_WINDOW_TIME = 0.4f;

	public GameObject cacTyan;

	public RectTransform questionButton;

	public RectTransform messageWindow;

	public RectTransform StartQuestionButton;

	private List<ButtonNode> currentQuestions = new List<ButtonNode>();
	//ステージの数だけ
	[System.NonSerialized]
	public List<ScriptNode>[] EndScriptNodes = {new List<ScriptNode> (), new List<ScriptNode> (), new List<ScriptNode> (), new List<ScriptNode> ()};

	private bool isQuestionTime = false;



	// Use this for initialization
	void Start () {
		int[] n = { 0 };//デバック用
		foreach(int i in n){
			addCurrentQuestion (i);
		}
	}

	public bool addCurrentQuestion(int id){
		ScriptNode[] scriptNodes = StageManager.instance.stageScriptData.scriptNodes;
		foreach (ButtonNode n in currentQuestions) {
			if (n.scriptNode.id == id) {
				return false;
			}
		}
		foreach (ScriptNode n in EndScriptNodes[StageManager.instance.stageScriptData.stage -1]) {
			if (n.id == id) {
				return false;
			}
		}
		RectTransform item = GameObject.Instantiate(questionButton) as RectTransform;
		item.SetParent(transform, false);
		//item.gameObject.SetActive (false);

		ButtonNode buttonNode = item.GetComponent<ButtonNode> ();
		buttonNode.Init (StageManager.instance.stageScriptData.stage, scriptNodes [id]);
		buttonNode.isNew = false;

		currentQuestions.Add (buttonNode);
		return true;
	}

	public void RemoveCurrentQuestion(int id){
		ScriptNode node = StageManager.instance.stageScriptData.scriptNodes [id];
		RemoveCurrentQuestion (node);
	}
	public void RemoveCurrentQuestion(ScriptNode node){
		for (int i = currentQuestions.Count - 1; i >= 0; i--) {
			if(currentQuestions [i].scriptNode == node){
				EndScriptNodes [StageManager.instance.stageScriptData.stage - 1].Add (node);
				Destroy (currentQuestions [i].gameObject);
				currentQuestions.RemoveAt (i);
			}
		}
	}

	public void AllRemoveCurrentQuestion(){
		for (int i = currentQuestions.Count - 1; i >= 0; i--) {
			Destroy (currentQuestions [i].gameObject);
			currentQuestions.RemoveAt (i);
		}
		currentQuestions.Clear();
	}
		
	public void ShowQuestions(){
		const float r = -350f;
		const float maxRad = Mathf.PI * 0.5f;
		StartQuestionButton.DOAnchorPos (Vector2.zero, QUESTION_MOVE_TIME);
		for(int i  = 0; i < currentQuestions.Count; i++){
			float theta = Mathf.Lerp (0, maxRad, ((float)i + 1.0f) / ((float)currentQuestions.Count + 1) ) + (Mathf.PI * 1.75f) - 0.1f;
			int id = currentQuestions.Count - 1 - i;
			currentQuestions [id].rectTransform.localRotation = Quaternion.Euler(0, 0, theta * Mathf.Rad2Deg);
			Sequence seq = DOTween.Sequence();
			seq.SetDelay((float)i / 10f);
			seq.Append (currentQuestions[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 1.3f, Mathf.Sin(theta) * r * 1.3f), QUESTION_MOVE_TIME, true));
			seq.Append (currentQuestions[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r * 0.9f, Mathf.Sin(theta) * r * 0.9f), QUESTION_MOVE_TIME, true));
			seq.Append (currentQuestions[id].rectTransform.DOAnchorPos (new Vector2(Mathf.Cos(theta) * r, Mathf.Sin(theta) * r), QUESTION_MOVE_TIME, true));
		}
		//HideMessage ();
	}

	/*使っていない*/
	public void EndQuestion(){
		cacTyan.transform.DOMove (new Vector3 (0, 0, 0), CAC_TYAN_MOVE_TIME);

		for(int i  = 0; i < currentQuestions.Count; i++){
			currentQuestions [i].rectTransform.DOAnchorPos (new Vector2(350, 0), QUESTION_MOVE_TIME);
		}
		isQuestionTime = false;
	}

	public void MainButton(){
		if (currentQuestions.Count == 0)return;
		if (isQuestionTime == false && currentQuestions[0].scriptNode.id == 0) {
			currentQuestions [0].gameObject.SetActive (false);
			cacTyan.transform.DOMove (new Vector3 (-8, 0, 0), CAC_TYAN_MOVE_TIME
			).OnComplete(() => StageManager.instance.DoScriptNodeAction (currentQuestions [0].scriptNode));
		} else {			
			ShowQuestions ();
		}
		isQuestionTime = true;
	}
		
	public void BackGroundButton(){
		if (isQuestionTime) {
			//ShowQuestions ();
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
					StageManager.instance.EventSelected(bNode.scriptNode);
				}
			)
		);

		//押されていないボタンの処理
		foreach(ButtonNode buttonNode in currentQuestions){
			if (bNode != buttonNode) {
				buttonNode.rectTransform.DOAnchorPos (new Vector2(350, 0), QUESTION_MOVE_TIME
				).OnComplete(() => {buttonNode.isNew = false;});
			}
		}
		/*StartQuestionButtonの処理*/
		StartQuestionButton.DOAnchorPos (new Vector2(92,0), QUESTION_MOVE_TIME);

		isQuestionTime = false;

	}

	public void ShowMessage(string str){
		messageWindow.gameObject.SetActive (true);
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		text.text = str;
		DOTween.ToAlpha (
			() => text.color, 
			color => text.color = color,
			1f,                                // 最終的なalpha値
			MESSAGE_WINDOW_TIME
		);

	}

	public void HideMessage(){
		if (!messageWindow.gameObject.activeSelf) return;
		var text = messageWindow.gameObject.GetComponentInChildren<Text>();
		DOTween.ToAlpha (
			() => text.color, 
			color => text.color = color,
			0f,                                // 最終的なalpha値
			MESSAGE_WINDOW_TIME
		).OnComplete (() => messageWindow.gameObject.SetActive (false));
	}


}
