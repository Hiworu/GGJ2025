using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullBehavior : MonoBehaviour
{
    private PlayerBehavior _playerBehavior;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerBehavior = player.GetComponent<PlayerBehavior>();
        
        _playerBehavior.SeagullHit += OnSeagullHit;
    }

    private void OnSeagullHit(Transform hitObject)
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        _playerBehavior.SeagullHit -= OnSeagullHit;
    }
}
