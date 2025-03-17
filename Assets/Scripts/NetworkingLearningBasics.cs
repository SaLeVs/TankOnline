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
}
