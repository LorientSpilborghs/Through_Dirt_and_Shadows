using System;
using System.Collections.Generic;
using System.Linq;
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
    public EventHandler<EventArgs> onRootCreation;

    public Vector3 m_nextFinalKnotPosition { get; set; }
    public Vector3 m_previousKnotPosition { get; set; }
    public RaycastHit m_hitData;
    private KnotData m_knotData;

    public struct KnotData
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

    public KnotData GetClosestSpline(Vector3 hitPosition) 
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
        
        if (Vector3.Distance(_splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position, m_nextFinalKnotPosition) <= 0 || !Input.GetKey(KeyCode.Mouse0))
        {
            _isInterpolating = false;
            return;
        }
        
        _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

        BezierKnot lastKnot = _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1];
        
        float newRotation = RotationUtility.LimitRotation(
            lastKnot.Rotation.value.y,
            _minRotation,
            _maxRotation);

        lastKnot.Rotation.value = new Vector4(
            lastKnot.Rotation.value.x, newRotation,lastKnot.Rotation.value.z);
        
        lastKnot.Position = Vector3.Lerp(m_previousKnotPosition, m_nextFinalKnotPosition, _normalizedTargetKnotPosition);
        
        _splineToModify.SetKnot(_splineToModify.Knots.Count() - 1, lastKnot);
        
        AddKnotWhileInterpolating(_splineToModify);
        
        _splineExtrude.Rebuild();
    }

    private void CheckForPlayerInstruction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out m_hitData, 100)) return;
        if (!Input.GetKey(KeyCode.Mouse0)) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_knotData = GetClosestSpline(m_hitData.point);
            
            if (!IsLastKnotFromSpline(m_knotData))
            {
                _splineContainer.AddSpline().Add(new BezierKnot(m_knotData.m_knot.Position), TangentMode.AutoSmooth);
                _splineContainer.Splines[^1].Add(new BezierKnot(m_knotData.m_knot.Position), TangentMode.AutoSmooth);
                _splinesList.Add(_splineContainer.Splines[^1]);
                _splineToModify = _splineContainer.Splines[^1];
            }
            else
            {
                _splineToModify = m_knotData.m_spline;
            }
        }
        
        m_previousKnotPosition = _splineToModify.Knots.ToArray()[_splineToModify.Knots.Count() - 1].Position;
        
        m_nextFinalKnotPosition = new Vector3(m_hitData.point.x, 0, m_hitData.point.z);
        
        _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(m_previousKnotPosition, m_nextFinalKnotPosition);
        
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
        spline.Add(new BezierKnot(m_previousKnotPosition), TangentMode.AutoSmooth);
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
    
    public static class RotationUtility
    {
        /// <param name="rotation">
        ///  Value of 0f to 360f.
        ///  Rotation that will be limitted to a certain angle.
        /// </param>
        public static float LimitRotation(
            float rotation,
            float minRotation,
            float maxRotation)
        {
            if (rotation > maxRotation ||
                rotation < minRotation)
            {
                float rotationDistanceFromMaxValue = Mathf.Abs(Mathf.DeltaAngle(rotation, maxRotation));
                float rotationDistanceFromMinValue = Mathf.Abs(Mathf.DeltaAngle(rotation, minRotation));

                if (rotationDistanceFromMaxValue < rotationDistanceFromMinValue)
                {
                    return maxRotation;
                }
                return minRotation;
            }

            return rotation;
        }
    }
    
    [SerializeField] private float _distancePerSeconds = 2.5f;
    [SerializeField] [Range(0.1f, 5f)] private float _distanceMinimum = 2;
    [SerializeField] private float _maxRotation;
    [SerializeField] private float _minRotation;

    private List<Spline> _splinesList;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Spline _splineToModify;
    private List<BezierKnot> _listBezierKnot;
    private bool _isInterpolating;
    private bool _isAddingKnotWhileInterpolating;
    private float _normalizedDistancePerSeconds;
    private float _normalizedTargetKnotPosition;
    private bool _hasStarted;
}