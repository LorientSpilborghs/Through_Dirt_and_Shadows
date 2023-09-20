using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRoot : MonoBehaviour
{
    [Range(0, 1)] public float m_minGrow;
    [Range(0, 1)] public float m_maxGrow;
    
    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();
        _material = GetComponent<MeshRenderer>().material;
        
        _material.SetFloat("Grow_", m_minGrow);

        StartCoroutine(DrawSpline());
        StartCoroutine(DisplayRoot());
    }

    IEnumerator DrawSpline()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            Spline spline = _splineContainer.Splines[0];

            BezierKnot lastKnot = spline.Knots.ToArray()[spline.Knots.Count() - 1];
            
            spline.Add(new BezierKnot(lastKnot.Position  + (float3) Vector3.forward));

            _splineExtrude.Rebuild();
        }
    }

    IEnumerator DisplayRoot()
    {
        float growValue = _material.GetFloat("Grow_");
        
        while (growValue < m_maxGrow)
        {
            growValue += 1 / (3 / 0.03f);
            _material.SetFloat("Grow_", growValue);

            yield return new WaitForSeconds(0.03f);
        }
    }
    
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private Material _material;
}
