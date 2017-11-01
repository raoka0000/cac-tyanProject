using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType{
	idle = 1<<0,
	qestionTime = 1<<1,
	talking = 1<<2,
	showRestButton = 1<<3,
	resting = 1<<4
}


public class StageController : SingletonMonoBehaviour<StageController> {
	private readonly string   STAGE_SCRIPT_DATA_PATH = "json/";
	private readonly string[] STAGE_SCRIPT_DATA_NAMES = {"Stage1","Stage2","Stage3","Stage4"};

	[System.NonSerialized]
	public int state = 1<<0;

	[SerializeField]
	private int stageNumNow = 0;

	[SerializeField]
	private GameObject dummy;


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
		StageModel.instance.loadjson (STAGE_SCRIPT_DATA_PATH + name);
		StageModel.instance.AllLossCurrentQuestion ();
		if(StageModel.instance.nextScriptNode != null){
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
		}
		ChangeStageEffect (stageNumNow);
	}

	/*ボタン*/
	public void MainButton(){
		if (BitUtil.Exist (state, (int)StateType.showRestButton)) {
			return;
		}
		if (BitUtil.Exist (state, (int)StateType.idle)) {
			StageView.instance.MoveLive2dModel (
				() => {
					DoScriptNodeAction (StageModel.instance.stageScriptData.scriptNodes [0]);
				}
			);
			BitUtil.Loss (ref state, (int)StateType.idle);
			return;
		} else if (StageModel.instance.nextScriptNode == null) {
			StageView.instance.ShowQuestions ();
		} else {
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
		}
	}

	public void QuestionSelected(ScriptNode node){
		StartTalk (node);
		DoScriptNodeAction (node);
	}

	public void StartTalk(ScriptNode node){
		StageView.instance.ShowMessage (node.serif);
		modelProxy.model.StartMotion (StageModel.instance.stageScriptData.stage, node.id - 1, LAppDefine.PRIORITY_TALK, doAudioCheckingForHideMessage);
	}

	public void EndTalk(){
		BitUtil.Loss (ref state, (int)StateType.talking);
		StageView.instance.HideMessage ();
		if(StageModel.instance.nextScriptNode != null){
			DoScriptNodeAction (StageModel.instance.nextScriptNode);
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
			Camera.main.backgroundColor = Color.white;
			AudioManager.instance.PlayBGM ("Y 04 日常-17");
			//AudioManager.instance.PlayBGM (1);
		}else if(n == 1){
			Camera.main.backgroundColor = Color.red;
			AudioManager.instance.PlayBGM("Y 05 黒");
		}else if(n == 2){
			Camera.main.backgroundColor = Color.yellow;
			AudioManager.instance.PlayBGM ("Y 04 日常-17");
		}else if(n == 3){
			Camera.main.backgroundColor = Color.blue;
			AudioManager.instance.PlayBGM("Y 05 黒");
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

	public void DoRest(){
		dummy.SetActive (true);
		Time.timeScale = 0.001f;
		AudioManager.instance.StopBGM ();
		modelProxy.model.StopAllMotion ();
		StageView.instance.DoRest ();
	}


}
