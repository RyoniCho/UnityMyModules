using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDamage : MonoBehaviour
{

  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckOnDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckOnDamage(collision);
    }

    private void CheckOnDamage(Collider2D collider)
    {
        
    }
}
