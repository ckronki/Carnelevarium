using UnityEditor;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    [SerializeField] Transform _target;

    [SerializeField] Material _material;
    [SerializeField, Range(1f, 100f)] float _noiseAmount, _glitchStrength;
    [SerializeField] float _activationDistance, _distancePadding;
    [SerializeField] Color _gizmoColor;

    [SerializeField] float _normalizedDistance;

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(_target.position, transform.position);
        _normalizedDistance = 1 - Mathf.Clamp01((distance - _distancePadding) / _activationDistance);

        _material.SetFloat("_NoiseAmount", _noiseAmount * _normalizedDistance);
        _material.SetFloat("_GlitchStrength", _glitchStrength * _normalizedDistance);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(transform.position, _activationDistance);

        if (_target == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _target.transform.position - transform.position);

    }

}
