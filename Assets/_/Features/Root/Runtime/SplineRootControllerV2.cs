using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace PlayerRuntime
{   
    public class RootInfoV2 : EventArgs
    {
        public Vector3 m_rootTransform { get; set; }
        public Quaternion m_rootRotation { get; set; }
    }

    public class SplineRootControllerV2 : MonoBehaviour
    {
        

        public Vector3 m_nextFinalKnotPosition { get; set; }
        public Vector3 m_previousKnotPosition { get; set; }
        public bool m_IsInterpolating { get; set; }

        [FormerlySerializedAs("_splineToModify")]
        public Spline m_splineToModify;

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

        private void Awake()
        {
            _splineContainer = GetComponent<SplineContainer>();
            _splineExtrude = GetComponent<SplineExtrude>();
            _splinesList = new List<Spline>();
        }

        private void Start()
        {
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
            if (!m_IsInterpolating) return;
            
            if (Vector3.Distance(m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 1].Position,m_nextFinalKnotPosition) <= 0 || !Input.GetKey(KeyCode.Mouse0))
            {
                m_IsInterpolating = false;
                return;
            }

            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(m_previousKnotPosition, m_nextFinalKnotPosition, _normalizedTargetKnotPosition);

            AddKnotWhileInterpolating(m_splineToModify);
            m_splineToModify.SetKnot(m_splineToModify.Knots.Count() - 1, lastKnot);
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
                    Instantiate(_rootPrefab, m_knotData.m_knot.Position, Quaternion.identity, transform.parent);
                    _splineContainer.AddSpline().Add(new BezierKnot(m_knotData.m_knot.Position), TangentMode.AutoSmooth);
                    _splineContainer.Splines[^1].Add(new BezierKnot(m_knotData.m_knot.Position), TangentMode.AutoSmooth);
                    _splinesList.Add(_splineContainer.Splines[^1]);
                    m_splineToModify = _splineContainer.Splines[^1];
                }
                else
                {
                    m_splineToModify = m_knotData.m_spline;
                }
            }

            m_previousKnotPosition = m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 1].Position;

            Vector3 pointerPosition = new Vector3(m_hitData.point.x, 0, m_hitData.point.z);
            Vector3 pointerDirection = m_nextFinalKnotPosition - m_previousKnotPosition;
            Vector3 splineDirection = m_previousKnotPosition -(Vector3)m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 2].Position;
            float angle = Vector3.SignedAngle(pointerDirection, splineDirection, Vector3.up);
            Debug.Log(angle);

            if (angle > _maxAngle)
            {
                m_nextFinalKnotPosition = m_previousKnotPosition + Quaternion.Euler(0, _maxAngle, 0) * pointerDirection;
            }
            else if (angle < -_maxAngle)
            {
                m_nextFinalKnotPosition = m_previousKnotPosition - Quaternion.Euler(0, _maxAngle, 0) * pointerDirection;
            }
            else
            {
                m_nextFinalKnotPosition = pointerPosition;
            }

            _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(m_previousKnotPosition, m_nextFinalKnotPosition);
            _normalizedTargetKnotPosition = 0;
            m_IsInterpolating = true;
        }

        private void AddKnotWhileInterpolating(Spline spline)
        {
            if (Vector3.Distance(
                    m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 1].Position,
                    m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 2].Position) < _distanceMinimum)
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
                m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 1].Position,
                m_splineToModify.Knots.ToArray()[m_splineToModify.Knots.Count() - 2].Position) < 1;
        }

        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceMinimum = 2;
        [SerializeField] private float _maxAngle;
        [SerializeField] private GameObject _rootPrefab;
        //[SerializeField] private CameraFollow _cameraFollow;


        private List<Spline> _splinesList;
        private SplineContainer _splineContainer;
        private SplineExtrude _splineExtrude;
        private List<BezierKnot> _listBezierKnot;
        private bool _isAddingKnotWhileInterpolating;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
        private bool _hasStarted;
    }
}