using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MetaGenerator : MonoBehaviour
{
  public int gridW = 3;
  public int gridH = 3;
  public float itemGap = .1f;
	public int seed;

  public Vector3 itemSize = new Vector3(1, 1, 1);

  public bool fitToGrid = true;
  public bool keepAspectRatio = true;

  public GameObject generator;
  public Transform parent;

	void Reset() {
		Randomize();
	}

	public void Randomize() {
		seed = Random.Range((int)-1e9, (int)1e9);
	}

  public void ClearAll()
  {
    for (var i = parent.childCount - 1; i >= 0; --i)
    {
      var t = parent.GetChild(i);
      DestroyImmediate(t.gameObject);
    }
  }

  public void GenerateAll()
  {
    if (parent == null)
    {
      parent = generator.transform;
    }

    ClearAll();
		Random.InitState(seed);

    var comps = generator.GetComponents<MonoBehaviour>();
    var genComps = comps.Where(c => c is IGenerator).Cast<IGenerator>();
    if (genComps.Count() == 0)
    {
      Debug.LogError("Invalid Generator object must have at least one component implementing IGenerator");
      return;
    }

    //var totalSize = itemSize + itemGap;
    var totalSize = itemSize + Vector3.one * itemGap;
    totalSize.Scale(new Vector3(gridW, gridH, 1));
    var halfSize = totalSize / 2;

    var genComp = genComps.First();
    for (var j = 0; j < gridH; ++j)
    {
      var y = j * (itemSize.y + itemGap);
      for (var i = 0; i < gridW; ++i)
      {
        var go = Generate(genComp);

        var x = i * (itemSize.x + itemGap);

        go.transform.localPosition = new Vector3(x, y, 0) - halfSize;
      }
    }
  }

  public GameObject Generate(IGenerator gen)
  {
    var seed = (int)(Random.Range(-1.0f, 1.0f) * 2e9);

    var obj = gen.GenerateObject();
    var go = obj.gameObject;

    FitBounds(go);

    go.transform.parent = parent;

    return go;
  }

  ///
  /// Compute bounds of GO recursively
  ///
  Bounds ComputeBoundsRecursively(GameObject go)
  {
    Bounds? _bounds = GetBounds(go);
    Bounds bounds;
    if (!_bounds.HasValue)
    {
      bounds = new Bounds(go.transform.position, Vector3.zero);
    }
    else
    {
      bounds = _bounds.Value;
    }
    ComputeBoundsRecursively(go, bounds);
    return bounds;
  }

  void ComputeBoundsRecursively(GameObject go, Bounds bounds)
  {
    Bounds? b = GetBounds(go);
    if (b.HasValue)
    {
      bounds.Encapsulate(b.Value);
    }
    foreach (Transform child in go.transform)
    {
      ComputeBoundsRecursively(child.gameObject, bounds);
    }
  }

  Bounds? GetBounds(GameObject go)
  {
    if (go.GetComponent<MeshFilter>())
    {
      return GetBounds(go.GetComponent<MeshFilter>());
    }
    if (go.GetComponent<SpriteRenderer>())
    {
      return GetBounds(go.GetComponent<SpriteRenderer>());
    }
    return null;
  }

  Bounds GetBounds(MeshFilter meshFilter)
  {
    var mesh = meshFilter.sharedMesh;
    return mesh.bounds;
  }

  Bounds GetBounds(SpriteRenderer spriteRenderer)
  {
    return spriteRenderer.bounds;
  }

  void FitBounds(GameObject go)
  {
    var bounds = ComputeBoundsRecursively(go);
    _FitBounds(go, bounds);
  }

  void _FitBounds(GameObject go, Bounds bounds)
  {
    var b = 2 * bounds.extents;

    if (fitToGrid)
    {
      var ratioX = b.x / itemSize.x;
      var ratioY = b.y / itemSize.y;
      var ratioZ = b.z / itemSize.z;

      var maxDim = Mathf.Max(
                     ratioX, ratioY, ratioZ
                   );

      //if (maxDim > 1)
      {
        Vector3 scale;
        if (keepAspectRatio)
        {
          scale = Vector3.one / maxDim;
        }
        else
        {
          scale = new Vector3(
            Mathf.Max(1, ratioX),
            Mathf.Max(1, ratioY),
            Mathf.Max(1, ratioZ)
          );
        }
        go.transform.localScale = scale;
      }
    }
  }

}
