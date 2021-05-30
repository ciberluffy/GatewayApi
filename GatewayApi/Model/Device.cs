using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusalaSoft.GatewayApi.Model
{
    public class Device
    {
        [Key]
        public int UID { get; set; }

        public virtual Gateway Gateway { get; set; }

        [Required]
        public string Vendor { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public bool Online { get; set; } 
    }
}