using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager> {
	public string loadJsonName = "json/Stage1";//jsonファイルの名前をいれる.
	[System.NonSerialized]
	public StageScriptData stageScriptData;

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

	[System.NonSerialized]
	public ScriptNode nextScriptNode;


	// Use this for initialization
	void Awake () {
		loadjson (loadJsonName);
		live2ModelAudioSource = modelProxy.gameObject.GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		
	}


	public void doAudioCheckingForHideMessage(){
		StartCoroutine (AudioChecking(live2ModelAudioSource, UiController.instance.HideMessage));
	}
	public delegate void functionType();
	private IEnumerator AudioChecking (AudioSource audio ,functionType callback) {
		while(true) {
			yield return new WaitForFixedUpdate();
			if (!audio.isPlaying) {
				callback();
				break;
			}
		}
	}


	private void loadjson(string fileName){
		TextAsset textAsset = Resources.Load (fileName) as TextAsset;//ResourcesファイルからfileNameのテキストファイルを探す.
		string jsonText = textAsset.text;
		stageScriptData = JsonUtility.FromJson<StageScriptData>(jsonText);//jsonファイルを上作ったデータ型に格納する.
		if(stageScriptData == null) return;
	}

	public void EventStopTalk(){
		if (nextScriptNode == null) return;
	}

	public void EventSelected(ScriptNode node){
		StartTalk (node);
		DoScriptNodeAction (node);
	}


	public void StartTalk(ScriptNode node){
		UiController.instance.ShowMessage (node.serif);
		modelProxy.model.StartMotion ("stage" + stageScriptData.stage, node.id - 1, LAppDefine.PRIORITY_TALK, doAudioCheckingForHideMessage);
	}
				
	public void DoScriptNodeAction(ScriptNode node){
		
		if (node.action2 == "lk") {
			//Lockがかかっている場合、アクションをすることができない.
			if (!IsLock(node)) {
				UiController.instance.RemoveCurrentQuestion(node);
				return;
			}
		}

		if (node.action == "tk") {
			//喋るだけ.
			//StartTalk(stageScriptData.stage, stageScriptData.scriptNodes[node.branch[0]]);
			nextScriptNode = stageScriptData.scriptNodes[node.branch[0]];
			UiController.instance.RemoveCurrentQuestion (node);
		}else if(node.action == "qt"){
			//新しい質問を作る.
			foreach (int n in node.branch) {
				UiController.instance.addCurrentQuestion (n);
			}
			UiController.instance.RemoveCurrentQuestion(node);
		}else if(node.action == "st"){
			//次のステージへ.
			ChangeStage (stageScriptData.stage + 1);
		}else if(node.action == "ls"){
			foreach (int n in node.branch) {
				UiController.instance.addCurrentQuestion (n);
			}
			UiController.instance.RemoveCurrentQuestion(node);
		}
			
		if (node.action2 == "rm") {
			foreach (int i in node.branch2) {
				UiController.instance.RemoveCurrentQuestion(i);
			}
		}
	}

	public bool IsLock(ScriptNode node){
		bool b = true;
		foreach (int i in node.branch2) {
			var n = stageScriptData.scriptNodes[i];
			b = b && UiController.instance.EndScriptNodes [stageScriptData.stage - 1].Contains (n);
		}
		return b;
	}

	public void ChangeStage(int NextStegeNumber){
		UiController.instance.AllRemoveCurrentQuestion();
		ChangeStageEffect (NextStegeNumber);
		loadJsonName = "json/Stage" + NextStegeNumber;
		loadjson (loadJsonName);
		UiController.instance.addCurrentQuestion (0);
		DoScriptNodeAction (stageScriptData.scriptNodes [0]);
	}

	public void ChangeStageEffect(int n){
		if (n == 1) {
			Camera.main.backgroundColor = Color.white;
			AudioManager.instance.PlayBGM (1);
		}else if(n == 2){
			Camera.main.backgroundColor = Color.red;
			AudioManager.instance.PlayBGM (2);
		}else if(n == 3){
			Camera.main.backgroundColor = Color.yellow;
			AudioManager.instance.PlayBGM (3);
		}else if(n == 4){
			Camera.main.backgroundColor = Color.blue;
			AudioManager.instance.PlayBGM (4);
		}


	}

}
