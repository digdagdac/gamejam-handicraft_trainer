using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampSound : MonoBehaviour
{
    private void OnEnable()
    {
        DOTween.Sequence().
            AppendInterval(0.4f)
            .AppendCallback(() => SoundManager.Instance.Play2DSound(SFX.Stamp));
    }
}
