using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Tree
{
    public partial record TreeNode: BaseTvProgEntityModel
    {
        public string id { get; set; }

        public string text { get; set; }

        public string icon { get; set; }

        public State state { get; set; }

        public List<TreeNode> children { get; set; }

        public static TreeNode NewNode(string Id)
        {
            return new TreeNode()
            {
                id = Id,
                text = $"Node {Id}",
                children = new List<TreeNode>()
            };
        }
    }

    public record State
    {
        public bool opened = false;
        public bool disabled = false;
        public bool selected = false;

        public State(bool Opened, bool Disabled, bool Selected)
        {
            opened = Opened;
            disabled = Disabled;
            selected = Selected;
        }
    }
}
