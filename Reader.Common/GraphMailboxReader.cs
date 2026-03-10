using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace Reader.Common {
    public class GraphMailboxReader {
        private readonly Microsoft.Graph.GraphServiceClient client;
        public GraphMailboxReader(string? tenantID, string? clientID, string? clientSecret) {
            client = new Microsoft.Graph.GraphServiceClient(
                new Azure.Identity.ClientSecretCredential(tenantID, clientID, clientSecret),
                ["https://graph.microsoft.com/.default"]
            );
        }

        private async Task<List<Message>?> getEmails([AllowNull] string mailbox, int count = 100, string? folderName = null) {
            ArgumentNullException.ThrowIfNull(mailbox);
            string[]? select = ["receivedDateTime"];
            string[]? expand = ["attachments($select=isInline,name,contentType)"];

            MessageCollectionResponse? inboxMessages;
            if (folderName != null) {
                var folders = await client.Users[mailbox].MailFolders.Delta.GetAsDeltaGetResponseAsync();
                string? folderID = folders?.Value?.Single(f => f.DisplayName == folderName).Id;
                folderID ??= folderName;

                inboxMessages = await client.Users[mailbox].MailFolders[folderID].Messages.GetAsync(config => {
                    config.QueryParameters.Top = count;
                    config.QueryParameters.Select = select;
                    config.QueryParameters.Expand = expand;
                });
            } else {
                inboxMessages = await client.Users[mailbox].Messages.GetAsync(config => {
                    config.QueryParameters.Top = count;
                    config.QueryParameters.Select = select;
                    config.QueryParameters.Expand = expand;
                });
            }
            return inboxMessages?.Value;
        }

        private Task<Attachment?> getAttachment(string? mailbox, string? messageID, string? attachmentID) =>
            client.Users[mailbox].Messages[messageID].Attachments[attachmentID].GetAsync();

        private Task<Message?> markEmailAsRead([AllowNull] string mailbox, [AllowNull] string messageID) {
            ArgumentNullException.ThrowIfNull(mailbox);
            ArgumentNullException.ThrowIfNull(messageID);

            return client.Users[mailbox].Messages[messageID].PatchAsync(new Message() {
                IsRead = true,
            });
        }

        private async Task<Message?> moveEmail([AllowNull] string mailbox, [AllowNull] string messageID, [AllowNull] string folderName) {
            ArgumentNullException.ThrowIfNull(mailbox);
            ArgumentNullException.ThrowIfNull(messageID);
            ArgumentNullException.ThrowIfNull(folderName);

            var folders = await client.Users[mailbox].MailFolders.Delta.GetAsDeltaGetResponseAsync();
            string? destinationID = folders?.Value?.Single(f => f.DisplayName == folderName).Id;
            destinationID ??= folderName;

            return await client.Users[mailbox].Messages[messageID].Move.PostAsync(new Microsoft.Graph.Users.Item.Messages.Item.Move.MovePostRequestBody() {
                DestinationId = destinationID,
            });
        }


        public async IAsyncEnumerable<FileAttachment> GetMailboxReportAttachments([AllowNull] string mailbox, string? folderName = null, bool markReadWhenDone = false, bool moveEmailWhenDone = false, string? targetFolderName = null) {
            var messages = await getEmails(mailbox, 1000, folderName);

            foreach (var message in (messages ?? []).OrderBy(m => m.ReceivedDateTime)) {
                foreach (var attachment in (message.Attachments ?? []).Where(a => !(a.IsInline ?? false) && a.OdataType == "#microsoft.graph.fileAttachment")) {
                    if (attachment.ContentType is "application/tlsrpt+gzip" or "application/tlsrpt+json") {

                        var attachmentWithContent = await getAttachment(mailbox, message.Id, attachment.Id);
                        if (attachmentWithContent is FileAttachment a && a.ContentBytes != null) {
                            yield return a;
                            if (markReadWhenDone)
                                await markEmailAsRead(mailbox, message.Id);
                            if (moveEmailWhenDone)
                                await moveEmail(mailbox, message.Id, folderName: targetFolderName);
                        }
                    }
                }
            }
        }
    }
}
