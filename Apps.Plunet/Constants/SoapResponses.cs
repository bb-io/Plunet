namespace Apps.Plunet.Constants
{
    public static class SoapResponses
    {
        public const string CustomerAndResourceOk = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:api=\"http://API.Integration/\"><soap:Header/><soap:Body><api:receiveNotifyCallbackResponse/></soap:Body></soap:Envelope>";
        public const string OtherOk = "<S:Envelope xmlns:S=\"http://www.w3.org/2003/05/soap-envelope\"><S:Body><ns2:ReceiveNotifyCallbackResponse xmlns:ns2=\"http://API.Integration/\"/></S:Body></S:Envelope>";
    }
}
