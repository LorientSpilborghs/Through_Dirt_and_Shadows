using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SplineRootController : MonoBehaviour
{
    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        InterpolateLastKnot();

        CheckForPlayerInstruction();
    }

    private void InterpolateLastKnot()
    {
        if (!_isInterpolating) return;
        
        Spline spline = _splineContainer.Splines[0];
        
        BezierKnot lastKnot = spline.Knots.ToArray()[spline.Knots.Count() - 1];
        
        
        
        if (_rootTimer < _totalGrowTime)
        {
            _rootTimer += Time.deltaTime;
        }

        spline.Knots.ToArray()[spline.Knots.Count() - 1].Position = Vector3.Lerp(_lastKnotPosition, _nextKnotPosition, _rootTimer/_totalGrowTime);
        _splineExtrude.Rebuild();
    }

    private void CheckForPlayerInstruction()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out _hitData, 20)) return;
        _nextKnotPosition = new Vector3(_hitData.point.x, Random.Range(0, 3), _hitData.point.z);
        Spline spline = _splineContainer.Splines[0];
        _lastKnotPosition = spline.Knots.ToArray()[spline.Knots.Count() - 1].Position;
        
        
        if (!_isInterpolating)
        {
            spline.Add(new BezierKnot(_nextKnotPosition, _tangentIn, _tangentOut));
            _isInterpolating = true;
            _rootTimer = 0;
        }
        
    }
    

    [SerializeField] private float3 _tangentIn;
    [SerializeField] private float3 _tangentOut;

    private float _totalGrowTime = 3;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Material _material;
    private RaycastHit _hitData;
    private float _rootTimer;
    private Vector3 _nextKnotPosition;
    private Vector3 _lastKnotPosition;
    private bool _isInterpolating;
}
