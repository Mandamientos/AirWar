using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class graphController : MonoBehaviour
{
    public GameObject nodoPrefab; // Prefab para los nodos (aeropuertos/portaaviones)
    public LineRenderer linePrefab; // Prefab para las líneas
    public List<Nodo> nodos = new List<Nodo>();

    private void Start()
    {
        GenerarNodos(5); // Genera 5 nodos
        GenerarConexiones();
        DibujarConexiones();
    }

    private void GenerarNodos(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject nuevoNodo = Instantiate(nodoPrefab, randomPosition, Quaternion.identity);
            Nodo nodo = nuevoNodo.GetComponent<Nodo>();
            nodo.type = (i % 2 == 0) ? "aeropuerto" : "portaaviones"; // Alterna tipos
            nodo.pos = randomPosition; // Asegúrate de actualizar la posición
            nodos.Add(nodo);
        }
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
        if (tipoDestino == "portaaviones") peso += 5; // Coste adicional para portaaviones
        return peso;
    }

    private void DibujarConexiones()
    {
        foreach (var nodo in nodos)
        {
            foreach (var conexion in nodo.edges)
            {
                Debug.Log($"Nodo: {nodo.pos} Destino: {conexion.destination.pos}");
                LineRenderer linea = Instantiate(linePrefab, Vector2.zero, Quaternion.identity);
                linea.SetPosition(0, nodo.pos);
                linea.SetPosition(1, conexion.destination.pos);
                linea.startColor = Color.gray;
                linea.endColor = Color.gray;
                linea.startWidth = 0.05f; // Asegúrate de que sea mayor que 0
                linea.endWidth = 0.05f;   // Asegúrate de que sea mayor que 0
                linea.sortingLayerName = "Foreground"; // Asegúrate de que esté en la capa correcta
            }
        }
    }

}
