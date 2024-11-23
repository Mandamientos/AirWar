using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

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
        GenerarNodos(8);
        GenerarConexiones();
        ConectarGrafo();
        DibujarConexiones();

        // Llamar a CreatePlane en nodos de tipo "aeropuerto" al inicio
        foreach (var nodo in nodos)
        {
            if (nodo.type == "aeropuerto")
            {
                StartCoroutine(nodo.CreatePlane(explodeSFX, scoreText));
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
                    nodo.nodes = this.nodos;

                    // Asignar el planePrefab desde graphController
                    nodo.planePrefab = planePrefab;
                    
                    // Crear el plane y asignar el TextMeshProUGUI dinámicamente
                    if (tipoNodo == "aeropuerto") {
                        GameObject newPlane = Instantiate(planePrefab, posicion, Quaternion.identity);
                        plane planeScript = newPlane.GetComponent<plane>();
                        planeScript.AssignScoreText(scoreText);  // Dynamically assign the score text
                        planeScript.explodeSFX = explodeSFX;
                        planeScript.currentNode = nodo;
                        planeScript.nodes = nodos;
                    }

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
        Nodo theOne = nodos[Random.Range(0, nodos.Count - 1)];
        theOne.theOne = true;
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
        int k = 2; // Número máximo de conexiones por nodo (k vecinos más cercanos)

        foreach (var nodo in nodos)
        {
            // Crear una lista de todos los demás nodos con sus distancias
            List<(Nodo otroNodo, float distancia)> vecinos = new List<(Nodo, float)>();

            foreach (var otroNodo in nodos)
            {
                if (nodo != otroNodo)
                {
                    float distancia = Vector2.Distance(nodo.pos, otroNodo.pos);
                    vecinos.Add((otroNodo, distancia));
                }
            }

            // Ordenar la lista de vecinos por distancia (más cercano primero)
            vecinos.Sort((a, b) => a.distancia.CompareTo(b.distancia));

            // Conectar solo con los k vecinos más cercanos
            for (int i = 0; i < Mathf.Min(k, vecinos.Count); i++)
            {
                var vecino = vecinos[i];
                float peso = CalcularPeso(vecino.distancia, nodo.type, vecino.otroNodo.type);
                nodo.edges.Add(new Edge(vecino.otroNodo, peso));
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

    private void ConectarGrafo()
    {
        // Lista para todas las aristas posibles entre los nodos
        List<(Nodo origen, Nodo destino, float peso)> todasLasAristas = new List<(Nodo, Nodo, float)>();

        // Generar todas las posibles aristas con sus pesos
        foreach (var nodo in nodos)
        {
            foreach (var otroNodo in nodos)
            {
                if (nodo != otroNodo)
                {
                    float distancia = Vector2.Distance(nodo.pos, otroNodo.pos);
                    float peso = CalcularPeso(distancia, nodo.type, otroNodo.type);
                    todasLasAristas.Add((nodo, otroNodo, peso));
                }
            }
        }

        // Ordenar las aristas por peso (ascendente)
        todasLasAristas.Sort((a, b) => a.peso.CompareTo(b.peso));

        // Usar Union-Find para garantizar la conexión del grafo
        Dictionary<Nodo, Nodo> parent = new Dictionary<Nodo, Nodo>();
        foreach (var nodo in nodos) parent[nodo] = nodo;

        Nodo Find(Nodo x)
        {
            if (parent[x] == x) return x;
            return parent[x] = Find(parent[x]);
        }

        void Union(Nodo x, Nodo y)
        {
            Nodo rootX = Find(x);
            Nodo rootY = Find(y);
            if (rootX != rootY) parent[rootX] = rootY;
        }

        // Crear conexiones asegurando que no se formen ciclos
        foreach (var arista in todasLasAristas)
        {
            if (Find(arista.origen) != Find(arista.destino))
            {
                Union(arista.origen, arista.destino);
                arista.origen.edges.Add(new Edge(arista.destino, arista.peso));
                arista.destino.edges.Add(new Edge(arista.origen, arista.peso)); // Si es bidireccional
            }
        }
    }

    private IEnumerator test( ) {
        yield return new WaitForSeconds( 1 );
        foreach(var nodo in nodos) {
            foreach (var conexion in nodo.edges) {
                Debug.Log($"{nodo.nodeName.text} : {conexion.destination.nodeName.text} : Weight: {conexion.weight}");
            }
        }
    }

    private IEnumerator test2() {
        yield return new WaitForSeconds( 2 );
        while (true) {
            Nodo currentNode = nodos[UnityEngine.Random.Range(0, nodos.Count - 1)];
            Nodo targetNode = currentNode;
            while (currentNode == targetNode) {
                targetNode = nodos[UnityEngine.Random.Range(0, nodos.Count - 1)];
            }

            List<Nodo> route = Djikstra.Dijkstra(currentNode, targetNode, nodos);

            Debug.Log($"Salida: {currentNode} | Llegada: {targetNode}");

            yield return new WaitForSeconds( 2f );

            foreach(var node in route) {
                Debug.Log(node.nodeName.text);
            }

            yield return new WaitForSeconds( 1 );
        }

    }
}
