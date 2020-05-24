using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public ParticleSystem smoke1;

    private static ParticlesManager _instance;
    public static ParticlesManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlaySmokeEffect(Vector3 position)
    {
        ParticleSystem ps = Instantiate(smoke1, position, Quaternion.identity) as ParticleSystem;
        ps.Play();
        Destroy(ps.gameObject, ps.main.startLifetime.constantMax);
    }

}
