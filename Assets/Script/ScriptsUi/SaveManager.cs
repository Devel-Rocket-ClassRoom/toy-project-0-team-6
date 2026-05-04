using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public SaveData CurrentData { get; private set; }
    

    private string Path => System.IO.Path.Combine(Application.persistentDataPath, "saveData.json");

    private float autoSaveInterval = 30f; //자동 저장 간격
    private float autoSaveTimer = 0f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }
    private void Start()
    {
        string dir = System.IO.Path.GetDirectoryName(Path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        Load();
    }
    private void Update()
    {
        autoSaveTimer = Time.unscaledDeltaTime;
        if(autoSaveTimer >= autoSaveInterval)
        {
            Save();
            autoSaveTimer = 0f;
        }
    }

    private void Save()
    {
        if (CurrentData == null) return;
        string json = JsonConvert.SerializeObject(CurrentData, Formatting.Indented);
        File.WriteAllText(Path, json);
    }
    private void Load()
    {
        if (!File.Exists(Path))
        {
            CurrentData = new SaveData(); //새 게임 데이터 생성
            return;
        }
        string json = File.ReadAllText(Path);
        CurrentData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    private void OnApplicationQuit()
    {
        Save();
    }



}

