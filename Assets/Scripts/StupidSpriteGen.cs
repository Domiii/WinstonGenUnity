using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidSpriteGen : MonoBehaviour, IGenerator {
	public Sprite[] sprites;

	public GameObject GenerateObject() {
		//var go = new GameObject (string.Format ("Sprite_{0:X4}", seed));
		var go = new GameObject ("StupidSprite");
		var s = sprites[Random.Range(0, sprites.Length)];
		var c = go.AddComponent<SpriteRenderer>();

		c.sprite = s;
		c.color = Random.ColorHSV();

		return go;
	}
}
