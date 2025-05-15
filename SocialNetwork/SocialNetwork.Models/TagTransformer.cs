using System.Text.RegularExpressions;

namespace SocialNetwork.Models
{
    public static class TagTransformer
    {
        public static string Transform(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return "#";
            }

            // Remove all spaces
            string transformedTag = tag.Replace(" ", "");

            // Add # if not present
            if (!transformedTag.StartsWith("#"))
            {
                transformedTag = "#" + transformedTag;
            }

            // Limit to 20 characters
            if (transformedTag.Length > 20)
            {
                transformedTag = transformedTag.Substring(0, 20);
            }

            return transformedTag;
        }
    }
}