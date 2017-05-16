namespace SecurityEssentials.Model
{
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Foreign keys
        public virtual Role Role { get; set; }

        public virtual User User { get; set; }
    }
}