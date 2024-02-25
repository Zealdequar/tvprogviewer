using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;

namespace TvProgViewer.Services.Forums
{
    /// <summary>
    /// Forum service interface
    /// </summary>
    public partial interface IForumService
    {
        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteForumGroupAsync(ForumGroup forumGroup);

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum group
        /// </returns>
        Task<ForumGroup> GetForumGroupByIdAsync(int forumGroupId);

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum groups
        /// </returns>
        Task<IList<ForumGroup>> GetAllForumGroupsAsync();

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertForumGroupAsync(ForumGroup forumGroup);

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateForumGroupAsync(ForumGroup forumGroup);

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteForumAsync(Forum forum);

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum
        /// </returns>
        Task<Forum> GetForumByIdAsync(int forumId);

        /// <summary>
        /// Gets forums by group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forums
        /// </returns>
        Task<IList<Forum>> GetAllForumsByGroupIdAsync(int forumGroupId);

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertForumAsync(Forum forum);

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateForumAsync(Forum forum);

        /// <summary>
        /// Deletes a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTopicAsync(ForumTopic forumTopic);

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum Topic
        /// </returns>
        Task<ForumTopic> GetTopicByIdAsync(int forumTopicId);

        /// <summary>
        /// Gets all forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum Topics
        /// </returns>
        Task<IPagedList<ForumTopic>> GetAllTopicsAsync(int forumId = 0,
            int userId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
            int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets active forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum Topics
        /// </returns>
        Task<IPagedList<ForumTopic>> GetActiveTopicsAsync(int forumId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTopicAsync(ForumTopic forumTopic, bool sendNotifications);

        /// <summary>
        /// Updates the forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTopicAsync(ForumTopic forumTopic);

        /// <summary>
        /// Moves the forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the moved forum topic
        /// </returns>
        Task<ForumTopic> MoveTopicAsync(int forumTopicId, int newForumId);

        /// <summary>
        /// Deletes a forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePostAsync(ForumPost forumPost);

        /// <summary>
        /// Gets a forum post
        /// </summary>
        /// <param name="forumPostId">The forum post identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum Post
        /// </returns>
        Task<ForumPost> GetPostByIdAsync(int forumPostId);

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the posts
        /// </returns>
        Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0,
            int userId = 0, string keywords = "",
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum Posts
        /// </returns>
        Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0, int userId = 0,
            string keywords = "", bool ascSort = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed users</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPostAsync(ForumPost forumPost, bool sendNotifications);

        /// <summary>
        /// Updates the forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdatePostAsync(ForumPost forumPost);

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePrivateMessageAsync(PrivateMessage privateMessage);

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="privateMessageId">The private message identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private message
        /// </returns>
        Task<PrivateMessage> GetPrivateMessageByIdAsync(int privateMessageId);

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all messages</param>
        /// <param name="fromUserId">The user identifier who sent the message</param>
        /// <param name="toUserId">The user identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private messages
        /// </returns>
        Task<IPagedList<PrivateMessage>> GetAllPrivateMessagesAsync(int storeId, int fromUserId,
            int toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPrivateMessageAsync(PrivateMessage privateMessage);

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdatePrivateMessageAsync(PrivateMessage privateMessage);

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSubscriptionAsync(ForumSubscription forumSubscription);

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum subscription
        /// </returns>
        Task<ForumSubscription> GetSubscriptionByIdAsync(int forumSubscriptionId);

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum subscriptions
        /// </returns>
        Task<IPagedList<ForumSubscription>> GetAllSubscriptionsAsync(int userId = 0, int forumId = 0,
            int topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertSubscriptionAsync(ForumSubscription forumSubscription);

        /// <summary>
        /// Check whether user is allowed to create new topics
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="forum">Forum</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToCreateTopicAsync(User user, Forum forum);

        /// <summary>
        /// Check whether user is allowed to edit topic
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToEditTopicAsync(User user, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to move topic
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToMoveTopicAsync(User user, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to delete topic
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToDeleteTopicAsync(User user, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to create new post
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToCreatePostAsync(User user, ForumTopic topic);

        /// <summary>
        /// Check whether user is allowed to edit post
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="post">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToEditPostAsync(User user, ForumPost post);

        /// <summary>
        /// Check whether user is allowed to delete post
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="post">Topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToDeletePostAsync(User user, ForumPost post);

        /// <summary>
        /// Check whether user is allowed to set topic priority
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToSetTopicPriorityAsync(User user);

        /// <summary>
        /// Check whether user is allowed to watch topics
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if allowed, otherwise false
        /// </returns>
        Task<bool> IsUserAllowedToSubscribeAsync(User user);

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the page index
        /// </returns>
        Task<int> CalculateTopicPageIndexAsync(int forumTopicId, int pageSize, int postId);

        /// <summary>
        /// Get a post vote 
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the post vote
        /// </returns>
        Task<ForumPostVote> GetPostVoteAsync(int postId, User user);

        /// <summary>
        /// Get post vote made since the parameter date
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="createdFromUtc">Date</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the post votes count
        /// </returns>
        Task<int> GetNumberOfPostVotesAsync(User user, DateTime createdFromUtc);

        /// <summary>
        /// Insert a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPostVoteAsync(ForumPostVote postVote);

        /// <summary>
        /// Delete a post vote
        /// </summary>
        /// <param name="postVote">Post vote</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePostVoteAsync(ForumPostVote postVote);

        /// <summary>
        /// Formats the forum post text
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Formatted text</returns>
        string FormatPostText(ForumPost forumPost);

        /// <summary>
        /// Strips the topic subject
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Formatted subject</returns>
        string StripTopicSubject(ForumTopic forumTopic);

        /// <summary>
        /// Formats the forum signature text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        string FormatForumSignatureText(string text);

        /// <summary>
        /// Formats the private message text
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Formatted text</returns>
        string FormatPrivateMessageText(PrivateMessage pm);

        /// <summary>
        /// Get first post
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum post
        /// </returns>
        Task<ForumPost> GetFirstPostAsync(ForumTopic forumTopic);

        /// <summary>
        /// Gets ForumGroup SE (search engine) name
        /// </summary>
        /// <param name="forumGroup">ForumGroup</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forumGroup SE (search engine) name
        /// </returns>
        Task<string> GetForumGroupSeNameAsync(ForumGroup forumGroup);

        /// <summary>
        /// Gets Forum SE (search engine) name
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum SE (search engine) name
        /// </returns>
        Task<string> GetForumSeNameAsync(Forum forum);

        /// <summary>
        /// Gets ForumTopic SE (search engine) name
        /// </summary>
        /// <param name="forumTopic">ForumTopic</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forumTopic SE (search engine) name
        /// </returns>
        Task<string> GetTopicSeNameAsync(ForumTopic forumTopic);
    }
}