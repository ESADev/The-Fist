using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DissolveOnDeath : MonoBehaviour
{
    private Material[] _materials;
    private float _duration = 0.5f;
    private float _randomness = 0.33f;

    public void Setup(Material[] materials, float duration)
    {
        _materials = materials;
        _duration = duration;
    }

    // Overload for backward compatibility with single material
    public void Setup(Material mat, float duration)
    {
        _materials = new Material[] { mat };
        _duration = duration;
    }

    public void Kill()
    {
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        if (_materials == null || _materials.Length == 0)
        {
            Debug.LogWarning("DissolveOnDeath: No materials to dissolve.");
            yield break;
        }

        float startValue = 0f;
        float endValue = 1f;
        
        // Set initial dissolve amount for all materials
        foreach (var material in _materials)
        {
            if (material != null)
                material.SetFloat("_DissolveAmount", startValue);
        }

        // Animate all materials simultaneously
        yield return DOTween.To(
            () => startValue,
            x => {
                foreach (var material in _materials)
                {
                    if (material != null)
                        material.SetFloat("_DissolveAmount", x);
                }
            },
            endValue,
            _duration.Randomized(_randomness) // Apply randomness to duration
        ).SetEase(Ease.OutSine).WaitForCompletion();
    }
}