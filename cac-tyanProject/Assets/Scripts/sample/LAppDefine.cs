/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */

public class LAppDefine
{
	// デバッグ。trueのときにログを表示する。
	public static bool DEBUG_LOG = false;
  public static bool DEBUG_TOUCH_LOG = false;
	public static bool DEBUG_DRAW_HIT_AREA = false;

	//  全体の設定-------------------------------------------------------------------------------------------
	// 画面
	public const float VIEW_MAX_SCALE = 2f;
	public const float VIEW_MIN_SCALE = 0.8f;

	public const float VIEW_LOGICAL_LEFT = -1;
	public const float VIEW_LOGICAL_RIGHT = 1;

	public const float VIEW_LOGICAL_MAX_LEFT = -2;
	public const float VIEW_LOGICAL_MAX_RIGHT = 2;
	public const float VIEW_LOGICAL_MAX_BOTTOM = -2;
	public const float VIEW_LOGICAL_MAX_TOP = 2;

	public const float SCREEN_WIDTH = 20.0f;
	public const float SCREEN_HEIGHT = 20.0f;




	// 外部定義ファイル(json)と合わせる
	public const string MOTION_GROUP_IDLE		= "idle";		// アイドリング
	public const string MOTION_GROUP_TAP_BODY	= "tap_body";	// 体をタップしたとき
	public const string MOTION_GROUP_TAP_HEAD	= "tap_head";	// 頭をタップしたとき
	public const string MOTION_GROUP_FLICK_BODY	= "flick_body";	// 頭を撫でた時
	public const string MOTION_GROUP_FLICK_HEAD	= "flick_head";	// 頭を撫でた時
	public const string MOTION_GROUP_PINCH_IN	= "pinch_in";	// 拡大した時
	public const string MOTION_GROUP_PINCH_OUT	= "pinch_out";	// 縮小した時
	public const string MOTION_GROUP_SHAKE		= "shake";		// シェイク

	// 外部定義ファイル(json)と合わせる
	public const string HIT_AREA_HEAD	= "head";
	public const string HIT_AREA_BODY	= "body";

	// モーションの優先度定数
	public const int PRIORITY_NONE	 = 0;
	public const int PRIORITY_IDLE	 = 1;
	public const int PRIORITY_NORMAL = 3;
 	public const int PRIORITY_TALK	 = 4;
	public const int PRIORITY_FORCE	 = 4;
}