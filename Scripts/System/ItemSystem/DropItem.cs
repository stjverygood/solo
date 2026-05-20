using Godot;
using Solo.Scripts.Global;
using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Solo.Scripts.System.ItemSystem
{
    //全部掉落物scene都用这个脚本
    public partial class DropItem : Node2D
    {
        [Export] public Label TextLb;
        [Export] public Sprite2D IconSprite;
        public ItemData Data;

        public override void _Ready()
        {
            
        }

        //public override void _Process(double delta)
        //{
        //}

        public void Init(ItemData data)
        {
            Data = data;
            TextLb.Text = Data.Type.ToString();
            TextLb.Visible = false;
            Texture2D texture = GD.Load<Texture2D>(Data.IconPath);
            Vector2 targetSize = new Vector2(8, 8);
            Vector2 texSize = texture.GetSize(); // 获取图片实际的像素大小
            IconSprite.Scale = new Vector2(targetSize.X / texSize.X, targetSize.Y / texSize.Y);// 计算缩放比例：目标尺寸 / 图片实际尺寸
            IconSprite.Texture = texture;
        }

        public void ShowText(bool isShow)
        {
            TextLb.Visible = isShow;
        }
    }
}


