namespace BlogSite.Client.Utility
{
    public static class Extensions
    {
        public static string TruncateWithEllipsis(this string input, int maxLength)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (maxLength <= 0)
                throw new ArgumentException("Max length should be greater than zero.", nameof(maxLength));

            if (input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength) + "...";
        }

        public static string ToRelativeTime(this DateTime inputDateTime)
        {
            TimeSpan timeDifference = DateTime.Now - inputDateTime;

            if (timeDifference.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (timeDifference.TotalMinutes < 2)
            {
                return "1 minute ago";
            }
            else if (timeDifference.TotalHours < 1)
            {
                return $"{(int)timeDifference.TotalMinutes} minutes ago";
            }
            else if (timeDifference.TotalHours < 2)
            {
                return "1 hour ago";
            }
            else if (timeDifference.TotalDays < 1)
            {
                return $"{(int)timeDifference.TotalHours} hours ago";
            }
            else if (timeDifference.TotalDays < 2)
            {
                return "1 day ago";
            }
            else if (timeDifference.TotalDays < 7)
            {
                return $"{(int)timeDifference.TotalDays} days ago";
            }
            else if (timeDifference.TotalDays < 14)
            {
                return "1 week ago";
            }
            else if (timeDifference.TotalDays < 30)
            {
                return $"{(int)(timeDifference.TotalDays / 7)} weeks ago";
            }
            else if (timeDifference.TotalDays < 60)
            {
                return "1 month ago";
            }
            else if (timeDifference.TotalDays < 365)
            {
                return $"{(int)(timeDifference.TotalDays / 30)} months ago";
            }
            else
            {
                return $"{(int)(timeDifference.TotalDays / 365)} years ago";
            }
        }
    }
}
