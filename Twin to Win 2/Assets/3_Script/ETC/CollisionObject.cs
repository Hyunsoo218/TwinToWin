using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionObject : MonoBehaviour
{
    public Action<Collider> OnEnterTrigger { get; set; }
    public Action<Collider> OnStayTrigger { get; set; }
    public Action<Collider> OnExitTrigger { get; set; }
    public Action<Collision> OnEnterCollision { get; set; }
    public Action<Collision> OnStayCollision { get; set; }
    public Action<Collision> OnExitCollision { get; set; }
	private void OnTriggerEnter(Collider other) => OnEnterTrigger?.Invoke(other);
	private void OnTriggerStay(Collider other) => OnStayTrigger?.Invoke(other);
	private void OnTriggerExit(Collider other) => OnExitTrigger?.Invoke(other);
    private void OnCollisionEnter(Collision collision) => OnEnterCollision?.Invoke(collision);
	private void OnCollisionStay(Collision collision) => OnStayCollision?.Invoke(collision);
	private void OnCollisionExit(Collision collision) => OnExitCollision?.Invoke(collision);
}
