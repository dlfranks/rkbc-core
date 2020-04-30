using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public class Comment : IEntity
    {
        [Required]
        public int id { get; set; }
        public string authorId { get; set; }
        public virtual ApplicationUser author { get; set; }
        [Required]
        public int postId { get; set; }
        public virtual Post post {get; set;}
        [Required]
        public string content { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public bool isUser { get; set; } = false;

        [Required]
        public DateTime pubDate { get; set; } = DateTime.UtcNow;

        int IEntity.id => throw new NotImplementedException();

        [SuppressMessage(
            "Security",
            "CA5351:Do Not Use Broken Cryptographic Algorithms",
            Justification = "We aren't using it for encryption so we don't care.")]
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "It is an email address.")]
        public string GetGravatar()
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(this.email.Trim().ToLowerInvariant());
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return $"https://www.gravatar.com/avatar/{sb.ToString().ToLowerInvariant()}?s=60&d=blank";
        }

        public string RenderContent() => this.content;
    }
}
