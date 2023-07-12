async function manejarErrorApi(respuesta) {
    let mensajeError = '';
    if (respuesta.status === 400) {
        mensajeError = await respuesta.text();
    } else if (respuesta.status === 404) {
        mensajeError = "Recurso no encontrado";
    } else {
        mensajeError = "Error Inesperado";
    }

    mostrarMensaje(mensaje);
}


function mostrarMensaje(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error ...',
        text : mensaje
    })
}