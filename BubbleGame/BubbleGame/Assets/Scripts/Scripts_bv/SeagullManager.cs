using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeagullManager : MonoBehaviour
{
   public float seagullSpawnTimer = 10f;
   public float seagullTimer = 5f;
   public float attackSeagullTime;
   
   
   public GameObject bancone;
   public GameObject seagullPrefab;
   public Transform seagullSpawnPoint;
   
  
   [NonSerialized] public bool SeagullHasWon;
   [NonSerialized] public bool IsAttackedBySeagull;

   private bool _isSeagullSpawned = false;
   private float _currentTime = 0f;
   private float _currentSeagullSpawnTime = 0;
   private float _currentAttackSeagullTime = 0;

   private GameObject _activeSeagull;
   private WaveManagerScript _waveManager;
   
   private void Start()
   {
      GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
      _waveManager = gameManager.GetComponent<WaveManagerScript>();
      
      _currentTime = 0f;
      _currentSeagullSpawnTime = 0;
      _currentAttackSeagullTime = 0;
      _isSeagullSpawned = false;
   }

   void Update()
   {
      if (!_isSeagullSpawned)
      {
         _currentTime += Time.deltaTime;
         if (_currentTime >= seagullSpawnTimer)    
         { SpawnSeagull(); }
      }

      else
      {
         MoveSeagull();
         CheckSeagullAttack();
      }
   }

   private void SpawnSeagull()
   {
      _activeSeagull = Instantiate(seagullPrefab, seagullSpawnPoint.position, Quaternion.identity);
      _isSeagullSpawned = true;
   }

   private void MoveSeagull()
   {
      _currentSeagullSpawnTime += Time.deltaTime;

      float step = Vector3.Distance(_activeSeagull.transform.position, bancone.transform.position) / seagullTimer *
                   Time.deltaTime;
      _activeSeagull.transform.position = Vector3.MoveTowards
         (_activeSeagull.transform.position, bancone.transform.position, step);
      if (_currentSeagullSpawnTime >= seagullTimer)
      {
         IsAttackedBySeagull = true;
      }
   }

   private void CheckSeagullAttack()
   {
      if (IsAttackedBySeagull)
      {
         _currentAttackSeagullTime += Time.deltaTime;
         if (_currentAttackSeagullTime >= attackSeagullTime && !SeagullHasWon)
         {
            SeagullAttack();
         }
      }
   }

   private void SeagullAttack()
   {
      SeagullHasWon = true;
      if (_waveManager.customers.Count > 0)
      {
         GameObject firstCustomer = _waveManager.customers[0];
         _waveManager.removeCustomer(firstCustomer);
         Debug.Log("Seagull removed the first customer!");
      }
      ResetSeagull();
   }

   private void ResetSeagull()
   {
      Destroy(_activeSeagull);
      _isSeagullSpawned = false;
      SeagullHasWon = false;
      IsAttackedBySeagull = false;
      _currentTime = 0f;
      _currentSeagullSpawnTime = 0f;
      _currentAttackSeagullTime = 0f;
   }
}
