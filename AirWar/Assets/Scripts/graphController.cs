using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class graphController : MonoBehaviour
{
    public GameObject nodoPrefab;
    public GameObject planePrefab;
    public LineRenderer linePrefab;
    public List<Nodo> nodos = new List<Nodo>();

    public TextMeshProUGUI scoreText;  // UI TextMeshProUGUI component
    public AudioSource explodeSFX;

    private void Start()
    {
        GenerarNodos(5);
        GenerarConexiones();
        DibujarConexiones();

        // Llamar a CreatePlane en nodos de tipo "aeropuerto" al inicio
        foreach (var nodo in nodos)
        {
            if (nodo.type == "aeropuerto")
            {
                nodo.InvokeRepeating("CreatePlane", 0f, 30f);
            }
        }
    }

    private void GenerarNodos(int cantidad)
    {
        Zona[] zonas = FindObjectsOfType<Zona>();
        zonas = ShuffleArray(zonas);

        for (int i = 0; i < cantidad; i++)
        {
            string tipoNodo = (i % 2 == 0) ? "aeropuerto" : "portaaviones";
            bool nodoCreado = false;

            foreach (Zona zona in zonas)
            {
                if ((tipoNodo == "aeropuerto" && zona.tipo == "tierra" || tipoNodo == "portaaviones" && zona.tipo == "agua") && !zona.ocupado)
                {
                    Vector2 posicion = zona.GetComponent<Collider2D>().bounds.center;
                    GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);
                    Nodo nodo = nuevoNodo.GetComponent<Nodo>();
                    nodo.type = tipoNodo;
                    nodo.pos = posicion;

                    // Asignar el planePrefab desde graphController
                    nodo.planePrefab = planePrefab; 

                    // Crear el plane y asignar el TextMeshProUGUI dinámicamente
                    GameObject newPlane = Instantiate(planePrefab, posicion, Quaternion.identity);
                    plane planeScript = newPlane.GetComponent<plane>();
                    planeScript.AssignScoreText(scoreText);  // Dynamically assign the score text

                    nodos.Add(nodo);
                    zona.ocupado = true;
                    nodoCreado = true;
                    break;
                }
            }

            if (!nodoCreado)
            {
                Debug.LogWarning($"No hay zonas disponibles para crear un nodo de tipo {tipoNodo}");
            }
        }
    }

    // Método para barajar un arreglo de manera aleatoria
    private Zona[] ShuffleArray(Zona[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Zona temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }

    private void GenerarConexiones()
    {
        foreach (var nodo in nodos)
        {
            foreach (var otroNodo in nodos)
            {
                if (nodo != otroNodo)
                {
                    float distancia = Vector2.Distance(nodo.pos, otroNodo.pos);
                    float peso = CalcularPeso(distancia, nodo.type, otroNodo.type);
                    nodo.edges.Add(new Edge(otroNodo, peso));
                }
            }
        }
    }

    private float CalcularPeso(float distancia, string tipoOrigen, string tipoDestino)
    {
        float peso = distancia;
        if (tipoDestino == "portaaviones") peso += 5;
        return peso;
    }

    private void DibujarConexiones()
    {
        foreach (var nodo in nodos)
        {
            foreach (var conexion in nodo.edges)
            {
                LineRenderer linea = Instantiate(linePrefab, Vector2.zero, Quaternion.identity);
                linea.SetPosition(0, nodo.pos);
                linea.SetPosition(1, conexion.destination.pos);
                linea.startColor = Color.gray;
                linea.endColor = Color.gray;
                linea.startWidth = 0.05f;
                linea.endWidth = 0.05f;
                linea.sortingLayerName = "Foreground";
            }
        }
    }
}
