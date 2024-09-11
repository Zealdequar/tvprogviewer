using System;
using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record PopularTvChannelTagsModel : BaseTvProgModel
    {
        public PopularTvChannelTagsModel()
        {
            Tags = new List<TvChannelTagModel>();
        }

        #region Utilities

        protected virtual int GetFontSize(double weight, double mean, double stdDev)
        {
            var factor = (weight - mean);

            if (factor != 0 && stdDev != 0)
                factor /= stdDev;

            return (factor > 2) ? 150 :
                (factor > 1) ? 120 :
                (factor > 0.5) ? 100 :
                (factor > -0.5) ? 90 :
                (factor > -1) ? 85 :
                (factor > -2) ? 80 :
                75;
        }

        protected virtual double Mean(IEnumerable<double> values)
        {
            double sum = 0;
            var count = 0;

            foreach (var d in values)
            {
                sum += d;
                count++;
            }

            if (count == 0)
                return 0;
            return sum / count;
        }

        protected virtual double StdDev(IEnumerable<double> values, out double mean)
        {
            mean = Mean(values);
            double sumOfDiffSquares = 0;
            var count = 0;

            foreach (var d in values)
            {
                var diff = (d - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            if (count == 0)
                return 0;
            return Math.Sqrt(sumOfDiffSquares / count);
        }

        #endregion

        #region Methods

        public virtual int GetFontSize(TvChannelTagModel tvChannelTag)
        {
            var itemWeights = new List<double>();
            foreach (var tag in Tags)
                itemWeights.Add(tag.TvChannelCount);
            var stdDev = StdDev(itemWeights, out double mean);

            return GetFontSize(tvChannelTag.TvChannelCount, mean, stdDev);
        }

        #endregion

        #region Properties

        public int TotalTags { get; set; }

        public List<TvChannelTagModel> Tags { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        #endregion
    }
}