using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    public Animator Animator;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.02f;
    public float refreshRate = 0.05f;
    public float dieDelay = 0.2f;

    private List<Material> dissolveMaterials = new List<Material>();

    void Start()
    {
        if(VFXGraph != null)
        {
            VFXGraph.Stop();
            VFXGraph.gameObject.SetActive(false);
        }

        // El modelo viene partido en muchas piezas y cada una tiene su propio
        // SkinnedMeshRenderer, asi que se juntan los materiales de todas
        foreach(SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
            dissolveMaterials.AddRange(smr.materials);

        // Arranca siempre entero, tenga el valor que tenga el material
        foreach(Material mat in dissolveMaterials)
            mat.SetFloat("DissolveAmount_", 0f);
    }

    // Lo llama EnemyBasic.Death() cuando el enemigo se queda sin vida
    public void StartDissolve()
    {
        StartCoroutine(Dissolve());
    }

    IEnumerator Dissolve()
    {
        if(Animator != null)
            Animator.SetTrigger("Die");

        yield return new WaitForSeconds(dieDelay);

        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }

        float counter = 0f;
        while (counter < 1)
        {
            counter += dissolveRate;
            foreach(Material mat in dissolveMaterials)
                mat.SetFloat("DissolveAmount_", counter);

            yield return new WaitForSeconds(refreshRate);
        }

        // Siempre se destruye al final, aunque no haya encontrado materiales,
        // porque EnemyBasic ya no lo desactiva cuando muere
        Destroy(gameObject, 1);
    }

    void OnDestroy()
    {
        // .materials crea copias de los materiales que Unity no borra solo
        foreach(Material mat in dissolveMaterials)
            Destroy(mat);
    }
}
