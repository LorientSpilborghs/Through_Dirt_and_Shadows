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
        
        public void Grow(RootV2 root, Vector3 positionToGo)
        {
            Vector3 modifiedPositionToGo = new Vector3(positionToGo.x, _heightOfTheRoot, positionToGo.z);
            _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(root.Container.Spline.ToArray()[^1].Position, modifiedPositionToGo);
            _normalizedTargetKnotPosition = 0f;
            _normalizedTargetKnotPosition += _normalizedDistancePerSeconds * Time.deltaTime;

            BezierKnot lastKnot = root.Container.Spline.Knots.ToArray()[root.Container.Spline.Knots.Count() - 1];
            lastKnot.Position = Vector3.Lerp(lastKnot.Position, modifiedPositionToGo, _normalizedTargetKnotPosition);
            
            AddKnotWhileInterpolating(root);
            root.Container.Spline.SetKnot(root.Container.Spline.Knots.Count() - 1, lastKnot);
            _splineExtrude.Rebuild();
            UpdateFrontColliderPosition();
        }

        public void DeleteIfTooClose(RootV2 root)
        {
            if (Vector3.Distance(root.Container.Spline.ToArray()[^1].Position, root.Container.Spline.ToArray()[^2].Position) < 1 
                && root.Container.Spline.Count() > 2)
            {
                root.Container.Spline.Remove(root.Container.Spline.ToArray()[^1]);
            }
        }
        
        private void AddKnotWhileInterpolating(RootV2 root)
        {
            Vector3 pos1 = root.Container.Spline.Knots.ToArray()[^2].Position;
            Vector3 pos2 = root.Container.Spline.Knots.ToArray()[^1].Position;
            if (Vector3.Distance(pos1 , pos2) < _distanceBetweenKnots) return;
            
            root.Container.Spline.Add(new BezierKnot(pos2), TangentMode.AutoSmooth);
        }

        private void UpdateFrontColliderPosition()
        {
            _frontCollider.transform.position = Container.Spline[^1].Position;
        }
        
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private Collider _frontCollider;
        [Space]
        [SerializeField] private float _heightOfTheRoot;
        [SerializeField] private float _distancePerSeconds = 2.5f;
        [SerializeField] [Range(0.1f, 5f)] private float _distanceBetweenKnots = 2;
        
        private SplineExtrude _splineExtrude;
        private float _normalizedDistancePerSeconds;
        private float _normalizedTargetKnotPosition;
    }
}
