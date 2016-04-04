namespace Netsphere.Network
{
    public enum AuthOpCode : ushort
    {
        CAuthInReq = 5001,
        CAuthInEuReq = 5002,
        CServerListReq = 5003,

        SAuthInAck = 5101,
        SAuthInEuAck = 5102,
        SServerListAck = 5103
    }
}
