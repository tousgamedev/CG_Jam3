using System.Timers;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 3f;

    private float timer;
    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
