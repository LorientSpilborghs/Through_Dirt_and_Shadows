using UnityEngine;
using UnityEngine.Splines;

public class SplineRootControllerForShow : MonoBehaviour
{
    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out _hitData, 20)) return;
        
        _nextKnotPosition = new Vector3(_hitData.point.x, Random.Range(0, 0.1f), _hitData.point.z);
        
        Spline spline = _splineContainer.Splines[0];
        
        spline.Add(new BezierKnot(_nextKnotPosition), TangentMode.Continuous);
        
        _splineExtrude.Rebuild();
    }

    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private RaycastHit _hitData;
    private Vector3 _nextKnotPosition;
    private Vector3 _lastKnotPosition;
}
