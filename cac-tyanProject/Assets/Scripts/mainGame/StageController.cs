using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum StateType{
	idle = 1<<0,
	qestionTime = 1<<1,
	talking = 1<<2,
	showRestButton = 1<<3,
	resting = 1<<4,
	end = 1<<4,
	idle2 = 1<<5,
}


public class StageController : SingletonMonoBehaviour<StageController> {
	private string   STAGE_SCRIPT_DATA_PATH = "json/";
	private string[] STAGE_SCRIPT_DATA_NAMES = {"Stage1","Stage2","Stage3","Stage4"};

	[System.NonSerialized]
	public int state = 1<<0;

	[SerializeField]
	private int stageNumNow = 0;

	[SerializeField]
	private MyGameController myGameCtrl;

	[SerializeField]
	private GameObject dummy;

	public FreeTime freeTime;


	[SerializeField]
	private LAppModelProxy _modelProxy;
	public LAppModelProxy modelProxy{
		get{
			if (_modelProxy == null) {
				_modelProxy = FindObjectOfType<LAppModelProxy> ();
			}
			return _modelProxy;
		}
		set{_modelProxy = value;}
	}
	private AudioSource live2ModelAudioSource;
		
	// Use this for initialization
	void Start () {
		NewStege (STAGE_SCRIPT_DATA_NAMES[stageNumNow]);
		live2ModelAudioSource = modelProxy.gameObject.GetComponent<AudioSource> ();
	}

	void NewStege(string name){
		BitUtil.Loss (ref state, (int)StateType.talking);
		StageModel.instance.AllLossCurrentQuestion ();
		StageModel.instance.loadjson (STAGE_SCRIPT_DATA_PATH + name);
		if(StageModel.instance.nextScriptNode != null){
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
		}
		ChangeStageEffect (stageNumNow);
	}

	/*ボタン*/
	private int frontEndTalkId = 4;
	public void MainButton(){
		if (BitUtil.Exist (state, (int)StateType.showRestButton)) {
			return;
		}
		if (BitUtil.Exist (state, (int)StateType.end) && !BitUtil.Exist (state, (int)StateType.talking)) {
			StageView.instance.HideQuestions ();
			int n;
			for(;;){
				n = Random.Range (0, StageModel.endTalks.Length);
				if (n != frontEndTalkId) {
					frontEndTalkId = n;
					break;
				}
			}
			StartTalk (StageModel.endTalks[n]);
			return;
		}

		if (BitUtil.Exist (state, (int)StateType.idle)) {
			//myGameCtrl.enabled = false;
			StageView.instance.MoveLive2dModel (
				true,
				() => {
					DoScriptNodeAction (StageModel.instance.stageScriptData.scriptNodes [0]);
				}
			);
			BitUtil.Loss (ref state, (int)StateType.idle);
			return;
		} else if(BitUtil.Exist (state, (int)StateType.idle2)){
			StageView.instance.MoveLive2dModel (
				true,
				() => {
					//DoScriptNodeAction (StageModel.instance.stageScriptData.scriptNodes [0]);
				}
			);
			BitUtil.Loss (ref state, (int)StateType.idle2);
		}else if (StageModel.instance.nextScriptNode == null && !BitUtil.Exist (state, (int)StateType.end)) {
			StageView.instance.ShowQuestions ();
		} else if(!BitUtil.Exist (state, (int)StateType.end)){
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
		}
	}

	public void HiddenModel(){
		if (!BitUtil.Exist (state, (int)StateType.idle2) && !BitUtil.Exist (state, (int)StateType.talking)) {
			StageView.instance.HideQuestions ();
			StageView.instance.MoveLive2dModel (
				false,
				() => {
					//myGameCtrl.enabled = true;
				}
			);
			BitUtil.Add (ref state, (int)StateType.idle2);
			return;
		}
	}

	public void QuestionSelected(ScriptNode node){
		StartTalk (node);
		DoScriptNodeAction (node);
	}

