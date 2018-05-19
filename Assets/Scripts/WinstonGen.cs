using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinstonGen : MonoBehaviour, IGenerator
{
  public Sprite circlePrefab;
	public Sprite eyeBrowPrefab;
  public Color faceColor, eyeColor, mouthColor, eyeBrowColor;

  delegate void EyeBrowMod(SpriteRenderer brow1, SpriteRenderer brow2);

  void SetupEyeBrow(SpriteRenderer brow)
  {
		brow.sortingOrder = 100;
    brow.transform.localPosition = new Vector2(-.15f, 0.3f + Random.Range(-.1f, .1f));

		var s = new Vector2(.25f + Random.Range(-.1f, .1f), 0.08f + Random.Range(-.04f, .04f));
		// s.Scale(new Vector2(.03f, .4f));
		brow.transform.localScale = s;

    var angle = Random.Range(-80, 80);
    brow.transform.transform.Rotate(Vector3.forward, angle);
  }

  EyeBrowMod[] eyeBrowMods;

  public WinstonGen()
  {
    eyeBrowMods = new EyeBrowMod[] {
		// equal
		(brow1, brow2) => {
      var p = brow1.transform.localPosition;
      brow2.transform.localPosition = new Vector2(-p.x, p.y);
      brow2.transform.localScale = brow1.transform.localScale;
      brow2.transform.rotation = brow1.transform.rotation;
    },

		// mirrored
		(brow1, brow2) => {
      var p = brow1.transform.localPosition;
      brow2.transform.localPosition = new Vector2(-p.x, p.y);
      brow2.transform.localScale = brow1.transform.localScale;

      var euler = brow1.transform.rotation.eulerAngles;
      brow2.transform.eulerAngles = -euler;
    }, 

		// different
		(brow1, brow2) => {
      SetupEyeBrow(brow2);
      var p = brow1.transform.localPosition;
      brow2.transform.localPosition = new Vector2(-p.x, p.y);
    }
  };
  }

  SpriteRenderer CreateSprite(GameObject parent, Sprite sprite, string name)
  {
    var go = new GameObject(string.Format(name));
    var c = go.AddComponent<SpriteRenderer>();
    c.sprite = sprite;
		if (parent != null) {
			go.transform.SetParent(parent.transform);
			c.sortingOrder = 10;
		}
    return c;
  }

  SpriteRenderer CreateCircle(GameObject parent)
  {
    return CreateSprite(parent, circlePrefab, "Circle");
  }

  SpriteRenderer CreateEyebrow(GameObject parent)
  {
    return CreateSprite(parent, eyeBrowPrefab, "EyeBrow");
  }

  public GameObject GenerateObject()
  {
    var face = CreateCircle(null);
    //face.transform.localScale = new Vector3(Random.Range(.4f, 1f), Random.Range(.4f, 1f), 1);
    face.color = faceColor;

    var eye1 = CreateCircle(face.gameObject);
    eye1.color = eyeColor;
    eye1.transform.localScale = new Vector3(Random.Range(.05f, .2f), Random.Range(.05f, .2f), 1);
    var eye2 = Instantiate(eye1);
		eye2.transform.parent = face.transform;
    eye1.transform.localPosition = new Vector2(-.15f, 0.1f);
    eye2.transform.localPosition = new Vector2(.15f, 0.1f);

    var eyeBrow1 = CreateEyebrow(face.gameObject);
    eyeBrow1.color = eyeBrowColor;
    SetupEyeBrow(eyeBrow1);
    var eyeBrow2 = Instantiate(eyeBrow1);
		eyeBrow2.transform.parent = face.transform;

    var f = eyeBrowMods[Random.Range(0, eyeBrowMods.Length)];
    f(eyeBrow1, eyeBrow2);


    var mouth = CreateCircle(face.gameObject);
    var mouthSize = Random.Range(.05f, .5f);
    mouth.color = mouthColor;
    mouth.transform.localScale = new Vector3(mouthSize, mouthSize, 1);
    mouth.transform.Translate(0, -.3f, 0);

    return face.gameObject;
  }
}
