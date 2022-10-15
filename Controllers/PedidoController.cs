using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inkasign.Models;
using Inkasign.Data;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;

namespace Inkasign.Controllers
{

    [Authorize(Roles = "Admin")]
    public class PedidoController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        
        public PedidoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ElegirAccion(){
            return View();
        }

        public IActionResult BuscarPedidos(){
            return View();
        }

        public IActionResult ListarPedidos(string usuario, decimal precioMenor, decimal precioMayor, string fechaInicio, string fechaFin)
        {   
            var lista = _context.DataPedido.Include(p => p.Pago).ToList();

            var query1 = lista;
            var query2 = lista;
            var query3 = lista;

            if(usuario != null){
                query1 = query1.Where(p => p.UserID.Contains(usuario)).ToList();
            }

            if(precioMenor > 0 || precioMayor > 0){
                
                if(precioMenor == 0){
                    query2 = query2.Where(p => p.Total <= precioMayor).ToList();
                }else if(precioMayor == 0){
                    query2 = query2.Where(p => p.Total >= precioMenor).ToList();
                }else{
                    query2 = query2.Where(p => p.Total >= precioMenor && p.Total <= precioMayor).ToList();
                }

            }

            if(fechaInicio != null || fechaFin != null){
                
                if(fechaInicio == null){
                    query3 = query3.Where(p => p.Pago.PaymentDate >= DateTime.ParseExact("0001-01-01", "yyyy-MM-dd", null) && p.Pago.PaymentDate <= DateTime.ParseExact(fechaFin, "yyyy-MM-dd", null)).ToList();
                }else if(fechaFin == null){
                    query3 = query3.Where(p => p.Pago.PaymentDate >= DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", null) && p.Pago.PaymentDate <= DateTime.Now).ToList();
                }else{
                    query3 = query3.Where(p => p.Pago.PaymentDate >= DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", null) && p.Pago.PaymentDate <= DateTime.ParseExact(fechaFin, "yyyy-MM-dd", null)).ToList();
                }

            }

            var resultado = query1.Intersect(query2).Intersect(query3).ToList();

            return View(resultado);
        }

        public IActionResult DetallePedido(int id){

            var pedido = _context.DataPedido.Include(p => p.Pago).FirstOrDefault(p => p.Id == id);
            var detalle = _context.DataDetallePedido.Include(d => d.Producto).Include(d => d.Pedido).Where(d => d.Pedido.Id == id).ToList();

            dynamic model = new ExpandoObject();
            
            model.pedido = pedido;
            model.detalle = detalle;
             

            return View(model);
        }

        public IActionResult Eliminar(int id){

            var detalle = _context.DataDetallePedido.Where(d => d.Pedido.Id == id).ToList();

            foreach (var item in detalle)
            {
                _context.Remove(item);
            }

            var pedido = _context.DataPedido.FirstOrDefault(p => p.Id == id);
            _context.Remove(pedido);
            _context.SaveChanges();

            return RedirectToAction(nameof(ListarPedidos));

        }

       public IActionResult DownLoadPedidos(int id){
        
           var pedidos = _context.DataPedido.Find(id);
           byte[] archivo = pedidos.archivo;
           
           if(archivo == null){
               return NotFound();
           }
           
           return File(archivo, "application/octet-stream", "Boleta.pdf");  
          

       }

    }
}