using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageModel : SingletonMonoBehaviour<StageModel> {
	public StageScriptData stageScriptData { get; private set;}
	public ScriptNode[] scriptNodes{
		get{ 
			return stageScriptData.scriptNodes;
		}
	}
	public ScriptNode nextScriptNode{ get; set;}

	[System.NonSerialized]
	public Dictionary<string, List<ScriptNode>> endScriptNodes = new Dictionary<string, List<ScriptNode>>();

	[System.NonSerialized]
	public List<ScriptNode> currentQuestions = new List<ScriptNode>();
	public Action<ScriptNode> AddCurrentQuestionsListener;
	public Action<ScriptNode> LossCurrentQuestionsListener;
	public Action AllLossCurrentQuestionsListener;

	public void loadjson(string fileName){
		TextAsset textAsset = Resources.Load (fileName) as TextAsset;//ResourcesファイルからfileNameのテキストファイルを探す.
		string jsonText = textAsset.text;
		stageScriptData = JsonUtility.FromJson<StageScriptData>(jsonText);//jsonファイルを上作ったデータ型に格納する.
		if(stageScriptData == null) return;
	}

	public void AddEndScriptNode(ScriptNode node){
		if(!endScriptNodes.ContainsKey(stageScriptData.stage)){
			endScriptNodes.Add (this.stageScriptData.stage, new List<ScriptNode> ());
		}
		endScriptNodes [this.stageScriptData.stage].Add (node);
	}

	public bool IsEndScriptNodeContin(int[] n){
		bool b = true;
		foreach (int i in n) {
			var node = scriptNodes[i];
			b = b && endScriptNodes [stageScriptData.stage].Contains(node);
		}
		return b;
	}

	public bool AddCurrentQuestion(int id){
		ScriptNode[] nodes = scriptNodes;
		foreach(ScriptNode n in currentQuestions){
			if (n.id == id) {
				return false;
			}
		}
		if(endScriptNodes.ContainsKey(stageScriptData.stage)){
			foreach (ScriptNode n in endScriptNodes[stageScriptData.stage]) {
				if (n.id == id) {
					return false;
				}
			}
		}

		currentQuestions.Add (nodes [id]);
		if(AddCurrentQuestionsListener != null){
			AddCurrentQuestionsListener (nodes [id]);
		}
		return true;
	}

	public bool LossCurrentQuestion(int id){return LossCurrentQuestion (scriptNodes [id]);}
	public bool LossCurrentQuestion(ScriptNode node){
		if (node == null) return false;
		for (int i = currentQuestions.Count - 1; i >= 0; i--) {
			if(currentQuestions [i] == node){
				if (LossCurrentQuestionsListener != null) {
					LossCurrentQuestionsListener (node);
				}
				AddEndScriptNode (node);
				currentQuestions.RemoveAt (i);
				return true;
			}
		}
		return false;
	}

	public bool AllLossCurrentQuestion(){
		if (AllLossCurrentQuestionsListener != null) {
			AllLossCurrentQuestionsListener ();
		}
		currentQuestions.Clear ();
		return true;
	}


	public ScriptNode GetScriputNode(int id){
		return stageScriptData.scriptNodes [id];
	}


}
