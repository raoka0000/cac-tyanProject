using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//jsonファイルのデータ型を作る.
[Serializable]
public class StageScriptData{
	public int stage;
	public ScriptNode[] scriptNodes;
}

[Serializable]
public class ScriptNode{
	public int id;
	public string serif;
	public string question;
	public string action;
	public int[] branch;
	public string action2;
	public int[] branch2;
}
