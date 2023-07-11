﻿using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }

        [StringLength(250)] //SIN USAR EL APIFLUENTE
        [Required] //campo no null
        public string Titulo { get; set; } 
        public string Descripcion { get; set; } 
        public int Orden { get; set; }  
        public DateTime FechaCreacion { get; set; }     

        public List<Paso> Pasos { get; set; }   

        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; }  
    }
}