using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Gravatar.Destro.Net
{
    public class GravatarProvider
    {
        public static byte[] GetBytes(string email)
        {
            var encoder = new UTF8Encoding();
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(email));
            var sb = new StringBuilder(hashedBytes.Length * 2);
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }
            var imageHashCode = sb.ToString().ToLower();
            string gravatarImageUrl = "http://www.gravatar.com/avatar/" + imageHashCode + "?d=404";
            var uri = new Uri(gravatarImageUrl);

            using (WebClient client = new WebClient())
            {
                try
                {
                    var imageByte = client.DownloadData(uri);
                    var headers = client.ResponseHeaders;
                    if (headers != null && headers["Content-Disposition"] != null)
                    {
                        var header = headers["Content-Disposition"];
                        if (header.Contains(imageHashCode))
                        {
                            return imageByte;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

                return null;
            }
        }
    }
}
