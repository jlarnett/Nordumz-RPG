using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

public class Harvest : MonoBehaviour, IRaycastable, IAction
{
    private Collider collider;
    private MeshRenderer myRender;

    public float health = 20;
    private float startingHealth;
    public float resources = 10;
    public float respawnTime = 3f;

    void Awake()
    {
        collider = GetComponent<Collider>();
        myRender = GetComponent<MeshRenderer>();
        startingHealth = health;
    }

    public void HarvestResources(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            StartCoroutine(WaitRespawn());
        }
    }

    IEnumerator WaitRespawn()
    {
        collider.enabled = false;
        myRender.enabled = false;
        print("Added Resource to inventory"); 

        yield return new WaitForSeconds(respawnTime);

        collider.enabled = true;
        myRender.enabled = true;
    }

    public CursorType GetCursorType()
    {
        return CursorType.Pickup;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        return false;
    }

    public Vector3 GetPosition()
    {
        if (transform.position == null) return Vector3.zero;
        return transform.position;
    }

    public void Cancel()
    {
    }
}
