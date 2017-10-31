/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System;
using live2d;

[ExecuteInEditMode]
public class LAppModelProxy : MonoBehaviour
{
    public String path="";
    public int sceneNo = 0;

    public LAppModel model;

	private bool isVisible = false ;

	void Awake()
    {
        if (path == "") return;
        model = new LAppModel(this);


        LAppLive2DManager.Instance.AddModel(this);

        var filename = FileManager.getFilename(path);
        var dir = FileManager.getDirName(path);

        Debug.Log("Load " + dir +"  filename:"+ filename);
		    model.LoadFromStreamingAssets(dir, filename);
    }


	void OnRenderObject()
	{
		if(!isVisible) return;
		if(model==null)return;

		if(model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH_NOW)
		{
      model.Draw();
		}

		if (LAppDefine.DEBUG_DRAW_HIT_AREA)
		{
			// デバッグ用当たり判定の描画
			model.DrawHitArea();
		}
	}


	void Update()
	{
		if(!isVisible) return;
        if (model == null) return;

        model.Update();
        if (model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH)
        {
            model.Draw();
        }
	}


    public LAppModel GetModel()
    {
        return model;
    }


	public void SetVisible(bool isVisible)
	{
		this.isVisible = isVisible;
	}


	public bool GetVisible()
	{
		return isVisible;
	}


    public void ResetAudioSource()
    {
        Component[] components = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < components.Length; i++)
        {
            Destroy(components[i]);
        }
    }
}