using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string theGun;

    private bool collected;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player" && !collected)
        {
            PlayerController.instance.AddGun(theGun);
            AudioManager.instance.PlaySFX(11);
            Destroy(gameObject);

            collected = true;
        }
    }
}
