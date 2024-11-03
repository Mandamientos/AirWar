using System.Collections.Generic;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public Vector2 pos;
    public string type;
    public List<Edge> edges = new List<Edge>();

    private void Start()
    {
        pos = transform.position;
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