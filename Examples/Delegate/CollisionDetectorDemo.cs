using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using NnanaSoft.CollisionDetector;

public class CollisionDetectionDemo : MonoBehaviour
{
	CollisionDetector GroundCollision;
	public float height;
	public float cycle;

	private Rigidbody rb;

	// Use this for initialization
	void Start()
	{
		// コンポーネントを取得
		rb = GetComponent<Rigidbody>();
		GroundCollision = GetComponent<CollisionDetector>();

		// 一定時間おきに地面から離れたところに移動
		InvokeRepeating("ResetPosition", 0.0f, cycle);

		// OnCollisionEnter
		GroundCollision.onCollisionEnterSmoothed += collision =>
		{
			// ここに処理を記述
			Debug.Log("OnCollisionEnterSmoothed");
		};
		// OnCollisionStay
		GroundCollision.onCollisionStaySmoothed += collision =>
		{
			// ここに処理を記述
			Debug.Log("OnCollisionStaySmoothed");
		};
		// OnCollisionExit
		GroundCollision.onCollisionExitSmoothed += collision =>
		{
			// ここに処理を記述
			Debug.Log("OnCollisionExitSmoothed");
		};
	}

	private void ResetPosition()
	{
		rb.position = height * Vector3.up;
	}

	// OnCollisionEnter
	private void OnCollisionEnterSmoothed(Collision collision)
	{
		// ここに処理を記述
		Debug.Log("OnCollisionEnterSmoothed");
	}
	// OnCollisionStay
	private void OnCollisionStaySmoothed(Collision collision)
	{
		// ここに処理を記述
		Debug.Log("OnCollisionEnterSmoothed");
	}
	// OnCollisionEnter
	private void OnCollisionExitSmoothed(Collision collision)
	{
		// ここに処理を記述
		Debug.Log("OnCollisionEnterSmoothed");
	}
}
