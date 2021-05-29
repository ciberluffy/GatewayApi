using System;
using System.ComponentModel.DataAnnotations;

namespace MusalaSoft.GatewayApi.Model
{
    public class Device
    {
        [Key]
        [Required]
        public int UID { get; set; }

        public virtual Gateway Gateway { get; set; }

        [Required]
        public string Vendor { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public bool Online { get; set; } 
    }
}