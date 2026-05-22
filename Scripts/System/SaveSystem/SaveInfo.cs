using System;

namespace Solo.Scripts.System.SaveSystem
{
    public class SaveInfo
    {
        //存档的概要信息, 用于全量加载渲染UI, 避免把全部存档的全部信息加载了
        public string Id { get; set; }//guidStr
        public string Name { get; set; }//存档名字
        public DateTime CreateDateTime { get; set; }//存档创建时间
        public DateTime LastSaveDateTime { get; set; }//存档上次保存时间
        public string PlayerLevel { get; set; }//玩家境界
        public string DataFileName { get; set; }//存档数据路径
    }
}
