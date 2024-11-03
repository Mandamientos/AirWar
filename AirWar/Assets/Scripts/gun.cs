using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class gun : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform muzzle;
    public float timeBetweenShots = 0.5f;
    private float actualTime;
    public AudioSource shootSFX;

    void Update()
    {
        actualTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && actualTime >= timeBetweenShots)
        {
            shoot();
            actualTime = 0;
        }
    }

    void shoot()
    {
        shootSFX.Play();

        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    }
}
