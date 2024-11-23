using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class addCrewItems : MonoBehaviour
{
    public VisualTreeAsset itemTemplate; // Plantilla para ítems
    private List<CrewMember> datos;           // Lista de datos para el ListView
    private ListView listView;           // Referencia al ListView
    private int SortTypeID = -1;

    void Start()
    {
        // Inicializa la lista de datos con la clase Plane
        datos = new List<CrewMember>();

        // Obtén la raíz del UIDocument
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Encuentra el ListView y el botón en la UI
        listView = root.Q<ListView>("crewList");
        var backButton = root.Q<Button>("BackButton");
        var sortType = root.Q<Button>("SortType");

        // Configura el ListView
        listView.itemsSource = datos;
        listView.makeItem = () => itemTemplate.Instantiate(); // Usa la plantilla
        listView.bindItem = (elementoVisual, indice) =>
        {
            var crewMember = datos[indice]; // Obtiene el objeto Plane correspondiente al índice

            // Encuentra los Labels en la plantilla
            var crewMemberRoleLabel = elementoVisual.Q<Label>("Role");
            var crewMemberHoursLabel = elementoVisual.Q<Label>("FlightHours");
            var crewMemberIdentifierLabel = elementoVisual.Q<Label>("Identifier");

            // Vincula los datos del objeto Plane a los Labels
            crewMemberRoleLabel.text = $"Rol: {crewMember.role}"; // Muestra planeGUID
            crewMemberHoursLabel.text = $"Horas de vuelo: {crewMember.flightHours}";
            crewMemberIdentifierLabel.text = $"ID: {crewMember.identifier}"; // Muestra planeName
        };
        listView.fixedItemHeight = 100; // Altura fija de los elementos (opcional)

        // Botón para agregar elementos dinámicos
        backButton.clicked += () => sceneHandler.changeScene(3);
        sortType.clicked += () => cambiarOrdenamiento(sortType);

        int index = PlayerPrefs.GetInt("Index");
        foreach(var member in saveDestroyedPlanes.getInstance.planes[index].crew.crewMembers) {
            AgregarElemento(member);
        }

        // Detectar selección de elementos
        listView.selectionChanged += objects =>
        {
            foreach (var obj in objects)
            {
                int index = listView.itemsSource.IndexOf(obj);
                Debug.Log($"Se seleccionó: {index}");
                PlayerPrefs.SetInt("Index", index);
            }
        };
    }

    private void AgregarElemento(CrewMember member)
    {
        datos.Add(member); // Agrega un nuevo Plane
        listView.Rebuild(); // Actualiza el ListView para reflejar los cambios
    }

    private void cambiarOrdenamiento (Button SortTypeBtn) {
        this.SortTypeID++;
        switch (SortTypeID) {
            case 0:
            SortTypeBtn.text = $"Ordenando por Identificador";
            CrewSorter.SelectionSort(datos, CompareByIdentifier);
            break;
            case 1:
            SortTypeBtn.text = $"Ordenando por Horas de Vuelo";
            CrewSorter.SelectionSort(datos, CompareByFlightHours);
            break;
            case 2:
            SortTypeBtn.text = $"Ordenando por Rol";
            CrewSorter.SelectionSort(datos, CompareByRole);
            break;
            case 3:
            SortTypeBtn.text = $"Ordenando por Enum";
            CrewSorter.SelectionSort(datos, CompareByEnum);
            break;
            case 4:
            this.SortTypeID = -1;
            cambiarOrdenamiento(SortTypeBtn);
            break;
        }
        listView.Rebuild();
    }

    private static int CompareByIdentifier(CrewMember a, CrewMember b)
    {
        return string.Compare(a.identifier, b.identifier, StringComparison.Ordinal);
    }

    private static int CompareByRole(CrewMember a, CrewMember b)
    {
        return string.Compare(a.role.ToString(), b.role.ToString(), StringComparison.Ordinal);
    }

    private static int CompareByFlightHours(CrewMember a, CrewMember b)
    {
        return a.flightHours.CompareTo(b.flightHours);
    }

    private static int CompareByEnum(CrewMember a, CrewMember b)
    {
        return a.role.CompareTo(b.role);
    }
}
