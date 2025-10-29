using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carkaashiv_angular_API.Models
{
    public class TablePart
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
    public class TableEmployee
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


        [Column("idt")]  // date time on idt column  db 
        public DateTime CreatedAt { get; set; }

    }
}
