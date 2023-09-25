using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowRoot : MonoBehaviour
{
    public List<MeshRenderer> m_growRootsMeshes;
    public float m_timeToGrow;
    public float m_refreshRate;
    [Range(0, 1)] public float m_minGrow;
    [Range(0, 1)] public float m_maxGrow;
    
    void Start()
    {
        for (int i = 0; i < m_growRootsMeshes.Count; i++)
        {
            for (int j = 0; j < m_growRootsMeshes[i].materials.Length; j++)
            {
                if (m_growRootsMeshes[i].materials[j].HasProperty("Grow_"))
                {
                    m_growRootsMeshes[i].materials[j].SetFloat("Grow_", m_minGrow);
                    _growRootsMaterials.Add(m_growRootsMeshes[i].materials[j]);
                }
            }
        }
    }

    public void StartGrowRoot()
    {
        foreach (var material in _growRootsMaterials)
        {
            StartCoroutine(GrowingRoot(material));
        }
    }

    IEnumerator GrowingRoot(Material mat)
    {
        float growValue = mat.GetFloat("Grow_");

        if (!_isFullyGrown)
        {
            while (growValue < m_maxGrow)
            {
                growValue += 1 / (m_timeToGrow / m_refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(m_refreshRate);
            }
        }
        else
        {
            while (growValue > m_minGrow)
            {
                growValue -= 1 / (m_timeToGrow / m_refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(m_refreshRate);
            }
        }

        if (growValue >= m_maxGrow)
        {
            _isFullyGrown = true;
        }
        else
        {
            _isFullyGrown = false;
        }
    }

    private List<Material> _growRootsMaterials = new List<Material>();
    private bool _isFullyGrown;
}
