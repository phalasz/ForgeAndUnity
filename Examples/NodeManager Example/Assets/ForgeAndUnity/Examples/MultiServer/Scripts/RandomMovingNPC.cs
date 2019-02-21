﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

/// <summary>
/// It randomly moves!!!!
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(NavMeshAgent))]
public class RandomMovingNPC : RandomMovingNPCBehavior, INetworkSceneObject, IRPCSerializable {
    //Fields
    MeshRenderer _meshRenderer;
    NavMeshAgent _agent;

    public NetworkSceneManager Manager { get; set; }


    // Functions
    #region Unity
    void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        SetColor(new Color(Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f)));
    }

    protected override void NetworkStart () {
        base.NetworkStart();
        _agent.enabled = true;
        InvokeRepeating("MoveRandom", 1f, 2f);
    }

    void Update() {
        if (networkObject == null) {
            return;
        }

        if (networkObject.IsServer) {
            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
        } else {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
        }
    }

    #endregion

    #region Helpers
    public void SetColor (Color pColor) {
        if (_meshRenderer == null || pColor == null) {
            return;
        }

        _meshRenderer.material.color = pColor;
    }

    public void MoveRandom () {
        _agent.SetDestination(transform.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)));
    }

    #endregion

    #region INetworkSceneObject-Implementation
    public void SetNetworkObject (NetworkObject pNetworkObject) {
        networkObject = pNetworkObject as RandomMovingNPCNetworkObject;
    }

    public NetworkObject GetNetworkObject () {
        return networkObject;
    }

    public uint GetNetworkId () {
        return (networkObject != null) ? networkObject.NetworkId : 0;
    }

    #endregion

    #region IRPCSerializable-Implementation
    public byte[] ToByteArray () {
        Color color = _meshRenderer.material.color;
        return new byte[] { (byte)color.r, (byte)color.g, (byte)color.b };
    }

    public void FromByteArray (byte[] pByteArray) {
        Color color = new Color(pByteArray[0], pByteArray[1], pByteArray[2]);
        _meshRenderer.material.color = color;
    }

    #endregion
}
