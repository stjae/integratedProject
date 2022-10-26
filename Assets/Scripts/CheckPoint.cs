using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Poly.Data;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour
{
    private BoxCollider boxCollider;

    // data manager
    private SaveManager saveManager;

    // checkpoint number (assign in inspector)
    public int chapter;
    public int level;
    public int checkpoint;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        saveManager = FindObjectOfType<SaveManager>();
    }

    private void Start()
    {
        GameObject player = GameObject.Find("Player");

        if(checkpoint == saveManager.GetSaveData().Checkpoint)
        {
            player.transform.position = transform.position;
            Debug.Log("move player");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            SaveData sd = new SaveData();
            sd.Chapter = chapter;
            sd.Level = level;
            sd.Checkpoint = checkpoint;

            saveManager.GetSaveData() = sd;
            saveManager.Save();
            Debug.Log("game saved");
        }
    }
}