using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace NhsLoginAndAppPoc
{
    public class TokenHelper
    {
        public static string CreateClientAuthJwt()
        {
            var payload = new Dictionary<string, object>
            {
                {"sub", "nhsloginandapppoc"},
                {"aud", "https://auth.sandpit.signin.nhs.uk/token"},
                {"iss", "nhsloginandapppoc"},
                {"exp", DateTimeOffset.Now.AddMinutes(60).ToUnixTimeSeconds() },
                {"jti", Guid.NewGuid()}
            };

            string[] manifestResourceNames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string keyPath = manifestResourceNames.Single(s => s.EndsWith("private_key.pem"));
            Stream keyStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(keyPath);
            using var reader = new StreamReader(keyStream);

            using var rsa = new RSACryptoServiceProvider();
            var key = (RsaPrivateCrtKeyParameters)new PemReader(reader)
                .ReadObject();
            var rsaParams = DotNetUtilities.ToRSAParameters(key);

            rsa.ImportParameters(rsaParams);

            return JWT.Encode(payload, rsa, JwsAlgorithm.RS512);
        }
    }
}