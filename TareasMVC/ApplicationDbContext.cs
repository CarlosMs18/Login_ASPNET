using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContext :IdentityDbContext //ahora heredanis de ubdentitydbcontext nos permite definir cuales son las tablas
                                                    //que vamos a usar en neustra bbdd, trae tablas por defecto para el logeo de usuario, roles, infomraciones relcionados a usuarios
        
                                                /*DbContext*/ //EL APLICATIONDEBCONTEXT ES UNA CLASE QUE HEREDA DE DBCONTEXT, AHORA EL DBCONTEXT
                                                  //ES LA PIEZA CENTRAL RELACIONADA CON ENTITYFRAMEWORKCORE, AHORA A TRAVES DE CUALUUIER CLASE
                                                  //QYUE HEREDE DE DB CONTEXT PODEMOS CONFIGURAR LAS TABLAS DE NJUESTRA BASE DE DATOS Y ASIMIMO CONSULTA
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)//DBCONTEXT OPTIONS SIGNIFICA ALGUNAAS CONFIGURACIONES QUE PODEMOS PASAR
                                                                             //HACIA NUESTRO DBCONTEXT PARA ASI ENTOPNCES INDICAR POR EJEM PLO QUE QUEREMOS UTILIZAR SQL SERVER
                                                                             // O CUAL ES EL CONNEXTION STRINF DE NUESTRA INSTANCIA, HAY CONFIGURACIONES BASICAS QUE PASARMOS A TRVES 
                                                                             //DE ESE DBCONTEXT OPTIONS
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Tarea>().Property(t => t.Titulo).HasMaxLength(250).IsRequired(); //si lo modificamos por aca seriamos incaapz de usar el modelstate.isValid
        }

        public DbSet<Tarea> Tareas { get; set; }    //le decimos que queremos crear una tabla a partir de la calse tarea  y cuyo nombre
                                                    //de la tabla sera tarea
        public DbSet<Paso> Pasos { get; set; }  

        public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; } 

    }
}
