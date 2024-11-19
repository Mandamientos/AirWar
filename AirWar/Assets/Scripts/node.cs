using System.Collections.Generic;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public Vector2 pos;
    public string type;
    public List<Edge> edges = new List<Edge>();

    // Atributos específicos para los nodos de tipo "aeropuerto"
    public int fuelCapacity; // Capacidad de combustible
    public int maxPlanes; // Número máximo de aviones permitidos
    private List<plane> planes = new List<plane>();

    // Reference to the Plane prefab
    public GameObject planePrefab;  // Add this line to reference the plane prefab

    private void Start()
    {
        pos = transform.position;

        // Inicializar atributos si el nodo es un aeropuerto
        if (type == "aeropuerto")
        {
            fuelCapacity = Random.Range(500, 1001); // Rango aleatorio de capacidad de combustible
            maxPlanes = 5; // Número máximo de aviones permitidos
            InvokeRepeating("CreatePlane", 0f, 30f); // Llamar a CreatePlane cada 30 segundos
        }
    }

    // Método para crear un avión si el nodo es un aeropuerto
    private void CreatePlane()
    {
        if (type == "aeropuerto" && planes.Count < maxPlanes)
        {
            GameObject newPlane = Instantiate(planePrefab, pos, Quaternion.identity); // Usa tu prefab de avión aquí
            plane planeScript = newPlane.GetComponent<plane>();
            planeScript.fuel = fuelCapacity; // Inicializar el combustible del avión con la capacidad del nodo
            planes.Add(planeScript);
            Debug.Log($"Avión creado en {pos} con GUID: {planeScript.GUID} y combustible: {planeScript.fuel}");
        }
        else if (type == "portaaviones")
        {
            Debug.LogWarning("No se pueden crear aviones en un nodo de tipo portaaviones.");
        }
        else
        {
            Debug.LogWarning("No se pueden crear más aviones, se ha alcanzado el límite.");
        }
    }
}

public class Edge
{
    public Nodo destination;
    public float weight;

    public Edge(Nodo destination, float weight)
    {
        this.destination = destination;
        this.weight = weight;
    }
}
