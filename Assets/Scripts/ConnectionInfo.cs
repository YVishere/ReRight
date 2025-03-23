using System.Net.Sockets;
using System.Threading.Tasks;

public class ConnectionInfo
{
    public TcpClient Client { get; set; }
    public NetworkStream Stream { get; set; }
    
    public bool IsConnected => Client != null && Client.Connected;
    public void Close()
    {
        if (Stream != null) {
            Stream.Close();
            Stream = null;
        }
        
        if (Client != null) {
            Client.Close();
            Client = null;
        }
    }
}