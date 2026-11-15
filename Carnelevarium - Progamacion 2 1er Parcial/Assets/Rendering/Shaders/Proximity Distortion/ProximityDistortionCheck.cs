using System;
using UnityEngine;

public class ProximityDistortionCheck : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _activationDistance, _distancePadding;
    [SerializeField, Range(0f, 1f)] float _minAmount;

    [SerializeField] float _normalizedDistance;

    private Material _material;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(_target.position, transform.position);
        _normalizedDistance = 1 - Mathf.Clamp01((distance - _distancePadding) / _activationDistance);

        if (_normalizedDistance < _minAmount) _normalizedDistance = _minAmount;

        _material.SetFloat("_Amount", _normalizedDistance);
    }
}
