using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeriesSelectTweener : MonoBehaviour
{
    [SerializeField] private Image image;

    private Color visible = Color.white;
    private Color hidden = new Color(1f, 1f, 1f, 0.25f);
    private float speed = 2.5f;
    private int cycles = 3;

    private void OnEnable()
    {
        image.color = hidden;
        StartCoroutine(PulsateImage());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator PulsateImage()
    {
        float alpha;
        float timer = 0;
        float halfTimer = 0;
        int count = 0;
        while(count < cycles)
        {
            if (timer >= 1)
            {
                timer = 0;
            }

            if (halfTimer > 0.5f)
            {
                ++count;
                halfTimer = 0f;
            }

            alpha = Mathf.SmoothStep(0f, 1f, timer);
            image.color = Color.Lerp(hidden, visible, alpha);
            timer += Time.deltaTime * speed;
            halfTimer += Time.deltaTime * speed;
            yield return null;
        }

        image.color = visible;
    }
}