using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ListViewController : MonoBehaviour
{
    public VisualTreeAsset itemTemplate; // Plantilla para ítems
    private List<string> datos;          // Lista de datos para el ListView
    private ListView listView;           // Referencia al ListView

    void Start()
    {
        // Inicializa la lista de datos
        datos = new List<string>();

        // Obtén la raíz del UIDocument
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Encuentra el ListView y el botón en la UI
        listView = root.Q<ListView>("planesList");
        var addButton = root.Q<Button>("AddButton");

        // Configura el ListView
        listView.itemsSource = datos;
        listView.makeItem = () => itemTemplate.Instantiate(); // Usa la plantilla
        listView.bindItem = (elementoVisual, indice) =>
        {
            var label = elementoVisual.Q<Label>("Label");
            label.text = datos[indice]; // Vincula el texto al ítem
        };
        listView.fixedItemHeight = 30; // Altura fija de los elementos (opcional)

        // Botón para agregar elementos dinámicos
        addButton.clicked += () => sceneHandler.changeScene(2);

        foreach(var obj in saveDestroyedPlanes.getInstance.planes) {
            AgregarElemento(obj.planeGUID.ToString());
        }

        listView.selectionChanged += objects =>
        {
            foreach (var obj in objects)
            {
                int index = listView.itemsSource.IndexOf(obj);
                Debug.Log($"Se seleccionó: {index}");
                PlayerPrefs.SetInt("Index", index);
                sceneHandler.changeScene(4);
            }
        };
    }

    private void AgregarElemento(string texto)
    {
        datos.Add(texto); // Agrega el nuevo ítem a la lista
        listView.Rebuild(); // Actualiza el ListView para reflejar los cambios
    }
}
