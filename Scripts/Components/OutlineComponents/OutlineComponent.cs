using Godot;
using Solo.Scripts.Components.Core;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Components.OutlineComponents
{
    internal class OutlineComponent : Component
    {
        //拿到owner的sprite
        //拿到sprite下的shader
        //拿到owner的namelabel
        private Node2D _sprite = null!;
        private ShaderMaterial _shaderMaterial = null!;
        private Label _nameLb = null!;
        public OutlineComponent(IEntity owner) : base(owner)
        {

        }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {

        }
        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }

        public void SetNodes(Node2D sprite, Label infoLb)
        {
            _sprite = sprite;
            if (_sprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();
                _sprite.Material = _shaderMaterial;
            }
            _nameLb = infoLb;
            Show(false);
        }
        public void Show(bool isShow)
        {
            if (isShow)
            {
                _nameLb.Visible = true;
                _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
                _shaderMaterial.SetShaderParameter("outline_width", 1);
            }
            else
            {
                _nameLb.Visible = false;
                _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
            }
        }
    }
}