	public void StartTalk(ScriptNode node){
		freeTime.timer = 0;
		StageView.instance.ShowMessage (node.serif);
		modelProxy.model.StartMotion (StageModel.instance.stageScriptData.stage, node.id - 1, LAppDefine.PRIORITY_TALK, doAudioCheckingForHideMessage);
	}
	public void StartTalk(TalkNode node){
		freeTime.timer = 0;
		StageView.instance.ShowMessage (node.serif);
		modelProxy.model.StartMotion (node.group, node.no, LAppDefine.PRIORITY_TALK, doAudioCheckingForHideMessage);
	}


	public void EndTalk(){
		if (!BitUtil.Exist (state, (int)StateType.talking)) return;
		BitUtil.Loss (ref state, (int)StateType.talking);
		StageView.instance.HideMessage ();
		if (StageModel.instance.nextScriptNode != null) {
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
		} else if (!BitUtil.Exist (state, (int)StateType.end)) {
			MainButton ();
		} else if(!StageView.instance.isShowQuestions){
			//StageView.instance.ShowQuestions ();
		}
	}

	public void doAudioCheckingForHideMessage(){
		if(!BitUtil.Exist(state, ((int)StateType.talking))){
			BitUtil.Add(ref state, (int) StateType.talking);
			StartCoroutine (UnityUtil.AudioChecking(live2ModelAudioSource, EndTalk));
		}
	}


	void DoScriptNodeAction(ScriptNode node){
		StageModel.instance.nextScriptNode = null;
		//Debug.Log (node.id + " : " + StageModel.instance.stageScriptData.stage);
		if (node.action2 == "lk") {
			//Lockがかかっている場合、アクションをすることができない.
			if (!StageModel.instance.IsEndScriptNodeContin(node.branch2)) {
				StageModel.instance.LossCurrentQuestion (node);
				return;
			}
		}
		if (node.action == "tk") {
			//喋るだけ.
			StartTalk(StageModel.instance.scriptNodes[node.branch[0]]);
			StageModel.instance.nextScriptNode = StageModel.instance.scriptNodes[node.branch [0]];
		}else if(node.action == "qt"){
			//新しい質問を作る.
			foreach (int n in node.branch) {
				StageModel.instance.AddCurrentQuestion (n);
			}
		}else if(node.action == "st"){
			//次のステージへ.
			stageNumNow += 1;
			NewStege(STAGE_SCRIPT_DATA_NAMES[stageNumNow]);
			StageModel.instance.nextScriptNode = StageModel.instance.scriptNodes[0];
		}else if(node.action == "ls"){
			//最後の質問へ.
			foreach (int n in node.branch) {
				StageModel.instance.AddCurrentQuestion (n);
			}
		}else if(node.action == "end"){
			BitUtil.Add (ref state, (int)StateType.end);
		}

		if (node.action2 == "rm") {
			//質問を削除する.
			foreach (int i in node.branch2) {
				StageModel.instance.LossCurrentQuestion(i);
			}
		}
		StageModel.instance.LossCurrentQuestion (node);


	}

