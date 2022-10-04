using UnityEngine;

public abstract class DataManager<T> : MonoBehaviour where T : Data, new()
{
    protected T data = new T();
    protected FileController fileController = new FileController();

    public ref T GetData() { return ref data; }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        data.Init();
    }

    public abstract void Open(string filepath);
    public abstract void Save();

    public void SaveAs(string filepath)
    {
        fileController.Filepath = filepath;
        Save();
    }
    public void Delete()
    {
        fileController.DeleteFile();
    }
}