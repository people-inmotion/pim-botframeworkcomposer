using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace com.pim_clientes.comunidad.flutter.web.Controllers
{
    public class Cliente
    {
        public int id { get; set; }
        public string empresa { get; set; }
        public string nombreContacto { get; set; }
        public string telefonoContacto { get; set; }
        public string correoContacto { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Cliente>> Get()
        {
            List<Cliente> clientes = new List<Cliente>();
            clientes.Add(new Cliente
            {
                id = 1,
                empresa = "Empresa 1",
                nombreContacto = "Contacto 1",
                telefonoContacto = "2441111",
                correoContacto = "administracion@empresa1.com"
            });
            clientes.Add(new Cliente
            {
                id = 2,
                empresa = "Empresa 2",
                nombreContacto = "Contacto 2",
                telefonoContacto = "2441111",
                correoContacto = "administracion@empresa1.com"
            });
            clientes.Add(new Cliente
            {
                id = 3,
                empresa = "Empresa 3",
                nombreContacto = "Contacto 3",
                telefonoContacto = "2441111",
                correoContacto = "administracion@empresa3.com"
            });
            clientes.Add(new Cliente
            {
                id = 4,
                empresa = "Empresa 4",
                nombreContacto = "Contacto 4",
                telefonoContacto = "2441111",
                correoContacto = "administracion@empresa4.com"
            });
            clientes.Add(new Cliente { id = 5, empresa = "Empresa 5", nombreContacto = "Contacto 5", telefonoContacto = "2441111", correoContacto = "administracion@empresa5.com" });

            return clientes;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Cliente> Get(int id)
        {
            Cliente cliente = new Cliente
            {
                id = id,
                empresa = $"Empresa {id}",
                nombreContacto = $"Contacto {id}",
                telefonoContacto = "2441111",
                correoContacto = $"administracion@empresa{id}.com"
            };

            return cliente;
        }

        // POST api/values
        [HttpPost]
        public int Post([FromBody] Cliente record)
        {
            return 10;
        }
    }
}
