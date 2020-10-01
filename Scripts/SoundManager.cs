using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    [SerializeField] private AudioClip[] sounds;
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

    public void PlayBlockAudio(ActionType action, Material mat)
    {

        switch (action)
        {
            case ActionType.Dig:
                {
                    switch (mat)
                    {
                        case Material.Grass:
                            {
                                source.PlayOneShot(sounds[0]);
                                return;
                            }
                        default: return;
                    }
                }
            case ActionType.Place:
                {
                    switch (mat) 
                    {
                        case Material.Grass:
                        {
                            source.PlayOneShot(sounds[1]);
                            return;
                        }
                        default: return;
                    }
                }
        default: return;
        }
    
        
    }





}
