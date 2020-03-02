using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : CharacterAI
{
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other, "Weapon");
    }
}