	public void ChangeStageEffect(int n){
		if (n == 0) {
			AudioManager.instance.PlayBGM ("Y 04 日常-17");
			Camera.main.backgroundColor = Color.white;
			modelProxy.model.tapHeadMotionGroupName   = "tap_head";
			modelProxy.model.tapBodyMotionGroupName   = "tap_body";
			modelProxy.model.flickHeadMotionGroupName = "flick_head";
			modelProxy.model.flickBodyMotionGroupName = "flick_body";
		}else if(n == 1){
			StageView.instance.ChengeBeseColor (new Color(169.0f/255.0f, 203.0f/255.0f, 225.0f/255.0f));
			AudioManager.instance.PlayBGM("Y 05 黒");
			modelProxy.model.tapHeadMotionGroupName   = "tap_head2";
			modelProxy.model.tapBodyMotionGroupName   = "tap_body2";
			modelProxy.model.flickHeadMotionGroupName = "flick_head2";
			modelProxy.model.flickBodyMotionGroupName = "flick_body2";
		}else if(n == 2){
			//StageView.instance.ChengeBeseColor (new Color(114.0f/255.0f, 147.0f/255.0f, 169.0f/255.0f));
			StageView.instance.ChengeBeseColor (new Color(221.0f/255.0f, 255.0f/255.0f, 221.0f/255.0f));
			AudioManager.instance.PlayBGM("comvini",10f);
			modelProxy.model.tapHeadMotionGroupName   = "tap_head2";
			modelProxy.model.tapBodyMotionGroupName   = "tap_body2";
			modelProxy.model.flickHeadMotionGroupName = "flick_head2";
			modelProxy.model.flickBodyMotionGroupName = "flick_body2";
		}else if(n == 3){
			StageView.instance.ChengeBeseColor (new Color(79.0f/255.0f, 101.0f/255.0f, 131.0f/255.0f));
			AudioManager.instance.PlayBGM("Y 02 -01");
			AudioManager.instance.ChangeVolume (0.8f);
			modelProxy.model.tapHeadMotionGroupName = "tap_head4";
			modelProxy.model.tapBodyMotionGroupName = "tap_body4";
			modelProxy.model.flickHeadMotionGroupName = "flick_head4";
			modelProxy.model.flickBodyMotionGroupName = "flick_body4";
		}
	}

	public void showRestButton(bool flg){
		if (flg) {
			BitUtil.Add (ref state, (int)StateType.showRestButton);
			modelProxy.model.tapBodyMotionGroupName = "";
		} else {
			BitUtil.Loss (ref state, (int)StateType.showRestButton);
			modelProxy.model.tapBodyMotionGroupName = LAppDefine.MOTION_GROUP_TAP_BODY;
		}
	}

	public void ShakeEvent(){
		if (BitUtil.Exist (state, (int)StateType.talking)) return;
		if (BitUtil.Exist (state, (int)StateType.showRestButton)) return;
		StageView.instance.HideQuestions ();
		BitUtil.Add (ref state, (int)StateType.showRestButton);
		Vector3 initPosison = modelProxy.transform.position;
		Sequence seq = DOTween.Sequence ();
		modelProxy.model.StartMotion ("shake", 0, LAppDefine.PRIORITY_FORCE);
		seq.PrependInterval(1.6f);
		seq.Append (modelProxy.transform.DOMoveY(-25,1.5f).SetEase(Ease.InOutQuart));
		seq.Append (modelProxy.transform.DOScale (new Vector3 (1.6f, 1.6f, 1.6f), 2));
		seq.Append (modelProxy.transform.DOMoveX(0,0.1f));
		seq.Append (modelProxy.transform.DOMove (new Vector2 (0, -2), 1).OnComplete(()=>{
			modelProxy.model.StartMotion ("shake", 1, LAppDefine.PRIORITY_FORCE);
		}));
		seq.AppendInterval (4f);
		seq.Append (modelProxy.transform.DOMove (new Vector2 (0, -25), 0.8f).SetEase(Ease.InOutQuart));
		seq.Append (modelProxy.transform.DOScale (new Vector3 (1, 1, 1), 1));
		seq.Append (modelProxy.transform.DOMoveX(initPosison.x,0.1f));
		seq.Append (modelProxy.transform.DOMove (initPosison, 1).OnComplete(()=>BitUtil.Loss (ref state, (int)StateType.showRestButton)));

	}

	public void DoRest(){
		if(BitUtil.Exist (state, (int)StateType.end)){
			StartTalk(new TalkNode("thank",0,"ありがとうございました"));
		}
		dummy.SetActive (true);
		Time.timeScale = 0.001f;
		AudioManager.instance.StopBGM ();
		//modelProxy.model.StopAllMotion ();
		StageView.instance.DoRest ();
	}


}
