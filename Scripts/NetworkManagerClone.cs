using Mirror;

public class NetworkManagerClone : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        print("Added player");
    }

}
