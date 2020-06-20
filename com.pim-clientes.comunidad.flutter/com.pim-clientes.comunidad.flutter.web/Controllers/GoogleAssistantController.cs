using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace com.pim_clientes.comunidad.flutter.web.Controllers
{
    public class FulfillmentResponse
    {
        public string fulfillmentText { get; set; }
        public string source { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public Google google { get; set; }
    }

    public class Google
    {
        public bool expectUserResponse { get; set; }
        public Richresponse richResponse { get; set; }
    }

    public class Richresponse
    {
        public Item[] items { get; set; }
        public Suggestion[] suggestions { get; set; }
    }

    public class Item
    {
        public Simpleresponse simpleResponse { get; set; }
    }

    public class Simpleresponse
    {
        public string textToSpeech { get; set; }
    }

    public class Suggestion
    {
        public string title { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAssistantController : ControllerBase
    {
        private string GetGoogleAssistantLocalTelephone(string telephone)
        {
            string response = telephone;

            if (telephone.Length == 8)
                response = telephone.Substring(0, 4) + "-" + telephone.Substring(4, 4);
            else if (telephone.Length == 7)
                response = telephone.Substring(0, 3) + "-" + telephone.Substring(3, 4);

            return response;
        }

        /// <summary>
        /// Prueba si el controlador funciona
        /// </summary>
        /// <returns>Mensaje</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok("It's alive!");
        }
        //[HttpGet]
        //[Route("Ping")]
        //public HttpResponseMessage Ping()
        //{
        //    return new HttpResponseMessage(HttpStatusCode.OK);
        //}

        /// <summary>
        /// Registrar un préstamo
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Fullfillment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult PostFullfillment([FromBody] dynamic fullfillment)
        {
            string userId = fullfillment.originalDetectIntentRequest.payload.user.userId;
            string session = fullfillment.session;
            string responseId = fullfillment.responseId;

            string empresaNombre = "";
            string contactoNombre = "";
            string contactoCelular = "";
            string contactoCorreoE = "";
            int empresaId = 0;

            string intent = fullfillment.queryResult.intent.displayName;
            StringBuilder respuestaMensaje = new StringBuilder();

            var queryResult = fullfillment.queryResult.outputContexts;

            if (intent == "EMPRESA_CONSULTAR")
            {
                foreach (var children in queryResult)
                {
                    string name = children.name;
                    if (name.EndsWith("/contexts/google_assistant_input_type_voice"))
                    {
                        empresaId = children.parameters.EMPRESA_ID;
                    }
                }
            }
            else if (intent == "EMPRESA_AGREGAR")
            {
                foreach (var children in queryResult)
                {
                    string name = children.name;
                    if (name.EndsWith("/contexts/google_assistant_input_type_voice"))
                    {
                        empresaNombre = children.parameters.EMPRESA_NOMBRE;
                        contactoNombre = children.parameters.CONTACTO_NOMBRE;
                        contactoCelular = children.parameters.CONTACTO_CELULAR;
                        contactoCorreoE = children.parameters.CONTACTO_CORREO_E;
                    }
                }
            }

            ValuesController valuesController = new ValuesController();

            if (intent == "EMPRESA_AGREGAR") {
                Cliente cliente = new Cliente();
                cliente.id = 0;
                cliente.empresa = empresaNombre;
                cliente.nombreContacto = contactoNombre;
                cliente.telefonoContacto = contactoCelular;
                cliente.correoContacto = contactoCorreoE;

                empresaId = valuesController.Post(cliente);

                respuestaMensaje.AppendLine("La empresa " + empresaNombre + " ha sido creada satisfactoriamente. Su identificador es " + empresaId.ToString());

            }
            else if (intent == "EMPRESA_CONSULTAR") {
                //Cliente cliente = new Cliente();
                var cliente = valuesController.Get(empresaId);

                respuestaMensaje.AppendLine("Los datos de la empresa consultada son:");
                respuestaMensaje.AppendLine("Nombre: " + cliente.Value.empresa);
                respuestaMensaje.AppendLine("Contacto: " + cliente.Value.nombreContacto);
                respuestaMensaje.AppendLine("Celular: " + GetGoogleAssistantLocalTelephone(cliente.Value.telefonoContacto));
                respuestaMensaje.AppendLine("Correo electrónico: " + cliente.Value.correoContacto);

            }

            FulfillmentResponse fullfillmentResponse = new FulfillmentResponse();
            fullfillmentResponse.fulfillmentText = respuestaMensaje.ToString();
            fullfillmentResponse.source = "people-inmotion.com";
            fullfillmentResponse.payload = new Payload
            {
                google = new Google
                {
                    expectUserResponse = true,
                    richResponse = new Richresponse
                    {
                        items = new Item[] {
                            new Item {
                                simpleResponse = new Simpleresponse {
                                    textToSpeech = respuestaMensaje.ToString()
                                }
                            }
                        },
                        suggestions = new Suggestion[] {
                            new Suggestion { title = "Agregar empresa" }
                            ,new Suggestion { title = "Consultar empresa" }
                        }
                    }
                }
            };

            return Ok(fullfillmentResponse);
        }
    }
}
