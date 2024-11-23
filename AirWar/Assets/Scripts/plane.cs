using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class plane : MonoBehaviour
{
    public Guid GUID;
    public int fuel;
    public TextMeshProUGUI text;  // This will be assigned dynamically
    public GameObject explosion;
    public AudioSource explodeSFX;
    private Crew crew;
    public Nodo currentNode;
    public Nodo nodeDestiny;
    public List<Nodo> nodes;
    private new Renderer renderer;
    private float angle;
    private bool isFlying;
    private List<Nodo> route;
    private float timeTravelledHelper;
    private float fuelTimeHelper;

    // Método Start modificado
    private void Start()
    {
        this.GUID = Guid.NewGuid();
        this.fuel = 100;
        this.crew = new Crew();
        this.renderer = gameObject.GetComponent<Renderer>();

        renderer.enabled = false;

        // Usar las referencias asignadas
        //if (text != null) Debug.Log("Texto asignado correctamente.");
        //if (explodeSFX != null) Debug.Log("Efecto de sonido asignado correctamente.");

        //Debug.Log("Nuevo avión de GUID: "+GUID);

        //foreach(CrewMember member in crew.crewMembers) {
       //     Debug.Log($"Rol: {member.role}, Horas voladas: {member.flightHours}, Identificador: {member.identifier}");
       // }
       this.route = chooseRandomRoute();
       StartCoroutine(fly(UnityEngine.Random.Range(0, 5)));
    }

    private void Update() {
        if (isFlying) {
            this.timeTravelledHelper += Time.deltaTime;
            this.fuelTimeHelper += Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, nodeDestiny.pos, 1f * Time.deltaTime);

            if (transform.position == nodeDestiny.pos) {
                renderer.enabled = false;
                this.isFlying = false;
                currentNode = nodeDestiny;
                fuel += currentNode.rationFuel(fuel);
                StartCoroutine(fly(UnityEngine.Random.Range(0, 5)));
            }

            if (timeTravelledHelper >= 1f) {
                foreach (CrewMember member in crew.crewMembers) {
                    member.flightHours++;
                }
                timeTravelledHelper = 0f;
            }

            if (fuelTimeHelper >= 0.15f) {
                this.fuel--;
                fuelTimeHelper = 0f;
            }

            if (fuel < 0f) {
                explode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.CompareTag("bullet") && isFlying)
        {
            var currentScore = int.Parse(text.text.ToString());
            currentScore += 100;
            text.text = $"{currentScore}";
            saveDestroyedPlanes.getInstance.score = currentScore;

            Destroy(obj.gameObject);
            explode();
        }
    }

    private void explode()
    {
        if (isFlying) {
            Destroy(gameObject);
            explodeSFX.Play();

            GameObject explosionObj = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(explosionObj, 2f);

            saveDestroyedPlanes.getInstance.planes.Add(new planeInfo(this.GUID, crew));
        }
    }

    public List<Nodo> chooseRandomRoute() 
    {
        Nodo targetNode = currentNode;
        while (currentNode == targetNode) {
            targetNode = nodes[UnityEngine.Random.Range(0, nodes.Count - 1)];
        }

        Debug.Log($"Nueva Ruta para {this.GUID} | {currentNode} -> {targetNode}");

        this.nodeDestiny = targetNode;

        List<Nodo> route = Djikstra.Dijkstra(currentNode, targetNode, nodes);

        route.RemoveAt(0);

        return route;
    }

    // Method to assign the score text dynamically
    public void AssignScoreText(TextMeshProUGUI scoreText)
    {
        this.text = scoreText;
    }

    public IEnumerator fly(int cooldown) 
    {
        yield return new WaitForSeconds(cooldown);

        if (!route.Any()) {
            this.route = chooseRandomRoute();
            yield return new WaitForSeconds(1f);
        }

        Nodo inmediateDestiny = route[0];
        this.nodeDestiny = inmediateDestiny;
        Debug.Log("Destino inmediato: "+inmediateDestiny.nodeName.text);
        
        float angle = math.atan2(inmediateDestiny.transform.position.y - transform.position.y, inmediateDestiny.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle-90);
        renderer.enabled = true;
        this.isFlying = true;
        route.RemoveAt(0);
    }
}

public class Crew {
    public List<CrewMember> crewMembers;
    public enum Role {Pilot, Copilot, Maintenance, Space_Awareness}

    public Crew() {
        this.crewMembers = new List<CrewMember>();

        foreach (Role role in Enum.GetValues(typeof(Role))) {
            CrewMember crewMember = new CrewMember(role);
            this.crewMembers.Add(crewMember);
        }
    }
}
public class CrewMember {
    public string identifier;
    public Crew.Role role;
    public int flightHours;

    public CrewMember(Crew.Role role) {
        this.identifier = new string(Enumerable.Range(0, 3)
            .Select(_ => (char)UnityEngine.Random.Range('A', 'Z' + 1))
            .ToArray());
        this.role = role;
        this.flightHours = 0;
    }
}
