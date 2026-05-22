using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Solo.Scripts.System.SaveSystem
{
    public class SaveManager
    {
        private static SaveManager _instance;
        public static SaveManager Instance => _instance ??= new SaveManager();
        public List<SaveInfo> SaveInfoList { get; set; } = new List<SaveInfo>();
        private static readonly string _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "solo", "save"); // C:\Users\用户名\AppData\Local\solo\save
        private static readonly string _saveInfoFileName = Path.Combine(_savePath, "save_info_list.json");// C:\Users\用户名\AppData\Local\solo\save\save_info_list.json
        private static readonly string _saveDataPath = Path.Combine(_savePath, "data");// C:\Users\用户名\AppData\Local\solo\save\
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        private SaveManager()
        {
            LoadSaveInfoList();
        }

        private void LoadSaveInfoList()
        {
            if (!File.Exists(_saveInfoFileName))
                return;
            try
            {
                string json = File.ReadAllText(_saveInfoFileName);
                SaveInfoList = JsonSerializer.Deserialize<List<SaveInfo>>(json, _jsonOptions);
            }
            catch (Exception e)
            {
                Godot.GD.Print($"[存档信息加载异常] : {e.Message}");
            }
        }

        public void CreateSave(string saveName)
        {
            try
            {
                Directory.CreateDirectory(_savePath);//创建存档根目录
                Directory.CreateDirectory(_saveDataPath);//创建存档数据根目录
                SaveInfo newInfo = new SaveInfo();
                newInfo.Id = Guid.NewGuid().ToString();
                newInfo.Name = saveName;
                newInfo.CreateDateTime = DateTime.Now;
                newInfo.LastSaveDateTime = DateTime.Now;
                newInfo.PlayerLevel = "一介凡人";
                newInfo.DataFileName = Path.Combine(_saveDataPath, $"{newInfo.Id}.json");
                File.WriteAllText(newInfo.DataFileName, "{}");
                SaveInfoList.Add(newInfo);
                WriteSaveInfoList();
            }
            catch (Exception e)
            {
                Godot.GD.Print($"[创建存档异常] : {e.Message}");
            }
        }

        public void RemoveSave(string saveInfoId)
        {
            SaveInfo removeInfo = SaveInfoList.Find(info => info.Id == saveInfoId);
            File.Delete(removeInfo.DataFileName);
            SaveInfoList.Remove(removeInfo);
            WriteSaveInfoList();
        }

        public void WriteSaveInfoList()
        {
            try
            {
                string json = JsonSerializer.Serialize(SaveInfoList, _jsonOptions);
                File.WriteAllText(_saveInfoFileName, json);
            }
            catch (Exception e)
            {
                Godot.GD.Print($"[写入存档信息异常] : {e.Message}");
            }
        }
        public SaveData CurSaveData = null;//外部拿这个恢复游戏, 游戏数据改变时也改这个的数据, 最后退出保存时把这个序列化写入本地
        private string _curSaveDataFileName;
        public void LoadSaveData(string saveInfoId)
        {
            try
            {
                SaveInfo curSaveInfo = SaveInfoList.Find(info => info.Id == saveInfoId);
                _curSaveDataFileName = curSaveInfo.DataFileName;
                string json = File.ReadAllText(_curSaveDataFileName);
                CurSaveData = JsonSerializer.Deserialize<SaveData>(json, _jsonOptions);
            }
            catch (Exception e)
            {
                Godot.GD.Print($"[加载存档数据异常] : {e.Message}");
            }
        }
        public void WriteCurSaveData()
        {
            try
            {
                string json = JsonSerializer.Serialize(CurSaveData, _jsonOptions);
                File.WriteAllText(_curSaveDataFileName, json);
            }
            catch (Exception e)
            {
                Godot.GD.Print($"[写入存档数据异常] : {e.Message}");
            }
        }
    }
}