using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
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
        
        // si la position finale est atteinte, on arrête l'interpolation
        if (Vector3.Distance(spline.Knots.ToArray()[spline.Knots.Count() - 1].Position, _nextFinalKnotPosition) <= 0)
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
        lastKnot.Rotation = Quaternion.LookRotation(_nextFinalKnotPosition);
        
        //direction de la tangeante à appliquer au knot courant (direction du knot précédent vers le knot courant)
        Vector3 tangentInDirection = (Vector3)lastKnot.Position - _previousKnotPosition;
        
        lastKnot.TangentIn = tangentInDirection.normalized;
        lastKnot.TangentOut = -tangentInDirection.normalized;

        //on applique les modifs au knot courant
        spline.SetKnot(spline.Knots.Count() - 1, lastKnot);
        
        //le knot précédent doit être actualisé en fonction de la position du knot courant
        BezierKnot lastKnotMinusOne = spline.Knots.ToArray()[spline.Knots.Count() - 2];
        //il doit être orienté vers le knot courant
        lastKnotMinusOne.Rotation = Quaternion.LookRotation(lastKnot.Position);
        
        if (spline.Knots.Count() > 2)
        {
            BezierKnot lastKnotMinusTwo = spline.Knots.ToArray()[spline.Knots.Count() - 3];
            tangentInDirection = (lastKnot.Position - lastKnotMinusTwo.Position);
        }

        //s'il existe un knot avant le knot précédent, alors la tangeante du knot précédent est la direction du knot d'avant vers le knot courant. Sinon on reprend la même tangeante qu'au dessus
        // la tangeante in est en fait toujours égale à  la direction du knot précédent vers le knot suivant
        lastKnotMinusOne.TangentIn = tangentInDirection.normalized;
        lastKnotMinusOne.TangentOut = -tangentInDirection.normalized;

        spline.SetKnot(spline.Knots.Count() - 2, lastKnotMinusOne);
        
        _splineExtrude.Rebuild();
    }

    private void CheckForPlayerInstruction()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out _hitData, 20)) return;
        
        _nextFinalKnotPosition = new Vector3(_hitData.point.x, Random.Range(0, 2), _hitData.point.z);
        
        Spline spline = _splineContainer.Splines[0];
        
        _previousKnotPosition = spline.Knots.ToArray()[spline.Knots.Count() - 1].Position;

        _normalizedDistancePerSeconds = _distancePerSeconds / Vector3.Distance(_previousKnotPosition, _nextFinalKnotPosition);
        
        _normalizedTargetKnotPosition = 0;
        
        if (!_isInterpolating)
        {
            spline.Add(new BezierKnot(_previousKnotPosition, _tangentIn, _tangentOut));
            _isInterpolating = true;
        }
    }
    
    [SerializeField] private float3 _tangentIn;
    [SerializeField] private float3 _tangentOut;
    [SerializeField] private float _distancePerSeconds;

    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Material _material;
    private RaycastHit _hitData;
    private Vector3 _nextFinalKnotPosition;
    private Vector3 _previousKnotPosition;
    private bool _isInterpolating;
    private float _normalizedDistancePerSeconds;
    private float _normalizedTargetKnotPosition;
}
