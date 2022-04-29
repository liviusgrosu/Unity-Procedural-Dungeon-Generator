using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReducer : MonoBehaviour
{
    [HideInInspector] public FieldOfView FOV;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstructionMask;

    [HideInInspector] public bool CanSeePlayer;

    private void Start()
    {
        FOV = GetComponent<FieldOfView>();
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        // Routinely check for player in FOV  
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, FOV.Radius, _targetMask);

        // Check if anything collides with the guards sphere
        foreach(Collider other in rangeChecks)
        {
            // Get distance to player
            Transform target = other.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= FOV.Radius)
            {
                other.transform.GetComponent<LightSource>().ChangeIntensity(1 - distanceToTarget / FOV.Radius);
            }
        }
    }
}