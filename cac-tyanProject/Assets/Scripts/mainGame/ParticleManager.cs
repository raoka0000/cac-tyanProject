using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : SingletonMonoBehaviour<ParticleManager> {
	[SerializeField] ParticleSystem tapEffect;
	ParticleSystem _tapEffect;


	// Use this for initialization
	void Start () {
		_tapEffect = UnityUtil.GetPrefub<ParticleSystem> (tapEffect, this.transform);
	}

	private Vector3 tmp = new Vector3 ();

	public void DoTapEffect(Vector3 pos,int n = 1){
		if (_tapEffect == null) {
			return;
		}
		_tapEffect.transform.position = pos;
		_tapEffect.Emit(n);
	}

}
