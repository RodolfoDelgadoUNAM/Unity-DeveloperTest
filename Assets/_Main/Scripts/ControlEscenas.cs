using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlEscenas : MonoBehaviour
{
    [Header("Objetos de referencia")]
    public GameObject PantallaDeCarga;
    public Slider BarraDeCarga;

    private void Start()
    {
        //Se desactiva el panel de carga de escena al inicio 
        PantallaDeCarga.SetActive(false);
    }

    //La escenas se llaman por medio de botones se parametriza el nombre de la escena a cargar
    public void CargaraNivel (string NombreDeEscena)
    {
        StartCoroutine(CargarAsync(NombreDeEscena));
    }

    //Una metodo para cerrar el juego

    public void CerrarJuego()
    {
        Application.Quit();
    }


    IEnumerator CargarAsync(string NombreDeEscena)
    {
        //Se inicia la operacion asincrona de la escena
        AsyncOperation Operacion = SceneManager.LoadSceneAsync(NombreDeEscena);

        //Se activa el panel de la pantalla de carga
        PantallaDeCarga.SetActive(true);
        
        //Mientras no se termine la operacion se mostrara el avance en la barra de carga
        while(!Operacion.isDone)
        {
            float Progreso = Mathf.Clamp01(Operacion.progress / 0.9f);
            //se le pasa el parametro al slider
            BarraDeCarga.value = Progreso;

            yield return null;
        }
    }
}
