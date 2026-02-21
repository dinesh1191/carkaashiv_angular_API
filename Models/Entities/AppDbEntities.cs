using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carkaashiv_angular_API.Models
{
    public class Part
    {
        [Key]

        [Column("part_id")] // Db backend column name, Primary key Auto-Increment
        public int PartId { get; set; } //frontend  form field name, 

       // [MinLength(3, ErrorMessage = "Employee Id Required")]
        [Required]
        [Column("emp_id")]
        public int? PEmpId { get; set; }

        [MinLength(3, ErrorMessage = "Name must 3 characters minimum,maximum 25 characters")]
        [Column("part_name")]
        public string? PName { get; set; }


        [Required, StringLength(100)]
        [Column("part_detail")]
        public string? PDetail { get; set; }

        [Required]
        [Column("part_price")]
        public decimal? PPrice { get; set; }


        [Required]
        [Column("part_stock")]
        public int? PStock { get; set; }

        [Required, StringLength(100)]
        [Column("part_image")]
        public string? ImagePath { get; set; }

        [Column("idt")]  // date time on idt column  db 
        public DateTime CreatedAt { get; set; }

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

    }




    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("u_id")]
        public int UId { get; set; }

        [Column("total_amount")]
        public decimal? TotalAmount { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

    }

    public class OrderItems
    {
        [Key]
        [Column("order_item_id")]
        public int? OrderItemId { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; }

        [Column("part_id")]
        public int? PartId { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("unit_price")]
        public decimal? UnitPrice { get; set; }

        [Column("total_price")]
        public decimal? totalPrice { get; set; }

    }

}
