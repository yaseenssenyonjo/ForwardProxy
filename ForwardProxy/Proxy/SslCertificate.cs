using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ForwardProxy.Proxy
{
    internal static class SslCertificate
    {
        public static X509Certificate2 Create(string host)
        {
            // https://stackoverflow.com/questions/13806299/how-to-create-a-self-signed-certificate-using-c

            // Generate an asymmetric key pair.
            var ecdsa = ECDsa.Create();
            
            // Request a certificate with the common name as the host using the key pair.
            // Common Name (AKA CN) represents the server name protected by the SSL certificate.
            var request = new CertificateRequest($"cn={host}", ecdsa, HashAlgorithmName.SHA512);
            
            var validFrom = DateTimeOffset.UtcNow;
            var validUntil = DateTimeOffset.UtcNow.AddYears(5);

            var certificate = request.CreateSelfSigned(validFrom, validUntil);
            var certificateBytes = certificate.Export(X509ContentType.Pfx);
            
            return new X509Certificate2(certificateBytes);
        }
    }
}