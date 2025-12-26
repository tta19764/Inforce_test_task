namespace URLShortener.Services.Database.Entities
{
    public class AccountType (int id = 0) : BaseEntity (id)
    {
        public string TypeName { get; set; } = null!;

        public virtual IList<User> Users { get; set; } = [];
    }
}
