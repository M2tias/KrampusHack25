using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private List<AudioClip> clips;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayClip()
    {
        if (clips.Count == 0) {
            Debug.LogError($"{gameObject.name} audio player has no clips!");
            return;
        }

        if (clips.Count > 0){
            source.clip = clips.OrderBy(x => Guid.NewGuid()).First();
        }
        else {
            source.clip = clips[0];
        }

        source.Play();
    }
}
