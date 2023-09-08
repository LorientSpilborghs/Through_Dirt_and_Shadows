using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace RootFeature.Runtime
{
    public class RootV2 : MonoBehaviour
    {
        public SplineContainer Container
        {
            get => _splineContainer;
            set => _splineContainer = value;
        }

        private void Awake()
        {
            _splineExtrude = GetComponent<SplineExtrude>();
        }

        private void Start()
        {
            _mySpline = Container.Spline;
        }
        
        public void Grow(RootV2 root, Vector3 positionToGo)
        {
            Vector3 modifiedPositionToGo = new Vector3(positionToGo.x, _heightOfTheRoot, positionToGo.z);
            _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(root._mySpline.ToArray()[^1].Position, modifiedPositionToGo);
            _normalizedTargetKnotPosition = 0f;
            
            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = root._mySpline.Knots.ToArray()[root._mySpline.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(lastKnot.Position, modifiedPositionToGo, _normalizedTargetKnotPosition);

            AddKnotWhileInterpolating(root);
            
            root._mySpline.SetKnot(root._mySpline.Knots.Count() - 1, lastKnot);
            _splineExtrude.Rebuild();
        }
        
        public void DeleteIfTooClose(RootV2 root)
        {
            if (Vector3.Distance(root._mySpline.ToArray()[^1].Position, root._mySpline.ToArray()[^2].Position) < 1 
                && root._mySpline.Count() > 2)
            {
                root._mySpline.Remove(root._mySpline.ToArray()[^1]);
            }
        }
        
        
        private void AddKnotWhileInterpolating(RootV2 root)
        {
            Vector3 pos1 = root._mySpline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = root._mySpline.Knots.ToArray()[^1].Position;
            if (Vector3.Distance(pos1 , pos2) < _distanceBetweenKnots) return;
            
            root._mySpline.Add(new BezierKnot(pos2), TangentMode.AutoSmooth);
        }

        public void CanRebuild(bool isActive)
        {
            _splineExtrude.RebuildOnSplineChange = isActive;
        }
        
        [SerializeField] private SplineContainer _splineContainer;
        [Space]
        [SerializeField] private float _heightOfTheRoot;
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        
        private Spline _mySpline;
        private SplineExtrude _splineExtrude;
        private Mesh _mesh;
        private float _deltaKnotPosition;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
    }
}
