namespace TareasMVC.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; }    //guid identificasdor global unico

        public int TareaId { get; set; }    //al decir tareaId ACA Y ABAJO TAREA YA SE CONFIGURO COMO UNA LLAVE FORNAEA QUE APJNTA HACIA TAREA


        
        public Tarea Tarea { get; set; }    

        public string Descripcion { get; set; }
        public bool Realizado { get; set; } 
        public int Orden { get; set; }
    }
}
