using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TareasController(
            ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios,
            IMapper mapper
            )
        {
          
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        //[HttpGet]
        //public async Task<List<Tarea>> Get()
        //{
        //    var usuarioId = servicioUsuarios.ObtenerUsuarioId();


        //    var tareas = await context.Tareas
        //        .Where(t => t.UsuarioCreacionId == usuarioId)
        //        .OrderBy(t => t.Orden)
        //        .Select(t => new
        //        {
        //            t.Id,  //estamois colocano un tipo anonimo tenemos quecambiar la funcion lo cambiaremos
        //            t.Titulo
        //        })
        //        .ToListAsync();
        //    return tareas;

        //}

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var usuarioId = servicioUsuarios.ObtenerUsuarioId();


        //    var tareas = await context.Tareas
        //        .Where(t => t.UsuarioCreacionId == usuarioId)
        //        .OrderBy(t => t.Orden)
        //        .Select(t => new
        //        {
        //            t.Id,  
        //            t.Titulo
        //        })
        //        .ToListAsync();
        //    return Ok(tareas);

        //}

        //CON MODELOD E DATOS ESPECIALIZADO Y DEVOLVEINDO 
        [HttpGet]
        public async Task<List<TareaDTO>> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();


            var tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId)
                .OrderBy(t => t.Orden)
                .ProjectTo<TareaDTO>(mapper.ConfigurationProvider) ///LE DECIMS A ENTIYFRAMEOWRK CORE QUE UTILIZ
                //.Select(t => new TareaDTO
                //{
                //    Id = t.Id,
                //    Titulo = t.Titulo
                //})
                .ToListAsync();
            return tareas;

        }



        [HttpPost] //action result puede retortnar un actionresult o una tarea
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            //ANYASYNC PARA VERFICIAR SI EXISTE UNA O TAREA
            var existenTareas = await context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;
            if (existenTareas)
            {
                ordenMayor = await context.Tareas.Where(t => t.UsuarioCreacionId == usuarioId)
                                .Select(t => t.Orden).MaxAsync();
            }

            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1
             };

            context.Add(tarea); //no agrega como tal , sino que la marca que sera agregadaen el momento que guardemos los cambios
            await context.SaveChangesAsync();

            return tarea; //devuelvo la tarea recien creada , hay varias formas para devolver un registro creado
                    //emn nuestro caso sera asi, extraeremos el di se lo pondremos al viweqmodel
        }

        [HttpPost("ordenar")]
         public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tareas = await context.Tareas.Where(t => t.UsuarioCreacionId == usuarioId).ToListAsync();

            var tareasId = tareas.Select(x => x.Id);

            var idsTareasNoPertenecenAlUsuario = ids.Except(tareasId).ToList();
            if (idsTareasNoPertenecenAlUsuario.Any()) //si hay algun eleemtno ahi
            {
                return Forbid();
            }
            var tareasDiccionario = tareas.ToDictionary(x => x.Id); //para tener de manera comoda cada tarea en
                                            //un diccionario donde la llave sera el id
            for(int i = 0; i < ids.Length; i++) { 
                var id = ids[i];
                var tarea = tareasDiccionario[id];
                tarea.Orden = i + 1;

            }
            await context.SaveChangesAsync();
            return Ok();
        }      
    }
}
