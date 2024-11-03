using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class plane : MonoBehaviour
{
    public Guid GUID;
    public int fuel;
    public TextMeshProUGUI text;
    public GameObject explosion;
    public AudioSource explodeSFX;
    private Crew crew;

    private void Start () {
        this.GUID = Guid.NewGuid();
        this.fuel = 100;
        this.crew = new Crew();

        Debug.Log("Nuevo avión de GUID: "+GUID);

        foreach(CrewMember member in crew.crewMembers) {
            Debug.Log($"Rol: {member.role}, Horas voladas: {member.flightHours}, Identificador: {member.identifier}");
        }
    }

    private void OnTriggerEnter2D(Collider2D obj) {
        if(obj.CompareTag("bullet")) {
            var currentScore = int.Parse(text.text.ToString());
            currentScore += 100;
            text.text = $"{currentScore}";

            Destroy(obj.gameObject);

            explode();
        }
    }

    private void explode() {
        Destroy(gameObject);
        explodeSFX.Play();

        GameObject explosionObj = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(explosionObj, 2f);
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
