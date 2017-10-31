/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using System.Collections.Generic;

interface ModelSetting 
{
	

	// モデルデータについて
	string GetModelName()		 ;
	string GetModelFile()		 ;

	// テクスチャについて
	int GetTextureNum()			 ;
	string GetTextureFile(int n) ;
	string[] GetTextureFiles() ;

	// あたり判定について
	int GetHitAreasNum()		;
	string GetHitAreaID(int n)	;
	string GetHitAreaName(int n);

	// 物理演算、パーツ切り替え、表情ファイルについて
	string GetPhysicsFile()	;
	string GetPoseFile() ;
	int GetExpressionNum() ;
	string GetExpressionFile(int n) ;
	string[] GetExpressionFiles() ;
	string GetExpressionName(int n) ;
	string[] GetExpressionNames() ;

	// モーションについて
	string[] GetMotionGroupNames()	;
	int GetMotionNum(string name)	;

	string GetMotionFile(string name,int n) ;
	string GetMotionSound(string name,int n) ;
	int GetMotionFadeIn(string name,int n) ;
	int GetMotionFadeOut(string name,int n) ;

	// 表示位置
	bool GetLayout(Dictionary<string, float> layout) ;
	
	// 初期パラメータについて
	int GetInitParamNum();
	float GetInitParamValue(int n);
	string GetInitParamID(int n);

	// 初期パーツ表示について
	int GetInitPartsVisibleNum();
	float GetInitPartsVisibleValue(int n);
	string GetInitPartsVisibleID(int n);
	
}