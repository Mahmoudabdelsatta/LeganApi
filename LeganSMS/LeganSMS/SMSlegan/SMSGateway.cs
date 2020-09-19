using System;

namespace SMSlegan
{
    public class SMSGateway
    {

        public static void SendSms(string text, string phone)
        {

            SMSV2.ApplicantMessagesServiceSoapClient service = new SMSV2.ApplicantMessagesServiceSoapClient(SMSV2.ApplicantMessagesServiceSoapClient.EndpointConfiguration.ApplicantMessagesServiceSoap);
            service.SendVerificationCodeFortawteenAsync(text, phone);

         
        }
    }
}
