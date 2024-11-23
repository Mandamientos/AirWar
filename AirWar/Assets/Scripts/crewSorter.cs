using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CrewSorter
{
    public static void SelectionSort<T>(List<T> list, Func<T, T, int> comparison)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < list.Count; j++)
            {
                if (comparison(list[j], list[minIndex]) < 0)
                {
                    minIndex = j;
                }
            }

            // Intercambia los elementos si es necesario
            if (minIndex != i)
            {
                T temp = list[i];
                list[i] = list[minIndex];
                list[minIndex] = temp;
            }
        }
    }
}
