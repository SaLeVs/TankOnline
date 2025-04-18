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
    // we have a list of prefabs, and we need to put all of the prefabs that we want to syncronize, but for some reason in unity 6 this is automatic


    // ------------------ NetworkObject --------------------
    // We put the network object in the root, and all of the children will be syncronized
    // network transform, we will put and mark only what we want to syncronize, for example, if a object is not moving and he is a child of a object that is moving, we dont need to syncronize the object that is not moving
    // Basically, the client dont have the authority for move objects for example, we need to specify this
    // This process is not automatic, we need to specify what we want to syncronize
    // we send a info for the server and server process and send back what happen for client
    // when you give the authority for client of something, you are giving a opportunity for cheat
    // every object will enhit the network behaviour
    // we use a netowrk variable when we need that server control, but clients also need to know


    // ------------------ All learnings --------------------
    // When we change the object of shorting layer, we need to change the layer of the light too
    // Create a scriptable object for the input can be good, in input class we have a reference for the interface, and we can implement the interface in the scriptable object
    //      in this scriptable object we need to create a void OnEnable for active the inputs
    // all think what data you want, for example, if you want to move the object, you only need the transform, not the game object
    // never doubt your ability 
    // to deal with the collision, is better make this on editor side, not on script
    // make sure to disable all layers on edit physcs
    // everytime you use a clamp, you need to store the value in a variable
    // We have to pay attention to create a separe class, off each class make something


    // ------------------ THINGS TO STUDY AFTER COURSE --------------------
    // Learn more about checks If (isServer), If (isClient), owner etc
    // What is dispose
    // Study about backfill
    // Study about patterns for create multiplayer games


    // ------------------ SERVER THINGS --------------------
    // In one part of the course, we just import scripts from the Unity Sample

    // ----------------- SOLVING PROBLEMS --------------------
    // We need to try and reconfig servers on the UGS, because we are having some issues
    // We solve the problems on UGS, but we need to understand how to do a build on il2cpp  

}
