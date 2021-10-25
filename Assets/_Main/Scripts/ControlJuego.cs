using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlJuego : MonoBehaviour
{
    #region Variables

    [Header("Elementos del UI")]
    public GameObject DanoLevel1;
    public GameObject DanoLevel2;
    public GameObject DanoLevel3;
    public GameObject SaltoLevel1;
    public GameObject SaltoLevel2;
    public GameObject SaltoLevel3;

    [Header("Paneles del UI")]
    public GameObject PanelItems;
    public GameObject PanelPausa;
    public GameObject PanelVictoria;
    public GameObject PanelDerrota;

    [Header("Textos del UI")]
    public Text TextoAlimento;
    public Text TextoPoderAtaque;
    public Text TextoPoderSalto;

    [Header("Variables de juego")]
    public int EnemigosIniciales;


    //Las cantidades que se incrementan desde el jugador
    [HideInInspector]
    public int CantidadInventarioSalto;
    [HideInInspector]
    public int CantidadInventarioAlimento;
    [HideInInspector]
    public int CantidadInventarioAtaque;
    [HideInInspector]
    public int EnemigosEliminados = 0;

    //variables privadas
    private bool Pausa;
    private Jugador ControlJugador;
    private float PoderSalto_Inicial;
    private int PoderSaltoAplicado = 0;
    private int PoderAtaque_Inicial;
    private int PoderAtaqueAplicado = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Se apagan los paneles al inicio, el time scale se regresa a 1 y se regresan a cero los contadores
        PanelVictoria.SetActive(false);
        PanelDerrota.SetActive(false);
        PanelPausa.SetActive(false);
        DanoLevel1.SetActive(false);
        DanoLevel2.SetActive(false);
        DanoLevel3.SetActive(false);
        SaltoLevel1.SetActive(false);
        SaltoLevel2.SetActive(false);
        SaltoLevel3.SetActive(false);
        Time.timeScale = 1f;
        Pausa = false;



        //Se busca el objeto jugador para recibir y mandar valores de variables como aumentos de poder y salud
        ControlJugador = FindObjectOfType<Jugador>();

        //Se guardan los valores iniciales del jugador
        PoderAtaque_Inicial = ControlJugador.PoderAtaqueJugador;
        PoderSalto_Inicial = ControlJugador.PoderSalto;
    }

    // Update is called once per frame
    void Update()
    {

        //Si se oprime el boton de escape el juego se pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pausa = !Pausa;
            if(Pausa && !ControlJugador.JugadorMuriendo)
            {
                PausaJuego();
            }
            else
            {
                PlayJuego();
            }

        }

        //Si se oprime el boton de scroll, el jugador no esta muriendo y no se esta en pausa
        if (Input.GetMouseButton(2) && !ControlJugador.JugadorMuriendo && !Pausa)
        {
            PanelItems.SetActive(true);
        }
        else
        {
            PanelItems.SetActive(false);
        }

        //Se comprueba si los enemigos elminados son iguales a los iniciales
        if(EnemigosEliminados >= EnemigosIniciales)
        {
            JuegoGanado();
        }

        //Se llama a la funcion que actualiza la GUI
        ActualizarUI();

    }

    #region Metodos Control de Juego
    public void PausaJuego()
    {
        Time.timeScale = 0f;
        PanelPausa.SetActive(true);
    }

    public void PlayJuego()
    {
        Time.timeScale = 1f;
        PanelPausa.SetActive(false);
    }

    public void ActualizarUI()
    {
        TextoAlimento.text = "x" + CantidadInventarioAlimento;
        TextoPoderAtaque.text = "x" + CantidadInventarioAtaque;
        TextoPoderSalto.text = "x" + CantidadInventarioSalto;
    }

    public void JuegoPerdido()
    {
        Time.timeScale = 0f;
        PanelDerrota.SetActive(true);
    }

    public void JuegoGanado()
    {
        Time.timeScale = 0f;
        PanelVictoria.SetActive(true);
    }


    #endregion

    #region Aumentos de Poder y vida
    // Aumentos de SAlud, Salto y poder de ataque, se activan con los botones del inventario
    public void AumentoSalto()
    {
        if(CantidadInventarioSalto > 0) //si hay existensia en el inventario
        {
            //se descuenta del inventario el item utilizado
            CantidadInventarioSalto -= 1;

            //Se aumenta el contador de saltos aplicados
            PoderSaltoAplicado += 1;


            //Se pasa el valor del poder del salto al jugador limitado a 3
            if(PoderSaltoAplicado <3)
            {
                ControlJugador.PoderSalto = PoderSaltoAplicado * PoderSalto_Inicial;

                switch (PoderSaltoAplicado)
                {
                    case 1:
                        SaltoLevel1.SetActive(true);
                        break;

                    case 2:
                        SaltoLevel2.SetActive(true);
                        break;

                    case 3:
                        SaltoLevel3.SetActive(true);
                        break;
                    default:

                        break;
                }
            }
        }
    }

    public void AumentoAtaque()
    {
        if (CantidadInventarioAtaque > 0) //si hay existensia en el inventario
        {
            //se descuenta del inventario el item utilizado
            CantidadInventarioAtaque -= 1;

            //Se aumenta el contador de saltos aplicados
            PoderAtaqueAplicado += 1;


            //Se pasa el valor del poder del salto al jugador limitado a 3
            if (PoderAtaqueAplicado < 3)
            {
                ControlJugador.PoderAtaqueJugador = PoderAtaqueAplicado * PoderAtaque_Inicial;

                switch (PoderAtaqueAplicado)
                {
                    case 1:
                        DanoLevel1.SetActive(true);
                        break;

                    case 2:
                        DanoLevel2.SetActive(true);
                        break;

                    case 3:
                        DanoLevel3.SetActive(true);
                        break;
                    default:

                        break;
                }
            }
        }
    }


    public void AumentoVida()
    {
        if(CantidadInventarioAlimento > 0)
        {
            //se descuenta el item de alimento que se acaba de ejercer
            CantidadInventarioAlimento -= 1;
            ControlJugador.Vida(10);
        }
    }

    #endregion

}
