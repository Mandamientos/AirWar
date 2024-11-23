using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class saveDestroyedPlanes {
    private static saveDestroyedPlanes instance { get; set; }
    public List<planeInfo> planes;
    public int score;

    private saveDestroyedPlanes() {
        this.planes = new List<planeInfo>();
        this.score = 0;
    }

    public static saveDestroyedPlanes getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new saveDestroyedPlanes();
            }
            return instance;
        }
    }
}

public class planeInfo {
    public Guid planeGUID;
    public Crew crew;
    public planeInfo(Guid guid, Crew crew) {
        this.crew = crew;
        this.planeGUID = guid;
    }
}