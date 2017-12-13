using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NnanaSoft.CollisionDetector
{
	public class CollisionDetector : MonoBehaviour
	{
		// Delegate
		public delegate void OnCollisionEnterSmoothedDelegate(Collision collision);
		public delegate void OnCollisionStaySmoothedDelegate(Collision collision);
		public delegate void OnCollisionExitSmoothedDelegate(Collision collision);
		public OnCollisionEnterSmoothedDelegate onCollisionEnterSmoothed;
		public OnCollisionStaySmoothedDelegate onCollisionStaySmoothed;
		public OnCollisionExitSmoothedDelegate onCollisionExitSmoothed;
		// 衝突情報保持
		private Collision _otherOnEnter;
		private Collision _otherOnStay;
		private Collision _otherOnExit;
		// 衝突相手のタグをチェックするかしないか
		public bool checkOtherTag = true;
		public string targetTag;
		// カットオフ周波数：小さいほど急な動きが抑制される
		[SerializeField] private float _cutOffFrequency = 30.0f;
		public float CutOffFrequency
		{
			get { return _cutOffFrequency; }
			set
			{
				_cutOffFrequency = value;
				CalculateFilterConstant();
			}
		}
		// フィルタ用の定数：大きいほど衝突判定が変化しにくくなり安定
		private float _filterConstant;
		// Time.fixedDeltaTimeの変化を監視
		private float _fixedDeltaTimeObserved;
		// 衝突判定の閾値：上限と下限を用意して判定の変化にヒステリシスを持たせる
		[Range(0.0f, 1.0f)] public float detectionThresholdUpper = 0.9f;
		[Range(0.0f, 1.0f)] public float detectionThresholdLower = 0.1f;
		// 衝突フラグ
		public bool IsOnCollisionEnter { get { return _isOnCollisionEnter; } }
		public bool IsOnCollisionStay { get { return _isOnCollisionStay; } }
		public bool IsOnCollisionExit { get { return _isOnCollisionExit; } }
		private bool _isOnCollision = false;
		[SerializeField] private bool _isOnCollisionEnter = false;
		[SerializeField] private bool _isOnCollisionStay = false;
		[SerializeField] private bool _isOnCollisionExit = false;
		// 衝突判定の平滑用
		private bool _onCollisionRaw = false;
		private float _onCollisionFloat = 0.0f;

		// カットオフ周波数から衝突判定平滑用の定数を計算
		public void CalculateFilterConstant()
		{
			_filterConstant = Mathf.Exp(-CutOffFrequency * _fixedDeltaTimeObserved);
		}
		// インスペクタ上でカットオフ周波数を変更した時用
		private void OnValidate()
		{
			CalculateFilterConstant();
		}

		// Use this for initialization
		private void Start()
		{
			// 値の初期化
			if (_isOnCollision)
				_onCollisionFloat = 1.0f;
			else
				_onCollisionFloat = 0.0f;
			_fixedDeltaTimeObserved = Time.fixedDeltaTime;
			CalculateFilterConstant();
		}

		// FixedUpdate内で平滑化した衝突判定を計算
		private void FixedUpdate()
		{
			// Time.fixedDeltaTimeが変更された場合はフィルタの定数を更新
			if (Mathf.Approximately(Time.fixedDeltaTime, _fixedDeltaTimeObserved) == false)
			{
				_fixedDeltaTimeObserved = Time.fixedDeltaTime;
				CalculateFilterConstant();
			}

			// 1サイクル前の衝突情報を保持してEnter, Stay, Exitの判定に使用
			bool onCollisionPrevious = _isOnCollision;
			int onCollisionCurrent = 0;
			if (_onCollisionRaw)
				onCollisionCurrent = 1;
			// 衝突しているかどうかを数値化。_filterConstantが大きいほど1サイクル前の状態を優先
			_onCollisionFloat = (1.0f - _filterConstant) * onCollisionCurrent + _filterConstant * _onCollisionFloat;

			// _onCollisionFloatの値を0.0~1.0に制限
			if (_onCollisionFloat > 1.0f)
				_onCollisionFloat = 1.0f;
			else if (_onCollisionFloat < 0.0f)
				_onCollisionFloat = 0.0f;

			if (onCollisionPrevious)
			{
				// 1つ前のサイクルで衝突している場合、下限閾値を下回るまで状態を変化させない
				if (_onCollisionFloat <= detectionThresholdLower)
					_isOnCollision = false;
				else
					_isOnCollision = true;
			}
			else
			{
				// 1つ前のサイクルで衝突していない場合、上限閾値を上回るまで状態を変化させない
				if (_onCollisionFloat >= detectionThresholdUpper)
					_isOnCollision = true;
				else
					_isOnCollision = false;
			}

			// OnCollisionEnterか、Stayか、Exitか判定
			if (_isOnCollision == onCollisionPrevious)
			{
				_isOnCollisionEnter = false;
				_isOnCollisionExit = false;
			}
			else
			{
				if (_isOnCollision)
				{
					_isOnCollisionEnter = true;
					// Delegateメソッドを実行
					OnCollisionEnterSmoothed(onCollisionEnterSmoothed);
				}
				else
				{
					_isOnCollisionExit = true;
					// Delegateメソッドを実行
					OnCollisionExitSmoothed(onCollisionExitSmoothed);
				}
			}
			_isOnCollisionStay = _isOnCollision;
			if (_isOnCollisionStay)
			{
				// Delegateメソッドを実行
				OnCollisionStaySmoothed(onCollisionStaySmoothed);
			}
		}
		// OnCollisionEnter
		private void OnCollisionEnter(Collision collision)
		{
			// タグが一致しない場合は何もしない
			if (checkOtherTag == false || collision.gameObject.CompareTag(targetTag) == true)
			{
				_onCollisionRaw = true;
				_otherOnEnter = collision;
				_otherOnStay = collision;
			}
		}
		// OnCollisionStay
		private void OnCollisionStay(Collision collision)
		{
			// タグが一致しない場合は何もしない
			if (checkOtherTag == false || collision.gameObject.CompareTag(targetTag) == true)
			{
				_otherOnStay = collision;
			}
		}
		// OnCollisionExit
		private void OnCollisionExit(Collision collision)
		{
			// タグが一致しない場合は何もしない
			if (checkOtherTag == false || collision.gameObject.CompareTag(targetTag) == true)
			{
				_onCollisionRaw = false;
				_otherOnExit = collision;
			}
		}
		// Delegate
		private void OnCollisionEnterSmoothed(OnCollisionEnterSmoothedDelegate onEnter)
		{
			onEnter(_otherOnEnter);
		}
		private void OnCollisionStaySmoothed(OnCollisionStaySmoothedDelegate onStay)
		{
			onStay(_otherOnStay);
		}
		private void OnCollisionExitSmoothed(OnCollisionExitSmoothedDelegate onExit)
		{
			onExit(_otherOnExit);
		}
	}
}
