using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WildlifeBehaviour : MonoBehaviour
{
    public SpriteRenderer[] Renderers { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Transform { get; private set; }

    private float fadeOutSeconds = 2f;
    private float fadeInSeconds = 1f;

    private Coroutine fadeInCoroutine;
    private bool fadeOutRunning = false;

    public abstract float TargetAlpha { get; }

    private void Awake()
    {
        Renderers = GetComponentsInChildren<SpriteRenderer>();
        Transform = transform;
        Animator = GetComponent<Animator>();

        Initialize();

        fadeInCoroutine = StartCoroutine(FadeIn(TargetAlpha));
    }

    public abstract void Initialize();

    public void StartFadeOutAndDestroy()
    {
        if (fadeOutRunning)
            return;

        if (fadeInCoroutine != null)
            StopCoroutine(fadeInCoroutine);

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        fadeOutRunning = true;

        Color32[] colors = new Color32[Renderers.Length];
        for (int i = 0; i < Renderers.Length; i++)
        {
            colors[i] = Renderers[i].color;
        }

        float timer = fadeOutSeconds;

        while (true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Destroy(gameObject);
                break;
            }
            else
            {
                for (int i = 0; i < Renderers.Length; i++) {
                    byte alpha = (byte)((timer / fadeOutSeconds * colors[i].a));
                    Color32 thisColor = colors[i];
                    thisColor.a = alpha;
                    Renderers[i].color = thisColor;
                }
                yield return 0;
            }
        }
    }

    private IEnumerator FadeIn(float targetAlpha)
    {
        float alpha = 0;

        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            alpha = (timer / fadeInSeconds) * targetAlpha;

            if (alpha >= targetAlpha)
                alpha = targetAlpha;

            foreach (SpriteRenderer r in Renderers)
            {
                Color32 thisColor = r.color;
                thisColor.a = (byte)(alpha * 255);
                r.color = thisColor;
            }

            if (alpha >= targetAlpha)
            {
                break;
            }
            else
                yield return 0;
        }
    }

    public virtual void Startle()
    {

    }

    public abstract void Despawn();
}