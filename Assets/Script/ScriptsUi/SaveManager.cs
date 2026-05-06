using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public SaveData CurrentData { get; private set; }
    

    private string Path => System.IO.Path.Combine(Application.persistentDataPath, "saveData.json");

    private float autoSaveInterval = 30f; //자동 저장 간격
    private float autoSaveTimer = 0f; //자동 저장 타이머
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        string dir = System.IO.Path.GetDirectoryName(Path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        Load(); 
    }
    private void Start()
    {
         
    }
    private void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if(autoSaveTimer >= autoSaveInterval)
        {
            Save();
            autoSaveTimer = 0f;
        }
    }

    public void Save()
    {
        if (CurrentData == null) return;
        string json = JsonConvert.SerializeObject(CurrentData, Formatting.Indented);
        File.WriteAllText(Path, json);
    }
    public void Load()
    {
        if (!File.Exists(Path))
        {
            CurrentData = new SaveData(); //새 게임 데이터 생성
            return;
        }
        string json = File.ReadAllText(Path);
        CurrentData = JsonConvert.DeserializeObject<SaveData>(json);
    }
    public void ResetData()
    {
        CurrentData = new SaveData();
        Save();
    }   

    //키 설정 값 저장 추가 예정.
    //New Input System 사용 예정

    private void OnApplicationQuit()
    {
        Save();
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        Save();
    }

    [ContextMenu("Save Test")]
    private void SaveTest()
    {
        Save();
        Debug.Log($"저장 완료: {Path}");
        Debug.Log(JsonConvert.SerializeObject(CurrentData, Formatting.Indented));
    }

    [ContextMenu("Load Test")]
    private void LoadTest()
    {
        Load();
        Debug.Log($"로드 완료");
        Debug.Log(JsonConvert.SerializeObject(CurrentData, Formatting.Indented));
    }
#endif



}

