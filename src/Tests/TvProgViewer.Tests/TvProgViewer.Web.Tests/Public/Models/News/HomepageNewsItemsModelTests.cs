using System;
using FluentAssertions;
using TvProgViewer.WebUI.Models.News;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Models.News
{
    [TestFixture]
    public class HomepageNewsItemsModelTests
    {
        [Test]
        public void CanClone()
        { 
            //create
            var model1 = new HomepageNewsItemsModel
            {
                WorkingLanguageId = 1,
            };
            var newsItemModel1 = new NewsItemModel
            {
                Id = 1,
                SeName = "SeName 1",
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                AllowComments = true,
                NumberOfComments = 2,
                CreatedOn = new DateTime(2010, 01, 01),
                AddNewComment = new AddNewsCommentModel
                {
                    CommentTitle = "CommentTitle 1",
                    CommentText = "CommentText 1",
                    DisplayCaptcha = true
                }
            };
            newsItemModel1.Comments.Add(new NewsCommentModel
            {
                Id = 3,
                UserId = 4,
                UserName = "UserName 1",
                UserAvatarUrl = "UserAvatarUrl 1",
                CommentTitle = "CommentTitle 1",
                CommentText = "CommentText 1",
                CreatedOn = new DateTime(2010, 01, 02),
                AllowViewingProfiles = true
            });
            model1.NewsItems.Add(newsItemModel1);

            //clone
            var model2 = model1 with { };
            model2.WorkingLanguageId.Should().Be(1);
            model2.NewsItems.Should().NotBeNull();
            model2.NewsItems.Count.Should().Be(1);
            var newsItemModel2 = model2.NewsItems[0];
            newsItemModel2.Id.Should().Be(1);
            newsItemModel2.SeName.Should().Be("SeName 1");
            newsItemModel2.Title.Should().Be("Title 1");
            newsItemModel2.Short.Should().Be("Short 1");
            newsItemModel2.Full.Should().Be("Full 1");
            newsItemModel2.AllowComments.Should().BeTrue();
            newsItemModel2.NumberOfComments.Should().Be(2);
            newsItemModel2.CreatedOn.Should().Be(new DateTime(2010, 01, 01));
            newsItemModel2.Comments.Should().NotBeNull();
            newsItemModel2.Comments.Count.Should().Be(1);
            newsItemModel2.Comments[0].Id.Should().Be(3);
            newsItemModel2.Comments[0].UserId.Should().Be(4);
            newsItemModel2.Comments[0].UserName.Should().Be("UserName 1");
            newsItemModel2.Comments[0].UserAvatarUrl.Should().Be("UserAvatarUrl 1");
            newsItemModel2.Comments[0].CommentTitle.Should().Be("CommentTitle 1");
            newsItemModel2.Comments[0].CommentText.Should().Be("CommentText 1");
            newsItemModel2.Comments[0].CreatedOn.Should().Be(new DateTime(2010, 01, 02));
            newsItemModel2.Comments[0].AllowViewingProfiles.Should().BeTrue();
            newsItemModel2.AddNewComment.Should().NotBeNull();
            newsItemModel2.AddNewComment.CommentTitle.Should().Be("CommentTitle 1");
            newsItemModel2.AddNewComment.CommentText.Should().Be("CommentText 1");
            newsItemModel2.AddNewComment.DisplayCaptcha.Should().BeTrue();
        }
    }
}
