using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OgilvyOne.Report.Model.Library
{
    public class PaginationProvider : IEnumerable<PaginationItem>
    {
        List<PaginationItem> Data;
        Func<int, string> urlHandler;
        public int CurrentPage { get; set; }
        public int TotalPage { get; private set; }
        public int TotalItem { get; private set; }
        public int MaxRecord { get; private set; }
        public PaginationProvider(int page, int maxRecord, Func<int, string> urlHandler)
        {
            if (page <= 0)
            {
                page = 1;
            }
            Data = new List<PaginationItem>();
            CurrentPage = page;
            MaxRecord = maxRecord;
            TotalPage = 0;
            TotalItem = 0;
            Begin = ((page - 1) * maxRecord) + 1;
            End = Begin + maxRecord - 1;
            this.urlHandler = urlHandler;
        }

        public int Begin { get; private set; }
        public int End { get; private set; }
        public void SetTotal(int total)
        {
            TotalPage = total / MaxRecord;
            if (total % MaxRecord > 0)
            {
                TotalPage++;
            }

            Data = new List<PaginationItem>();
            for (var i = 0; i < TotalPage; i++)
            {
                var page = i + 1;
                Data.Add(new PaginationItem(page, urlHandler(page)));
            }
        }

        public bool HasNext()
        {
            return TotalPage > CurrentPage;
        }
        public bool HasPrev()
        {
            return CurrentPage > 1;
        }
        public PaginationItem Next()
        {
            var page = CurrentPage + 1;
            return new PaginationItem(page, urlHandler(page));
        }
        public PaginationItem Prev()
        {
            var page = CurrentPage - 1;
            return new PaginationItem(page, urlHandler(page));
        }
        public PaginationItem First()
        {
            var page = 1;
            return new PaginationItem(page, urlHandler(page));
        }
        public PaginationItem Last()
        {
            var page = TotalPage;
            return new PaginationItem(page, urlHandler(page));
        }

        public IEnumerator<PaginationItem> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public PaginationCollection GetPaginationCollection(int preNumber, int nextNumber)
        {

            var begin = this.CurrentPage - preNumber;
            if (begin <= 0)
            {
                begin = 1;
            }
            var end = begin + preNumber + nextNumber;
            if (end > this.TotalPage)
            {
                end = this.TotalPage;
                begin = end - nextNumber - preNumber;
                if (begin <= 0)
                {
                    begin = 1;
                }
            }
            List<PaginationItem> list = new List<PaginationItem>();
            for (var i = begin - 1; i < end; i++)
            {
                list.Add(Data[i]);
            }
            PaginationCollection kq = new PaginationCollection(list, this.TotalPage);
            return kq;
        }
    }
    public class PaginationItem
    {
        public int Page { get; set; }
        public string Url { get; set; }

        public PaginationItem(int page, string url)
        {
            this.Page = page;
            this.Url = url;
        }
    }

    public class PaginationCollection : IEnumerable<PaginationItem>
    {
        private IEnumerable<PaginationItem> data;
        public PaginationCollection(IEnumerable<PaginationItem> Data, int totalPage)
        {
            this.data = Data;
            if (Data.Any())
            {
                this.HasFirst = this.FirstOrDefault().Page != 1;
                this.HasLast = this.LastOrDefault().Page != totalPage;
            }
        }

        public bool HasFirst { get; private set; }
        public bool HasLast { get; private set; }

        public IEnumerator<PaginationItem> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }
    }
}