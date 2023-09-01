using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace RootFeature.Runtime
{
    public class Root : MonoBehaviour
    {
        public List<Spline> SplinesList
        {
            get => _splinesList;
            set => _splinesList = value;
        }


        private void Awake()
        {
            _splineExtrude = GetComponent<SplineExtrude>();
            _splineContainer = GetComponent<SplineContainer>();
        }

        private void Start()
        {
            _splinesList.Add(_splineContainer.Spline);
        }

        public Spline AddSpline(Vector3 position)
        {
            _splineContainer.AddSpline().Add(new BezierKnot(position), TangentMode.AutoSmooth);
            _splineContainer.Splines[^1].Add(new BezierKnot(position), TangentMode.AutoSmooth);
            _splinesList.Add(_splineContainer.Splines[^1]);
            return _splineContainer.Splines[^1];
        }
        
        public void Grow(Spline spline, Vector3 positionToGo)
        {
            _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(spline.ToArray()[^1].Position, positionToGo);
            _normalizedTargetKnotPosition = 0f;
            
            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = spline.Knots.ToArray()[spline.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(lastKnot.Position, positionToGo, _normalizedTargetKnotPosition);

            AddKnotWhileInterpolating(spline);
            
            spline.SetKnot(spline.Knots.Count() - 1, lastKnot);
        }
        
        public void DeleteIfTooClose(Spline spline)
        {
            if (Vector3.Distance(spline.ToArray()[^1].Position, spline.ToArray()[^2].Position) < 1 
                && spline.Count() > 2)
            {
                spline.Remove(spline.ToArray()[^1]);
            }
        }
        
        private void AddKnotWhileInterpolating(Spline spline)
        {
            Vector3 pos1 = spline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = spline.Knots.ToArray()[^1].Position;
            if (Vector3.Distance(pos1 , pos2) < _distanceBetweenKnots) return;
            
            spline.Add(new BezierKnot(pos2), TangentMode.AutoSmooth);
        }

        public void CanRebuild(bool isActive)
        {
            _splineExtrude.RebuildOnSplineChange = isActive;
        }
        
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        
        private List<Spline> _splinesList = new();
        private SplineExtrude _splineExtrude;
        private SplineContainer _splineContainer;
        private float _deltaKnotPosition;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
    }
}
