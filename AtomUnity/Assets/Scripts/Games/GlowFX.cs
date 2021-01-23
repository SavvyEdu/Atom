using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class GlowFX : MonoBehaviour
{
    private SpriteRenderer renderer;
    private AudioSource source;

    [SerializeField] private AudioClip PositiveSound;
    [SerializeField] private AudioClip NegativeSound;

    public bool IsAnimating { get; private set; } = false;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        gameObject.SetActive(false);
    }

    public void BeginAnimation(bool positive, Color color, float maxSize = 10f, Action callback = null)
    {
        gameObject.SetActive(true);
        source.clip = positive ? PositiveSound : NegativeSound;
        source.Play();
        StartCoroutine(Animate(source.clip.length, color, maxSize, callback));
    }
    private IEnumerator Animate(float time, Color color, float maxSize, Action callback)
    {
        IsAnimating = true;
        float t = 0, p = 0;
        Color c = color;
        while (t < time)
        {
            t += Time.deltaTime;
            p = t / time;

            c.a = 1 - (p * p); //alpha fade 1 -> 0
            renderer.color = c;
            transform.localScale = Vector3.one * Mathf.Lerp(0.5f, maxSize, p * p); //scale up 

            yield return new WaitForEndOfFrame();
        }
        
        IsAnimating = false;
        //invoke the callback function if there is one
        callback?.Invoke();
    }
}
