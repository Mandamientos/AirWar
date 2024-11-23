using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public Vector3 pos;
    public string type;
    public List<Edge> edges = new List<Edge>();

    // Atributos específicos para los nodos de tipo "aeropuerto"
    public int fuelCapacity; // Capacidad de combustible
    public int maxPlanes; // Número máximo de aviones permitidos
    private List<plane> planes = new List<plane>();

    // Reference to the Plane prefab
    public GameObject planePrefab;  // Add this line to reference the plane prefab
    public TextMeshPro nodeName;
    public List<Nodo> nodes;
    public bool theOne = false;

    private void Start()
    {
        pos = transform.position;

        nodeName.text = new string(Enumerable.Range(0, 3)
            .Select(_ => (char)UnityEngine.Random.Range('A', 'Z' + 1))
            .ToArray());
        
        gameObject.name = nodeName.text;
        
        // Inicializar atributos si el nodo es un aeropuerto
        if (type == "aeropuerto")
        {
            fuelCapacity = UnityEngine.Random.Range(500, 1001); // Rango aleatorio de capacidad de combustible
            maxPlanes = 10; // Número máximo de aviones permitidos
        }
    }

    public IEnumerator CreatePlane(AudioSource explodeSFX, TextMeshProUGUI scoreText)
    {
        while(true) {
            if (planes.Count < maxPlanes)
            {
                GameObject newPlane = Instantiate(planePrefab, pos, Quaternion.identity); // Usa tu prefab de avión aquí
                plane planeScript = newPlane.GetComponent<plane>();
                planeScript.explodeSFX = explodeSFX;
                planeScript.text = scoreText;
                planeScript.currentNode = this;
                planeScript.nodes = this.nodes;
                planes.Add(planeScript);
            }
            else
            {
                Debug.LogWarning("No se pueden crear más aviones, se ha alcanzado el límite.");
            }
            yield return new WaitForSeconds(15);
        }

    }

    public int rationFuel (int fuelLeft) {
        if ((100-fuelLeft)/2 > fuelCapacity) {
            Debug.Log("No hay combustible suficiente");
            return 0;
        } else {
            this.fuelCapacity = fuelCapacity - (100-fuelLeft)/2;
            return (100-fuelLeft)/2;
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
