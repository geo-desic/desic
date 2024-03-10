namespace Desic.EntityFrameworkCore.Entities
{
    public class User : ModifiableEntity
    {
        public string? Username { get; set; }
        public bool IsActive
        {
            get => _iaActive ?? true;
            set => _iaActive = value;
        }
        public bool IsHidden
        {
            get => _isHidden ?? false;
            set => _isHidden = value;
        }
        private bool? _iaActive;
        private bool? _isHidden;
    }
}