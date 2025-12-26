namespace URLShortener.Services.Models
{
    public abstract class AbstractModel (int id)
    {
        public int Id { get; set; } = id;
    }
}
