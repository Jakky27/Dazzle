using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] ParticleSystem tablePoundParticles;

    [SerializeField] AudioSource tableSlamAudio;
    [SerializeField] AudioSource chairScreechAudio;
    [SerializeField] AudioSource pointGrunt;
    [SerializeField] AudioSource slamGrunt;
    [SerializeField] AudioSource thinkGrunt;

    public void TablePound() {
        tablePoundParticles.Emit(25);
        tableSlamAudio.Play();
    }

    public void ChairScreech() {
        chairScreechAudio.Play();
    }

    public void PointGrunt() {
        pointGrunt.Play();
    }

    public void SlamGrunt() {
        slamGrunt.Play();
    }

    public void ThinkGrunt() {
        thinkGrunt.Play();
    }
}
