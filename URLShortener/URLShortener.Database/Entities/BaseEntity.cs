namespace URLShortener.Services.Database.Entities
{
    public abstract class BaseEntity (int id)
    {
        public int Id { get; set; } = id;
    }
}
