using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeSortPlaneInfo
{
    // Método para ordenar una lista usando MergeSort
    public static void MergeSort(List<planeInfo> list)
    {
        if (list.Count <= 1) return;

        int middle = list.Count / 2;

        // Divide la lista en dos mitades
        List<planeInfo> left = new List<planeInfo>(list.GetRange(0, middle));
        List<planeInfo> right = new List<planeInfo>(list.GetRange(middle, list.Count - middle));

        // Ordena recursivamente las mitades
        MergeSort(left);
        MergeSort(right);

        // Fusiona las dos mitades ordenadas
        Merge(list, left, right);
    }

    // Método para fusionar dos listas ordenadas
    private static void Merge(List<planeInfo> list, List<planeInfo> left, List<planeInfo> right)
    {
        int i = 0, j = 0, k = 0;

        while (i < left.Count && j < right.Count)
        {
            // Compara los GUIDs para decidir el orden
            if (left[i].planeGUID.CompareTo(right[j].planeGUID) <= 0)
            {
                list[k] = left[i];
                i++;
            }
            else
            {
                list[k] = right[j];
                j++;
            }
            k++;
        }

        // Agrega los elementos restantes de la lista izquierda
        while (i < left.Count)
        {
            list[k] = left[i];
            i++;
            k++;
        }

        // Agrega los elementos restantes de la lista derecha
        while (j < right.Count)
        {
            list[k] = right[j];
            j++;
            k++;
        }
    }
}
