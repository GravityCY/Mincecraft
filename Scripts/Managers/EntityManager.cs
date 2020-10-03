using UnityEngine;

public class EntityManager : MonoBehaviour
{

    public static EntityManager instance;

    private void Awake()
    {
        if(instance == null){ instance = this; } else if (instance != this) { Destroy(gameObject); return; }
    }

    private void SpawnEntity(EntityType type)
    {

    }
}
