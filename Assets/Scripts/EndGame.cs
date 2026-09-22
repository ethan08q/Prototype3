using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            SceneManager.LoadScene("EndScene");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
   

