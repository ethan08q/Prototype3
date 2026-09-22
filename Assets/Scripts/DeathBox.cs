using UnityEngine;

public class DeathBox : MonoBehaviour
{
   void OnTriggerEnter(Collider other)
   {
       if (other.CompareTag("Player"))
       {
          Destroy(other.gameObject);
       }
    }

}
