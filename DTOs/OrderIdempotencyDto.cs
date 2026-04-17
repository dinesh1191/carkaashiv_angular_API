namespace carkaashiv_angular_API.DTOs
{
    public class OrderIdempotency
    {
        public int Id { get; set; }
        public string? IdempotencyKey { get; set; } 
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
