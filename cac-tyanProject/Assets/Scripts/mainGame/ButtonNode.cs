using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ButtonNode : MonoBehaviour {

	public int stage = 1;
	private ScriptNode _scriptNode;
	public ScriptNode scriptNode{
		get { return _scriptNode; }
		set { SetSerifNode (value); }
	}

	private bool _isNew = true;
	public bool isNew{
		get { return _isNew; }
		set { 
			_isNew = value;
			SetNewbadge (value);
		}
	}

	private Image _image;
	public Image image{
		get{
			if(_image == null){
				_image = gameObject.GetComponent<Image> ();
			}
			return _image;
		}
		set{ _image = value;}
	}

	public RectTransform rectTransform;


	public Text text;

	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Init(int stage, ScriptNode node){
		this.stage = stage;
		this.scriptNode = node;
		//this.isNew = true;
	}

	private void SetSerifNode(ScriptNode item){
		this._scriptNode = item;
		text = gameObject.GetComponentInChildren<Text>();
		text.text = item.question;
	}

	private void SetNewbadge(bool n){
		if (n) {
			image.color = new Color (0.8f, 1f, 1f, 1f);
		} else {
			image.color = Color.white;
		}
	}

	public void OnClick(){
		UiController.instance.Selected (this);
		AudioManager.instance.PlayQuestionButtonSound ();
	}
}
