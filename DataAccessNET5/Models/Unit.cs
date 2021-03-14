using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Unit")]
    public partial class Unit
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
