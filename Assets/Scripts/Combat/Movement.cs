using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	bool isMoving = false;
	Vector2 input;
	Vector3 startPosition;
	Vector3 endPosition;
	float time;

	public float moveSpeed = 3f;

	// Update is called once per frame
	void Update() {
		if (!isMoving) {
			input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if (input != Vector2.zero) {
				StartCoroutine(Move(transform));
			}
		}
	}

	public IEnumerator Move(Transform entity) {
		isMoving = true;
		startPosition = entity.position;
		time = 0;

		endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x), startPosition.y + System.Math.Sign(input.y), startPosition.z);

		while (time < 1f) {
			time += Time.deltaTime * moveSpeed;
			entity.position = Vector3.Lerp(startPosition, endPosition, time);
			yield return null;
		}

		isMoving = false;
		yield return 0;
	}
}
