using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carkaashiv_angular_API.Models
{
    public class Part
    {
        [Key]

        [Column("part_id")] // Db backend column name, Primary key Auto-Increment
        public int PartId { get; set; } //frontend  form field name, 
        
        [Column("emp_id")]
        public int PEmpId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [Column("part_name")]
        public string PName { get; set; } = string.Empty;


        [Required, StringLength(1000)]
        [Column("part_detail")]
        public string PDetail { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        [Column("part_price")]
        public decimal PPrice { get; set; }


        [Required]
        [Range(0, int.MaxValue)]
        [Column("part_stock")]
        public int PStock { get; set; }

        [Required]
        [StringLength(500)]
        [Column("part_image_key")]
        public string Imagekey { get; set; } = string.Empty;

        [Required, StringLength(500)]
        [Column("part_image")]
        public string ImagePath { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime  CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class User
    {
        [Key]

        [Column("u_id")] // Db Column name
        public int Id { get; set; }  // Primary key Auto-increment 

        [Required, StringLength(50)]
        [Column("u_name")]
        public string? Name { get; set; }    // non-nullable with default

        [Required, StringLength(50)]
        [Column("u_phone")]
        public string? Phone { get; set; }


        [Required, StringLength(50)]
        [Column("u_email")]
        public String? Email { get; set; }

        [Required, StringLength(255)]
        [Column("u_pass")]
        public string? PasswordHash { get; set; }

        [Column("u_role")] 
        public string? Role { get; set; }
    }

    public class Employee
    {
        [Key]

        [Column("emp_id")] // Db backend column name, Primary key Auto-Increment
        public int Id { get; set; } //frontend  form field name, 

        [Required, StringLength(50)]
        [Column("emp_name")]
        public string? Name { get; set; }


        [Required, StringLength(50)]
        [Column("emp_phone")]
        public string? Phone { get; set; }

        [Required, StringLength(50)]       

        [Column("emp_email")]
        public string? Email { get; set; }

        [Required, StringLength(10)]
        [Column("emp_role")]
        public string? Role { get; set; }


        [Required, StringLength(255)]
        [Column("emp_pass")]
        public string? EmpPasswordHash { get; set; }

        public bool IsActive {  get; set; } = true; // Explicit
    }


    public class Cart
    {
        [Key]
        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("u_id")]
        public int UId { get; set; }

        [Column("part_id")]
        public int PartID { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("added_date")]
        public DateTime? AddedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public User? User { get; set; } // help Entity Framework understand relationships between tables.

        public Part? Part { get; set; } 

      }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("u_id")]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))] // navigation property support taking details of user by user_id
        public required User User { get; set; }

        [Column("subtotal_amount")]
        public decimal SubtotalAmount { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }
    
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("order_status")]
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;//Pending =1 /Confirmed /Shipped /Delivered

        [Column("invoice_number")]
        public string? InvoiceNumber {  get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("payment_method")]
        public string? PaymentMethod { get; set; } // UPI/Bank Transfer/COD
        [Column("payment_reference")]
        public string? PaymentReference { get; set; } //UTR /Ref number

        [Column("payment_proof_url")]
        public string? PaymentProofUrl { get; set; } // s3 /local upload path

        [Column("payment_proof_key")]                  
        public string? PaymentProofKey { get; set; }

        [Column("payment_status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
       
        [Column("payment_submitted_at")]
        public DateTime? PaymentSubmittedAt { get; set; }

        [Column("verified_amount")]
        public decimal? VerifiedAmount { get; set; }

        [Column("payment_mismatch_amount")]
        public decimal? PaymentMismatchAmount { get; set; }

        [Column("payment_verified_at")]
        public DateTime? PaymentVerifiedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string DeliveryName { get; set; } = string.Empty;

        public string DeliveryPhone { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        public string? Landmark { get; set; }

    }

    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("part_id")]
        public int PartId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }
        public Part Part { get; set; } = null!;

    }

    
    public class OrderPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = default!;

        [Column("payment_reference")]
        public string PaymentReference { get; set; } = default!;

        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }  
}
public class OrderIdempotency
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdempotencyKey")]
    public string IdempotencyKey { get; set; } = default!;

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("OrderId")]
    public int OrderId { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}
