function agregarNuevaTareaAlListado() {
    /*tareaListadoViewModel.tareas.push({titulo : 'nueva tarea'})*/

    tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel({ id: 0, titulo: '' }));

    //el titulo sera vacia pq esperaremos a que el usuario llene un titulo, ahora le permitiremos al usuarioe scribir el titulod e la tarea
    $("[name=titulo-tarea]").last().focus(); //jquery aopra que enfoque cuando pulsamos en nueva tarea
}

async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo(); //es un observable por eso usamos parentesis
    if (!titulo) {
        tareaListadoViewModel.tareas.pop();
        return;
    }

    //tarea.id(1); //prueba : pondremos un id a la tarwea para decir que es una tara existente


    const data = JSON.stringify(titulo); //mnecesitaos colocar en formato en json el tiktulod e la tarea pq le mandaremos a la web api que espera un JSON
    const respuesta = await fetch(urlTareas, {
        method: 'POST',
        body: data, //titulo serialozado a json
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id)
    } else {
        
        //mostrar mensaje de error
        manejarErrorApi(respuesta);
    }

}

async function obtenerTareas() {
    tareaListadoViewModel.cargando(true);

    const respuesta = await fetch(urlTareas, {
        method: 'GET',
        headers: {
            'Content-Type' : 'application/json'
        }
    })

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();
    tareaListadoViewModel.tareas([]);

    json.forEach(valor => {
        tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel(valor));
    }); 

    tareaListadoViewModel.cargando(false);
}


async function actualizarOrdenTareas() {
    const ids = obtenerIdsTareas();
    enviarIdsTareasAlBackend(ids);

    //ahora tenemos que reordenar las tareas pero en memoria ya lo hicimos en la bbd 
    const arregloOrdenado = tareaListadoViewModel.tareas.sorted(function (a, b) {
        return ids.indexOf(a.id().toString() - ids.indexOf(b.id().toString())); //infexofpara verificar los ids la posicion el indice
    });
}

function obtenerIdsTareas() {
    const ids = $("[name=titulo-tarea]").map(function () {
        return $(this).attr("data-id"); //data-id
    }).get(); //para volverlo un arreglo de ids

    return ids;
}

async function enviarIdsTareasAlBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTareas}/ordernar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function () {
    console.log('e')
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {  //ejecutara cuando terminemos de arrastrar la tarea

        }
    })
})