
using live2d;
using live2d.framework;
using UnityEngine;

class PlatformManager : IPlatformManager
{
    public byte[] loadBytes(string path)
	{
		var assetsPath = path.Replace(".json","");
		return FileManager.LoadBin(assetsPath);
    }

    public string loadString(string path)
	{
		var assetsPath = path.Replace(".json","");
		return FileManager.LoadString(assetsPath);
    }

    public ALive2DModel loadLive2DModel(string path)
    {
		var data = FileManager.LoadBin(path);
        var live2DModel = Live2DModelUnity.loadModel(data);

        return live2DModel;
    }

    public void loadTexture(live2d.ALive2DModel model, int no, string path)
    {
        if (LAppDefine.DEBUG_LOG) Debug.Log("Load texture " + path);
		var texPath = path.Replace (".png", "");
		Texture2D texture = FileManager.LoadTexture(texPath);

        ((Live2DModelUnity)model).setTexture(no, texture);
    }

    public void log(string txt)
    {
        Debug.Log(txt);
    }
}