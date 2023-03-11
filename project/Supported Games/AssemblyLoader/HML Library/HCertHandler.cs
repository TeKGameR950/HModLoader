using UnityEngine.Networking;

namespace HMLLibrary
{
    public class HttpCertHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}