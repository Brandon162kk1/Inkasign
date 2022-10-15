using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Inkasign.Models
{

    [Table("t_proforma")]

    public class Proforma
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set;}
        public String UserID {get; set;}
        public Producto? Producto {get; set;}
        public int Cantidad{get; set;}
        public Decimal Precio { get; set; }
        public string Status { get; set; } = "PENDIENTE";
    }
}