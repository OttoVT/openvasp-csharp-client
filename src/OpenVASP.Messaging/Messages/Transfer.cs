namespace OpenVASP.Messaging.Messages
{
    public class Transfer
    {
        public string VirtualAssetType { get; set; }

        public string TransferType { get; set; }

        //ChooseType as BigInteger
        public string Amount { get; set; }

        public string DestinationAddress { get; set; }
    }
}