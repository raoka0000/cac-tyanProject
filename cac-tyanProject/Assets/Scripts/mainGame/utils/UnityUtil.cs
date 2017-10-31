using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FunctionVoid();//コールバック用.

static class Extensions{
	//コールバック用関数FunctionVoidにnullの時起動しないようにする拡張メソッド
	public static void NullGuard(this FunctionVoid x){
		if(x != null){
			x ();
		}
	}
}

public class UnityUtil {
	public static IEnumerator AudioChecking (AudioSource audio ,FunctionVoid callback) {
		while(true) {
			yield return new WaitForFixedUpdate();
			if (!audio.isPlaying) {
				callback();
				break;
			}
		}
	}

}
