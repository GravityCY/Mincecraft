using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private string[] soundId;
    private AudioSource source;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }

        source = GetComponent<AudioSource>();
    }

    public void PlayAudio(string inputId)
    {
        for(int i = 0; i < soundId.Length; i++)
        {
            string id = soundId[i];
            if(inputId == id)
            {
                source.PlayOneShot(sounds[i]);
                return;
            }
        }     
    }





}
