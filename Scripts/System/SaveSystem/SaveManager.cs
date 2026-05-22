using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Solo.Scripts.System.SaveSystem
{
    public class SaveManager
    {
        private static SaveManager _instance;
        public static SaveManager Instance => _instance ??= new SaveManager();
        public List<SaveInfo> SaveInfoList { get; set; } = new List<SaveInfo>();
        private static readonly string _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "solo", "save"); // C:\Users\用户名\AppData\Local\solo\save
        private static readonly string _saveInfoFileName = Path.Combine(_savePath, "save_info_list.json");// C:\Users\用户名\AppData\Local\solo\save\save_info_list.json
        private static readonly string _saveDataPath = Path.Combine(_savePath, "save", "data");// C:\Users\用户名\AppData\Local\solo\save\
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

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
                SaveInfoList = JsonSerializer.Deserialize<List<SaveInfo>>(json, JsonOptions);
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
                if (!Directory.Exists(_savePath))
                    Directory.CreateDirectory(_savePath);//创建存档根目录
                if (!Directory.Exists(_saveDataPath))
                    Directory.CreateDirectory(_saveDataPath);//创建存档数据根目录
                SaveInfo newInfo = new SaveInfo();
                newInfo.Id = Guid.NewGuid().ToString();
                newInfo.Name = saveName;
                newInfo.CreateDateTime = DateTime.Now;
                newInfo.LastSaveDateTime = DateTime.Now;
                newInfo.PlayerLevel = "一介凡人";
                newInfo.DataFileName = $"{newInfo.Id}";
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"[SaveManager] 创建存档异常: {e.Message}");
            }
        }

        public void RemoveSave(string saveId)
        {
            // 纯 C# 删除文件和文件夹示例：
            // File.Delete(filePath);
            // Directory.Delete(dirPath, true);
        }
    }
}