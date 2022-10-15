using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Inkasign.Data;
using Inkasign.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Inkasign.Controllers
{
    public class PagoController : Controller
    {
        private readonly IConverter _converter;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PagoController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, IConverter converter)
        {      
             _converter = converter;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(Decimal monto)
        {
            Pago pago = new Pago();
            pago.UserID = _userManager.GetUserName(User);
            pago.MontoTotal = monto;
            return View(pago);
        }
        public IActionResult Confirmacion(){

            return View();
        }

        [HttpPost]

        public IActionResult Pagar(Pago pago){
            
            pago.PaymentDate = DateTime.UtcNow;
            pago.UserID = _userManager.GetUserName(User);
            _context.Add(pago);


            var itemsProforma = from o in _context.DataProforma select o;
            itemsProforma = itemsProforma.
                Include(p => p.Producto).
                Where(p => p.UserID.Equals(pago.UserID) && p.Status.Equals("PENDIENTE"));
             

             Console.WriteLine("sadasdsad");

            Pedido pedido = new Pedido();
            pedido.UserID = pago.UserID;
            pedido.Total = pago.MontoTotal;
            pedido.archivo=GeneratePdfReport(itemsProforma.ToList());
            pedido.Pago = pago;
            _context.Add(pedido);


            List<DetallePedido> itemsPedido = new List<DetallePedido>();
            foreach(var item in itemsProforma.ToList()){
                DetallePedido detallePedido = new DetallePedido();
                detallePedido.Pedido=pedido;
                detallePedido.Precio = item.Precio;
                detallePedido.Producto = item.Producto;
                detallePedido.Cantidad = item.Cantidad;
                itemsPedido.Add(detallePedido);
            }

            _context.AddRange(itemsPedido);

            foreach (Proforma p in itemsProforma.ToList())
            {
                p.Status="Procesado";
            }
            _context.UpdateRange(itemsProforma);

            _context.SaveChanges();





            ViewData["Message"] = "¡PAGO REALIZADO!";
                     
            return RedirectToAction("Confirmacion");


        }
        

        public byte[] GeneratePdfReport(List<Proforma> proformas)
{   

     var total = proformas.Sum(c => c.Cantidad * c.Precio );

     var cabecera = $@"
     
      <!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC"" crossorigin=""anonymous"">

    <title>Reporte de Pago</title>
</head>
<body>
 
    <div class=""container mt-10"">
       <div class=""flex-header"">
        <img src=""~/images/logo.jpg"" alt="" width=""15%"">
        <div class=""texto"">
          <p> <span clas=""fw-bold"">N° Boleta: </span> 9484848484</p>
          <p><span clas=""fw-bold"">Direccion: </span> Calle Camino Real 1801 Surco</p>
          <p><span clas=""fw-bold"">Ruc: </span> 20603301871</p>
        </div>
    </div>
     <div class=""card mb-5"">
            <div class=""card-header"">
              Resumen de Compra
            </div>
            <div class=""card-body"">
              <h5 class=""card-title"">Gracias por Confiar en nosotros</h5>
              <p class=""card-text"">Cualquier consulta contactarnos al: +511 652 1010 - 989 302 342</p>
            </div>
          </div>
    <table class=""table table-striped"">
       <thead>
          <tr>
           <th>Nombre</th>
           <th>Cantidad</th>
           <th>Precio</th>
           <th>Imagen</th>
          </tr>
       </thead>
       <tbody>
     
     ";

     var nombre = "";
     var cantidad = 0;
     var precio = new decimal();

      proformas.ForEach( item => {
          

          nombre = item.Producto.Name;
          cantidad = item.Cantidad;
          precio = item.Precio;

         var cuerpoTabla = $@"
         
            <tr>
              
               <td>{nombre}</td>
               <td>{cantidad}</td>
               <td>{precio}</td>
               <td><img width=""30%"" src=""{item.Producto.ImageName}""></td>
            </tr>
         
         "; 

         cabecera+=cuerpoTabla;    
  

      });


     var footer = $@"
     
          </tbody>
         </table>
           <div class=""mt-5"">
        <p class=""text-end fs-1"">El monto a pagar fue: S./{total}</p>
    
    
     </div>
       </div>
    
      </body>
   </html>
     
     ";

     cabecera += footer; 

  
GlobalSettings globalSettings = new GlobalSettings();
globalSettings.ColorMode = ColorMode.Color;
globalSettings.Orientation = Orientation.Portrait;
globalSettings.PaperSize = PaperKind.A4;
 globalSettings.Margins = new MarginSettings { Top = 25, Bottom = 25 };
 ObjectSettings objectSettings = new ObjectSettings();
 objectSettings.PagesCount = true;
 objectSettings.HtmlContent = cabecera;
 WebSettings webSettings = new WebSettings();
 webSettings.DefaultEncoding = "utf-8";
 HeaderSettings headerSettings = new HeaderSettings();
 headerSettings.FontSize = 18;
 headerSettings.FontName = "Ariel";
 headerSettings.Right = "Boleta Electronica";
 headerSettings.Line = true;
 FooterSettings footerSettings = new FooterSettings();
 footerSettings.FontSize = 18;
 footerSettings.FontName = "Ariel";
 footerSettings.Center = "Este recibo sirve para cualquier reclamo";
 footerSettings.Line = true;
 objectSettings.HeaderSettings = headerSettings;
 objectSettings.FooterSettings = footerSettings;
 objectSettings.WebSettings = webSettings;

 
 HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
 {
      GlobalSettings = globalSettings,
      Objects = { objectSettings },
 };
  return _converter.Convert(htmlToPdfDocument);
}
    }
}