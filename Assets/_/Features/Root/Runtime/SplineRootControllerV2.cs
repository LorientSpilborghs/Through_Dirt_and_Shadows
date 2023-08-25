using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RootInfoV2 : EventArgs
{
    public Vector3 m_rootTransform { get; set; }
    public Quaternion m_rootRotation { get; set; }
}

public class SplineRootControllerV2 : MonoBehaviour
{
    public EventHandler<RootInfoV2> onRootControlling;
    
    private struct KnotData
    {
        public KnotData(BezierKnot knot, Spline spline)
        {
            m_knot = knot;
            m_spline = spline;
        }
        
        public BezierKnot m_knot;
        public Spline m_spline;
    }
    
    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();
        _splinesList = new List<Spline>();
        foreach (Spline spline in _splineContainer.Splines)
        {
            _splinesList.Add(spline);
        }
    }

    private void Update()
    {
        InterpolateLastKnot();
        CheckForPlayerInstruction();
        Release();
    }
    
    private KnotData GetClosestSpline(Vector3 hitPosition) 
    {
        Spline associatedSpline = _splinesList[0];
        BezierKnot closestKnot = _splinesList[0].ToArray()[0];

        for (int i = 0; i < _splinesList.Count; i++)
        {
            BezierKnot[] currentKnots = _splinesList[i].Knots.ToArray();
            for (int j = 0; j < currentKnots.Length; j++)
            {
                if (Vector3.Distance(currentKnots[j].Position, hitPosition)
                    < Vector3.Distance(closestKnot.Position, hitPosition))
                {
                    associatedSpline = _splinesList[i];
                    closestKnot = currentKnots[j];
                }
            }
        }
        return new KnotData(closestKnot, associatedSpline);
    }

    private bool IsLastKnotFromSpline(KnotData knotData)
    {
        return knotData.m_spline.ToArray()[^1].Equals(knotData.m_knot);
    }
    
    private void InterpolateLastKnot()
    {
        if (!_isInterpolating) return;
        
        if (Vector3.Distance(_splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position, _nextFinalKnotPosition) <= 0 || !Input.GetKey(KeyCode.Mouse0))
        {
            _isInterpolating = false;
            return;
        }
        
        _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

        BezierKnot lastKnot = _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1];

        lastKnot.Position = Vector3.Lerp(_previousKnotPosition, _nextFinalKnotPosition, _normalizedTargetKnotPosition);
        
        _splineToModify.SetKnot(_splineToModify.Knots.Count() - 1, lastKnot);
        
        AddKnotWhileInterpolating(_splineToModify);
        
        _splineExtrude.Rebuild();
    }

    private void CheckForPlayerInstruction()
    {
        if (!Input.GetKey(KeyCode.Mouse0)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out _hitData, 100)) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _knotData = GetClosestSpline(_hitData.point);
            
            if (!IsLastKnotFromSpline(_knotData))
            {
                _splineContainer.AddSpline().Add(new BezierKnot(_knotData.m_knot.Position), TangentMode.AutoSmooth);
                _splineContainer.Splines[^1].Add(new BezierKnot(_knotData.m_knot.Position), TangentMode.AutoSmooth);
                _splinesList.Add(_splineContainer.Splines[^1]);
                _splineToModify = _splineContainer.Splines[^1];
            }
            else
            {
                _splineToModify = _knotData.m_spline;
            }
        }
        
        _nextFinalKnotPosition = new Vector3(_hitData.point.x, 0, _hitData.point.z);
        
        _previousKnotPosition = _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position;

        _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(_previousKnotPosition, _nextFinalKnotPosition);
        
        _normalizedTargetKnotPosition = 0;
        
        _isInterpolating = true;
    } 
    
    private void AddKnotWhileInterpolating(Spline spline)
    {
        if (Vector3.Distance(
            _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position, 
            _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 2].Position) < _distanceMinimum)
        {
            return;
        }
        if (Release()) return;
        spline.Add(new BezierKnot(_previousKnotPosition), TangentMode.AutoSmooth);
    }

    private bool Release()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _hasStarted = false;
            return IsTooClose();
        }
        return false;
    }
    
    private bool IsTooClose()
    {
        return Vector3.Distance(
             _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position, 
             _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 2].Position) < 1;
    }
    
    [SerializeField] private float _distancePerSeconds;
    [SerializeField] [Range(0.1f, 5f)] private float _distanceMinimum = 1;

    private List<Spline> _splinesList;
    private KnotData _knotData;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Spline _splineToModify;
    private RaycastHit _hitData;
    private Vector3 _nextFinalKnotPosition;
    private Vector3 _previousKnotPosition;
    private List<BezierKnot> _listBezierKnot;
    private bool _isInterpolating;
    private bool _isAddingKnotWhileInterpolating;
    private float _normalizedDistancePerSeconds;
    private float _normalizedTargetKnotPosition;

    private float currentTime;
    private bool _hasStarted;
}