using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moonglade.Caching;
using Moonglade.Configuration;
using Moonglade.Configuration.Abstraction;
using Moonglade.Core;
using Moonglade.Model;
using Moonglade.Web.Controllers;
using Moq;
using NUnit.Framework;
using X.PagedList;

namespace Moonglade.Tests.Web
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class HomeControllerTests
    {
        private MockRepository _mockRepository;

        private Mock<IPostService> _mockPostService;
        private Mock<IBlogCache> _mockBlogCache;
        private Mock<IBlogConfig> _mockBlogConfig;
        private Mock<ILogger<HomeController>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Strict);

            _mockPostService = _mockRepository.Create<IPostService>();
            _mockBlogCache = _mockRepository.Create<IBlogCache>();
            _mockBlogConfig = _mockRepository.Create<IBlogConfig>();
            _mockLogger = _mockRepository.Create<ILogger<HomeController>>();
        }

        private HomeController CreateHomeController()
        {
            return new(
                _mockPostService.Object,
                _mockBlogCache.Object,
                _mockBlogConfig.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task Index_View()
        {
            _mockBlogConfig.Setup(p => p.ContentSettings).Returns(new ContentSettings()
            {
                PostListPageSize = 10
            });

            var fakePosts = new List<PostListEntry>
            {
                new()
                {
                    Title = "��996�������ƣ���ÿ���� 9 �㵽�ڣ�һֱ���������� 9 �㣬ÿ�ܹ��� 6 �졣",
                    ContentAbstract = "�й���½��ʱ����ֿ�����׼��ʱ���� һ�칤��ʱ��Ϊ 8 Сʱ��ƽ��ÿ�ܹ�ʱ������ 40 Сʱ���Ӱ�����Ϊһ�� 3 Сʱ��һ���� 36 Сʱ����ʱ����н�𲻵���ƽ�չ��ʵ� 150%����һ����߹�ʱ��Ϊ 48 Сʱ��ƽ��ÿ�¼�н����Ϊ 21.75 �졣",
                    LangCode = "zh-CN",
                    PubDateUtc = new (996,9,6),
                    Slug = "996-icu",
                    Tags = new Tag[]{
                        new ()
                        {
                            DisplayName = "996",
                            Id = 996,
                            NormalizedName = "icu"
                        }
                    }
                }
            };

            _mockPostService.Setup(p => p.GetPagedPostsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid?>()))
                .Returns(Task.FromResult((IReadOnlyList<PostListEntry>)fakePosts));

            _mockPostService.Setup(p => p.CountVisiblePosts()).Returns(996);

            _mockBlogCache.Setup(p =>
                    p.GetOrCreate(CacheDivision.General, "postcount", It.IsAny<Func<ICacheEntry, int>>()))
                .Returns(996);

            var ctl = CreateHomeController();
            var result = await ctl.Index();

            Assert.IsInstanceOf<ViewResult>(result);

            var model = ((ViewResult) result).Model;
            Assert.IsInstanceOf<StaticPagedList<PostListEntry>>(model);

            var pagedList = (StaticPagedList<PostListEntry>) model;
            Assert.AreEqual(996, pagedList.TotalItemCount);
        }

        [Test]
        public async Task Tags_Index()
        {
            var fakeTags = new List<DegreeTag>
            {
                new() { Degree = 251, DisplayName = "Huawei", Id = 35, NormalizedName = "aiguo" },
                new() { Degree = 996, DisplayName = "Ali", Id = 35, NormalizedName = "fubao" }
            };

            var mockTagService = new Mock<ITagService>();
            mockTagService.Setup(p => p.GetTagCountListAsync())
                .Returns(Task.FromResult((IReadOnlyList<DegreeTag>)fakeTags));

            var ctl = CreateHomeController();
            var result = await ctl.Tags(mockTagService.Object);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(fakeTags, ((ViewResult)result).Model);
        }

        [Test]
        public async Task Archive_Index()
        {
            var fakeArchives = new List<Archive>
            {
                new (996,9,6),
                new (251,3,5)
            };

            var mockArchiveService = new Mock<IPostArchiveService>();
            mockArchiveService.Setup(p => p.ListAsync())
                .Returns(Task.FromResult((IReadOnlyList<Archive>)fakeArchives));

            var ctl = CreateHomeController();
            var result = await ctl.Archive(mockArchiveService.Object);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(fakeArchives, ((ViewResult)result).Model);
        }
    }
}
