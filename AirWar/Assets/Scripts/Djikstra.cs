using System.Collections.Generic;

public static class Djikstra {
    public static List<Nodo> Dijkstra(Nodo start, Nodo target, List<Nodo> nodes) {
        Dictionary<Nodo, float> distances = new Dictionary<Nodo, float>();
        Dictionary<Nodo, Nodo> previous = new Dictionary<Nodo, Nodo>();
        List<Nodo> unvisited = new List<Nodo>(nodes);

        // Inicializar todas las distancias a infinito, excepto el nodo inicial
        foreach (Nodo node in nodes)
        {
            distances[node] = float.MaxValue;
            previous[node] = null; // Sin predecesor
        }
        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            // Ordenar nodos no visitados por distancia mínima
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            Nodo current = unvisited[0];
            unvisited.Remove(current);

            // Si llegamos al nodo objetivo, reconstruir el camino
            if (current == target)
            {
                List<Nodo> path = new List<Nodo>();
                while (current != null)
                {
                    path.Insert(0, current);
                    current = previous[current];
                }
                return path; // Retorna la lista de nodos en el camino más corto
            }

            // Si la distancia actual es infinita, no hay un camino hacia el objetivo
            if (distances[current] == float.MaxValue)
                break;

            // Procesar los vecinos
            foreach (Edge edge in current.edges)
            {
                float newDist = distances[current] + edge.weight;
                if (newDist < distances[edge.destination])
                {
                    distances[edge.destination] = newDist;
                    previous[edge.destination] = current;
                }
            }
        }

        return null; // Retornar null si no hay camino
    }
}