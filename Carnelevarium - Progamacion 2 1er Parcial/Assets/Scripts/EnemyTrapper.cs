using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyTrapper : Enemy
{
    [Header("Patrulla")]
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float normalSpeed = 3f;
    [SerializeField] float runSpeed = 8f;
    private int currentWaypointIndex = 0;

    [Header("Trampas")]
    [SerializeField] GameObject trapPrefab;
    [SerializeField] float trapLifetime = 150f;
    [SerializeField] int maxTraps = 5;
    [SerializeField] float trapCooldown = 5f;
    [SerializeField] float trapPlaceDelay = 2f;

    private List<GameObject> activeTraps = new List<GameObject>();
    private float trapTimer;
    private bool isPlacingTrap = false;

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            speed = runSpeed;
        }
        else
        {
            speed = normalSpeed;
        }

        if (!isPlacingTrap)
        {
            Patrol();
        }

        trapTimer += Time.deltaTime;
        if (!isPlacingTrap && trapTimer >= trapCooldown && activeTraps.Count < maxTraps)
        {
            StartCoroutine(PlaceTrapRoutine());
        }
    }

    private void Patrol()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
                currentWaypointIndex = 0;
        }
    }

    private IEnumerator PlaceTrapRoutine()
    {
        isPlacingTrap = true;
        float originalSpeed = speed;
        speed = 0f;

        yield return new WaitForSeconds(trapPlaceDelay);

        if (trapPrefab != null)
        {
            GameObject trap = Instantiate(trapPrefab, transform.position, Quaternion.identity);
            activeTraps.Add(trap);
            Destroy(trap, trapLifetime);
        }

        trapTimer = 0f;
        speed = originalSpeed;
        isPlacingTrap = false;

        activeTraps.RemoveAll(t => t == null);
    }

    public override void Death()
    {
        // Cambiado para que sea compatible con el SaveSystem
        gameObject.SetActive(false);
    }

    // Implementación de métodos abstractos obligatorios de Enemy
    protected override void AttackPlayer() { }
    protected override void ChasePlayer(float distance) { }
}