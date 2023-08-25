using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RootInfo : EventArgs
{
    public Vector3 m_rootTransform { get; set; }
    public Quaternion m_rootRotation { get; set; }
}

public class SplineRootController : MonoBehaviour
{
    public EventHandler<RootInfo> onRootControlling;
    
    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();
        _splines = new List<Spline>();
        foreach (Spline spline in _splineContainer.Splines)
        {
            _splines.Add(spline);
        }
    }

    private void Update()
    {
        // AutomaticGrowth();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _knotData = GetClosestSpline(_hitData.point);
            Debug.Log(_knotData.m_knot.Position);
            
            if (!IsLastKnotFromSpline(_knotData))
            {
                _splineContainer.AddSpline().Add(new BezierKnot(_knotData.m_knot.Position), TangentMode.AutoSmooth);
                _splineToModify = _splineContainer.Splines[^1];
            }
        }
        
        InterpolateLastKnot(_knotData, _splineToModify);

        CheckForPlayerInstruction(_knotData);
    }
    
    private KnotData GetClosestSpline(Vector3 hitPosition) 
    {
        Spline associatedSpline = _splines[0];
        BezierKnot closestKnot = _splines[0].ToArray()[0];

        for (int i = 0; i < _splines.Count; i++)
        {
            BezierKnot[] currentKnots = _splines[i].Knots.ToArray();
            for (int j = 0; j < currentKnots.Length; j++)
            {
                if (Vector3.Distance(currentKnots[j].Position, hitPosition)
                    < Vector3.Distance(closestKnot.Position, hitPosition))
                {
                    associatedSpline = _splines[i];
                    closestKnot = currentKnots[j];
                }
            }
        }

        return new KnotData(closestKnot, associatedSpline);
    }

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

    private bool IsLastKnotFromSpline(KnotData knotData)
    {
        return knotData.m_spline.ToArray()[^1].Equals(knotData.m_knot);
    }
    
    private void InterpolateLastKnot(KnotData knotData, Spline spline)
    {
        if (!_isInterpolating) return;
        
        // si la position finale est atteinte, on arrête l'interpolation
        if (Vector3.Distance(spline.Knots.ToArray()[spline.Knots.Count() - 1].Position, _nextFinalKnotPosition) <= 0 || !Input.GetKey(KeyCode.Mouse0))
        {
            _isInterpolating = false;
            return;
        }
        
        //on incrémente la position entre 0 et 1 que le knot courant doit avoir
        _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

        BezierKnot lastKnot = spline.Knots.ToArray()[spline.Knots.Count() - 1];

        //lerp pour obtenir la position absolue en fonction de la position entre 0 et 1
        lastKnot.Position = Vector3.Lerp(_previousKnotPosition, _nextFinalKnotPosition, _normalizedTargetKnotPosition);
        //le knot courant doit être orienté vers la position qu'il doit atteindre
        //lastKnot.Rotation = Quaternion.LookRotation(_nextFinalKnotPosition);
        
        //direction de la tangeante à appliquer au knot courant (direction du knot précédent vers le knot courant)
        // Vector3 tangentInDirection = (Vector3)lastKnot.Position - _previousKnotPosition;
        //
        // if (_nextFinalKnotPosition.z >= 0)
        // {
        //     lastKnot.TangentIn = tangentInDirection.normalized;
        //     lastKnot.TangentOut = -tangentInDirection.normalized;
        // }
        // else if (_nextFinalKnotPosition.z <= 0)
        // {
        //     lastKnot.TangentIn = -tangentInDirection.normalized;
        //     lastKnot.TangentOut = tangentInDirection.normalized;
        // }
        //
        // //on applique les modifs au knot courant
         spline.SetKnot(spline.Knots.Count() - 1, lastKnot);
        //
        // if (spline.Knots.Count() > 2)
        // {
        //     //le knot précédent doit être actualisé en fonction de la position du knot courant
        //     BezierKnot lastKnotMinusOne = spline.Knots.ToArray()[spline.Knots.Count() - 2];
        //     //il doit être orienté vers le knot courant
        //     lastKnotMinusOne.Rotation = Quaternion.LookRotation(lastKnot.Position);
        //
        //     if (spline.Knots.Count() > 2)
        //     {
        //         BezierKnot lastKnotMinusTwo = spline.Knots.ToArray()[spline.Knots.Count() - 3];
        //         tangentInDirection = (lastKnot.Position - lastKnotMinusTwo.Position);
        //     }
        //
        //     //s'il existe un knot avant le knot précédent, alors la tangeante du knot précédent est la direction du knot d'avant vers le knot courant. Sinon on reprend la même tangeante qu'au dessus
        //     // la tangeante in est en fait toujours égale à  la direction du knot précédent vers le knot suivant
        //
        //     if (_nextFinalKnotPosition.z >= 0)
        //     {
        //         lastKnotMinusOne.TangentIn = -tangentInDirection.normalized;
        //         lastKnotMinusOne.TangentOut = tangentInDirection.normalized;
        //     }
        //
        //     else if (_nextFinalKnotPosition.z <= 0)
        //     {
        //         lastKnotMinusOne.TangentIn = tangentInDirection.normalized;
        //         lastKnotMinusOne.TangentOut = -tangentInDirection.normalized;
        //     }
        //
        //     spline.SetKnot(spline.Knots.Count() - 2, lastKnotMinusOne);
        // }
        
        if (!_isAddingKnotWhileInterpolating)
        {
            StartCoroutine(AddKnotWhileInterpolating(spline));
        }
        _splineExtrude.Rebuild();
    }

    private BezierKnot CreateNewSpline(KnotData knotData)
    {
        _splineContainer.AddSpline().Add(new BezierKnot(knotData.m_knot.Position), TangentMode.AutoSmooth);
        return _splineContainer.Splines[^1].Knots.ToArray()[^1];
    }

    private void CheckForPlayerInstruction(KnotData knotData)
    {
        if (!Input.GetKey(KeyCode.Mouse0)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out _hitData, 100)) return;
        
        _nextFinalKnotPosition = new Vector3(_hitData.point.x, _hitData.point.y + 1, _hitData.point.z);

        Spline spline = knotData.m_spline;
        
        _previousKnotPosition = spline.Knots.ToArray()[spline.Knots.Count() - 1].Position;

        _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(_previousKnotPosition, _nextFinalKnotPosition);
        
        _normalizedTargetKnotPosition = 0;
        
        // onRootControlling?.Invoke(this, new RootInfo(){m_rootTransform = _previousKnotPosition,
        //     m_rootRotation = spline.Knots.ToArray()[spline.Knots.Count() - 1].Rotation});

        if (!_isInterpolating)
        {
            CreateNewSpline(knotData);
        }
        
        _isInterpolating = true;

        // if (!_isInterpolating)
        // {
        //     spline.Add(new BezierKnot(_previousKnotPosition), TangentMode.AutoSmooth);
        //     _isInterpolating = true;
        // }
    }

    // private void AutomaticGrowth()
    // {
    //     Spline spline = _splineContainer.Splines[0];
    //
    //     if (spline.Knots.Count() > 1 && !_isInterpolating && !_isAutoGrowing)
    //     {
    //         StartCoroutine(WaitForAutoGrowth(spline));
    //     }
    // }

    // IEnumerator WaitForAutoGrowth(Spline spline)
    // {
    //     _isAutoGrowing = true;
    //     yield return new WaitForSeconds(_autoGrowthPerSeconds);
    //     spline.Add(new BezierKnot(), TangentMode.Mirrored);
    //     _nextFinalKnotPosition = spline.Knots.ToArray()[spline.Knots.Count() - 1].Position;
    //     _isAutoGrowing = false;
    // }
    
    IEnumerator AddKnotWhileInterpolating(Spline spline)
    {
        _isAddingKnotWhileInterpolating = true;
        yield return new WaitForSeconds(_knotPerSecondsWhileDragging);
        spline.Add(new BezierKnot(_previousKnotPosition), TangentMode.AutoSmooth);
        _isAddingKnotWhileInterpolating = false;
    }
    
    [SerializeField] private float _distancePerSeconds;
    [SerializeField] private float _knotPerSecondsWhileDragging;
    [SerializeField] private float _growthPerSeconds;
    [Range(0,1)][SerializeField] private float _sharpToRoundedCurves;

    private List<Spline> _splines;
    private KnotData _knotData;
    private float3 _tangentIn;
    private float3 _tangentOut;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Spline _splineToModify;
    private RaycastHit _hitData;
    private Vector3 _nextFinalKnotPosition;
    private Vector3 _previousKnotPosition;
    private List<BezierKnot> _listBezierKnot;
    private bool _isInterpolating;
    private bool _isAddingKnotWhileInterpolating;
    private bool _isAutoGrowing;
    private float _normalizedDistancePerSeconds;
    private float _normalizedTargetKnotPosition;
}
