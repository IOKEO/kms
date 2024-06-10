using Microsoft.Extensions.Options;

namespace Kms.Services
{
    public class SharePointList
    {
        public string SiteId { get; set; }
        public string ListId { get; set; }
        public string ListCommentId { get; set; }

        private readonly SharePointList _sharePointList;

        public SharePointList()
        {
            // Constructeur sans paramètres
        }
        public SharePointList(IOptions<SharePointList> sharePointList)
        {
            SiteId = sharePointList.Value.SiteId;
            ListId = sharePointList.Value.ListId;
            ListCommentId = sharePointList.Value.ListCommentId;

        }

    }
}
