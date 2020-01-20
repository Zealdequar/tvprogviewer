using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.Infrastructure;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.WebUI.Models;

namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Контроллер для работы с деревьями
    /// </summary>
    public class TreeController : Controller
    {
        /// <summary>
        /// Словарь с днями недели для обтображения пиктограмм
        /// </summary>
        private Dictionary<string, string> dictWeek = new Dictionary<string, string>()
                                                       {
                                                           {"monday", "Mon"},
                                                           {"tuesday", "Tue"},
                                                           {"wednesday", "Wen"},
                                                           {"thursday", "Ths" },
                                                           {"friday", "Fri" },
                                                           {"saturday", "Sat"},
                                                           {"sunday", "Sun"}
                                                       };
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }

        /// <summary>
        /// Репозиторий для деревьев
        /// </summary>
        private ITreeRepository repository;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="treeRepository">Репозиторий для деревьев</param>
        public TreeController(ITreeRepository treeRepository)
        {
            this.repository = treeRepository;
        }

        /// <summary>
        /// Вывод по умолчанию
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Получение дерева в формате JSON
        /// </summary>
        /// <param name="providerId">Идентификатор поставщика программы телепередач</param>
        /// <param name="typeProg">Тип программы телепередач</param>
        /// <param name="mode">Режим отображения: 1 - группировка под датам; 2 - группировка по телеканалам</param>
        public JsonResult GetTreeData(int providerId, int typeProg, int mode)
        {
            if (UserId == null)
                return new JsonResult();

            TreeNode root = new TreeNode()
            {
                id = Guid.NewGuid().ToString(),
                text = "",
                state = new State(true, false, false),
                icon = "/imgs/i/satellite_dish2.png",
                children = new List<TreeNode>()
            };

            ProgPeriod periodMinMax = repository.GetSystemProgrammePeriod(typeProg);
            DateTime tsMin = periodMinMax.dtStart.DateTime;
            tsMin = new DateTime(tsMin.Year, tsMin.Month, tsMin.Day, 5, 45, 0);
            DateTime tsMiddle = tsMin.AddDays(6);
            DateTime tsMax = periodMinMax.dtEnd.DateTime;
            tsMax = new DateTime(tsMax.AddDays(-1).Year, tsMax.AddDays(-1).Month, tsMax.AddDays(-1).Day, 5, 45, 0);
                        
            List<UserChannel> chanList = repository.GetUserChannelList(UserId.Value, providerId);
            TreeNode oneHalfRoot = new TreeNode()
            {
                id = $"({tsMin.ToShortDateString()}—{tsMiddle.ToShortDateString()})",
                text = $"({tsMin.ToShortDateString()}—{tsMiddle.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/imgs/i/All.png",
                children = new List<TreeNode>()
            };
            TreeNode twoHalfRoot = new TreeNode()
            {
                id = $"({tsMiddle.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                text = $"({tsMiddle.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/imgs/i/All.png",
                children = new List<TreeNode>()
            };
            if (mode == 1) // Группировка по датам
            {
                for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                {
                    string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                    TreeNode dayNode = new TreeNode()
                    {
                        id = tsCurDate.ToShortDateString(),
                        text = $"({tsCurDate.ToShortDateString()})",
                        state = new State(false, false, false),
                        icon = $"/imgs/i/{strKey}.png",
                        children = new List<TreeNode>()
                    };
                    oneHalfRoot.children.Add(dayNode);
                    foreach (UserChannel channel in chanList)
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelID}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "imgs/i/satellite_25.png")}"
                        };
                        dayNode.children.Add(channelNode);
                    }
                }

                for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                {
                    string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                    TreeNode dayNode = new TreeNode()
                    {
                        id = tsCurDate.ToShortDateString(),
                        text = $"({tsCurDate.ToShortDateString()})",
                        state = new State(false, false, false),
                        icon = $"/imgs/i/{strKey}.png",
                        children = new List<TreeNode>()
                    };
                    twoHalfRoot.children.Add(dayNode);
                    foreach (UserChannel channel in chanList)
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelID}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "imgs/i/satellite_25.png")}"
                        };
                        dayNode.children.Add(channelNode);
                    }
                }
            }
            else
            {
                if (mode == 2)
                {
                    foreach (UserChannel channel in chanList)
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"1_{channel.ChannelID}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "imgs/i/satellite_25.png")}",
                            children = new List<TreeNode>()
                        };
                        oneHalfRoot.children.Add(channelNode);
                        for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                        {
                            string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                            TreeNode dayNode = new TreeNode()
                            {
                                id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelID}",
                                text = $"({tsCurDate.ToShortDateString()})",
                                state = new State(false, false, false),
                                icon = $"/imgs/i/{strKey}.png"
                            };
                            channelNode.children.Add(dayNode);
                        }
                    }
                    foreach (UserChannel channel in chanList)
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"2_{channel.ChannelID}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "imgs/i/satellite_25.png")}",
                            children = new List<TreeNode>()
                        };
                        twoHalfRoot.children.Add(channelNode);
                        for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                        {
                            string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                            TreeNode dayNode = new TreeNode()
                            {
                                id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelID}",
                                text = $"({tsCurDate.ToShortDateString()})",
                                state = new State(false, false, false),
                                icon = $"/imgs/i/{strKey}.png"
                            };
                            channelNode.children.Add(dayNode);
                        }
                    }
                }
            }

            root.children.Add(oneHalfRoot);
            root.children.Add(twoHalfRoot);
            return Json(root, JsonRequestBehavior.AllowGet);
        }
    }
}