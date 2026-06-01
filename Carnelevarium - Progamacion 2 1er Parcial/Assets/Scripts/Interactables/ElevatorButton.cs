using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [SerializeField] Animator doorAnimator;
    [SerializeField] Transform ascensor; 
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeMagnitude;
    [SerializeField] string nextSceneName;

    public void Interact()
    {
        StartCoroutine(ActivateElevator());
    }

    private IEnumerator ActivateElevator()
    {
        doorAnimator.SetBool("Open", false);
        yield return new WaitForSeconds(1.5f);
        Vector3 originalPos = ascensor.position;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            ascensor.position = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ascensor.position = originalPos;

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nextSceneName);
    }
}
