using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private Sound[] sounds;
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
         foreach(Sound sound in sounds)
        {
            if (sound.id == inputId)
                PlaySound(sound);
        }
    }

    private void PlaySound(Sound sound)
    {
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip);
    }




}
