using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative() // we no need this more because we have a dropdown on network transform that do that
    {
        return false;
    }
}
