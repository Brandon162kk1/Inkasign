using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Inkasign.Models;


namespace Inkasign.Data;



public class ApplicationDbContext : IdentityDbContext

{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

        : base(options)

    {

    }



    public DbSet<Inkasign.Models.Contacto> DataContactos { get; set; }

    public DbSet<Inkasign.Models.Producto> DataProductos { get; set; }

    public DbSet<Inkasign.Models.Proforma> DataProforma { get; set; }

    public DbSet<Inkasign.Models.Pago> DataPago { get; set; }
    public DbSet<Inkasign.Models.Pedido> DataPedido { get; set; }

    public DbSet<Inkasign.Models.DetallePedido> DataDetallePedido { get; set; }

    public DbSet<Inkasign.Models.Contacto> DataContacto { get; set; }




}