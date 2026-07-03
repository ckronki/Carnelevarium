using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Light))]
public class LightLOD : MonoBehaviour
{
    private Light Light;
    public bool lightShouldBeOn = true;
    [Range(0, 1)]
    private float updateDelay = 0.1f;
    [SerializeField] private List<LODAdjustment> LODLevels = new();


    private void Awake()
    {
        Light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        StartCoroutine(AdjustLODQuality());
    }

    private IEnumerator AdjustLODQuality()
    {
        float delay = updateDelay + updateDelay == 0 ? updateDelay : UnityEngine.Random.value / 20f;
        WaitForSeconds wait = new(delay);

        while (true)
        {
            if (LightLODCamera.instance != null)
            {
                yield return wait;
                continue;
            }

            if (lightShouldBeOn)
            {
                float squareDistanceFromCamera = Vector3.SqrMagnitude(LightLODCamera.instance.transform.position - transform.position);

                for (int i = 0; i < LODLevels.Count; i++)
                {
                    if (i == LODLevels.Count - 1 || (
                        squareDistanceFromCamera > LODLevels[i].minSquareDistance
                        && squareDistanceFromCamera <= LODLevels[i].maxSquareDistance
                        ))
                    {
                        Light.enabled = true;
                        Light.shadows = LODLevels[i].lightShadows;
                        if (QualitySettings.shadowResolution <= LODLevels[i].shadowResolution)
                        {
                            Light.shadowResolution = (LightShadowResolution)QualitySettings.shadowResolution;
                        }
                        else
                        {
                            Light.shadowResolution = (LightShadowResolution)LODLevels[i].shadowResolution;
                        }

                        break;
                    }
                }
            }
            else
            {
                Light.enabled = false;
            }

            yield return wait;
        }
    }
}
