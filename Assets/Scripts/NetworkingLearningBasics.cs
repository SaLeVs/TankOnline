using UnityEngine;

public class NetworkingLearningBasics : MonoBehaviour
{
    // Client-Server Model (self hosted)
    // server handle all the logic
    // client only send input to server

    // Pros
    // Simple, we dont worry about having dedicated servers
    // Cheap

    // Cons:
    // Lag 
    // Advantage, player that is hosting the server has the lowest ping
    // Cheaters, how code works in host side, can be manipulated by host

    // Client-Server Model (dedicated server)
    // PLayers are justing speaking to the server

    // Pros:
    // Security
    // We we use the relay, for dont allow other people connect directly to us
    // Fair, all player are in the same boat
    // Performance, when we are a client and host, we are running the code two times
    // this normally is called a headless server

    // Cons:
    // price, but we can use the UGS (Unity Game Server) for free
    // Complexity, is more complex to use than the self hosted server


    // ------------------ NetworkManager --------------------

    // We create a object called NetworkManager and put the script NetworkManager in it
    // All of objects that we want to synce, we need to put the NetworkObject


    // ------------------ NetworkObject --------------------
    // We put the network object in the root, and all of the children will be syncronized
    // network transform, we will put and mark only what we want to syncronize, for example, if a object is not moving and he is a child of a object that is moving, we dont need to syncronize the object that is not moving

}
