using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeTIme : MonoBehaviour {
	
	// Update is called once per frame
	private float timer = 0;
	int frontEndTalkId = 4;
	void Update () {
		timer += Time.deltaTime;
		if (Input.GetMouseButtonDown (0)) {
			timer = 0;
		}
		if(timer > 10f){
			timer = -10;
			int n;
			for(;;){
				n = Random.Range (0, StageModel.freeTalks.Length);
				if (n != frontEndTalkId) {
					frontEndTalkId = n;
					break;
				}
			}
			StageController.instance.modelProxy.model.StartMotion (StageModel.freeTalks[n].group, StageModel.freeTalks[n].no, LAppDefine.PRIORITY_NORMAL);
		}
	}
}
