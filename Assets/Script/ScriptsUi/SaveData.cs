using UnityEngine;
using System.Collections.Generic;

//세이브 데이터 클래스
//설정 관련 저장을 최소치로 잡고 키 설정/fov는 추후 추가할 예정
public class SaveData
{
    public float bgmVolume = 0.1f;
    public float seVolume = 0.5f;
    public int targetFPS = 60;
    public float totalPlayTime = 0f;
    public float fov = 80f;
    public int stageClearCount = 0;
    public Dictionary<string, string> keyBindings = new();  
      
}
